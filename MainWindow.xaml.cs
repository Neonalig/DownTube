#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DownTube.Common;
using DownTube.Controls;
using DownTube.Extensions;
using DownTube.Headers;
using DownTube.Models;
using MahApps.Metro.IconPacks;
using Ookii.Dialogs.Wpf;
using Debug = DownTube.Common.Debug;

#endregion

namespace DownTube {
    public partial class MainWindow {
        public MainWindow() {
            Debug.SetupHandlers();
            InitializeComponent();
            DetailsPanel.IsEnabled = false;

            GlobalStatus.Init(( Indeterminate, Progress ) => {
                //Debug.WriteLine(@$"\t\t\tIndet: {Indeterminate}; Prog: {Progress}");
                if (Indeterminate) {
                    GlobalProgress.Indeterminate = Indeterminate;
                } else {
                    GlobalProgress.Progress = Progress;
                }
            }, ( ProgressState, ProgressValue ) => {
                //Debug.WriteLine(@$"\t\t\t\tStat: {ProgressState}; Val: {ProgressValue}");
                TaskbarItemInfo.ProgressState = ProgressState;
                TaskbarItemInfo.ProgressValue = ProgressValue;
            });

            //GlobalProgress = 0.0;
            new Thread(() => {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => Header.Content = new HeaderLogo());
                Thread.Sleep(260);
                Dispatcher.Invoke(() => DetailsPanel.IsEnabled = true);
            }).Start();

            //Debug.WriteLine($"Got root: {Path.GetPathRoot(Assembly.GetExecutingAssembly().Location)}");
            InitializeAsync();
            //IsRemovable("");
        }

        YouTubeVideo? _VidToDownload = null;

        public async void InitializeAsync() {
            string[] Args = Environment.GetCommandLineArgs();
            bool FirstArg = true;
            bool ArgAud = false;
            foreach (string A in Args) {
                //First argument in the path to this executable, which is still a valid Uri
                if (FirstArg) { FirstArg = false; continue; }

                string AL = A.ToLowerInvariant();
                if (AL.Contains("audio")) {
                    ArgAud = true;
                } else if (AL.Contains("video")) {
                    ArgAud = false;
                }

                _VidToDownload = await IsValidVideo(A);
                if (_VidToDownload != null) {
                    await Download(A, ArgAud);
                }
            }
        }

        Process ? _RecentCheck = null;
        async Task<YouTubeVideo?> IsValidVideo( string URL ) {
            GlobalStatus.Set(Status.Loading);
            //GlobalProgress = double.NegativeInfinity;

            CLIBlock.Inlines.Clear();
            if (string.IsNullOrWhiteSpace(URL)) {
                GlobalStatus.Set(Status.None);
                return null;
            }
            if (!TryGetUri(URL, out _)) {
                GlobalStatus.Set(Status.Error); //Invalid URL
                AddCLIBoxInline("Invalid URL");
                return null;
            }

            if (_RecentCheck != null && !_RecentCheck.HasExited) { _RecentCheck.Kill(); }
            _RecentCheck = Process.Start(new ProcessStartInfo(SysStruct.YouTubeDL.FullName, $"{URL} -j") {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true 
            });
            TaskCompletionSource<YouTubeVideo?> TCS = new TaskCompletionSource<YouTubeVideo?>();
            _RecentCheck.OutputDataReceived += ( Sender, EA ) => {
                //Debug.WriteLine($"][][][][{EA.Data}");
                if (EA.Data?.StartsWith("{\"") ?? false) {
                    TCS.TrySetResult(EA.Data.TryRead(out YV_JP R) ? R.GetVideo() : (YouTubeVideo?)null);
                    //Debug.WriteLine($"Got video: {Result?.ToString() ?? "<NULL>"}");
                }
            };

            _RecentCheck.BeginOutputReadLine();
            //await WaitForExitAsync(_RecentCheck);
            YouTubeVideo? Result = await TCS.Task;

            Debug.WriteLine($"Got Video: {Result?.ToString() ?? "<null>"}.");
            if (!_RecentCheck.HasExited) { return null; } //Process was cancelled prematurely for a newer validation

            int E = _RecentCheck.ExitCode;
            Debug.WriteLine($"Verification program exited with code: {E}");

            if (E != 0) {
                AddCLIBoxInline($"The given URL '{URL}' points to an invalid YouTube video.");
                AddCLIBoxInline("Make sure that the video domain is an expected domain, AND is readily available (exists, is available in your country, and is not privated.)");
                GlobalStatus.Set(Status.Error); //Invalid URL
                return null;
            }

            AddCLIBoxInline("Ready.");
            GlobalStatus.Set(Status.Success);
            return Result;
        }

        public static Task WaitForExitAsync( Process P, CancellationToken Token = default ) {
            if (P.HasExited) {
                return Task.CompletedTask;
            }

            TaskCompletionSource<object?> TCS = new TaskCompletionSource<object?>();
            P.EnableRaisingEvents = true;
            P.Exited += ( Sender, E ) => TCS.TrySetResult(null);
            if (Token != default) {
                Token.Register(() => TCS.SetCanceled());
            }

            return P.HasExited ? Task.CompletedTask : TCS.Task;
        }

        void AddCLIBoxInline(string Text) {
            Dispatcher.Invoke(() => {
                if (CLIBlock.Inlines.Count > 0) {
                    CLIBlock.Inlines.Add(new LineBreak());
                }
                CLIBlock.Inlines.Add(new Run(Text));
                CLIScroller.ScrollToBottom();
            });
        }

        async void URLBox_OnTextChanged( object Sender, TextChangedEventArgs E ) {
            string URL = ((TextBox)Sender).Text;
            bool Empty = string.IsNullOrEmpty(URL);
            URLBoxPlaceholder.Visibility = Empty ? Visibility.Visible : Visibility.Hidden;
            DetailsPanelMode.IsEnabled = false;
            DetailsPanelMode.Opacity = 0.25;

            _VidToDownload = await IsValidVideo(URL);
            switch (Empty) {
                case false when _VidToDownload != null:
                    DetailsPanelMode.IsEnabled = true;
                    DetailsPanelMode.Opacity = 1.0;
                    break;
                //case true:
                default:
                    DetailsPanelMode.IsEnabled = false;
                    DetailsPanelMode.Opacity = 0.25;
                    break;
            }

            //new Thread(() => {
            //    GlobalProgress = double.NaN;
            //    Debug.WriteLine("Set NaN");
            //    Thread.Sleep(3000);
            //    for (double I = 0; I <= 100; I++) {
            //        Debug.WriteLine($"Set {I / 100.0}");
            //        GlobalProgress = I / 100.0;
            //        Thread.Sleep(20);
            //    }

            //    SystemSounds.Beep.Play();
            //}).Start();
        }

        static bool TryGetUri( string Text, out Uri U ) {
            try {
                U = new Uri(Text);
                return true;
            } catch { }
            U = null!;
            return false;
        }

        async void ButtonDownMusic_Click(object Sender, RoutedEventArgs E) {
            string T = URLBox.Text;
            if (string.IsNullOrWhiteSpace(T) || _VidToDownload == null) { return; }
            await Download(URLBox.Text, true);
        }

        async void ButtonDownVideo_Click( object Sender, RoutedEventArgs E ) {
            string T = URLBox.Text;
            if (string.IsNullOrWhiteSpace(T) || _VidToDownload == null) { return; }
            await Download(URLBox.Text, false);
        }

        DownloadRequest<YouTubeVideo, bool>? _Active = null;
        public async Task Download(string Text, bool AudioOnly) {
            if (!TryGetUri(Text, out Uri _) || _VidToDownload == null) { return; }
            FileInfo? WantedDest = GetSaveLocation(AudioOnly);
            bool Success = true;
            if (WantedDest == null) { return; }

            Themer.SetTheme(AudioOnly ? Themer.KnownTheme.DarkMagenta : Themer.KnownTheme.DarkGreen);

            URLBox.IsEnabled = false;
            DetailsPanelMode.IsEnabled = false;
            GlobalStatus.Set(Status.Loading);

            HeaderETA HeaderETA = new HeaderETA();
            Header.Content = HeaderETA;

            CLIBlock.Inlines.Clear();
            _Active = AudioOnly ? (DownloadRequest<YouTubeVideo, bool>)new YouTubeAudioDownloadRequest(WantedDest, AddCLIBoxInline) : new YouTubeVideoDownloadRequest(WantedDest, AddCLIBoxInline);
            Success = await _Active.Run(_VidToDownload ?? throw new ArgumentNullException(), GlobalStatus.Set);
            GlobalStatus.Set(Success ? Status.Success : Status.Error);

            //Done:
            Dispatcher.Invoke(() => {
                HeaderLogo HL = new HeaderLogo();
                HL.UpdateLogo();
                Header.Content = HL;
                URLBox.IsEnabled = true;
                DetailsPanelMode.IsEnabled = true;
            });

            //Debug.WriteLine("fin. " + WantedDest.FullName + ";" + WantedDest.Exists);
            if (Success && File.Exists(WantedDest.FullName)) {
                Process.Start("explorer.exe", $"/select,\"{WantedDest.FullName}\"");
            } else {
                FileInfo PartsFile = new FileInfo($"{WantedDest.FullName}.part");
                if (PartsFile.Exists) {
                    Dispatcher.Invoke(() => new MessageBoxWindow("DownTube - Download Cancelled", "Delete cancelled download's progress?", "Doing this will free up storage space, but will lose any download progress for the given video.", PackIconFeatherIconsKind.HelpCircle, PartsFile.Delete, () => Process.Start("explorer.exe", $"/select,\"{PartsFile.FullName}\""), () => { }) {
                        MinWidth = 590,
                        Width = 590,
                        MinHeight = 170,
                        Height = 170,
                        ResizeMode = ResizeMode.NoResize
                    }.Show());
                }
            }
        }

        public FileInfo ? GetSaveLocation( bool AudioOnly ) {
            string Ext = AudioOnly ? "mp3" : "mp4";
            string DriveRoot = SysStruct.DriveRoot.FullName;
            //string DriveRoot = Path.GetPathRoot(Assembly.GetExecutingAssembly().Location);
            bool USB = IsRemovable(DriveRoot);
            VistaSaveFileDialog SFD = new VistaSaveFileDialog {
                AddExtension = true,
                FileName = _VidToDownload.HasValue ? $"{_VidToDownload.Value.Title}.{Ext}" : (AudioOnly ? "Audio" : "Video") + Ext,
                Filter = AudioOnly ? "Audio File (*.mp3)|*.mp3|Any File (*.*)|*.*" : "Video File (*.mp4)|*.mp4|Any File (*.*)|*.*",
                FilterIndex = 0,
                InitialDirectory = USB ? DriveRoot : Environment.GetFolderPath(AudioOnly ? Environment.SpecialFolder.CommonMusic : Environment.SpecialFolder.CommonVideos),
                OverwritePrompt = true,
                ValidateNames = true,
                RestoreDirectory = true,
                Title = $"Pick a location to save the {(AudioOnly ? "audio" : "video")} file"
            };
            if (SFD.ShowDialog() != true) { return null; }
            if (SFD.FileName != null) {
                try {
                    string FN = SFD.FileName;
                    if (!FN.EndsWith(Ext, StringComparison.InvariantCultureIgnoreCase)) { FN += $".{Ext}"; }
                    FileInfo FI = new FileInfo(FN);
                    if (FI.Exists) { FI.Delete(); }
                    return FI;
                } catch { }
            }
            return null;
        }

        internal static bool IsRemovable(string Root) {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach(DriveInfo Drive in DriveInfo.GetDrives()) {
                if (Drive.RootDirectory.FullName.Equals(Root, StringComparison.InvariantCultureIgnoreCase)) {
                    return Drive.DriveType == DriveType.Removable;
                }
                //Debug.WriteLine($"Got Drive: {Drive} ({Drive.Name}; {Drive.RootDirectory}; {Drive.DriveType})");
            }
            return false;
        }

        void GlobalCancelButton_OnClick( object Sender, RoutedEventArgs E ) => _Active?.Cancel();
    }
}
