using System.Windows;

namespace DownTube.Views.Controls;
/// <summary>
/// Interaction logic for UtilityDownloaderWindow_FFmpegLegal.xaml
/// </summary>
public partial class UtilityDownloaderWindow_FFmpegLegal {
    public UtilityDownloaderWindow_FFmpegLegal() {
        InitializeComponent();
    }

    /// <summary>
    /// Navigates to the website using the user's default browser.
    /// </summary>
    /// <param name="URL">The URL.</param>
    public static void NavigateToWebsite( string URL ) => _ = Process.Start(new ProcessStartInfo(URL) { UseShellExecute = true });

    void FFmpegLicenseHyperlink_Click( object Sender, RoutedEventArgs E ) => NavigateToWebsite("https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html");

    void FFmpegLegalHyperlink_Click( object Sender, RoutedEventArgs E ) => NavigateToWebsite("https://www.ffmpeg.org/legal.html");
}
