#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Newtonsoft.Json;

using PropertyChanged;

using ReactiveUI;

using YoutubeSnoop;
using YoutubeSnoop.Api.Entities;
using YoutubeSnoop.Enums;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Contains the found data from the YouTube V3 API.
/// </summary>
/// <seealso cref="ReactiveObject" />
public class SearchResult : ReactiveObject {
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title of the video.
    /// </value>
    [JsonProperty] public string Title { get; set; }

    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    /// <value>
    /// The author of the video.
    /// </value>
    [JsonProperty] public string Author { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail URL.
    /// </summary>
    /// <value>
    /// The URL of the video's thumbnail.
    /// </value>
    [JsonProperty] public string ThumbnailUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    /// <value>
    /// The URL of the video.
    /// </value>
    [JsonProperty] public string Url { get; set; }

    /// <summary>
    /// Gets the thumbnail.
    /// </summary>
    /// <value>
    /// The thumbnail of the video.
    /// </value>
    [JsonIgnore] public BitmapSource? Thumbnail { get; private set; }


    /// <summary>
    /// The <see cref="YoutubeSearchResult"/> this result originated from, or <see langword="null"/> if it didn't.
    /// </summary>
    [JsonIgnore] public YoutubeSearchResult? ResultOrigination = null;

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <value>
    /// The current state of the <see cref="SearchResult"/>.
    /// </value>
    [JsonIgnore] public SearchResultState State { get; set; } = SearchResultState.Polling;

    #region State Visibilities

    /// <summary>
    /// Gets the visibility of elements related to the <see cref="SearchResultState.Polling"/> state.
    /// </summary>
    /// <value>
    /// The visibility of <see cref="SearchResultState.Polling"/> related elements.
    /// </value>
    [ JsonIgnore][ DependsOn(nameof(State))]
    public Visibility PollingVisibility => State == SearchResultState.Polling ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility of elements related to the <see cref="SearchResultState.Idle"/> state.
    /// </summary>
    /// <value>
    /// The visibility of <see cref="SearchResultState.Idle"/> related elements.
    /// </value>
    [ JsonIgnore][ DependsOn(nameof(State))]
    public Visibility IdleVisibility => State == SearchResultState.Idle ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility of elements related to the <see cref="SearchResultState.Downloading"/> state.
    /// </summary>
    /// <value>
    /// The visibility of <see cref="SearchResultState.Downloading"/> related elements.
    /// </value>
    [ JsonIgnore][ DependsOn(nameof(State))]
    public Visibility DownloadingVisibility => State == SearchResultState.Downloading ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility of elements related to the <see cref="SearchResultState.Complete"/> state.
    /// </summary>
    /// <value>
    /// The visibility of <see cref="SearchResultState.Complete"/> related elements.
    /// </value>
    [ JsonIgnore][ DependsOn(nameof(State))]
    public Visibility CompleteVisibility => State == SearchResultState.Complete ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Gets the visibility of elements related to the <see cref="SearchResultState.Error"/> state.
    /// </summary>
    /// <value>
    /// The visibility of <see cref="SearchResultState.Error"/> related elements.
    /// </value>
    [ JsonIgnore][ DependsOn(nameof(State))]
    public Visibility ErrorVisibility => State == SearchResultState.Error ? Visibility.Visible : Visibility.Collapsed;

    #endregion

    /// <summary>
    /// Initialises a new instance of the <see cref="SearchResult"/> class.
    /// </summary>
    /// <remarks>Constructor should only be used by <see cref="Newtonsoft.Json"/> (de/)serialisation.
    /// <br/><see cref="GenerateThumbnail()"/> must be invoked manually when ready.</remarks>
    SearchResult() : this("", "", "", "") { }

    /// <summary>
    /// Downloads and generates the <see cref="Thumbnail"/> from the <see cref="ThumbnailUrl"/>.
    /// </summary>
    public void GenerateThumbnail() {
        _ = Dispatcher.CurrentDispatcher.Invoke(async () => {
            Thumbnail = await this.GetThumbnailAsync();
            if ( State == SearchResultState.Polling ) {
                State = SearchResultState.Idle;
            }
        }, DispatcherPriority.Background);
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="SearchResult"/> class.
    /// </summary>
    /// <param name="Title">The title of the video.</param>
    /// <param name="Author">The author of the video.</param>
    /// <param name="ThumbnailUrl">The URL of the video's thumbnail.</param>
    /// <param name="Url">The URL of the video.</param>
    public SearchResult( string Title, string Author, string ThumbnailUrl, string Url ) {
        this.Title = Title.Truncate(70, "...");
        this.Author = Author.Truncate(70, "...");
        this.ThumbnailUrl = ThumbnailUrl;
        this.Url = Url;

        GenerateThumbnail();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="SearchResult"/> class.
    /// </summary>
    /// <param name="Result">The search result from the Youtube V3 API.</param>
    public SearchResult( YoutubeSearchResult Result ) {
        Title = HttpUtility.HtmlDecode(Result.Title).Truncate(70, "...");
        Author = HttpUtility.HtmlDecode(Result.ChannelTitle).Truncate(70, "...");
        // ReSharper disable once ExceptionNotDocumentedOptional //Justification: Exception cannot occur.
        ThumbnailUrl = Result.Thumbnails.TryGetFirst( out KeyValuePair<ThumbnailSize, Thumbnail> KVP ) ? KVP.Value.Url : "https://picsum.photos/120/90";
        Url = Result.Url;
        ResultOrigination = Result;

        GenerateThumbnail();
    }

}

/// <summary>
/// Indicates the current state of a <see cref="SearchResult"/>.
/// </summary>
public enum SearchResultState {
    /// <summary>
    /// The result is currently loading initial data from YouTube (i.e. the thumbnail is still downloading)
    /// </summary>
    Polling     = 0,
    /// <summary>
    /// The result is ready to download but is currently just sitting idle.
    /// </summary>
    Idle        = 1,
    /// <summary>
    /// The result is now downloading.
    /// </summary>
    Downloading = 2,
    /// <summary>
    /// The result was successfully downloaded.
    /// </summary>
    Complete    = 3,
    /// <summary>
    /// The result had an error during download.
    /// </summary>
    Error       = 4
}