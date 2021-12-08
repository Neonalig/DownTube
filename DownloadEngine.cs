using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Newtonsoft.Json;

using ReactiveUI;

using YoutubeSnoop;
using YoutubeSnoop.Api;
using YoutubeSnoop.Api.Entities;
using YoutubeSnoop.Enums;

namespace DownTube;

/// <summary>
/// Contains all the necessary components of the application: Searching for videos; parsing video metadata; and invoking youtube-dl to download videos.
/// </summary>
public static class DownloadEngine {

    public static readonly FileInfo YtKeyFile = FileSystemInfoExtensions.AppDir.CreateSubfile("Key.json");

    public static void Init() {
        JsonKeyFile JKF = YtKeyFile.Deserialise<JsonKeyFile>().CatchNull();
        Request.ApiKey = JKF.Key;
    }

    public static List<YoutubeSearchResult> Search( string Query, int TakeAmount = 20 ) => new YoutubeSearch(new SearchSettings {
        Query = Query,
        PublishedBefore = DateTime.Now,
        Order = SearchOrder.Relevance
    }).Take(TakeAmount).ToList();

    public static async Task<BitmapSource> GetThumbnailAsync( this YoutubeSearchResult Result, ThumbnailSize Size, CancellationToken Token = default ) {
        Thumbnail Thumb = Result.Thumbnails[Size];
        string Url = Thumb.Url;
        //Url to ImageSource
        Bitmap Bmp = await GetBitmapAsync(Url, Token);
        BitmapSource Source = Bmp.ConvertToBitmapSource();
        return Source;
    }
    public static async Task<BitmapSource> GetThumbnailAsync( this SearchResult Result, CancellationToken Token = default ) {
        string Url = Result.ThumbnailUrl;
        //Url to ImageSource
        Bitmap Bmp = await GetBitmapAsync(Url, Token);
        BitmapSource Source = Bmp.ConvertToBitmapSource();
        return Source;
    }

    public static readonly HttpClient WebClient = new HttpClient();
    public static async Task<Bitmap> GetBitmapAsync( string Url, CancellationToken Token = default ) {
        Stream ResponseStream = await WebClient.GetStreamAsync(Url, Token);
        return new Bitmap(ResponseStream);
    }
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

}

[JsonObject]
public class JsonKeyFile {
    [JsonProperty] public string Key { get; set; }

    //Below key is YoutubeSnoop.Api.Request._defaultApiKey
    public JsonKeyFile() : this("AIzaSyAHVb6LDoO5aARmDlUe9PIeU_U1et1bWd8") { }

    public JsonKeyFile( string Key ) => this.Key = Key;
}

public class SearchResult : ReactiveObject {
    [JsonProperty] public string Title { get; set; }
    [JsonProperty] public string Author { get; set; }
    [JsonProperty] public string ThumbnailUrl { get; set; }
    [JsonProperty] public string Url { get; set; }

    [JsonIgnore] public BitmapSource? Thumbnail { get; set; }

    [JsonIgnore] public YoutubeSearchResult? ResultOrigination = null;

    [JsonIgnore] public WPFUI.Common.Icon? LoadingGlyph => Thumbnail is null ? WPFUI.Common.Icon.Clock48 : WPFUI.Common.Icon.ArrowDownload48;

    public SearchResult() : this("", "", "", "") { } //Must generate thumbnail manually (constructor used by Newtonsoft.Json

    public void GenerateThumbnail() {
        Dispatcher.CurrentDispatcher.Invoke(async () => {
            Thumbnail = await this.GetThumbnailAsync();
        }, DispatcherPriority.Background);
    }

    public SearchResult( string Title, string Author, string ThumbnailUrl, string Url ) {
        this.Title = Title.Truncate(70, "...");
        this.Author = Author.Truncate(70, "...");
        this.ThumbnailUrl = ThumbnailUrl;
        this.Url = Url;

        GenerateThumbnail();
    }

    public SearchResult( YoutubeSearchResult Result ) {
        Title = Result.Title.Truncate(70, "...");
        Author = Result.ChannelTitle.Truncate(70, "...");
        ThumbnailUrl = Result.Thumbnails.FirstOrDefault().Value.Url ?? "https://picsum.photos/120/90";
        Url = Result.Url;
        ResultOrigination = Result;

        GenerateThumbnail();
    }
}