using System.Windows;

namespace DownTube.Views.Controls;
/// <summary>
/// Interaction logic for UtilityDownloaderWindow_FFmpegLegal.xaml
/// </summary>
public partial class UtilityDownloaderWindow_YoutubeDLLegal {
    public UtilityDownloaderWindow_YoutubeDLLegal() {
        InitializeComponent();
    }

    /// <summary>
    /// Navigates to the website using the user's default browser.
    /// </summary>
    /// <param name="URL">The URL.</param>
    public static void NavigateToWebsite( string URL ) => _ = Process.Start(new ProcessStartInfo(URL) { UseShellExecute = true });

    void YoutubeDLLicenseHyperlink_Click( object Sender, RoutedEventArgs E ) => NavigateToWebsite("https://unlicense.org/");

    void YoutubeDLLegalHyperlink_Click( object Sender, RoutedEventArgs E ) => NavigateToWebsite("https://github.com/ytdl-org/youtube-dl/blob/master/LICENSE");
}
