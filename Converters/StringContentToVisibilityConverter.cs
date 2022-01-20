#region Copyright (C) 2017-2022  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.Globalization;
using System.Windows;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="string"/> to <see cref="Visibility"/>.
/// </summary>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
public class StringContentToVisibilityConverter : ValueConverter<string?, Visibility> {

    /// <summary>
    /// Gets or sets a value returned when the string is not <see langword="null"/>, <see cref="string.Empty"/>, or <see cref="char.IsWhiteSpace(char)"/>.
    /// </summary>
    /// <value>
    /// The value returned when the string is not <see langword="null"/>, <see cref="string.Empty"/>, or <see cref="char.IsWhiteSpace(char)"/>.
    /// </value>
    public Visibility DefaultReturn { get; set; } = Visibility.Visible;

    /// <summary>
    /// Gets or sets the value returned when the <see cref="string"/> is <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The value returned when the <see cref="string"/> is <see langword="null"/>.
    /// </value>
    public Visibility ReturnWhenNull { get; set; } = Visibility.Collapsed;

    /// <summary>
    /// Gets or sets the value returned when the <see cref="string"/> is <see cref="string.Empty"/>.
    /// </summary>
    /// <value>
    /// The value returned when the <see cref="string"/> is <see cref="string.Empty"/>.
    /// </value>
    public Visibility ReturnWhenEmpty { get; set; } = Visibility.Collapsed;

    /// <summary>
    /// Gets or sets the value returned when the <see cref="string"/> is <see cref="char.IsWhiteSpace(char)"/>.
    /// </summary>
    /// <value>
    /// The value returned when the <see cref="string"/> is <see cref="char.IsWhiteSpace(char)"/>.
    /// </value>
    public Visibility ReturnWhenWhitespace { get; set; } = Visibility.Collapsed;

    /// <inheritdoc />
    public override bool CanReverse => false;

    /// <inheritdoc />
    public override bool CanForward => true;

    /// <inheritdoc />
    public override bool CanReverseWhenNull => false;

    /// <inheritdoc />
    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override Visibility Forward( string? From, object? Parameter = null, CultureInfo? Culture = null ) => From is null ? ReturnWhenNull : string.IsNullOrEmpty(From) ? ReturnWhenEmpty : string.IsNullOrWhiteSpace(From) ? ReturnWhenEmpty : DefaultReturn;

    /// <inheritdoc />
    public override Visibility ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => ReturnWhenNull;

    /// <inheritdoc />
    public override string? Reverse( Visibility To, object? Parameter = null, CultureInfo? Culture = null ) => null;
}