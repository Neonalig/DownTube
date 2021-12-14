using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using DownTube.Engine;

using MVVMUtils;

using Ookii.Dialogs.Wpf;

using WPFUI.Controls;

using YoutubeSnoop;

using Button = System.Windows.Controls.Button;
using Hyperlink = System.Windows.Documents.Hyperlink;

namespace DownTube.Views.Pages;

/// <summary>
/// Interaction logic for MusicDownloadPage.xaml
/// </summary>
public partial class MusicDownloadPage : IView<MusicDownloadPage_ViewModel> {
    /// <inheritdoc />
    public MusicDownloadPage_ViewModel VM { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="MusicDownloadPage"/> class.
    /// </summary>
    public MusicDownloadPage() {
        InitializeComponent();
        // ReSharper disable once ExceptionNotDocumented
        VM = DataContext.Cast<MusicDownloadPage_ViewModel>();
        InitializeAsync();
    }

    // ReSharper disable once NotAccessedPositionalProperty.Local
    /// <summary> Class made for testing purposes. </summary>
    record struct TestClass( List<SearchResult> Results );

    //[JsonObject] public class TestClass {
    //    [JsonProperty] public List<SearchResult> Results { get; set; }

    //    public TestClass( List<SearchResult> Res ) => Results = Res;

    //    public TestClass() : this(new List<SearchResult>()) { }
    //}

    /// <summary>
    /// Invokes a search with the given <paramref name="Query"/> to the YouTube V3 API, populating <see cref="MusicDownloadPage_ViewModel.CachedResults"/> with the returned results.
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

        FileInfo TestFile = FileSystemInfoExtensions.Desktop.CreateSubfile("testfile.json")!;

        const bool Search = false;
#pragma warning disable CS0162
        // ReSharper disable HeuristicUnreachableCode
        if ( Search ) {
            PromptUserSearch("aha take on me");

            new TestClass(VM.CachedResults.ToList()).Serialise(TestFile);
        }/* else {
            TestClass SR = TestFile.Deserialise<TestClass>().CatchNull();
            foreach ( SearchResult S in SR.Results ) {
                S.GenerateThumbnail();
                VM.CachedResults.Add(S);
            }
        }*/
        // ReSharper restore HeuristicUnreachableCode
#pragma warning restore CS0162
        //First.ThrowIfNull();

        //VM.CachedResults.Add(new SearchResult("aka On Me - a-ha - Brooklyn Duo at Carnegie Hall", "Brooklyn Duo", "https://i.ytimg.com/vi/DHbLuIxw3y4/default.jpg", "https://www.google.com"));
    }

    //void ActionCardIcons_Click( object Sender, RoutedEventArgs E ) => (Application.Current.MainWindow as MainWindow)?.RootNavigation.Navigate("icons");

    //void ActionCardColours_Click( object Sender, RoutedEventArgs E ) => (Application.Current.MainWindow as MainWindow)?.RootNavigation.Navigate("colours");

    //void ActionCardControls_Click( object Sender, RoutedEventArgs E ) => (Application.Current.MainWindow as MainWindow)?.RootNavigation.Navigate("controls");

    /// <summary>
    /// Occurs when the <see cref="Hyperlink"/> text is clicked on.
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
    /// Occurs when the <see cref="Button.OnClick"/> <see langword="event"/> is raised.
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
    /// Occurs when the <see cref="Button.OnClick"/> <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void SearchButton_Click( object Sender, RoutedEventArgs E ) {
        string Query = VM.SearchQuery;
        if ( string.IsNullOrEmpty(Query) ) { return; }
        PromptUserSearch(Query);
    }

    /// <summary>
    /// Occurs when the <see cref="CardAction.OnClick"/> <see langword="event"/> is raised.
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

/// <summary> Viewmodel for <see cref="MusicDownloadPage"/>. </summary>
public class MusicDownloadPage_ViewModel : ViewModel<MusicDownloadPage> {
    /// <summary> The save folder location. </summary>
    public Uri SaveFolderLocation { get; set; } = FileSystemInfoExtensions.Desktop.GetUri();

    /// <summary> The current user search query. </summary>
    public string SearchQuery { get; set; } = string.Empty;

    /// <summary>
    /// The collection of cached youtube search results.
    /// </summary>
    //public ObservableCollection<SearchResult> CachedResults { get; } = new ObservableCollection<SearchResult>();
    public ObservableCollection<SearchResult> CachedResults { get; } = new ObservableCollection<SearchResult> {
        new SearchResult("a-ha - Take On Me (Official Video) [Remastered in 4K]", "a-ha", "https://i.ytimg.com/vi/djV11Xbc914/default.jpg", "https://www.youtube.com/watch?v=djV11Xbc914"),
        new SearchResult("a-ha - Take On Me (Live From MTV Unplugged)", "ahaVEVO", "https://i.ytimg.com/vi/-xKM3mGt2pE/default.jpg", "https://www.youtube.com/watch?v=-xKM3mGt2pE"),
        new SearchResult("a-ha - Take On Me (Lyrics)", "SuperbLyrics", "https://i.ytimg.com/vi/Ktb44YLL8Nw/default.jpg", "https://www.youtube.com/watch?v=Ktb44YLL8Nw"),
        new SearchResult("a-ha - Take On Me (Live 8 2005)", "Live 8", "https://i.ytimg.com/vi/3meKlaJL3qo/default.jpg", "https://www.youtube.com/watch?v=3meKlaJL3qo"),
        new SearchResult("Take on Me (2016 Remaster)", "A-ha - Topic", "https://i.ytimg.com/vi/NaQ083rNUwc/default.jpg", "https://www.youtube.com/watch?v=NaQ083rNUwc"),
        new SearchResult(@"Weezer - Take On Me (Official Video)", @"weezer", "https://i.ytimg.com/vi/f7RwDnZI7Tw/default.jpg", "https://www.youtube.com/watch?v=f7RwDnZI7Tw"),
        new SearchResult("Take on Me", "A-ha - Topic", "https://i.ytimg.com/vi/MIgK3zOk0zg/default.jpg", "https://www.youtube.com/watch?v=MIgK3zOk0zg"),
        new SearchResult("a-ha - Take On Me (Radio 2 In Concert)", "BBC Radio 2", @"https://i.ytimg.com/vi/IHDCMQjNimg/default.jpg", @"https://www.youtube.com/watch?v=IHDCMQjNimg"),
        new SearchResult("A-ha - Take On Me, Countdown 1985", "A-ha South America Play", "https://i.ytimg.com/vi/NY2LHIMUcgU/default.jpg", "https://www.youtube.com/watch?v=NY2LHIMUcgU"),
        new SearchResult("[A-ha FR] a-ha Take On Me Live BBC One 09-11-2018", "A-ha France", "https://i.ytimg.com/vi/2SHnAj0yMFE/default.jpg", "https://www.youtube.com/watch?v=2SHnAj0yMFE"),
        new SearchResult(@"a-ha - Take On Me (Dimitri Vegas  Like Mike vs Ummet Ozcan Remix) [...", "Central Bass Boost", "https://i.ytimg.com/vi/CNWgQxrcZ4k/default.jpg", "https://www.youtube.com/watch?v=CNWgQxrcZ4k"),
        new SearchResult(@"A-HA feat KYGO - TAKE ON ME - EXCLUSIVE - The 2015 Nobel Peace Priz...", "Nobel Peace Prize Concert", "https://i.ytimg.com/vi/0FJFKCnYtN4/default.jpg", "https://www.youtube.com/watch?v=0FJFKCnYtN4"),
        new SearchResult("A-ha - Take on me (Live Afterglow 360) - 2016", @"German Irazabal", "https://i.ytimg.com/vi/UHAXKQtaeG4/default.jpg", "https://www.youtube.com/watch?v=UHAXKQtaeG4"),
        new SearchResult("A1 - Take on Me", "A1VEVO", "https://i.ytimg.com/vi/EbCsIKI6HEU/default.jpg", "https://www.youtube.com/watch?v=EbCsIKI6HEU"),
        new SearchResult("A-HA Take On Me 1984 Version", "MagneticNorth71", @"https://i.ytimg.com/vi/liq-seNVvrM/default.jpg", @"https://www.youtube.com/watch?v=liq-seNVvrM"),
        new SearchResult(@"The Last of Us 2 - Ellie &quot;Take on Me&quot; Cover Song", "Boss Fight Database", "https://i.ytimg.com/vi/NKeU1twQYX4/default.jpg", "https://www.youtube.com/watch?v=NKeU1twQYX4"),
        new SearchResult("a ha - Take On Me (Official Video Music) 4K Remastered", "Daniel CLASSIC Remastered AI", "https://i.ytimg.com/vi/AYjpwHQ66ts/default.jpg", "https://www.youtube.com/watch?v=AYjpwHQ66ts"),
        new SearchResult("a-Ha - Take On Me [lyrics]", @"Lyricosaurus", "https://i.ytimg.com/vi/irljBY9J5ig/default.jpg", "https://www.youtube.com/watch?v=irljBY9J5ig"),
        new SearchResult("a-ha - The Sun Always Shines on T.V. (Official Video)", "a-ha", "https://i.ytimg.com/vi/a3ir9HC9vYg/default.jpg", "https://www.youtube.com/watch?v=a3ir9HC9vYg"),
        new SearchResult("Take On Me - a-ha - Brooklyn Duo at Carnegie Hall", "Brooklyn Duo", "https://i.ytimg.com/vi/DHbLuIxw3y4/default.jpg", "https://www.youtube.com/watch?v=DHbLuIxw3y4")
    };
}