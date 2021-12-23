#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;

using MVVMUtils;

#endregion

namespace DownTube.Views.Pages;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : IView<SettingsPage_ViewModel>{
    /// <summary>
    /// Initialises a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage() {
        InitializeComponent();
        VM = new SettingsPage_ViewModel();
        DataContext = VM;
    }

    /// <inheritdoc />
    public SettingsPage_ViewModel VM { get; }

    /// <summary>
    /// Occurs when the Click <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void MainRepo_Click( object Sender, RoutedEventArgs E ) => Process.Start(new ProcessStartInfo(VM.RepoUrl) { UseShellExecute = true });

    /// <summary>
    /// Occurs when the Click <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void CurrentVersion_Click( object Sender, RoutedEventArgs E ) => Process.Start(new ProcessStartInfo(VM.CurrentReleaseUrl) { UseShellExecute = true });
}