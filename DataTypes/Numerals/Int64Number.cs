namespace DownTube.DataTypes.Numerals;

public readonly struct Int64Number : INumber<Int64Number> {
    /// <summary> The value of the number. </summary>
    public Int64 Value { get; }

    public Int64Number( Int64 Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static Int64Number operator +( Int64Number Left, Int64Number Right ) => Left.Value + Right.Value;

    /// <inheritdoc />
    public static Int64Number operator -( Int64Number Left, Int64Number Right ) => Left.Value - Right.Value;

    /// <inheritdoc />
    public static Int64Number operator *( Int64Number Left, Int64Number Right ) => Left.Value * Right.Value;

    /// <inheritdoc />
    public static Int64Number operator /( Int64Number Left, Int64Number Right ) => Left.Value / Right.Value;

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="Int64Number(Int64)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator Int64Number( Int64 Value ) => new Int64Number(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator Int64( Int64Number Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="Int64.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="Int64.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="Int64.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="Int64.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}