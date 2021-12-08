namespace DownTube.DataTypes.Numerals;

public readonly struct UInt32Number : INumber<UInt32Number> {
    /// <summary> The value of the number. </summary>
    public UInt32 Value { get; }

    public UInt32Number( UInt32 Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static UInt32Number operator +( UInt32Number Left, UInt32Number Right ) => Left.Value + Right.Value;

    /// <inheritdoc />
    public static UInt32Number operator -( UInt32Number Left, UInt32Number Right ) => Left.Value - Right.Value;

    /// <inheritdoc />
    public static UInt32Number operator *( UInt32Number Left, UInt32Number Right ) => Left.Value * Right.Value;

    /// <inheritdoc />
    public static UInt32Number operator /( UInt32Number Left, UInt32Number Right ) => Left.Value / Right.Value;

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="UInt32Number(UInt32)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator UInt32Number( UInt32 Value ) => new UInt32Number(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator UInt32( UInt32Number Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="UInt32.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="UInt32.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( IFormatProvider? Provider ) => Value.ToString(Provider);

    /// <inheritdoc cref="UInt32.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString( string? Format ) => Value.ToString(Format);

    /// <inheritdoc cref="UInt32.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}