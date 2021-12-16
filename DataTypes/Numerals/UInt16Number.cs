#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes.Numerals;

public readonly struct UInt16Number : INumber<UInt16Number> {
    /// <summary> The value of the number. </summary>
    public UInt16 Value { get; }

    public UInt16Number( UInt16 Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static UInt16Number operator +( UInt16Number Left, UInt16Number Right ) => (ushort)(Left.Value + Right.Value);

    /// <inheritdoc />
    public static UInt16Number operator -( UInt16Number Left, UInt16Number Right ) => (ushort)(Left.Value - Right.Value);

    /// <inheritdoc />
    public static UInt16Number operator *( UInt16Number Left, UInt16Number Right ) => (ushort)(Left.Value * Right.Value);

    /// <inheritdoc />
    public static UInt16Number operator /( UInt16Number Left, UInt16Number Right ) => (ushort)(Left.Value / Right.Value);

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="UInt16Number(UInt16)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator UInt16Number( UInt16 Value ) => new UInt16Number(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator UInt16( UInt16Number Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="UInt16.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="UInt16.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="UInt16.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="UInt16.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}