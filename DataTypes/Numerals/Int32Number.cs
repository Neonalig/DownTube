#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes.Numerals;

public readonly struct Int32Number : INumber<Int32Number> {
    /// <summary> The value of the number. </summary>
    public Int32 Value { get; }

    public Int32Number( Int32 Value = default) => this.Value = Value;

    /// <inheritdoc />
    public static Int32Number operator +( Int32Number Left, Int32Number Right ) => Left.Value + Right.Value;

    /// <inheritdoc />
    public static Int32Number operator -( Int32Number Left, Int32Number Right ) => Left.Value - Right.Value;

    /// <inheritdoc />
    public static Int32Number operator *( Int32Number Left, Int32Number Right ) => Left.Value * Right.Value;

    /// <inheritdoc />
    public static Int32Number operator /( Int32Number Left, Int32Number Right ) => Left.Value / Right.Value;

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="Int32Number(Int32)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator Int32Number( Int32 Value ) => new Int32Number(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator Int32( Int32Number Number ) => Number.Value;
    
    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="Int32.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="Int32.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="Int32.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="Int32.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}