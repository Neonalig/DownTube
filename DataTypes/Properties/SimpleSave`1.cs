#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

namespace DownTube.DataTypes.Properties;

/// <summary>
/// Simplifies some of the basic implementation of the <see cref="ISave"/> <see langword="interface"/>.
/// </summary>
/// <typeparam name="T">The intended value type to provide equality checking of.</typeparam>
/// <seealso cref="ISave" />
public abstract class SimpleSave<T> : SimpleSave {
    
    /// <summary>
    /// Determines whether the two values are different.
    /// </summary>
    /// <param name="Left">The left operand, or <see langword="null"/>.</param>
    /// <param name="Right">The right operand, or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="Left"/> and <paramref name="Right"/> are different values.</returns>
    public static bool IsDifferent( T? Left, T? Right ) => Left is null ? Right is not null : Right is null || !Left.Equals(Right);
}