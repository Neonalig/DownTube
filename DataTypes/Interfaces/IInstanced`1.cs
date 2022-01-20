#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a <see langword="class"/> which provides an instance of itself.
/// </summary>
/// <typeparam name="T">The type of the deriving <see langword="class"/>.</typeparam>
public interface IInstanced<out T> where T : IInstanced<T> {

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <value>
    /// The singleton.
    /// </value>
    public static abstract T Instance { get; }

}