using System.Diagnostics.CodeAnalysis;

namespace DownTube.DataTypes.Numerals;

public interface IAdd<in TSelf, in TOther, out TResult> where TSelf : IAdd<TSelf, TOther, TResult> {
    /// <inheritdoc cref="Add(TSelf, TOther)"/>
    [return: NotNull]
    public static abstract TResult operator +( [DisallowNull] TSelf Left, [DisallowNull] TOther Right );

    /// <summary>
    /// Calculates the sum of two value types.
    /// </summary>
    /// <param name="Left">The left value.</param>
    /// <param name="Right">The right value.</param>
    /// <returns>The mathematical equivalent of <paramref name="Left"/><c> + </c><paramref name="Right"/></returns>
    [return: NotNull]
    public static TResult Add( [DisallowNull] TSelf Left, [DisallowNull] TOther Right ) => Left + Right;
}