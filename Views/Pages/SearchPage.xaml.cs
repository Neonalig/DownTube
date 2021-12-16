#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

using DownTube.Engine;

using MVVMUtils;

using Ookii.Dialogs.Wpf;

using WPFUI.Controls;

using YoutubeSnoop;

using Hyperlink = System.Windows.Documents.Hyperlink;

#endregion

namespace DownTube.Views.Pages;

/// <summary>
/// Interaction logic for SearchPage.xaml
/// </summary>
public partial class SearchPage : IView<SearchPage_ViewModel> {
    /// <inheritdoc />
    public SearchPage_ViewModel VM { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="SearchPage"/> class.
    /// </summary>
    public SearchPage() {
        InitializeComponent();
        VM = DataContext.Cast<SearchPage_ViewModel>();
        InitializeAsync();
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    /// <summary> Class made for testing purposes. </summary>
    record struct TestClass( List<SearchResult> Results );

    /// <summary>
    /// Invokes a search with the given <paramref name="Query"/> to the YouTube V3 API, populating <see cref="SearchPage_ViewModel.CachedResults"/> with the returned results.
    /// </summary>
    /// <param name="Query">The video search query.</param>
    public void PromptUserSearch( string Query ) {
        VM.CachedResults.Clear();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( YoutubeSearchResult Res in DownloadEngine.Search(Query) ) {
            SearchResult R = new SearchResult(Res);
            Debug.WriteLine($"{R.Title} by {R.Author}");
            VM.CachedResults.Add(R);
        }
    }

    /// <summary>
    /// Runs asynchronous initialisation methods.
    /// </summary>
    void InitializeAsync() {
        DownloadEngine.Init();

        //VM.CachedResults.Clear();
        //return;
        FileInfo TestFile = FileSystemInfoExtensions.Desktop.CreateSubfile("testfile.json");

        const bool Search = false;
#pragma warning disable CS0162
        // ReSharper disable HeuristicUnreachableCode
        // ReSharper disable once RedundantIfElseBlock
        if ( Search ) {
            PromptUserSearch("aha take on me");

            new TestClass(VM.CachedResults.ToList()).Serialise(TestFile);
        } else {
            TestClass SR = TestFile.Deserialise<TestClass>().CatchNull();
            foreach ( SearchResult S in SR.Results ) {
                S.GenerateThumbnail();
                VM.CachedResults.Add(S);
            }
        }
        // ReSharper restore HeuristicUnreachableCode
#pragma warning restore CS0162

        KeyDown += ( _, E ) => {
            switch ( E.Key ) {
                case Key.E when E.IsDown:
                    Props.TimesDownloaded += 1;
                    break;
            }
        };
    }

    /// <summary>
    /// Occurs when the <see cref="System.Windows.Documents.Hyperlink"/> text is clicked on.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void Hyperlink_Path_Click( object Sender, RoutedEventArgs E ) {
        static Uri? GetUri( object Snd ) {
            if ( Snd is Hyperlink Hl ) {
                foreach( Inline Inline in Hl.Inlines ) {
                    if ( Inline is InlineUIContainer { Child: TextBlock Tb } ) {
                        if ( Tb.Text.GetUri().Out(out Uri U) ) {
                            return U;
                        }
                    }
                }
                return Hl.NavigateUri;
            }
            return null;
        }

        if ( GetUri(Sender) is { } U ) {
            string Url = U.GetRawPath();
            Debug.WriteLine($"Clicked on {Url}");
            if ( File.Exists(Url) ) {
                Debug.Write($"Invoking explorer.exe /select, \"{Url}\"");
                Process.Start("explorer.exe", $"/select, \"{Url}\"");
            } else if ( Directory.Exists(Url) ) {
                Debug.Write($"Invoking explorer.exe \"{Url}\"");
                Process.Start("explorer.exe", $"\"{Url}\"");
            } else {
                Process.Start(Url);
            }
        }
    }

    /// <summary>
    /// Occurs when the <see cref="System.Windows.Controls.Button.OnClick"/> <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void Hyperlink_PathEdit_Click( object Sender, RoutedEventArgs E ) {
        VistaFolderBrowserDialog VFBD = new VistaFolderBrowserDialog {
            Description = "Select a path to save downloaded songs in",
            SelectedPath = VM.SaveFolderLocation.GetRawPath(),
            ShowNewFolderButton = true
        };
        
        if ( VFBD.ShowDialog() == true && VFBD.SelectedPath.TryGetDirectory(out DirectoryInfo Dir) ) {
            VM.SaveFolderLocation = new Uri(Dir.FullName);
        }
    }

    /// <summary>
    /// Occurs when the <see cref="System.Windows.Controls.Button.OnClick"/> <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void SearchButton_Click( object Sender, RoutedEventArgs E ) {
        string Query = VM.SearchQuery;
        if ( string.IsNullOrEmpty(Query) ) { return; }
        PromptUserSearch(Query);
    }
    
    /// <summary>
    /// Occurs when a web-based hyperlink raises the RequestNavigate <see langword="event"/>.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void WebHyperlink_RequestNavigate( object Sender, RequestNavigateEventArgs E ) {
        if (Sender is Hyperlink Hl ) {
            Process.Start(new ProcessStartInfo(Hl.NavigateUri.ToString()) { UseShellExecute = true });
            E.Handled = true;
        }
    }

    /// <summary>
    /// Occurs when the <see cref="WPFUI.Controls.CardAction.OnClick"/> <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void SearchResult_Click( object Sender, RoutedEventArgs E ) {
        if ( Sender is CardAction { TemplatedParent: ContentPresenter { Content: SearchResult SR } } ) {
            SR.State = SearchResultState.Downloading;
            Task.Run(() => {
                Thread.Sleep(3500); //Simulate expensive download work.
                Dispatcher.Invoke(() => SR.State = SearchResultState.Complete);
            });
        }
    }
}

/// <summary> Viewmodel for <see cref="SearchPage"/>. </summary>
public class SearchPage_ViewModel : ViewModel<SearchPage> {
    /// <summary> The save folder location. </summary>
    [ SuppressMessage("ReSharper", "ExceptionNotDocumented")][ SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
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
        Props.PropertyChangedEventHandler += E => {
            if ( E.PropertyName == nameof( Props.TimesDownloaded ) ) {
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