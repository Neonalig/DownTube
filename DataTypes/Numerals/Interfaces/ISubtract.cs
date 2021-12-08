using System.Diagnostics.CodeAnalysis;

namespace DownTube.DataTypes.Numerals;

public interface ISubtract<in TSelf, in TOther, out TResult> where TSelf : ISubtract<TSelf, TOther, TResult> {
    /// <inheritdoc cref="Subtract(TSelf, TOther)"/>
    [return: NotNull]
    public static abstract TResult operator -( [DisallowNull] TSelf Left, [DisallowNull] TOther Right );

    /// <summary>
    /// Calculates the subtraction of one value type from another.
    /// </summary>
    /// <param name="Left">The left value.</param>
    /// <param name="Right">The right value.</param>
    /// <returns>The mathematical equivalent of <paramref name="Left"/><c> - </c><paramref name="Right"/></returns>
    [return: NotNull]
    public static TResult Subtract( [DisallowNull] TSelf Left, [DisallowNull] TOther Right ) => Left - Right;
}