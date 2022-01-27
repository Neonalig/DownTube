#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

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

        if ( Args.GitHub && await UpdateChecker.Client.Repository.Release.GetLatest("starflash-studios", "DownTube-Updater") is { } UpdaterRelease ) {
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
            true,
            CTS);
        VM.InstallProgress = 1;
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    async void AutomaticInstall_OnClick( object Sender, RoutedEventArgs E ) {
        UpdateLastCheckedDate();
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
    /// Updates <see cref="Props.LastCheckDate"/> to ensure the user's preferences for <see cref="UpdateCheckFrequency"/> are respected.
    /// </summary>
    public static void UpdateLastCheckedDate() {
        Props.LastCheckDate.Value = DateOnly.FromDateTime(DateTime.UtcNow);
        Props.Save();
        Debug.WriteLine($"Update 'LastCheckDate' was changed to {Props.LastCheckDate}.");
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void ManualInstall_OnClick( object Sender, RoutedEventArgs E ) {
        UpdateLastCheckedDate();
        if ( UpdateChecker.LatestRelease?.HtmlUrl is { } Url ) {
            Url.NavigateToWebsite();
            VM.UpdateDialogVisible = false;
        }
    }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void SkipVersionButton_OnClick( object Sender, RoutedEventArgs E ) {
        UpdateLastCheckedDate();
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