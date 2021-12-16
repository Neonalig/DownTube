#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes.Numerals;

public readonly struct SByteNumber : INumber<SByteNumber> {
    /// <summary> The value of the number. </summary>
    public SByte Value { get; }

    public SByteNumber( SByte Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static SByteNumber operator +( SByteNumber Left, SByteNumber Right ) => (sbyte)(Left.Value + Right.Value);

    /// <inheritdoc />
    public static SByteNumber operator -( SByteNumber Left, SByteNumber Right ) => (sbyte)(Left.Value - Right.Value);

    /// <inheritdoc />
    public static SByteNumber operator *( SByteNumber Left, SByteNumber Right ) => (sbyte)(Left.Value * Right.Value);

    /// <inheritdoc />
    public static SByteNumber operator /( SByteNumber Left, SByteNumber Right ) => (sbyte)(Left.Value / Right.Value);

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="SByteNumber(SByte)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator SByteNumber( SByte Value ) => new SByteNumber(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator SByte( SByteNumber Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="SByte.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="SByte.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="SByte.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="SByte.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}