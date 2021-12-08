namespace DownTube.DataTypes.Numerals;

/// <summary>
/// Represents any numeral type which supports addition, subtraction, multiplication and division.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public interface INumber<T> : INumber, IAdd<T, T, T>, ISubtract<T, T, T>, IMultiply<T, T, T>, IDivide<T, T, T> where T : IAdd<T, T, T>, ISubtract<T, T, T>, IMultiply<T, T, T>, IDivide<T, T, T> {
}

public interface INumber {
    /// <summary> The value of the number. </summary>
    internal dynamic InnerValue { get; }
}