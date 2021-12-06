#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using Newtonsoft.Json;

#endregion

namespace DownTube.Common {
    #if (UNITY_EDITOR)
using UD = UnityEngine.Debug;
    #endif

    #endregion

    public class Debug {
        public static readonly TraceLog TraceLog = new TraceLog(typeof(Debug));
        public static void WriteLine( object Content, string? Category = null ) => WriteLine(Content.ToString(), Category);

        public static void WriteLine( string Text, string? Category = null) {
            TraceLog.AddTrace(Text, Category);
            if (string.IsNullOrEmpty(Category)) { System.Diagnostics.Debug.WriteLine(Text); } else { System.Diagnostics.Debug.WriteLine(Text, Category); }

        }

        public static void Write( object Content, string? Category = null ) => Write(Content.ToString(), Category);

        public static void Write( string Text, string? Category = null ) {
            TraceLog.AddTrace(Text, $"APPEND・{Category}");
            if (string.IsNullOrEmpty(Category)) { System.Diagnostics.Debug.Write(Text); } else { System.Diagnostics.Debug.Write(Text, Category); }
        }

        public static void Log( string Text ) => WriteLine(Text, "INFO");
        public static void LogWarning( string Text ) => WriteLine(Text, "WARNING");
        public static void LogError( string Text ) => WriteLine(Text, "ERROR");

        public static void Log( object Content ) => Log(Content.ToString());
        public static void LogWarning( object Content ) => LogWarning(Content.ToString());
        public static void LogError( object Content ) => LogError(Content.ToString());

        public static void Catch( Exception E ) {
            System.Diagnostics.Debug.WriteLine($"Adding caught exception with details: {E.Message}; {E.GetType()}; {E}");
            TraceLog.AddTrace(new TraceLog.Trace(E.Message, $"{E.GetType()}", E));
            System.Diagnostics.Debug.WriteLine(E, "ERROR");
            //SD.Fail(E.GetType().Name, E.ToString());
        }

        public static void Save( FileInfo Destination ) => TraceLog.WriteToFile(Destination);

        public static void SaveToTemp( out FileInfo Dest ) {
            FileInfo App = new FileInfo(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo AppLogs = new DirectoryInfo(Path.Combine(App.Directory.FullName, "Logs"));
            AppLogs.Create();
            //SD.WriteLine("Save Path: " + $"Log-{DateTime.Now:D}.{Format.ToString().ToLowerInvariant()}");
            Dest = new FileInfo(Path.Combine(AppLogs.FullName, SanitiseWindows($"Log-{DateTime.Now:F}.json".Replace(':', '-'))));
            Save(Dest);
        }

        protected static string SanitiseWindows( string NonWindowsString, char? Replacement = '�' ) {
            StringBuilder SB = new StringBuilder();
            foreach (char C in NonWindowsString) {
                switch (C) {
                    case '\\':
                    case '/':
                    case ':':
                    case '*':
                    case '?':
                    case '"':
                    case '<':
                    case '>':
                    case '|':
                        if (Replacement.HasValue) { SB.Append(Replacement); }

                        continue;
                    default:
                        SB.Append(C);
                        continue;
                }
            }

            return SB.ToString();
        }

        public static void SetupHandlers() {
            static void QCatch( Exception E ) {
                Catch(E);
                SaveToTemp(out FileInfo Dest);
                MessageBox.Show("An unhandled exception was caught, and the application may become unstable. Restarting the process is recommended, and we are not responsible for any loss of data from this point onward.\n\nPlease send any related debugging logs to the GitHub Issues page.", E.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Process.Start("explorer.exe", $"/select,\"{Dest.FullName}\"");
            }

            void CatchDispatcherUnhandled( object Sender, DispatcherUnhandledExceptionEventArgs E ) {
                WriteLine($"Unhandled Dispatcher Exception detected. {E}");
                QCatch(E.Exception);
                E.Handled = true;
            }

            void CurrentDomain_UnhandledException( object Sender, UnhandledExceptionEventArgs E ) {
                System.Diagnostics.Debug.WriteLine($"Unhandled Domain Exception detected. {E}; {E.ExceptionObject as Exception}");
                if (E.ExceptionObject is Exception Exc) {
                    QCatch(Exc);
                } else {
                    LogError($"[EXCEPTION] {E.ExceptionObject}");
                }
            }

            void Current_Exit( object Sender, ExitEventArgs E ) {
                System.Diagnostics.Debug.WriteLine($"Exiting...; {E.ApplicationExitCode}");
                WriteLine($"Exiting with code: {E.ApplicationExitCode}", E.ApplicationExitCode != 0 ? "ERROR" : null);
                if (TraceLog.ContainsIssue()) {
                    SaveToTemp(out FileInfo Dest);
                    Process.Start("explorer.exe", $"/select,\"{Dest.FullName}\"");
                }
            }

            Application.Current.DispatcherUnhandledException += CatchDispatcherUnhandled;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.Exit += Current_Exit;
            System.Diagnostics.Debug.WriteLine("Handlers added.");
        }

        public static void ErrorCheck() {
            System.Diagnostics.Debug.WriteLine("Checking for errors...");
            if (TraceLog.ContainsIssue()) {
                System.Diagnostics.Debug.WriteLine("\tFound at least 1.");
                SaveToTemp(out FileInfo Dest);
                Process.Start("explorer.exe", $"/select,\"{Dest.FullName}\"");
            } else { System.Diagnostics.Debug.WriteLine("\tAll clear. 👍"); }
        }

    }

    [XmlRoot("Trace Log")]
    public readonly struct TraceLog {
        [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
        public readonly struct Trace {
            [JsonProperty] [XmlElement] public readonly string Text;
            [JsonProperty] [XmlElement] public readonly string? Category;
            [JsonProperty] [XmlIgnore] public readonly Exception? Error;
            [JsonIgnore] [XmlElement] public readonly string? ErrorSummary;

            public Trace( string Text, string? Category = null, Exception? Error = null ) {
                this.Text = Text;
                this.Category = Category;
                this.Error = Error;
                ErrorSummary = Error == null ? null : $"{Error.GetType().Name}] {Error.Message};\r\n\t{Error.ToString().Replace("\n", "\n\t")}";
            }

            public override string ToString() => $"{(Category != null ? $"{Category}] " : string.Empty)}{Text}{Error}";
        }

        [JsonProperty] [XmlArray] public readonly List<Trace> Traces;

        #pragma warning disable IDE0060 // Remove unused parameter
        public TraceLog( object Sender ) => Traces = new List<Trace>();
        #pragma warning restore IDE0060 // Remove unused parameter

        // ReSharper disable twice MethodOverloadWithOptionalParameter
        public void AddTrace( string Text, Exception? Error ) => Traces.Add(new Trace(Text, "ERROR", Error));
        public void AddTrace( string Text, string? Category) => Traces.Add(new Trace(Text, Category, null));
        public void AddTrace( Exception Error ) => Traces.Add(new Trace(Error.GetType().Name, "ERROR", Error));
        public void AddTrace( string Text ) => Traces.Add(new Trace(Text, null, null));
        public void AddTrace( Trace Trace ) => Traces.Add(Trace);

        public void RemoveTrace( Trace Trace ) => Traces.Remove(Trace);

        public void Clear() => Traces.Clear();

        public bool ContainsIssue() {
            ReadOnlyCollection<Trace> SavedTraces = Traces.AsReadOnly();

            System.Diagnostics.Debug.WriteLine($"\t{SavedTraces.Count} traces found;");
            foreach (Trace Trace in SavedTraces) {
                System.Diagnostics.Debug.WriteLine($"\t\tWithin trace {Trace}");
                switch (Trace.Category?.ToUpperInvariant()) {
                    case "ERROR":
                    case "WARNING":
                        return true;
                    default:
                        if (Trace.Error != null) { return true; }

                        break;
                }
            }

            return false;
        }

        public void WriteToFile( FileInfo Destination ) {
            string Extension = Destination.Extension.ToLowerInvariant();
            switch (Extension) {
                case ".xml":
                    Destination.Delete();

                    using (FileStream FS = Destination.Create()) {
                        XmlSerializer Xml = new XmlSerializer(GetType());
                        Xml.Serialize(FS, this);
                    }

                    break;
                default:
                    if (Extension != @".json") { Debug.LogWarning("Unknown destination format; assuming JSON."); }

                    Destination.Delete();

                    using (StreamWriter SW = File.CreateText(Destination.FullName)) {
                        JsonSerializer Json = new JsonSerializer {
                            Formatting = Formatting.Indented
                        };
                        Json.Serialize(SW, this);
                    }

                    File.WriteAllText(Destination.FullName, File.ReadAllText(Destination.FullName).Replace("\\t", "\t").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\r\n    {", "{").Replace("\r\n      \"Text\": \"", "\r\n      \"Text\": \"\t"));
                    break;
            }
        }
    }
}