#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using DownTube.Engine;

using MVVMUtils;

using ReactiveUI;

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

public class SettingsPage_ViewModel : ViewModel<SettingsPage> {
    public SettingsPage_ViewModel() {
        RepoUrl = UpdateChecker.RepoUrl ?? "https://github.com/starflash-studios/DownTube"; //<-- fallback
        CurrentReleaseUrl = UpdateChecker.CurrentRelease?.HtmlUrl ?? $"https://github.com/starflash-studios/DownTube/releases/tag/v{StaticBindings.AppVersion.ToString(3)}"; //<-- fallback
        //fallback URLs (above) are dynamically resolved at runtime after checking for updates, and are only used for the interim period before the initial check for updates, or if any error occurs during the update checking.
        UpdateChecker.StaticPropertyChanged += ( _, _, PropertyName ) => {
            Debug.WriteLine($"Prop changed {PropertyName}");
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch ( PropertyName ) {
                case nameof(UpdateChecker.RepoUrl) when UpdateChecker.RepoUrl is { } RepU:
                    Debug.WriteLine($"Updating repo url to: {UpdateChecker.RepoUrl}");
                    RepoUrl = RepU;
                    break;
                case nameof(UpdateChecker.CurrentRelease) when UpdateChecker.CurrentRelease is { } CurR:
                    Debug.WriteLine($"Updating current release url to: {CurR.HtmlUrl}");
                    CurrentReleaseUrl = CurR.HtmlUrl!;
                    break;
            }
        };
    }

    /// <summary>
    /// Gets the GitHub repository's URL.
    /// </summary>
    /// <value>
    /// The GitHub repository's <c>HtmlUrl</c>.
    /// </value>
    public string RepoUrl { get; private set; }

    /// <summary>
    /// Gets the current version's release URL.
    /// </summary>
    /// <value>
    /// The current version's release URL.
    /// </value>
    public string CurrentReleaseUrl { get; private set; }
}

/// <summary>
/// Represents a named field for the <see cref="SettingsPage"/>.
/// </summary>
[ContentProperty(nameof(Content))]
public class SettingsField : ReactiveObject {
    /// <summary>
    /// Initialises a new instance of the <see cref="SettingsField"/> class.
    /// </summary>
    public SettingsField() : this(string.Empty, string.Empty, new TextBlock { Text = "Error." }) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="SettingsField"/> class.
    /// </summary>
    /// <param name="FieldName">The setting name.</param>
    /// <param name="ToolTip">The tooltip.</param>
    /// <param name="Content">The displayed content.</param>
    public SettingsField( string FieldName, string ToolTip, UIElement Content ) {
        this.FieldName = FieldName;
        this.ToolTip = ToolTip;
        _Content = Content;
    }

    /// <summary>
    /// Gets or sets the name of the setting.
    /// </summary>
    /// <value>
    /// The setting name.
    /// </value>
    public string FieldName { get; set; }

    /// <summary>
    /// Gets or sets the tooltip.
    /// </summary>
    /// <value>
    /// The tooltip.
    /// </value>
    public string ToolTip { get; set; }

    /// <summary> The data used to generate the child element. </summary>
    UIElement _Content;

    /// <summary> Gets or sets the data used to generate the child element. </summary>
    public UIElement Content {
        get => _Content;
        set => this.SetAndRaise(ref _Content, value);
    }
}