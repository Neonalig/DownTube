using System.Windows;

using MVVMUtils;

namespace DownTube.Views.Windows;

/// <summary>
/// Interaction logic for UtilityDownloaderWindow.xaml
/// </summary>
public partial class UtilityDownloaderWindow : IView<UtilityDownloaderWindow_ViewModel> {
    public UtilityDownloaderWindow() {
        InitializeComponent();

        VM = new UtilityDownloaderWindow_ViewModel();
        DataContext = VM;

        VM.Setup(this);
    }

    /// <inheritdoc />
    public UtilityDownloaderWindow_ViewModel VM { get; }

    void ReturnToMain() {
        MainWindow.Instance.Show();
        Close();
    }

    void CancelButton_Click( object Sender, RoutedEventArgs E ) => ReturnToMain();

    void InstallButton_Click( object Sender, RoutedEventArgs E ) => VM.UpdateDialogVisible = true;

    void CancelInstall_OnClick( object Sender, RoutedEventArgs E ) => VM.AwaitingRelease = null;

    void AutomaticInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( VM.AwaitingRelease is not null ) {
            Debug.WriteLine("Unexpected scenario. AwaitingRelease should be null for the button to be visible in the first place.");
            return;
        }

        //TODO: Remove below
        KnownUtilityRelease KUR = new KnownUtilityRelease("youtube-dl 2021.12.17", null!, null);

        static KnownUtilityDownloadMatchType GetMatch( string S ) =>
            S == "youtube-dl.exe"
                ? KnownUtilityDownloadMatchType.Supported
                : S.EndsWith(".exe")
                  || S.EndsWith(".zip")
                  || S.EndsWith(".7z")
                  || S.EndsWith(".tar")
                  || S.EndsWith(".gz")
                  //|| S.EndsWith(".tar.gz")
                  || S.EndsWith(".rar")
                    ? KnownUtilityDownloadMatchType.Recommended
                    : KnownUtilityDownloadMatchType.Unknown;
        KUR.Downloads.AddRange(
            new[] {
                new KnownUtilityDownload("MD5SUMS", KUR, null!),
                new KnownUtilityDownload("SHA1SUMS", KUR, null!),
                new KnownUtilityDownload("SHA2-256SUMS", KUR, null!),
                new KnownUtilityDownload("SHA2-512SUMS", KUR, null!),
                new KnownUtilityDownload("youtube-dl", KUR, null!),
                new KnownUtilityDownload("youtube-dl-2021.12.17.tar.gz", KUR, null!),
                new KnownUtilityDownload("youtube-dl-2021.12.17.tar.gz.sig", KUR, null!),
                new KnownUtilityDownload("youtube-dl.exe", KUR, null!),
                new KnownUtilityDownload("youtube-dl.exe.sig", KUR, null!),
                new KnownUtilityDownload("youtube-dl.sig", KUR, null!),
                new KnownUtilityDownload("Source Code.zip", KUR, null!),
                new KnownUtilityDownload("Source Code.tar.gz", KUR, null!)
            }.Passthrough(KUD => KUD.Match = GetMatch(KUD.FileName.ToLowerInvariant()))
        );

        foreach ( KnownUtilityDownload Down in KUR ) {
            Debug.WriteLine($"KUR has {Down.FileName} as an option.");
        }

        VM.AwaitingRelease = KUR;
    }

    void StartInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( VM.AwaitingRelease is { ChosenAsset: { } CA } AR ) {
            Debug.WriteLine($"{AR} will download {CA}");
        } else {
            Debug.WriteLine("No asset chosen. Ignored.");
        }
    }
}
