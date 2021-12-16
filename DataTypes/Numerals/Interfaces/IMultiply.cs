#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;

#endregion

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