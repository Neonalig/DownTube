namespace DownTube.DataTypes.Numerals;

public readonly struct ByteNumber : INumber<ByteNumber> {
    /// <summary> The value of the number. </summary>
    public Byte Value { get; }

    public ByteNumber( Byte Value = default ) => this.Value = Value;

    /// <inheritdoc />
    public static ByteNumber operator +( ByteNumber Left, ByteNumber Right ) => (byte)(Left.Value + Right.Value);

    /// <inheritdoc />
    public static ByteNumber operator -( ByteNumber Left, ByteNumber Right ) => (byte)(Left.Value - Right.Value);

    /// <inheritdoc />
    public static ByteNumber operator *( ByteNumber Left, ByteNumber Right ) => (byte)(Left.Value * Right.Value);

    /// <inheritdoc />
    public static ByteNumber operator /( ByteNumber Left, ByteNumber Right ) => (byte)(Left.Value / Right.Value);

    // ReSharper disable once InvalidXmlDocComment
    /// <summary>
    /// Constructs a new instance via the <see cref="ByteNumber(Byte)"/> constructor.
    /// </summary>
    /// <param name="Value">The value of the number.</param>
    public static implicit operator ByteNumber( Byte Value ) => new ByteNumber(Value);

    /// <summary>
    /// Retrieves the current <see cref="Value"/> of the <paramref name="Number"/>.
    /// </summary>
    /// <param name="Number">The number to retrieve the <see cref="Value"/> from.</param>
    public static implicit operator Byte( ByteNumber Number ) => Number.Value;

    /// <inheritdoc />
    dynamic INumber.InnerValue => Value;

    /// <inheritdoc cref="Byte.ToString()"/>
    public override string ToString() => Value.ToString();

    /// <inheritdoc cref="Byte.ToString(IFormatProvider?)"/>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString(IFormatProvider? Provider) => Value.ToString(Provider);

    /// <inheritdoc cref="Byte.ToString(string?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    public string ToString(string? Format) => Value.ToString(Format);

    /// <inheritdoc cref="Byte.ToString(string?, IFormatProvider?)"/>
    /// <param name="Format">A standard or custom numeric format string.</param>
    /// <param name="Provider">An object that supplies culture-specific formatting information.</param>
    public string ToString( string? Format, IFormatProvider? Provider ) => Value.ToString(Format, Provider);
}