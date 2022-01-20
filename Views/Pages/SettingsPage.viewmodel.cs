#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;
using System.Reflection;
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

        RepoUrl = UpdateChecker.RepoUrl ?? "https://github.com/starflash-studios/DownTube"; //<-- fallback
        CurrentReleaseUrl = UpdateChecker.CurrentRelease?.HtmlUrl ?? $"https://github.com/starflash-studios/DownTube/releases/tag/v{StaticBindings.AppVersion.ToString(3)}"; //<-- fallback
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

        switch ( Content ) {
            case TextBlock TB:
                EventHandler Listener = ( _, _ ) => {
                    Debug.WriteLine("Heard back from InheritanceContextChanged");
                };
                TB.GetType().GetEvent("InheritanceContextChanged", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).AddMethod.Invoke(Content, new object[] { Listener });
                //TB.PropertyChange += 
                //TypedDependencyProperty<string> Prop = Content.GetDependencyProperty(TB.Text).CatchNull();
                //Debug.WriteLine($"Found {Prop} :: {Prop.Get(Content)}");
                break;
        }
        //Content.InvokeOnPropertyChanged
        //Content.AddHandler("NotifyPropertyChange", )
        ////TB.Raise(TB.DataContextChanged, new DependencyPropertyChangedEventArgs());
        ////TB.GetValue
        //Debug.WriteLine($"New SettingsField constructed with type {Content.GetType()}");
        //((DependencyObject)Content).InvokeOnPropertyChanged += ( _, E ) => {
        //    Debug.WriteLine($"Verifying new data from property {E.PropertyName}");
        //    //E.NewValue
        //    VerifyEventArgs Args = new VerifyEventArgs("egg", true);
        //    OnVerify(Args);
        //    IsValid = Args.Valid;
        //};
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

    /// <summary>
        /// Represents the method that will handle the Verify <see langword="event"/> on a <see cref="SettingsField"/> instance, and provide the relevant event arguments.
        /// </summary>
        /// <param name="Sender">The <see langword="event"/> raiser.</param>
        /// <param name="E">The raised <see langword="event"/> arguments.</param>
        /// <seealso cref="VerifyEventArgs"/>
    public delegate void VerifyEventHandler( SettingsField? Sender, VerifyEventArgs E );

    /// <summary>
    /// Provides additional data for the <see cref="VerifyEventHandler"/>.
    /// </summary>
    /// <seealso cref="VerifyEventHandler"/>
    public class VerifyEventArgs : EventArgs {
        /// <summary>
        /// Constructs an instance of the <see cref="VerifyEventArgs"/> <see langword="class"/>.
        /// </summary>
        /// <param name="Value">The property value to validate.</param>
        /// <param name="Valid">A value indicating whether the new value is valid.</param>
        public VerifyEventArgs( object Value, bool Valid ) {
            this.Value = Value;
            this.Valid = Valid;
        }

        /// <summary>
        /// The property value to validate.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the new value is valid.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the new value is valid; otherwise, <see langword="false" />.
        /// </value>
        public bool Valid { get; set; }
    }

    /// <summary>
    /// <see langword="event"/> raised when a property value is changed and must be verified again.
    /// </summary>
    public event VerifyEventHandler? Verify;

    /// <summary>
    /// Raises the <see cref="Verify" /> event.
    /// </summary>
    /// <param name="E">The <see cref="VerifyEventArgs"/> instance containing the event data.</param>
    protected virtual void OnVerify( VerifyEventArgs E ) => Verify?.Invoke(this, E);
}