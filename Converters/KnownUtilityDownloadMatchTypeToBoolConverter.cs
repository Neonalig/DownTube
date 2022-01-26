#region Copyright (C) 2017-2022  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.Globalization;
using System.Windows.Data;

using DownTube.Views.Windows;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="KnownUtilityDownloadMatchType"/> to <see cref="bool"/>.
/// </summary>
/// <seealso cref="EnumToBoolConverter{TEnum}"/>
[ValueConversion(typeof(KnownUtilityDownloadMatchType), typeof(bool))]
public sealed class KnownUtilityDownloadMatchTypeToBoolConverter : EnumToBoolConverter<KnownUtilityDownloadMatchType> {

    /// <summary>
    /// Gets or sets the value returned when <see cref="KnownUtilityDownloadMatchType.Supported"/> is supplied.
    /// </summary>
    /// <value>
    /// The specific return value.
    /// </value>
    public bool Supported { get; set; }

    /// <summary>
    /// Gets or sets the value returned when <see cref="KnownUtilityDownloadMatchType.Recommended"/> is supplied.
    /// </summary>
    /// <value>
    /// The specific return value.
    /// </value>
    public bool Recommended { get; set; }

    /// <summary>
    /// Gets or sets the value returned when <see cref="KnownUtilityDownloadMatchType.Unknown"/> is supplied.
    /// </summary>
    /// <value>
    /// The specific return value.
    /// </value>
    public bool Unknown { get; set; }

    /// <inheritdoc />
    public override bool Forward( KnownUtilityDownloadMatchType From, object? Parameter = null, CultureInfo? Culture = null ) => From switch {
        KnownUtilityDownloadMatchType.Supported   => Supported,
        KnownUtilityDownloadMatchType.Recommended => Recommended,
        KnownUtilityDownloadMatchType.Unknown     => Unknown,
        _                                         => throw new EnumValueOutOfRangeException<KnownUtilityDownloadMatchType>(From)
    };
}