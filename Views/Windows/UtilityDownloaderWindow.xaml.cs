using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

using DownTube.DataTypes;
using DownTube.Engine;

using MVVMUtils;

namespace DownTube.Views.Windows;

/// <summary>
/// Interaction logic for UtilityDownloaderWindow.xaml
/// </summary>
public partial class UtilityDownloaderWindow : IView<UtilityDownloaderWindow_ViewModel> {
    /// <summary>
    /// Whether any <see cref="UtilityDownloaderWindow"/> instances are currently constructed.
    /// </summary>
    static bool _AnyCto = false;

    public UtilityDownloaderWindow() {
        _AnyCto = true;
        InitializeComponent();

        VM = new UtilityDownloaderWindow_ViewModel();
        DataContext = VM;

        VM.Setup(this);

        Closed += ( _, _ ) => _AnyCto = false;
    }

    ~UtilityDownloaderWindow() => _AnyCto = false;

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

    /// <summary>
    /// Occurs when the <see cref="System.Windows.Controls.Button.Click"/> <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    async void StartInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( VM.AwaitingRelease is { ChosenAsset: { } CA } ) {
            await BeginDownloadAsync(CA, new CancellationTokenSource());
        } else {
            Debug.WriteLine("No asset chosen. Ignored.");
        }
    }

    /// <summary>
    /// Attempts to open a <see cref="UtilityDownloaderWindow"/> for the given <see cref="DownloadUtilityType"/>.
    /// </summary>
    /// <param name="Type">The utility type.</param>
    /// <param name="UDW">The constructed window, or <see langword="null"/> if returning <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if no pre-existing utility downloader window is open and one is successfully made; otherwise <see langword="false"/>.</returns>
    public static bool TryOpen( DownloadUtilityType Type, [NotNullWhen(true)] out UtilityDownloaderWindow? UDW ) {
        if ( _AnyCto ) {
            UDW = null;
            return false;
        }

        UDW = new UtilityDownloaderWindow {
            VM = {
                Utility = Type
            }
        };
        return true;
    }

    public async Task BeginDownloadAsync( KnownUtilityDownload Download, CancellationTokenSource CTS ) {
        VM.InstallProgress = -1;
        Debug.WriteLine($"Will download {Download}...");
        VM.AwaitingRelease = null;

        string DownloadUrl, FileName;
        if ( Args.Offline ) {
            Debug.WriteLine("Mocking download (in offline mode).", "WARNING");
            switch ( VM.Utility ) {
                case DownloadUtilityType.FFmpeg:
                    DownloadUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-n5.0-latest-win64-lgpl-shared-5.0.zip";
                    FileName = "ffmpeg-n5.0-latest-win64-lgpl-shared-5.0.zip";
                    break;
                case DownloadUtilityType.YoutubeDL:
                    DownloadUrl = "https://github.com/ytdl-org/youtube-dl/releases/download/2021.12.17/youtube-dl.exe";
                    FileName = "youtube-dl.exe";
                    break;
                default:
                    throw new EnumValueOutOfRangeException<DownloadUtilityType>(VM.Utility);
            }
        } else {
            DownloadUrl = Download.Asset.BrowserDownloadUrl;
            FileName = Download.FileName;
        }

        DirectoryInfo DestDir = FileSystemInfoExtensions.Downloads;
        FileInfo Dest = DestDir.CreateSubfile(FileName, false);

        Debug.WriteLine($"Download starting {DownloadUrl} --> {Dest.FullName}");
        await DownloadRequest.DownloadReleaseAsset(
            DownloadUrl,
            FileName,
            DestDir,
            _ => {
                Debug.WriteLine("Download started.");
            },
            ( _, P ) => {
                Debug.WriteLine($"Download: {P:P2}");
                VM.InstallProgress = P;
            },
            ( _, Fl ) => {
                Debug.WriteLine($"Download finished. {Fl.FullName}");
                //Process.Start("explorer.exe", $"/select,\"{Fl.FullName}\"");
                VM.InstallProgress = -1;

                switch ( VM.Utility ) {
                    case DownloadUtilityType.FFmpeg:
                        InstallFFmpeg(Download, Dest);
                        break;
                    case DownloadUtilityType.YoutubeDL:
                        InstallYoutubeDL(Download, Dest);
                        break;
                    default:
                        Debug.WriteLine("Installing other utilities not yet supported.", "WARNING");
                        break;
                }
                //Debug.WriteLine("Extracting...");
                //DirectoryInfo Ext = Fl.Extract(true);
                //Debug.WriteLine($"Extracted to {Ext.FullName}");
            },
            16384,
            false,
            false,
            CTS);
        VM.InstallProgress = 1;
        //DR.
    }

    /// <summary>
    /// Installs the ffmpeg utility.
    /// </summary>
    /// <param name="KUD">The download reference.</param>
    /// <param name="File">The downloaded file.</param>
    void InstallFFmpeg( KnownUtilityDownload KUD, FileInfo File ) {
        //TODO: Check setting is assigned before actually downloading. If the setting is assigned, return immediately as there is no reason to download the utility again.
        VM.InstallProgress = -1;

        if ( KUD.Match == KnownUtilityDownloadMatchType.Supported
            || File.Extension.EqualsAny(StringComparison.InvariantCultureIgnoreCase, ".zip", ".7z", ".rar", ".tar", ".gz", ".tar.gz")
            ) {
            InstallViaBinFolder(File);
        } else if (File.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase)) {
            Debug.WriteLine("This method of installation (direct/exe) for ffmpeg is not expected, but is supported to a degree. This may not work as other dependencies (DLLs, ffprobe, etc.) may not be found.", "WARNING");
            InstallViaDirectMethod(File);
        } else {
            Debug.WriteLine($"This method of installation (unknown) for ffmpeg is invalid. Unsure how to handle '{File.Name}' files, so installation is directed to user.", "WARNING");
            InstallUnknown(File);
        }
        VM.InstallProgress = 1.0;
        Debug.WriteLine("\t*. Complete!");
    }

    /// <summary>
    /// Installs the youtube-dl utility.
    /// </summary>
    /// <param name="KUD">The download reference.</param>
    /// <param name="File">The downloaded file.</param>
    void InstallYoutubeDL( KnownUtilityDownload KUD, FileInfo File ) {
        //TODO: Check setting is assigned before actually downloading. If the setting is assigned, return immediately as there is no reason to download the utility again.
        VM.InstallProgress = -1;

        if ( KUD.Match == KnownUtilityDownloadMatchType.Supported || File.Extension.Equals(".exe", StringComparison.InvariantCultureIgnoreCase) ) {
            InstallViaDirectMethod(File);
        } else if ( File.Extension.EqualsAny(StringComparison.InvariantCultureIgnoreCase, ".zip", ".7z", ".rar", ".tar", ".gz", ".tar.gz")
                  ) {
            Debug.WriteLine("This method of installation (archive/bin) for youtube-dl is not expected, but is supported to a degree.", "WARNING");
            InstallViaBinFolder(File);
        } else {
            Debug.WriteLine($"This method of installation (unknown) for youtube-dl is invalid. Unsure how to handle '{File.Name}' files, so installation is directed to user.", "WARNING");
            InstallUnknown(File);
        }
        VM.InstallProgress = 1.0;
        Debug.WriteLine("\t*. Complete!");
    }

    void InstallViaBinFolder( FileInfo File ) {
        Debug.WriteLine("Installing via extraction/bin method.");

        //1. Extract to relative folder.
        DirectoryInfo FileExFolder = File.Extract(true);
        Debug.WriteLine($"1. Extracted {File} to {FileExFolder}");
        VM.InstallProgress = 0.2;

        //2. Find 'bin' folder within the extracted path.
        DirectoryInfo BinFolder = FileExFolder.GetDirectories("bin", SearchOption.AllDirectories).First();
        Debug.WriteLine($"2. Retrieved {BinFolder} from {FileExFolder}");
        VM.InstallProgress = 0.4;

        //3. Move 'bin' folder to new 'Utilities' folder in AppDir.
        BinFolder.SmartMoveTo(Path.Combine(FileSystemInfoExtensions.AppDir.FullName, "Utilities"));
        Debug.WriteLine($"3. Moved bin folder to {BinFolder}");
        VM.InstallProgress = 0.6;

        //4. Delete archive & archive extraction folder.
        File.Delete();
        FileExFolder.Delete(true);
        Debug.WriteLine("4. Deleted archive & relative extraction folder.");
        VM.InstallProgress = 0.8;

        //5. Assign the extracted 'ffmpeg.exe' path to the settings.
        Props.YoutubeDLPath.Value = BinFolder.CreateSubfile("youtube-dl.exe", false);
        Props.Save();
        Props.Write();
        Debug.WriteLine($"5. Changed related property & saved changes ( --> {Props.YoutubeDLPath} )");
        VM.InstallProgress = 1.0;
    }

    void InstallViaDirectMethod( FileInfo File ) {
        Debug.WriteLine("Installing via direct method.");
        //1. Move file to AppDir.
        FileInfo Dest = File.MoveTo(FileSystemInfoExtensions.AppDir.CreateSubdirectory("Utilities"));
        Debug.WriteLine($"1. Moved the file to {Dest}");
        VM.InstallProgress = 0.5;

        //2. Assign the 'ffmpeg.exe' path to the settings
        Props.YoutubeDLPath.Value = Dest;
        Props.Save();
        Props.Write();
        Debug.WriteLine($"2. Changed related property & saved changes ( --> {Props.YoutubeDLPath} )");
        VM.InstallProgress = 1.0;
    }

    void InstallUnknown( FileInfo File ) {
        Debug.WriteLine("Installing via unknown method.");
        _ = File.SelectInExplorer(out _);
        Debug.WriteLine("1. Opened in file explorer so that the user can manage installation themselves.");
        VM.InstallProgress = 1.0;
    }
}
