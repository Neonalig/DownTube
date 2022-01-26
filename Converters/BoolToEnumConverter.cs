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
/// Provides value conversions from <see cref="bool"/> to <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
//[ValueConversion(typeof(bool), typeof(TEnum))]
public abstract class BoolToEnumConverter<TEnum> : ValueConverter<bool, TEnum> where TEnum : struct, Enum {

    /// <summary>
    /// Gets or sets the enum value returned when <see langword="true"/>.
    /// </summary>
    /// <value>
    /// The value returned when <see langword="true"/>.
    /// </value>
    public TEnum True { get; set; }

    /// <summary>
    /// Gets or sets the enum value returned when <see langword="false"/>.
    /// </summary>
    /// <value>
    /// The value returned when <see langword="false"/>.
    /// </value>
    public TEnum False { get; set; }

    /// <summary>
    /// Gets or sets the enum value returned when <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The value returned when <see langword="null"/>.
    /// </value>
    public TEnum Null { get; set; }

    /// <inheritdoc />
    public override bool CanReverse => false;

    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override TEnum Forward( bool From, object? Parameter = null, CultureInfo? Culture = null ) => From ? True : False;

    /// <inheritdoc />
    public override TEnum ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Null;

    /// <inheritdoc />
    public override bool Reverse( TEnum To, object? Parameter = null, CultureInfo? Culture = null ) => false;
}