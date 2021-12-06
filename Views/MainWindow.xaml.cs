using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

using WPFUI.Background;
using WPFUI.Common;
using WPFUI.Controls;

namespace DownTube {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        //const string assetsPath = "pack://application:,,,/Assets/";

        public MainWindow() {
            if ( Mica.IsSupported() ) {
                Debug.WriteLine("System supports Mica.");
                Mica.Apply(this);
            } else {
                Debug.WriteLine("System does not support Mica.");
            }

            InitializeComponent();
            InitializeNavigation();

            MainBorder.Background = WithA((SolidColorBrush)Background, 242);
            Background = new SolidColorBrush(new Color { R = 0, G = 0, B = 0, A = 0 });
        }

        internal static Color WithA( Color Col, byte A ) => new Color {  R = Col.R, G = Col.G, B = Col.B, A = A };

        internal static SolidColorBrush WithA( SolidColorBrush Col, byte A ) => new SolidColorBrush(WithA(Col.Color, A));

        void InitializeNavigation() {
            RootNavigation.Frame = RootFrame;
            RootNavigation.Items = new ObservableCollection<NavItem> {
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
            //RootNavigation.Navigate("dashboard");
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
}