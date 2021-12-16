#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

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