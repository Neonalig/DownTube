using System.Windows;

namespace DownTube.Views.Controls;
/// <summary>
/// Interaction logic for UtilityDownloaderWindow_FFmpegLegal.xaml
/// </summary>
public partial class UtilityDownloaderWindow_YoutubeDLLegal {
    public UtilityDownloaderWindow_YoutubeDLLegal() {
        InitializeComponent();
    }

    void YoutubeDLLicenseHyperlink_Click( object Sender, RoutedEventArgs E ) => "https://unlicense.org/".NavigateToWebsite();

    void YoutubeDLLegalHyperlink_Click( object Sender, RoutedEventArgs E ) => "https://github.com/ytdl-org/youtube-dl/blob/master/LICENSE".NavigateToWebsite();
}
