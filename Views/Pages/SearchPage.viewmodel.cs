#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;

using DownTube.Engine;

using MVVMUtils;

#endregion

namespace DownTube.Views.Pages;

/// <summary> Viewmodel for <see cref="SearchPage"/>. </summary>
public class SearchPage_ViewModel : ViewModel<SearchPage> {
    /// <summary> The save folder location. </summary>
    public Uri SaveFolderLocation { get; set; } = FileSystemInfoExtensions.Desktop.GetUri();

    /// <summary> The current user search query. </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <inheritdoc cref="Props.TimesDownloaded"/>
    public int TimesDownloaded { get; private set; } = 0;

    /// <summary>
    /// Initialises a new instance of the <see cref="SearchPage_ViewModel"/> class.
    /// </summary>
    public SearchPage_ViewModel() {
        TimesDownloaded = Props.TimesDownloaded;
        Props.SavedPropertyChanged += ( _, N ) => {
            if ( N.PropertyName == nameof( Props.TimesDownloaded ) ) {
                TimesDownloaded = Props.TimesDownloaded;
            }
        };
    }

    //public ObservableCollection<SearchResult> CachedResults { get; } = new ObservableCollection<SearchResult>();
    /// <summary>
    /// The collection of cached youtube search results.
    /// </summary>
    public ObservableCollection<SearchResult> CachedResults { get; } = new ObservableCollection<SearchResult> {
        new SearchResult(
            "a-ha - Take On Me (Official Video) [Remastered in 4K]",
            "a-ha",
            "https://i.ytimg.com/vi/djV11Xbc914/default.jpg",
            "https://www.youtube.com/watch?v=djV11Xbc914"),
        new SearchResult(
            "a-ha - Take On Me (Live From MTV Unplugged)",
            "ahaVEVO",
            "https://i.ytimg.com/vi/-xKM3mGt2pE/default.jpg",
            "https://www.youtube.com/watch?v=-xKM3mGt2pE"),
        new SearchResult(
            "a-ha - Take On Me (Lyrics)",
            "SuperbLyrics",
            "https://i.ytimg.com/vi/Ktb44YLL8Nw/default.jpg",
            "https://www.youtube.com/watch?v=Ktb44YLL8Nw"),
        new SearchResult(
            "a-ha - Take On Me (Live 8 2005)",
            "Live 8",
            "https://i.ytimg.com/vi/3meKlaJL3qo/default.jpg",
            "https://www.youtube.com/watch?v=3meKlaJL3qo"),
        new SearchResult(
            "Take on Me (2016 Remaster)",
            "A-ha - Topic",
            "https://i.ytimg.com/vi/NaQ083rNUwc/default.jpg",
            "https://www.youtube.com/watch?v=NaQ083rNUwc"),
        new SearchResult(
            @"Weezer - Take On Me (Official Video)",
            @"weezer",
            "https://i.ytimg.com/vi/f7RwDnZI7Tw/default.jpg",
            "https://www.youtube.com/watch?v=f7RwDnZI7Tw"),
        new SearchResult(
            "Take on Me",
            "A-ha - Topic",
            "https://i.ytimg.com/vi/MIgK3zOk0zg/default.jpg",
            "https://www.youtube.com/watch?v=MIgK3zOk0zg"),
        new SearchResult(
            "a-ha - Take On Me (Radio 2 In Concert)",
            "BBC Radio 2",
            @"https://i.ytimg.com/vi/IHDCMQjNimg/default.jpg",
            @"https://www.youtube.com/watch?v=IHDCMQjNimg"),
        new SearchResult(
            "A-ha - Take On Me, Countdown 1985",
            "A-ha South America Play",
            "https://i.ytimg.com/vi/NY2LHIMUcgU/default.jpg",
            "https://www.youtube.com/watch?v=NY2LHIMUcgU"),
        new SearchResult(
            "[A-ha FR] a-ha Take On Me Live BBC One 09-11-2018",
            "A-ha France",
            "https://i.ytimg.com/vi/2SHnAj0yMFE/default.jpg",
            "https://www.youtube.com/watch?v=2SHnAj0yMFE"),
        new SearchResult(
            @"a-ha - Take On Me (Dimitri Vegas  Like Mike vs Ummet Ozcan Remix) [...",
            "Central Bass Boost",
            "https://i.ytimg.com/vi/CNWgQxrcZ4k/default.jpg",
            "https://www.youtube.com/watch?v=CNWgQxrcZ4k"),
        new SearchResult(
            @"A-HA feat KYGO - TAKE ON ME - EXCLUSIVE - The 2015 Nobel Peace Priz...",
            "Nobel Peace Prize Concert",
            "https://i.ytimg.com/vi/0FJFKCnYtN4/default.jpg",
            "https://www.youtube.com/watch?v=0FJFKCnYtN4"),
        new SearchResult(
            "A-ha - Take on me (Live Afterglow 360) - 2016",
            @"German Irazabal",
            "https://i.ytimg.com/vi/UHAXKQtaeG4/default.jpg",
            "https://www.youtube.com/watch?v=UHAXKQtaeG4"),
        new SearchResult(
            "A1 - Take on Me",
            "A1VEVO",
            "https://i.ytimg.com/vi/EbCsIKI6HEU/default.jpg",
            "https://www.youtube.com/watch?v=EbCsIKI6HEU"),
        new SearchResult(
            "A-HA Take On Me 1984 Version",
            "MagneticNorth71",
            @"https://i.ytimg.com/vi/liq-seNVvrM/default.jpg",
            @"https://www.youtube.com/watch?v=liq-seNVvrM"),
        new SearchResult(
            @"The Last of Us 2 - Ellie &quot;Take on Me&quot; Cover Song",
            "Boss Fight Database",
            "https://i.ytimg.com/vi/NKeU1twQYX4/default.jpg",
            "https://www.youtube.com/watch?v=NKeU1twQYX4"),
        new SearchResult(
            "a ha - Take On Me (Official Video Music) 4K Remastered",
            "Daniel CLASSIC Remastered AI",
            "https://i.ytimg.com/vi/AYjpwHQ66ts/default.jpg",
            "https://www.youtube.com/watch?v=AYjpwHQ66ts"),
        new SearchResult(
            "a-Ha - Take On Me [lyrics]",
            @"Lyricosaurus",
            "https://i.ytimg.com/vi/irljBY9J5ig/default.jpg",
            "https://www.youtube.com/watch?v=irljBY9J5ig"),
        new SearchResult(
            "a-ha - The Sun Always Shines on T.V. (Official Video)",
            "a-ha",
            "https://i.ytimg.com/vi/a3ir9HC9vYg/default.jpg",
            "https://www.youtube.com/watch?v=a3ir9HC9vYg"),
        new SearchResult(
            "Take On Me - a-ha - Brooklyn Duo at Carnegie Hall",
            "Brooklyn Duo",
            "https://i.ytimg.com/vi/DHbLuIxw3y4/default.jpg",
            "https://www.youtube.com/watch?v=DHbLuIxw3y4")
    };

}