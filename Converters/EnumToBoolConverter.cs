#region Copyright (C) 2017-2022  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.Globalization;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <typeparamref name="TEnum"/> to <see cref="bool"/>.
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
//[ValueConversion(typeof(TEnum), typeof(bool))]
public abstract class EnumToBoolConverter<TEnum> : ValueConverter<TEnum, bool> where TEnum : struct, Enum {

    /// <summary>
    /// Gets or sets the default return value.
    /// </summary>
    /// <value>
    /// The default value returned when the supplied <typeparamref name="TEnum"/> is unexpected.
    /// </value>
    public bool Default { get; set; }

    /// <summary>
    /// Gets or sets the value returned when the reverse conversion is supplied a <see langword="true"/> reference.
    /// </summary>
    /// <value>
    /// The <see langword="true"/> return value.
    /// </value>
    public TEnum True { get; set; }

    /// <summary>
    /// Gets or sets the value returned when the reverse conversion is supplied a <see langword="false"/> reference.
    /// </summary>
    /// <value>
    /// The <see langword="false"/> return value.
    /// </value>
    public TEnum False { get; set; }

    /// <summary>
    /// Gets or sets the value returned when the reverse conversion is supplied a <see langword="null"/> reference.
    /// </summary>
    /// <value>
    /// The <see langword="null"/> return value.
    /// </value>
    public TEnum Null { get; set; }

    /// <inheritdoc />
    public override bool CanForward => true;

    /// <inheritdoc />
    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override bool CanReverse => true;

    /// <inheritdoc />
    public override bool CanReverseWhenNull => true;

    /// <inheritdoc />
    public override bool ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Default;

    /// <inheritdoc />
    public override TEnum Reverse( bool To, object? Parameter = null, CultureInfo? Culture = null ) => To ? True : False;

    /// <inheritdoc />
    public override TEnum ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Null;
}