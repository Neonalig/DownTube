#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;

using DownTube.Converters;
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

        KeyDown += ( _, E ) => {
            switch ( E.Key ) {
                case Key.E when E.IsDown:
                    Props.TimesDownloaded.Value += 1;
                    break;
            }
        };
    }

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
        
        if ( VFBD.ShowDialog() == true && VFBD.SelectedPath.GetDirectory().Out(out DirectoryInfo Dir) ) {
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
            Hl.NavigateUri.NavigateToWebsite();
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