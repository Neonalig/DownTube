#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a type of json serialisable data which automatically determines when values become dirty and provides a method to save/revert any changes.
/// </summary>
/// <seealso cref="IJsonSerialisable" />
public interface ISaveData : IJsonSerialisable {
    /// <summary>
    /// Gets a value indicating whether <see langword="this"/> instance is dirty.
    /// </summary>
    /// <value>
    ///   <see langword="true"/> if this instance is dirty; otherwise, <see langword="false"/>.
    /// </value>
    bool IsDirty { get; }

    /// <summary>
    /// Gets the names of the dirty properties.
    /// </summary>
    /// <returns>A collection of the dirty property names.</returns>
    IEnumerable<string> GetDirty();
}