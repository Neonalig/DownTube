#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

using DownTube.Converters;
using DownTube.Engine;
using DownTube.Views.Controls;

using MVVMUtils;

using ReactiveUI;

#endregion

namespace DownTube.Views.Pages;

public class SettingsPage_ViewModel : ViewModel<SettingsPage> {
    public SettingsPage_ViewModel() {
        IgnoredVersionsSummary = string.Empty;

        Props.SavedPropertyChanged += ( _, P ) => {
            if ( View is null ) { return; }
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch ( P.PropertyName ) {
                case nameof(Props.IgnoredVersions):
                    UpdateIgnoredVersions();
                    break;
            }
        };

        RepoUrl = UpdateChecker.RepoUrl ?? "https://github.com/Neonalig/DownTube"; //<-- fallback
        CurrentReleaseUrl = UpdateChecker.CurrentRelease?.HtmlUrl ?? $"https://github.com/Neonalig/DownTube/releases/tag/v{StaticBindings.AppVersion.ToString(3)}"; //<-- fallback
        //fallback URLs (above) are dynamically resolved at runtime after checking for updates, and are only used for the interim period before the initial check for updates, or if any error occurs during the update checking.
        UpdateChecker.StaticPropertyChanged += ( _, _, PropertyName ) => {
            //Debug.WriteLine($"Prop changed {PropertyName}");
            // ReSharper disable once ConvertSwitchStatementToSwitchExpression
            switch ( PropertyName ) {
                case nameof(UpdateChecker.RepoUrl) when UpdateChecker.RepoUrl is { } RepU:
                    //Debug.WriteLine($"Updating repo url to: {UpdateChecker.RepoUrl}");
                    RepoUrl = RepU;
                    break;
                case nameof(UpdateChecker.CurrentRelease) when UpdateChecker.CurrentRelease is { } CurR:
                    //Debug.WriteLine($"Updating current release url to: {CurR.HtmlUrl}");
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

    /// <summary>
    /// Gets the ignored update versions summary.
    /// </summary>
    /// <value>
    /// The ignored update versions summary.
    /// </value>
    public string IgnoredVersionsSummary { get; private set; }

    /// <summary>
    /// Updates the <see cref="IgnoredVersionsSummary"/> <see langword="string"/>.
    /// </summary>
    void UpdateIgnoredVersionsSummary() => IgnoredVersionsSummary = string.Join(", ", Props.IgnoredVersions.Value.Select(V => VersionToStringConverter.Instance.Forward(V)));

    /// <summary>
    /// Indicates the next update could cause an infinite loop and should be ignored.
    /// </summary>
    internal bool IgnoreNextUpdate = false;

    /// <summary>
    /// Updates the ignored versions <see cref="TagViewer"/>.
    /// </summary>
    void UpdateIgnoredVersions() {
        if ( IgnoreNextUpdate ) { IgnoreNextUpdate = false; return; }
        Debug.WriteLine("Updating versions...");
        lock ( Props.IgnoredVersions ) {
            View.TV.ClearTags();
            foreach ( Version IgVer in Props.IgnoredVersions.Value ) {
                Debug.WriteLine($"Got ignored version {IgVer}");
                View.TV.AddTag(new Tag(VersionToStringConverter.Instance.Forward(IgVer)));
            }
            UpdateIgnoredVersionsSummary();
        }
    }

    FileInfo? Int_FFmpegPath { get; set; } = null;

    public FileInfo? FFmpegPath {
        get => Int_FFmpegPath;
        set =>
            Int_FFmpegPath = value switch {
                { } Pth when Pth.FullName.ToLowerInvariant().EndsWith(".exe") => value,
                _                                                             => null
            };
    }

    public override void OnSetup() => UpdateIgnoredVersions();
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

        Height = 40;
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

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    /// <value>
    /// The height.
    /// </value>
    public double Height { get; set; }

    /// <summary>
    /// Returns <see langword="true"/> if the field is in a valid state.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the field content is valid; otherwise, <see langword="false" />.
    /// </value>
    public bool IsValid { get; set; } = true;

    /// <summary> The data used to generate the child element. </summary>
    UIElement _Content;

    /// <summary> Gets or sets the data used to generate the child element. </summary>
    public UIElement Content {
        get => _Content;
        set => this.SetAndRaise(ref _Content, value);
    }
}