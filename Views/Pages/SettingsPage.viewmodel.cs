using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;

using DownTube.Engine;

using MVVMUtils;

using ReactiveUI;

namespace DownTube.Views.Pages;

public class SettingsPage_ViewModel : ViewModel<SettingsPage> {
    public SettingsPage_ViewModel() {
        RepoUrl = UpdateChecker.RepoUrl                           ?? "https://github.com/starflash-studios/DownTube"; //<-- fallback
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