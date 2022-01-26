using System.Windows;

namespace DownTube.Views.Controls;
/// <summary>
/// Interaction logic for UtilityDownloaderWindow_FFmpegLegal.xaml
/// </summary>
public partial class UtilityDownloaderWindow_FFmpegLegal {
    public UtilityDownloaderWindow_FFmpegLegal() {
        InitializeComponent();
    }

    void FFmpegLicenseHyperlink_Click( object Sender, RoutedEventArgs E ) => "https://www.gnu.org/licenses/old-licenses/lgpl-2.1.html".NavigateToWebsite();

    void FFmpegLegalHyperlink_Click( object Sender, RoutedEventArgs E ) => "https://www.ffmpeg.org/legal.html".NavigateToWebsite();
}
