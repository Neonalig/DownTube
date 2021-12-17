using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using DownTube.DataTypes.Helpers;
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
    void AutomaticInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( UpdateChecker.LatestRelease?.Assets is { } Assets ) {
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach ( ReleaseAsset Asset in Assets ) {
                if ( Asset.Name.ToUpperInvariant().EndsWith(".ZIP") ) {
                    VM.InstallProgress = - 1;
                    DownloadRequest Request = new DownloadRequest(Asset.BrowserDownloadUrl, FileSystemInfoExtensions.AppDir.CreateSubfile(Asset.Name, false));
                    Request.DownloadStarted += _ => {
                        Debug.WriteLine("Download started.");
                    };
                    Request.ProgressUpdated += ( _, P ) => {
                        Debug.WriteLine($"Download: {P:P2}");
                        VM.InstallProgress = P;
                    };
                    Request.DownloadComplete += ( _, Fl ) => {
                        Debug.WriteLine($"Download finished. {Fl.FullName}");
                        Process.Start("explorer.exe", $"/select,\"{Fl.FullName}\"");
                        VM.InstallProgress = 1;

                        Debug.WriteLine("Extracting...");
                        DirectoryInfo Ext = Fl.Extract(true);
                        Debug.WriteLine($"Extracted to {Ext.FullName}");
                    };
                    Debug.WriteLine("Starting request...");
                    Request.Start();
                    Debug.WriteLine("Download delegated to new thread.");
                    break;
                }
            }
        }
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

    /// <summary>
    /// Gets or sets the current automatic installation progress.
    /// </summary>
    /// <value>
    /// The automatic installation progress.
    /// </value>
    public double InstallProgress { get; set; }
}

public class InstallProgressToVisibilityConverter : ValueConverter<double, Visibility> {
    /// <inheritdoc />
    public override bool CanReverse => false;

    /// <inheritdoc />
    public override Visibility Forward( double From, object? Parameter = null, CultureInfo? Culture = null ) => From switch {
        0 => Visibility.Collapsed,
        _ => Visibility.Visible
    };

    /// <inheritdoc />
    public override double Reverse( Visibility To, object? Parameter = null, CultureInfo? Culture = null ) => 0;
}

public class InstallProgressToIntermediateConverter : ValueConverter<double, bool> {
    /// <inheritdoc />
    public override bool CanReverse => false;

    /// <inheritdoc />
    public override bool Forward( double From, object? Parameter = null, CultureInfo? Culture = null ) => From switch {
        < 0 => true,
        _   => false
    };

    /// <inheritdoc />
    public override double Reverse( bool To, object? Parameter = null, CultureInfo? Culture = null ) => 0;
}

public class InstallProgressToStringConverter : ValueConverter<double, string> {
    /// <inheritdoc />
    public override bool CanReverse => false;

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public override string Forward( double From, object? Parameter = null, CultureInfo? Culture = null ) => From switch {
        < 0 => "Downloading...",
        _   => From.ToString("P2")
    };

    /// <inheritdoc />
    public override double Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => 0d;
}