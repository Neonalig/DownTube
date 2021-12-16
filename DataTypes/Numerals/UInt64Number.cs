#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes.Numerals;

public readonly struct UInt64Number : INumber<UInt64Number> {
    /// <summary> The value of the number. </summary>
    public UInt64 Value { get; }

    public UInt64Number( UInt64 Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static UInt64Number operator +( UInt64Number Left, UInt64Number Right ) => Left.Value + Right.Value;

    /// <inheritdoc />
    public static UInt64Number operator -( UInt64Number Left, UInt64Number Right ) => Left.Value - Right.Value;

    /// <inheritdoc />
    public static UInt64Number operator *( UInt64Number Left, UInt64Number Right ) => Left.Value * Right.Value;

    /// <inheritdoc />
    public static UInt64Number operator /( UInt64Number Left, UInt64Number Right ) => Left.Value / Right.Value;

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="UInt64Number(UInt64)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator UInt64Number( UInt64 Value ) => new UInt64Number(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator UInt64( UInt64Number Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="UInt64.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="UInt64.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="UInt64.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="UInt64.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}