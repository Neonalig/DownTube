#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Globalization;
using System.Windows.Data;

using DownTube.Views.Windows;

#endregion

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="KnownUtilityDownloadMatchType"/> to <see cref="string"/>.
/// </summary>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
[ValueConversion(typeof(KnownUtilityDownloadMatchType), typeof(string))]
public sealed class KnownUtilityDownloadMatchTypeToStringConverter : ValueConverter<KnownUtilityDownloadMatchType, string> {

    /// <summary>
    /// Gets or sets the value returned when <see cref="KnownUtilityDownloadMatchType.Supported"/> is supplied.
    /// </summary>
    /// <value>
    /// The specific return value.
    /// </value>
    public string Supported { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value returned when <see cref="KnownUtilityDownloadMatchType.Supported"/> is supplied.
    /// </summary>
    /// <value>
    /// The specific return value.
    /// </value>
    public string Recommended { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value returned when <see cref="KnownUtilityDownloadMatchType.Supported"/> is supplied.
    /// </summary>
    /// <value>
    /// The specific return value.
    /// </value>
    public string Unknown { get; set; } = string.Empty;

    /// <inheritdoc />
    public override bool CanReverse => false;

    /// <inheritdoc />
    public override string Forward( KnownUtilityDownloadMatchType From, object? Parameter = null, CultureInfo? Culture = null ) => From switch {
        KnownUtilityDownloadMatchType.Supported   => Supported,
        KnownUtilityDownloadMatchType.Recommended => Recommended,
        KnownUtilityDownloadMatchType.Unknown     => Unknown,
        _                                         => throw new EnumValueOutOfRangeException<KnownUtilityDownloadMatchType>(From)
    };

    /// <inheritdoc />
    public override KnownUtilityDownloadMatchType Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => KnownUtilityDownloadMatchType.Unknown;
}