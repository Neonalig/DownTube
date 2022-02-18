#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Globalization;

#endregion

namespace DownTube.Converters;

public class BoolToDoubleConverter : ValueConverter<bool, double> {
    /// <summary>
    /// Gets or sets the value returned when <see langword="true"/>.
    /// </summary>
    /// <value>
    /// The <see langword="true"/> return value.
    /// </value>
    public double True { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the value returned when <see langword="false"/>.
    /// </summary>
    /// <value>
    /// The <see langword="false"/> return value.
    /// </value>
    public double False { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the value returned when <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The <see langword="null"/> return value.
    /// </value>
    public double Null { get; set; } = double.NaN;

    /// <summary>
    /// Gets or sets the value returned when performing a reverse conversion and the value is neither <see cref="True"/>, <see cref="False"/> or <see cref="Null"/>.
    /// </summary>
    /// <value>
    /// The default return value for reverse conversions.
    /// </value>
    public bool ReverseDefault { get; set; } = false;
    /// <summary>
    /// Gets or sets the value returned when performing a reverse conversion with a <see langword="null"/> value.
    /// </summary>
    /// <value>
    /// The return value for <see langword="null"/> reverse conversions.
    /// </value>
    public bool ReverseNull { get; set; } = false;

    /// <inheritdoc />
    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override bool CanReverse => true;

    /// <inheritdoc />
    public override bool CanReverseWhenNull => true;

    /// <inheritdoc />
    public override double Forward( bool From, object? Parameter = null, CultureInfo? Culture = null ) => From ? True : False;

    public override double ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Null;

    /// <inheritdoc />
    public override bool Reverse( double To, object? Parameter = null, CultureInfo? Culture = null ) => To == True || To != False && ReverseDefault;
    //To == True ? true : To == False ? false : ReverseDefault
    //To == True ? true : To != False && ReverseDefault
    //To == True || To != False && ReverseDefault

    /// <inheritdoc />
    public override bool ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => ReverseNull;
}
