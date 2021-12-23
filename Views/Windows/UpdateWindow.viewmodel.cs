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
using System.Windows;
using System.Windows.Controls;

using DownTube.Engine;

using MVVMUtils;

#endregion

namespace DownTube.Views.Windows;

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
        _ => false
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
        _ => From.ToString("P2")
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