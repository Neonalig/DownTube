#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using DownTube.Engine;
using DownTube.Views.Pages;

using MVVMUtils;

using WPFUI.Background;
using WPFUI.Common;
using WPFUI.Controls;

using Icon = WPFUI.Common.Icon;

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
        //Debug.WriteLine($"Props YTDL: {Props.YoutubeDLPath?.FullName}");

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

        UpdateChecker.CheckForUpdates(Res => {
            if ( Res.HasUpdate ) {
                Debug.Write($"Update was found! ({Res.Current} -> {Res.Newest})");
                Dispatcher.Invoke(() => {
                    UpdateWindow UW = new UpdateWindow();
                    UW.Show();
                    Hide();
                });
            } else {
                Debug.WriteLine("Program is up-to-date.");
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

/// <summary>
/// The viewmodel for <see cref="MainWindow"/>.
/// </summary>
public class MainWindow_ViewModel : Window_ViewModel<MainWindow> {

    /// <summary>
    /// Gets the collection of navigation items to display on the sidebar.
    /// </summary>
    /// <value>
    /// The collection of available sidebar navigation items.
    /// </value>
    public ObservableCollection<NavItem> NavItems { get; set; } = new ObservableCollection<NavItem> {
        new NavItem {
            Name = "Search",
            Tag = nameof(SearchPage),
            Type = typeof(SearchPage),
            Icon = Icon.Search48
        }
    };

    /// <summary>
    /// Gets the collection of navigation items to display on the footer.
    /// </summary>
    /// <value>
    /// The collection of available footer navigation items.
    /// </value>
    public ObservableCollection<NavItem> NavFooterItems { get; set; } = new ObservableCollection<NavItem> {
        new NavItem {
            Name = "Settings",
            Tag = nameof(SettingsPage),
            Type = typeof(SettingsPage),
            Icon = Icon.Settings48
        }
    };

    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;
}