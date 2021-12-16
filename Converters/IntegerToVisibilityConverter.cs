#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Globalization;
using System.Windows;

using MVVMUtils;

#endregion

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="int"/> to <see cref="Visibility"/>.
/// </summary>
public class IntegerToVisibilityConverter : ValueConverter<int, Visibility> {
    /// <summary>
    /// Gets or sets the integer value responsible for returning <see cref="Visibility.Visible"/>.
    /// </summary>
    /// <value> An integer value. </value>
    public int Visible { get; set; }

    /// <summary>
    /// Gets or sets the integer value responsible for returning <see cref="Visibility.Hidden"/>.
    /// </summary>
    /// <value> An integer value. </value>
    public int Hidden { get; set; }

    /// <summary>
    /// Gets or sets the integer value responsible for returning <see cref="Visibility.Collapsed"/>.
    /// </summary>
    /// <value> An integer value. </value>
    public int Collapsed { get; set; }

    /// <summary>
    /// Gets or sets the default return value when the integer value is neither that of <see cref="Visible"/>, <see cref="Hidden"/> or <see cref="Collapsed"/>.
    /// </summary>
    /// <value>
    /// The default <see cref="Visibility"/> to return when the integer value is not recognised.
    /// </value>
    public Visibility DefaultVisibility { get; set; }

    /// <inheritdoc />
    public override bool CanReverse => true;

    /// <inheritdoc />
    public override Visibility Forward( int From, object? Parameter = null, CultureInfo? Culture = null ) =>
        From == Visible
            ? Visibility.Visible
            : From == Hidden
                ? Visibility.Hidden
                : From == Collapsed
                    ? Visibility.Collapsed
                    : DefaultVisibility;

    // ReSharper disable once ExceptionNotThrown
    /// <inheritdoc />
    /// <exception cref="EnumValueOutOfRangeException"><paramref name="To"/> is outside of the acceptable range of values.</exception>
    public override int Reverse( Visibility To, object? Parameter = null, CultureInfo? Culture = null ) => To switch {
        Visibility.Visible   => Visible,
        Visibility.Hidden    => Hidden,
        Visibility.Collapsed => Collapsed,
        // ReSharper disable once ExceptionNotDocumentedOptional
        _                    => throw new EnumValueOutOfRangeException<Visibility>(To)
    };
}