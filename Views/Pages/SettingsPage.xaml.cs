#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;

using DownTube.Converters;
using DownTube.Engine;
using DownTube.Views.Controls;

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
        VM.Setup(this);
        DataContext = VM;

        void UpdateIV() {
            lock ( Props.IgnoredVersions ) {
                VM.IgnoreNextUpdate = true;
                Props.IgnoredVersions.Clear();
                VM.IgnoreNextUpdate = false;
                TVField.IsValid = true;
                foreach ( Tag Tg in TV.VM.Tags ) {
                    if ( VersionToStringConverter.Instance.Reverse(Tg.Name) is { } Ver ) {
                        VM.IgnoreNextUpdate = true;
                        Props.IgnoredVersions.Add(Ver);
                        VM.IgnoreNextUpdate = false;
                    } else {
                        TVField.IsValid = false;
                    }
                }
                if ( TVField.IsValid ) {
                    VM.IgnoreNextUpdate = true;
                    Props.Save();
                    VM.IgnoreNextUpdate = false;
                }
            }
        }

        TV.VM.TagAdded += ( _, _ ) => UpdateIV();
        TV.VM.TagRemoved += ( _, _ ) => UpdateIV();
        TV.VM.TagChanged += ( _, _ ) => UpdateIV();
        //TV.VM.TagsCleared += _ => UpdateIV(); //<-- Not required, as ClearTags() first invokes 'TagRemoved' for each tag manually.
    }

    /// <summary>
    /// Determines whether the collection only contains a single element, and returns it if so.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Enum">The enumerable.</param>
    /// <param name="Found">The found item.</param>
    /// <returns>
    /// <see langword="true" /> if the collection only has one item; otherwise, <see langword="false" />.
    /// </returns>
    internal static bool HasOnlySingle<T>( ICollection<T>? Enum, out T Found ) {
        if (Enum is not null ) {
            bool AnyFound = false;
            foreach( T Item in Enum ) {
                if ( AnyFound ) {
                    Found = default!;
                    return false;
                }
                Found = Item;
                AnyFound = true;
            }
        }
        Found = default!;
        return false;
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

    void SettingsField_Verify( SettingsField Sender, SettingsField.VerifyEventArgs E ) {
        Debug.WriteLine($"Verifying on {Sender} w/ new value {E.Value}.");
    }
}