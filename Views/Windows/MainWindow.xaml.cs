#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;

using DownTube.Engine;

using MVVMUtils;

using WPFUI.Background;
using WPFUI.Controls;

#endregion

namespace DownTube.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : IView<MainWindow_ViewModel> {
    //const string assetsPath = "pack://application:,,,/Assets/";

    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static MainWindow Instance { get; private set; } = null!;

    /// <summary>
    /// Initialises a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow() {
        //Props.Write();
        Debug.WriteLine($"FFmpeg path: {Props.FFmpegPath}");
        Debug.WriteLine($"YoutubeDL path: {Props.YoutubeDLPath}");
        Debug.WriteLine($"OutputFolder path: {Props.OutputFolder}");
        Debug.WriteLine($"Times downloaded: {Props.TimesDownloaded}");
        Debug.WriteLine($"Ignored versions: {Props.IgnoredVersions.Count}");
        foreach ( Version V in Props.IgnoredVersions ) {
            Debug.WriteLine($"\t: {V}");
        }
        //Debugger.Break();

        AppDomain.CurrentDomain.UnhandledException += ( _, E )=> {
            Debug.WriteLine(E.ExceptionObject, "EXCEPTION");
        };

        if ( Mica.IsSupported() ) {
            Debug.WriteLine("System supports Mica.");
            // ReSharper disable once ExceptionNotDocumentedOptional
            Mica.Apply(this);
        } else {
            Debug.WriteLine("System does not support Mica.");
        }

        InitializeComponent();

        VM = new MainWindow_ViewModel();
        DataContext = VM;

        InitialiseNavigation();

        VM.Setup(this);

        Instance = this;

        UpdateChecker.CheckForUpdates(true, Res => {
            if ( Res.HasUpdate) {
                Debug.WriteLine($"Update was found! ({Res.Current} -> {Res.Newest})");
                if ( Props.IgnoredVersions.Contains(Res.Newest) ) {
                    Debug.WriteLine("Update was ignored by user.");
                    return;
                }
                Dispatcher.Invoke(() => {
                    UpdateWindow UW = new UpdateWindow();
                    UW.Show();
                    Hide();
                });
            } else {
                Debug.WriteLine("Program is up-to-date.");

                if ( Props.LastCheckDate.Value is null ) {
                    UpdateWindow.UpdateLastCheckedDate();
                }
            }
        });
    }

    /// <summary> Initialises frame navigation in the window. </summary>
    void InitialiseNavigation() {
        RootNavigation.Frame = RootFrame;
        RootNavigation.Navigated += RootNavigation_Navigated;

        void ForceNavigate( object? Sender, EventArgs _ ) { //An issue with data binding the 'Items' property of a NavigationFluent stops it from selecting the first item automatically. To resolve this, we check every time a layout update occurs if there are any items, and if so we navigate to the first found item and stop checking afterwards.
            if ( RootNavigation.Items.Count > 0 ) {
                RootNavigation.Navigate(RootNavigation.Items[0].Tag);
                RootNavigation.LayoutUpdated -= ForceNavigate;
            }
        }

        RootNavigation.LayoutUpdated += ForceNavigate;

        //RootNavigation.Navigate("SearchPage", true);
    }

    /// <summary>
    /// Handles the OnNavigate event of the <see cref="INavigation"/> control.
    /// </summary>
    /// <param name="Sender">The source of the event.</param>
    /// <param name="E">The name of the page that was navigated to.</param>
    static void RootNavigation_Navigated( object Sender, RoutedEventArgs E ) {
        if ( Sender is NavigationFluent NF ) {
            Debug.WriteLine($"Navigated to {NF.PageNow}!");
        }
    }

    /// <summary>
    /// Handles the Click event of the RootDialog control.
    /// </summary>
    /// <param name="Sender">The source of the event.</param>
    /// <param name="E">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    void RootDialog_Click( object Sender, RoutedEventArgs E ) {
        Debug.WriteLine("Root dialog action button was clicked!");
    }

    /// <summary>
    /// Handles the RightButtonClick event of the RootDialog control.
    /// </summary>
    /// <param name="Sender">The source of the event.</param>
    /// <param name="E">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
    void RootDialog_RightButtonClick( object Sender, RoutedEventArgs E ) {
        Debug.WriteLine("Root dialog custom right button was clicked!");

        RootDialog.Show = false;
    }

    /// <inheritdoc />
    public MainWindow_ViewModel VM { get; }
}