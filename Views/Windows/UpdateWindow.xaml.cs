#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using DownTube.Engine;

using MVVMUtils;

using Octokit;

#endregion

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
    /// Checks if the updater already exists, and downloads it if not.
    /// </summary>
    async Task DownloadUpdater( DirectoryInfo UpdaterDest, CancellationTokenSource CTS ) {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( FileInfo Fl in UpdaterDest.GetFiles() ) {
            if (Fl.Name.Equals("updater.exe", StringComparison.InvariantCultureIgnoreCase) ) {
                return;
            }
        }

        if ( await UpdateChecker.Client.Repository.Release.GetLatest("starflash-studios", "DownTube-Updater") is { } UpdaterRelease ) {
            await Download(UpdaterRelease, UpdaterDest, CTS);
        }
    }

    /// <summary>
    /// Downloads the specified release.
    /// </summary>
    /// <param name="Release">The release.</param>
    /// <param name="Destination">The destination.</param>
    /// <param name="CTS">The cancellation token.</param>
    async Task Download( Release Release, DirectoryInfo Destination, CancellationTokenSource CTS ) {
        await DownloadRequest.DownloadRelease(
            Release,
            Destination,
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

                //Debug.WriteLine("Extracting...");
                //DirectoryInfo Ext = Fl.Extract(true);
                //Debug.WriteLine($"Extracted to {Ext.FullName}");
            },
            16384,
            false,
            CTS);
        VM.InstallProgress = 1;
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    async void AutomaticInstall_OnClick( object Sender, RoutedEventArgs E ) {
        VM.InstallProgress = -1;
        if ( UpdateChecker.LatestRelease is null ) { return; }

        DirectoryInfo
            UpdaterDest = FileSystemInfoExtensions.AppDir.CreateSubdirectory("Updater"),
            ExtractedUpdateDest = UpdaterDest.CreateSubdirectory("_Update");
        CancellationTokenSource CTS = new CancellationTokenSource();
        await DownloadUpdater(UpdaterDest, CTS);
        await Download(UpdateChecker.LatestRelease, ExtractedUpdateDest, CTS);
        VM.InstallProgress = -1;
        FileInfo Updater = UpdaterDest.CreateSubfile("updater.exe", false);
        if ( Updater.Exists
             && Process.Start(new ProcessStartInfo(Updater.FullName) {
                UseShellExecute = false,
                CreateNoWindow = true
            }) is { } UpdateProcess ) {
            await UpdateProcess.WaitForExitAsync(CTS.Token);
        }
        //No need to 'ReturnToMain()' etc, as the automatic updater process will kill the running application automatically anyways.
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void ManualInstall_OnClick( object Sender, RoutedEventArgs E ) {
        if ( UpdateChecker.LatestRelease?.HtmlUrl is { } Url ) {
            _ = Process.Start(new ProcessStartInfo(Url) { UseShellExecute = true });
            VM.UpdateDialogVisible = false;
        }
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void SkipVersionButton_OnClick( object Sender, RoutedEventArgs E ) {
        if ( UpdateChecker.LatestVersion is { } Ver ) {
            Props.IgnoredVersions.Add(Ver);
            ReturnToMain();
        }
    }

    /// <summary>
    /// Hides the update window then returns to the main window.
    /// </summary>
    void ReturnToMain() {
        MainWindow.Instance.Show();
        Close();
    }

    /// <summary>
    /// Occurs when the Click <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void NotifyLaterButton_Click( object Sender, RoutedEventArgs E ) => ReturnToMain();

    /// <summary>
    /// Occurs when the Click <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void InstallNowButton_Click( object Sender, RoutedEventArgs E ) => VM.UpdateDialogVisible = true;
}

/// <summary>
/// Viewmodel for the <see cref="UpdateWindow"/> view.
/// </summary>
/// <seealso cref="UpdateWindow"/>
/// <seealso cref="Window_ViewModel{T}"/>
public class UpdateWindow_ViewModel : Window_ViewModel<UpdateWindow> {
    /// <summary>
    /// Initialises a new instance of the <see cref="UpdateWindow_ViewModel"/> class.
    /// </summary>
    public UpdateWindow_ViewModel() => LatestVersion = UpdateChecker.LatestVersion;

    /// <summary>
    /// Gets or sets the latest version available.
    /// </summary>
    /// <value>
    /// The latest version available from GitHub.
    /// </value>
    public Version? LatestVersion { get; set; }

    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;

    /// <summary>
    /// Gets or sets a value indicating whether the update dialog should be visible.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the update dialog is visible; otherwise, <see langword="false" />.
    /// </value>
    public bool UpdateDialogVisible { get; set; }

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

/// <summary>
/// A collection of ignored update versions.
/// </summary>
public class IgnoredVersions : ICollection<Version> {

    /// <summary>
    /// The collection of ignored versions.
    /// </summary>
    readonly HashSet<Version> _Ignored = new HashSet<Version>();

    /// <inheritdoc />
    public IEnumerator<Version> GetEnumerator() => _Ignored.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// The current version. All ignored versions below <see cref="Current"/> are irrelevant.
    /// </summary>
    public static readonly Version Current = UpdateChecker.CurrentVersion;

    /// <inheritdoc />
    /// <remarks>Only versions greater than <see cref="Current"/> will be added, as those below it are irrelevant.</remarks>
    public void Add( Version Item ) {
        if ( Item > Current ) {
            _Ignored.Add(Item);
        }
    }

    /// <inheritdoc />
    public void Clear() => _Ignored.Clear();

    /// <summary>
    /// Whether the version should be ignored.
    /// </summary>
    /// <param name="Version">The version.</param>
    /// <returns><see langword="true"/> if <paramref name="Version"/> is &lt;= <see cref="Current"/> or if the collection of ignored versions contains it.</returns>
    public bool ShouldIgnore( Version Version ) => Version <= Current || _Ignored.Contains(Version);

    /// <inheritdoc />
    public bool Contains( Version Item ) => _Ignored.Contains(Item);

    /// <inheritdoc />
    public void CopyTo( Version[] Array, int ArrayIndex ) => _Ignored.CopyTo(Array, ArrayIndex);

    /// <inheritdoc />
    public bool Remove( Version Item ) => _Ignored.Remove(Item);

    /// <inheritdoc />
    public int Count => _Ignored.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;
}