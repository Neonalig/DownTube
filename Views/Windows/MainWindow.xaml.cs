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
using System.Windows.Media;
using System.Windows.Media.Effects;

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
    /// Initialises a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow() {
        // ReSharper disable ExceptionNotDocumented
        // ReSharper disable ExceptionNotDocumentedOptional
        Debug.WriteLine($"Props YTDL: {Props.YoutubeDLPath?.FullName}");
        // ReSharper restore ExceptionNotDocumentedOptional
        // ReSharper restore ExceptionNotDocumented

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

        SolidColorBrush SCB = (SolidColorBrush)Background;
        MainBorder.Background = SCB.WithA(242);
        const byte Contrast = 8;
        MainBorder.BorderBrush = SCB.With(222, (byte)(SCB.Color.R - Contrast), (byte)(SCB.Color.G - Contrast), (byte)(SCB.Color.B - Contrast));
        Background = new SolidColorBrush(new Color { R = 0, G = 0, B = 0, A = 0 });
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
public class MainWindow_ViewModel : ViewModel<MainWindow> {
    /// <summary>
    /// Gets or sets the border margin.
    /// </summary>
    /// <value>
    /// The border margin.
    /// </value>
    public Thickness BorderMargin { get; set; } = _BorderMargin_Normal;

    /// <summary> (<see langword="const"/>) Border margin. </summary>
    static readonly Thickness
        _BorderMargin_Normal = new Thickness(0, 0, 7, 7),
        _BorderMargin_Maximised = new Thickness(0, 0, 0, 0);

    /// <summary>
    /// Gets or sets the border effect.
    /// </summary>
    /// <value>
    /// The border effect.
    /// </value>
    public Effect? BorderEffect { get; set; } = _BorderEffect_Normal;

    /// <summary> (<see langword="const"/>) Border effect. </summary>
    static readonly Effect?
        _BorderEffect_Normal = new DropShadowEffect { Opacity = 0.7 },
        _BorderEffect_Maximised = null;

    /// <summary>
    /// Gets or sets the border thickness.
    /// </summary>
    /// <value>
    /// The border thickness.
    /// </value>
    public double BorderThickness { get; set; } = _BorderThickness_Normal;

    // ReSharper disable InconsistentNaming
    /// <summary> (<see langword="const"/>) Border thickness. </summary>
    const double
        _BorderThickness_Normal = 1d,
        _BorderThickness_Maximised = 0d;
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// The window state.
    /// </summary>
    WindowState _WindowState = WindowState.Normal;

    /// <summary>
    /// Gets or sets the state of the window.
    /// </summary>
    /// <value>
    /// The state of the window.
    /// </value>
    public WindowState WindowState {
        get => _WindowState;
        set {
            if ( _WindowState != value ) {
                SetProperty(ref _WindowState, value);
                switch ( value ) {
                    case WindowState.Maximized:
                        BorderMargin = _BorderMargin_Maximised;
                        BorderEffect = _BorderEffect_Maximised;
                        BorderThickness = _BorderThickness_Maximised;
                        break;
                    default:
                        BorderMargin = _BorderMargin_Normal;
                        BorderEffect = _BorderEffect_Normal;
                        BorderThickness = _BorderThickness_Normal;
                        break;
                }

            }
        }
    }

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
}