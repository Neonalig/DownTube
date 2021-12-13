using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

using DownTube.Views.Pages;

using MVVMUtils;

using WPFUI.Background;
using WPFUI.Common;
using WPFUI.Controls;

namespace DownTube.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : IView<MainWindow_ViewModel> {
    //const string assetsPath = "pack://application:,,,/Assets/";

    /// <summary>
    /// Initialises a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
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
        RootNavigation.Items = new ObservableCollection<NavItem> {
            new() { Name = "Music Downloads", Tag = "MusicDownloadPage", Type = typeof(MusicDownloadPage) }
            //new() { ImageUri = assetsPath + "microsoft-shell-desktop.ico", Name = "Dashboard", Tag = "dashboard", Type = typeof(Pages.Dashboard)},
            //new() { ImageUri = assetsPath + "microsoft-shell-accessibility.ico", Name = "Forms", Tag = "forms", Type = typeof(Pages.Forms)},
            //new() { ImageUri = assetsPath + "microsoft-shell-settings.ico", Name = "Controls", Tag = "controls", Type = typeof(Pages.Controls)},
            //new() { ImageUri = assetsPath + "microsoft-shell-workspace.ico", Name = "Actions", Tag = "actions", Type = typeof(Pages.Actions)},
            //new() { ImageUri = assetsPath + "microsoft-shell-colours.ico", Name = "Colors", Tag = "colours", Type = typeof(Pages.Colors)},
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

    /// <summary>
    /// Handles the OnNavigate event of the <see cref="INavigation"/> control.
    /// </summary>
    /// <param name="Sender">The source of the event.</param>
    /// <param name="Page">The name of the page that was navigated to.</param>
    // ReSharper disable once MemberCanBeMadeStatic.Local
    void OnNavigate( INavigation Sender, string Page ) {
        Debug.WriteLine("Page now is: " + Page);
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
}