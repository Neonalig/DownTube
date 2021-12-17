using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;

using DownTube.Engine;

using MVVMUtils;

using Octokit;

namespace DownTube.Views.Windows;

/// <summary>
/// Interaction logic for UpdateWindow.xaml
/// </summary>
public partial class UpdateWindow : IView<UpdateWindow_ViewModel> {
    public UpdateWindow() {
        InitializeComponent();

        VM = new UpdateWindow_ViewModel();
        DataContext = VM;

        VM.Setup(this);
    }

    /// <inheritdoc />
    public UpdateWindow_ViewModel VM { get; }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    async void AutomaticInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( UpdateChecker.LatestRelease?.Assets is { } Assets ) {
            foreach ( ReleaseAsset Asset in Assets ) {
                if ( Asset.Name.ToUpperInvariant().EndsWith(".ZIP") ) {
                    Debug.WriteLine($"Found asset: {Asset.Name} :: {Asset.BrowserDownloadUrl}");
                    FileInfo TestFile = FileSystemInfoExtensions.AppDir.CreateSubfile(Asset.Name, false);
                    byte[] BtBuffer = new byte[16384];
                    Memory<byte> Buffer = new Memory<byte>(BtBuffer);
                    Debug.WriteLine("Downloading...");
                    await DownloadFileAsync(Asset.BrowserDownloadUrl, TestFile, Buffer, new CancellationToken());
                    Debug.WriteLine($"Fin. {TestFile.FullName}");
                    Process.Start("explorer.exe", $"/select,\"{TestFile.FullName}\"");
                }
            }
        }
    }

    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static async Task DownloadFileAsync(string Url, FileInfo Dest, Memory<byte> Buffer, CancellationToken Token = default) {
        Debug.WriteLine("Creating client...");
        HttpClient Client = new HttpClient();
        Debug.WriteLine("Creating get request...");
        HttpResponseMessage Msg = await Client.GetAsync(Url, Token);
        Debug.WriteLine("Creating file write stream...");

        await using ( FileStream FS = Dest.Create() ) {
            Debug.WriteLine("Creating read stream...");
            await using ( Stream Str = await Msg.Content.ReadAsStreamAsync(Token) ) {
                long L = Str.Length;
                int B = Buffer.Length;
                while ( Str.Position + B < L ) {
                    Debug.WriteLine("\tRead");
                    await Str.ReadAsync(Buffer, Token);
                    Debug.WriteLine("\tWrite");
                    await FS.WriteAsync(Buffer, Token);
                }
                Debug.WriteLine("Some remainder exists.");
                int R = (int)(L - Str.Position);
                if ( R > 0 ) {
                    Memory<byte> Remainder = new byte[R];
                    Debug.WriteLine($"\tReading remainder {R}...");
                    await Str.ReadAsync(Remainder, Token);
                    Debug.WriteLine("Writing remainder...");
                    await FS.WriteAsync(Remainder, Token);
                }
            }
        }
        Debug.WriteLine("Download complete.");
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void ManualInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( UpdateChecker.LatestRelease?.HtmlUrl is { } Url ) {
            Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
            VM.UpdateDialogVisible = false;
        }
    }
}

public class UpdateWindow_ViewModel : Window_ViewModel<UpdateWindow> {
    /// <summary>
    /// Gets or sets the latest version available.
    /// </summary>
    /// <value>
    /// The latest version available from GitHub.
    /// </value>
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public Version LatestVersion { get; set; } = new Version(0, 1, 1, 0);

    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;

    /// <summary>
    /// Gets or sets a value indicating whether the update dialog should be visible.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the update dialog is visible; otherwise, <see langword="false" />.
    /// </value>
    public bool UpdateDialogVisible { get; set; } = true;
}