#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using YoutubeDLSharp;
using YoutubeDLSharp.Options;

using YoutubeSnoop;
using YoutubeSnoop.Api;
using YoutubeSnoop.Api.Entities;
using YoutubeSnoop.Enums;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Contains all the necessary components of the application: Searching for videos; parsing video metadata; and invoking youtube-dl to download videos.
/// </summary>
public static class DownloadEngine {

    /// <summary>
    /// The file containing the YouTube V3 API key.
    /// </summary>
    public static readonly FileInfo YtKeyFile = FileSystemInfoExtensions.AppDir.CreateSubfile("Key.json");

    /// <summary>
    /// Initialises the instance.
    /// </summary>
    public static void Init() {
        JsonKeyFile JKF = YtKeyFile.Deserialise<JsonKeyFile>().CatchNull();
        Request.ApiKey = JKF.Key;
    }

    /// <summary>
    /// Searches for videos with the specified query on YouTube.
    /// </summary>
    /// <param name="Query">The query to search for.</param>
    /// <param name="ReturnAmount">The amount of results to return.</param>
    /// <returns>A collection of search results.</returns>
    public static List<YoutubeSearchResult> Search( string Query, int ReturnAmount = 20 ) => new YoutubeSearch(new SearchSettings {
        Query = Query,
        PublishedBefore = DateTime.Now,
        Order = SearchOrder.Relevance,
        Type = ResourceKind.Video
    }).Grab(ReturnAmount).AsList();

    /// <summary>
    /// Asynchronously gets the thumbnail.
    /// </summary>
    /// <param name="Result">The result.</param>
    /// <param name="Size">The size.</param>
    /// <param name="Token">The token.</param>
    /// <returns>The thumbnail as a <see cref="BitmapSource"/></returns>
    /// <exception cref="ArgumentNullException"><paramref name="Size"/> is <see langword="null" />.</exception>
    /// <exception cref="KeyNotFoundException">The thumbnails are retrieved and <paramref name="Size"/> is not found.</exception>
    public static async Task<BitmapSource> GetThumbnailAsync( this YoutubeSearchResult Result, ThumbnailSize Size, CancellationToken Token = default ) {
        Thumbnail Thumb = Result.Thumbnails[Size];
        string Url = Thumb.Url;
        //Url to ImageSource
        Bitmap Bmp = await GetBitmapAsync(Url, Token);
        BitmapSource Source = Bmp.ConvertToBitmapSource();
        return Source;
    }

    /// <summary>
    /// Asynchronously gets the thumbnail.
    /// </summary>
    /// <param name="Result">The result.</param>
    /// <param name="Token">The token.</param>
    /// <returns></returns>
    public static async Task<BitmapSource> GetThumbnailAsync( this SearchResult Result, CancellationToken Token = default ) {
        string Url = Result.ThumbnailUrl;
        //Url to ImageSource
        Bitmap Bmp = await GetBitmapAsync(Url, Token);
        BitmapSource Source = Bmp.ConvertToBitmapSource();
        return Source;
    }

    /// <summary>
    /// The web client used for accessing and downloading video thumbnails.
    /// </summary>
    public static readonly HttpClient WebClient = new HttpClient();

    /// <summary>
    /// Asynchronously gets the bitmap from the given <paramref name="Url"/>.
    /// </summary>
    /// <param name="Url">The URL.</param>
    /// <param name="Token">The cancellation token.</param>
    /// <returns>The retrieved <see cref="Bitmap"/>.</returns>
    public static async Task<Bitmap> GetBitmapAsync( string Url, CancellationToken Token = default ) {
        Stream ResponseStream = await WebClient.GetStreamAsync(Url, Token);
        return new Bitmap(ResponseStream);
    }

    /// <summary>
    /// Converts the given <paramref name="Bitmap"/> to a <see cref="BitmapSource"/>.
    /// </summary>
    /// <param name="Bitmap">The bitmap to convert.</param>
    /// <returns>The converted <paramref name="Bitmap"/> as a <see cref="BitmapSource"/>.</returns>
    public static BitmapSource ConvertToBitmapSource( this Bitmap Bitmap ) {
        BitmapData BitmapData = Bitmap.LockBits(
            new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
            ImageLockMode.ReadOnly, Bitmap.PixelFormat);

        BitmapSource BitmapSource = BitmapSource.Create(
            BitmapData.Width, BitmapData.Height,
            Bitmap.HorizontalResolution, Bitmap.VerticalResolution,
            PixelFormats.Bgr24, null,
            BitmapData.Scan0, BitmapData.Stride * BitmapData.Height, BitmapData.Stride);

        Bitmap.UnlockBits(BitmapData);

        return BitmapSource;
    }

    /// <summary>
    /// Gets the initial <see cref="YoutubeDL"/> instance required for a download request.
    /// </summary>
    /// <param name="Progress">The progress changed event handler.</param>
    /// <param name="Dl">The constructed <see cref="YoutubeDL"/> instance.</param>
    /// <param name="Prog">The constructed <see cref="Progress{T}"/> instance.</param>
    /// <returns>A <see cref="Result{T}"/> to return and abort early on failure, or <see langword="null"/> if the instance is ready.</returns>
    internal static Result<string>? GetYoutubeDL( Action<DownloadProgress> Progress, out YoutubeDL Dl, out Progress<DownloadProgress> Prog ) {
        if ( Props.YoutubeDLPath?.FullName is not { } YDlPath ) {
            Dl = null!;
            Prog = null!;
            return KnownError.NoYoutubeDL.GetResult<string>();
        }
        if ( Props.FFmpegPath?.FullName is not { } FFmPath ) {
            Dl = null!;
            Prog = null!;
            return KnownError.NoFFmpeg.GetResult<string>();
        }
        if ( Props.OutputFolder?.FullName is not { } OutFld ) {
            Dl = null!;
            Prog = null!;
            return KnownError.NoOutputFolder.GetResult<string>();
        }

        // ReSharper disable ExceptionNotDocumentedOptional
        Prog = new Progress<DownloadProgress>(Progress);
        // ReSharper restore ExceptionNotDocumentedOptional

        Dl = new YoutubeDL {
            YoutubeDLPath = YDlPath,
            FFmpegPath = FFmPath,
            OutputFolder = OutFld
        };

        return null;
    }

    /// <summary>
    /// Asynchronously downloads the search result's video, returning the destination path when complete.
    /// </summary>
    /// <param name="Result">The search result to download the video of.</param>
    /// <param name="DownloadProgress">The download progress change handler.</param>
    /// <param name="Token">The cancellation token.</param>
    /// <returns>The downloaded file's path.</returns>
    public static async Task<Result<string>> DownloadVideoAsync( SearchResult Result, Action<DownloadProgress> DownloadProgress, CancellationToken Token = default ) {
        if ( GetYoutubeDL(DownloadProgress, out YoutubeDL Dl, out Progress<DownloadProgress> Prog) is { } ErrRes ) {
            return ErrRes;
        }

        RunResult<string> Res = await Dl.RunVideoDownload(Result.Url, progress: Prog, ct: Token);
        return new Result<string>(Res.Data);
    }

    /// <summary>
    /// Asynchronously downloads the search result's video, returning the destination path when complete.
    /// </summary>
    /// <param name="Result">The search result to download the video of.</param>
    /// <param name="AudioFormat">The format to convert the audio track into (i.e. <see cref="AudioConversionFormat.Mp3"/>)</param>
    /// <param name="DownloadProgress">The download progress change handler.</param>
    /// <param name="Token">The cancellation token.</param>
    /// <returns>The downloaded file's path.</returns>
    public static async Task<Result<string>> DownloadAudioAsync( SearchResult Result, AudioConversionFormat AudioFormat, Action<DownloadProgress> DownloadProgress, CancellationToken Token = default ) {
        if ( GetYoutubeDL(DownloadProgress, out YoutubeDL Dl, out Progress<DownloadProgress> Prog) is { } ErrRes ) {
            return ErrRes;
        }

        RunResult<string> Res = await Dl.RunAudioDownload(Result.Url, AudioFormat, progress: Prog, ct: Token);
        return new Result<string>(Res.Data);
    }

    /// <summary>
    /// Simple <see langword="record"/> type which contains a YouTube V3 API key.
    /// </summary>
    public record JsonKeyFile( string Key ) {
        /// <summary>
        /// Initialises a new instance of the <see cref="JsonKeyFile"/> class.
        /// </summary>
        /// <remarks>
        /// The default <see cref="Key"/> is equivalent to <see cref="Request._defaultApiKey"/></remarks>
        JsonKeyFile() : this("AIzaSyAHVb6LDoO5aARmDlUe9PIeU_U1et1bWd8") { }
    }

}