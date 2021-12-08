using System.Diagnostics.CodeAnalysis;

namespace DownTube.DataTypes.Numerals;

public interface IMultiply<in TSelf, in TOther, out TResult> where TSelf : IMultiply<TSelf, TOther, TResult> {
    /// <inheritdoc cref="Multiply(TSelf, TOther)"/>
    [return: NotNull]
    public static abstract TResult operator *( [DisallowNull] TSelf Left, [DisallowNull] TOther Right );

    /// <summary>
    /// Calculates the product of two value types.
    /// </summary>
    /// <param name="Left">The left value.</param>
    /// <param name="Right">The right value.</param>
    /// <returns>The mathematical equivalent of <paramref name="Left"/><c> * </c><paramref name="Right"/></returns>
    [return: NotNull]
    public static TResult Multiply( [DisallowNull] TSelf Left, [DisallowNull] TOther Right ) => Left * Right;
}