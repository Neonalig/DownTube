using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

using DownTube.Views.Pages;

using WPFUI.Background;
using WPFUI.Common;
using WPFUI.Controls;

using YoutubeSnoop;
using YoutubeSnoop.Api.Entities;
using YoutubeSnoop.Enums;

namespace DownTube;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow {
    //const string assetsPath = "pack://application:,,,/Assets/";

    public MainWindow() {
        AppDomain.CurrentDomain.UnhandledException += ( _, E )=> {
            Debug.WriteLine(E.ExceptionObject, "EXCEPTION");
        };

        if ( Mica.IsSupported() ) {
            Debug.WriteLine("System supports Mica.");
            Mica.Apply(this);
        } else {
            Debug.WriteLine("System does not support Mica.");
        }

        InitializeComponent();
        InitializeNavigation();

        SolidColorBrush SCB = (SolidColorBrush)Background;
        MainBorder.Background = SCB.WithA(242);
        const byte Contrast = 8;
        MainBorder.BorderBrush = SCB.With(222, (byte)(SCB.Color.R - Contrast), (byte)(SCB.Color.G - Contrast), (byte)(SCB.Color.B - Contrast));
        Background = new SolidColorBrush(new Color { R = 0, G = 0, B = 0, A = 0 });
    }

    void InitializeNavigation() {
        RootNavigation.Frame = RootFrame;
        RootNavigation.Items = new ObservableCollection<NavItem> {
            new() { Name = "Music Downloads", Tag = "MusicDownloadPage", Type = typeof(MusicDownloadPage) }
            //new() { ImageUri = assetsPath + "microsoft-shell-desktop.ico", Name = "Dashboard", Tag = "dashboard", Type = typeof(Pages.Dashboard)},
            //new() { ImageUri = assetsPath + "microsoft-shell-accessibility.ico", Name = "Forms", Tag = "forms", Type = typeof(Pages.Forms)},
            //new() { ImageUri = assetsPath + "microsoft-shell-settings.ico", Name = "Controls", Tag = "controls", Type = typeof(Pages.Controls)},
            //new() { ImageUri = assetsPath + "microsoft-shell-workspace.ico", Name = "Actions", Tag = "actions", Type = typeof(Pages.Actions)},
            //new() { ImageUri = assetsPath + "microsoft-shell-colors.ico", Name = "Colors", Tag = "colors", Type = typeof(Pages.Colors)},
            //new() { ImageUri = assetsPath + "microsoft-shell-gallery.ico", Name = "Icons", Tag = "icons", Type = typeof(Pages.Icons)},
            //new() { ImageUri = assetsPath + "microsoft-shell-monitor.ico", Name = "Windows", Tag = "windows", Type = typeof(Pages.WindowsPage)}
        };

        RootNavigation.Footer = new ObservableCollection<NavItem>
        {
            //new() { Icon = Common.Icon.Accessibility48, Name = "Settings", Tag = "settings", Type = typeof(Pages.Dashboard)}
        };

        RootNavigation.Navigated += OnNavigate;
        RootNavigation.Navigate("MusicDownloadPage");
    }

    void RootDialog_Click( object Sender, RoutedEventArgs E ) {
        Debug.WriteLine("Root dialog action button was clicked!");
    }

    void RootDialog_RightButtonClick( object Sender, RoutedEventArgs E ) {
        Debug.WriteLine("Root dialog custom right button was clicked!");

        RootDialog.Show = false;
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    void OnNavigate( INavigation Sender, string Page ) {
        Debug.WriteLine("Page now is: " + Page);
    }
}