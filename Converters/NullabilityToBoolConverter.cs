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

#endregion

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="object"/><c>?</c> to <see cref="bool"/>.
/// </summary>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
[ValueConversion(typeof(Nullable), typeof(bool))]
internal class NullabilityToBoolConverter : ValueConverter<object?, bool> {

    /// <summary>
    /// Gets or sets the value returned when <b>not</b> <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The default return value.
    /// </value>
    public bool Default { get; set; } = true;

    /// <summary>
    /// Gets or sets the value returned when <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The return value when <see langword="null"/>.
    /// </value>
    public bool Null { get; set; } = false;

    /// <inheritdoc />
    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override bool CanReverse => false;

    /// <inheritdoc />
    public override bool Forward( object? From, object? Parameter = null, CultureInfo? Culture = null ) => From is null ? Null : Default;

    /// <inheritdoc />
    public override bool ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Null;

    /// <inheritdoc />
    public override object? Reverse( bool To, object? Parameter = null, CultureInfo? Culture = null ) => null;
}
