#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a simple type which supports saving/loading.
/// </summary>
public interface ISave {
    /// <summary>
    /// Gets a value indicating whether the <see cref="ISave"/> data is dirty.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if dirty; otherwise, <see langword="false" />.
    /// </value>
    bool IsDirty { get; }

    /// <summary>
    /// Saves the <see cref="ISave"/> data.
    /// </summary>
    void Save();

    /// <summary>
    /// Reverts the unsaved changes.
    /// </summary>
    void Revert();

    /// <summary>
    /// <see langword="event"/> raised when the data has just became clean (i.e. it was just saved / reverted.)
    /// </summary>
    event BecameCleanEventHandler BecameClean;

    /// <summary>
    /// <see langword="event"/> raised when the data has just became dirty (i.e. it was just changed and hasn't yet been saved.)
    /// </summary>
    event BecameDirtyEventHandler BecameDirty;
}