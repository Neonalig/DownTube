#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes.Numerals;

public readonly struct Int16Number : INumber<Int16Number> {
    /// <summary> The value of the number. </summary>
    public Int16 Value { get; }

    public Int16Number( Int16 Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static Int16Number operator +( Int16Number Left, Int16Number Right ) => (short)(Left.Value + Right.Value);

    /// <inheritdoc />
    public static Int16Number operator -( Int16Number Left, Int16Number Right ) => (short)(Left.Value - Right.Value);

    /// <inheritdoc />
    public static Int16Number operator *( Int16Number Left, Int16Number Right ) => (short)(Left.Value * Right.Value);

    /// <inheritdoc />
    public static Int16Number operator /( Int16Number Left, Int16Number Right ) => (short)(Left.Value / Right.Value);

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="Int16Number(Int16)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator Int16Number( Int16 Value ) => new Int16Number(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator Int16( Int16Number Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="Int16.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="Int16.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="Int16.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="Int16.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}