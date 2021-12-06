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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DownTube.Common;

using Newtonsoft.Json;

using Debug = DownTube.Common.Debug;

#endregion

namespace DownTube.Models {

    #endregion

    //internal interface IYTVid : IJsonCereal {
    //    Uri? URL { get; }
    //    string? Uploader { get; }
    //    string? Title { get; }
    //    TimeSpan? Duration { get; }
    //    public YouTubeFormat[] Formats { get; }
    //}

    public readonly struct YouTubeVideo /* : IYTVid*/ {
        [JsonProperty("webpage_url")] public Uri? URL { get; }
        [JsonProperty("uploader")] public string? Uploader { get; }
        [JsonProperty("title")] public string? Title { get; }
        [JsonProperty("duration")] public TimeSpan? Duration { get; }
        [JsonProperty("formats")] public YouTubeFormat[] Formats { get; }

        public YouTubeVideo( Uri? URL, string? Uploader = null, string? Title = null, TimeSpan? Duration = null, YouTubeFormat[]? Formats = null ) {
            this.URL = URL;
            this.Uploader = Uploader;
            this.Title = Title;
            this.Duration = Duration;
            this.Formats = Formats ?? Array.Empty<YouTubeFormat>();
        }

        public override string ToString() => $"'{URL?.ToString() ?? "<NULL URL>"}' '{Uploader ?? "<NULL Uploader>"}' '{Title ?? "<NULL TITLE>"}' '{Duration?.ToString() ?? "<NULL DURATION>"}' '{(Formats != null ? string.Join("', '", Formats) : "<NULL FORMATS>")}'";

        //public static async Task<YouTubeVideo?> GetVideoAsync(Url URL) {

        //}
    }

    public class AsyncWorker<TArgs, TReturn> {
        internal Func<AsyncWorker<TArgs, TReturn>, TArgs, Task<TReturn>> DoWork;
        internal Action<double> ProgressChanged;
        internal Action<TReturn> OnFinished;

        public AsyncWorker( Func<AsyncWorker<TArgs, TReturn>, TArgs, Task<TReturn>> DoWork, Action<double> ProgressChanged, Action<TReturn> OnFinished ) {
            this.DoWork = DoWork;
            this.ProgressChanged = ProgressChanged;
            this.OnFinished = OnFinished;
        }

        public double Progress { get; private set; }

        public bool Started { get; private set; }

        public bool Finished { get; private set; }

        public bool Cancelling { get; private set; }

        public void Cancel() => Cancelling = true;

        public void ReportProgress( double Progress ) {
            this.Progress = Progress;
            ProgressChanged.Invoke(Progress);
        }

        public async Task Run(TArgs Args) {
            Started = true;
            TReturn Result = await DoWork.Invoke(this, Args);
            Debug.WriteLine("Finished Running Async Worker");
            Finished = true;
            OnFinished.Invoke(Result);
        }
    }

    public abstract class DownloadRequest<T, TR> {

        public AsyncWorker<T, TR> AW { get; private set; }

        //public BackgroundWorker BW = null!;

        protected DownloadRequest() => AW = null!;

        //public void Cancel() => BW.CancelAsync();

        public abstract Task<TR> DoWork( AsyncWorker<T, TR> Worker, T Args );

        public void Cancel() => AW.Cancel();

        public async Task<TR> Run( T RunArgs, Action<double> OnProgress ) {

            TaskCompletionSource<TR> TCS = new TaskCompletionSource<TR>();

            AW = new AsyncWorker<T, TR>(
                async ( Worker, Args ) => await DoWork(Worker, Args),
                OnProgress,
                Result => _ = TCS.TrySetResult(Result)
            );

            await AW.Run(RunArgs);

            return await TCS.Task;
        }
    }

    public abstract class YouTubeDownloadRequest : DownloadRequest<YouTubeVideo, bool> {
        public FileInfo? ResultDest = null;
        public FileInfo Dest;
        public Action<string> DataReceived;

        protected YouTubeDownloadRequest( FileInfo Dest, Action<string> DataReceived ) {
            this.Dest = Dest;
            this.DataReceived = DataReceived;
        }

        public abstract string OverrideFormat { get; }

        public abstract string GetCLIArgs( string URL );

        public override async Task<bool> DoWork( AsyncWorker<YouTubeVideo, bool> Worker, YouTubeVideo Vid ) {
            //Create the process
            Debug.WriteLine($"w/ url: {Vid.URL?.ToString() ?? "none"}");
            Process? P = Process.Start(new ProcessStartInfo(SysStruct.YouTubeDL.FullName) {
                Arguments = GetCLIArgs(Uri.UnescapeDataString(Vid.URL?.AbsoluteUri ?? throw new ArgumentNullException())),
                //RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (P == null) { return false; }

            Debug.WriteLine($"Running with args: {P.StartInfo.Arguments}");

            Regex RegexResultDest = new Regex(RegexResultDestStr);

            bool Done = false;
            P.OutputDataReceived += async ( A, B ) => await OnOutputDataReceived(() => {
                if (Worker.Finished) { return; }

                //Debug.WriteLine("Uniquely, but safely, finished.");
                Worker.ReportProgress(100);
                Done = true;
            }, RegexResultDest, A, B);

            Regex RegexDownloadProgress = new Regex("\\[download] +(?<Progress>\\d+\\.?\\d+?)\\% +of +(?<Size>\\d+\\.?\\d+?.+?) +(?:at +(?:(?<Rate>\\d+\\.?\\d+?.+?)\\/s|Unknown speed) +ETA +(?:(?<ETA>\\d+:\\d+)|Unknown ETA)|in +(?<FinalTime>\\d+\\:\\d+))");

            P.OutputDataReceived += ( _, E ) => {
                if (E.Data != null) {
                    DataReceived.Invoke(E.Data);
                    if (Worker.Finished) { return; }

                    Match M = RegexDownloadProgress.Match(E.Data);
                    Group GProg = M.Groups["Progress"];
                    if (GProg.Success && double.TryParse(GProg.Value, out double Prog)) {
                        //Debug.WriteLine("Got new progress: " + Prog);
                        Worker.ReportProgress(Prog / 100.0);
                    }
                }
            };

            //Thread.Sleep(100);
            P.BeginOutputReadLine();
            for (;;) {
                if (Worker.Cancelling) {
                    Debug.WriteLine("Cancelled!");
                    P!.Kill();
                    return false;
                }

                if (Done || P != null && P.HasExited) {
                    Debug.WriteLine("Properly finished!");
                    Worker.ReportProgress(100);
                    Done = true;
                    if (ResultDest != null) { File.Move(ResultDest.FullName, Dest.FullName); }

                    return true;
                }

                await Task.Delay(100);
                //Debug.WriteLine("Work Running.");
            }
        }

        public abstract string RegexResultDestStr { get; }

        public virtual async Task OnOutputDataReceived( Action Finished, Regex RegexResultDest, object ODSender, DataReceivedEventArgs ODE ) {
            string Data = ODE.Data;
            if (Data == null) {
                Finished.Invoke();
                return;
            }

            Debug.WriteLine($"OUT>>> {Data}");

            Match M = RegexResultDest.Match(Data);
            Group RDG = M.Groups["Dest"];

            if (RDG.Success) {
                ResultDest = new FileInfo(Path.Combine(SysStruct.AppDir.FullName, $"{Path.GetFileNameWithoutExtension(RDG.Value)}.{OverrideFormat}"));
                Debug.WriteLine($"\tDestination: {ResultDest.FullName}");
            }

            //A.ManageData(Data);

            await Task.Delay(10);
        }
    }

    public class YouTubeVideoDownloadRequest : YouTubeDownloadRequest {

        public YouTubeVideoDownloadRequest( FileInfo Dest, Action<string> DataReceived ) : base(Dest, DataReceived) { }

        public override string GetCLIArgs( string URL ) {
            string Args = "";
            //Args += $"-o \"{Dest.FullName}\"";
            Args += "-f bestvideo[ext!=webm]‌​+bestaudio[ext!=webm]‌​/best[ext!=webm]";
            Args += " --merge-output-format mp4";
            Args += " --add-metadata";
            //Args += " --embed-thumbnail";
            Args += $" {URL}";
            return Args;
        }

        public override string OverrideFormat => "mp4";

        public override string RegexResultDestStr => "\\[ffmpeg] (?:Merging formats into \"|Adding metadata to ')(?<Dest>.+?)(?:\"|')";
        //public override string RegexResultDestStr => "\\[ffmpeg] Destination: (?<Dest>.+?\\.[a-zA-Z0-9]+)";

    }

    public class YouTubeAudioDownloadRequest : YouTubeDownloadRequest {

        public YouTubeAudioDownloadRequest( FileInfo Dest, Action<string> DataReceived ) : base(Dest, DataReceived) { }

        public override string GetCLIArgs( string URL ) {
            string Args = "";
            //Args += $"-o \"{Dest.FullName}\"";
            Args += "--extract-audio";
            Args += " --audio-format mp3";
            Args += " --add-metadata";
            Args += " --embed-thumbnail";
            Args += @" --postprocessor-args ""-write_id3v1 1 -id3v2_version 3""";
            Args += $" {URL}";
            return Args;
        }

        public override string OverrideFormat => "mp3";

        public override string RegexResultDestStr => "\\[ffmpeg] Destination: (?<Dest>.+?\\.mp3)";

    }
}