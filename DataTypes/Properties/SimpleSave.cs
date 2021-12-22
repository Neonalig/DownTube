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
/// <seealso cref="ISave" />
public abstract class SimpleSave : ISave {
    /// <inheritdoc />
    public abstract bool IsDirty { get; }

    /// <inheritdoc />
    public abstract void Save();

    /// <inheritdoc />
    public abstract void Revert();

    /// <summary>
    /// Raises the <see cref="BecameClean"/> event.
    /// </summary>
    /// <param name="E">The <see cref="BecameCleanEventArgs"/> instance containing the event data.</param>
    public void OnBecameClean( BecameCleanEventArgs E ) => BecameClean?.Invoke(this, E);

    /// <summary>
    /// Raises the <see cref="BecameClean"/> event with the default arguments.
    /// </summary>
    public void OnBecameClean() => OnBecameClean(new BecameCleanEventArgs());

    /// <summary>
    /// Raises the <see cref="BecameDirty"/> event.
    /// </summary>
    /// <param name="E">The <see cref="BecameDirtyEventArgs"/> instance containing the event data.</param>
    public void OnBecameDirty( BecameDirtyEventArgs E ) => BecameDirty?.Invoke(this, E);

    /// <summary>
    /// Raises the <see cref="BecameDirty"/> event with the default arguments.
    /// </summary>
    public void OnBecameDirty() => OnBecameDirty(new BecameDirtyEventArgs());

    /// <inheritdoc />
    public event BecameCleanEventHandler? BecameClean;

    /// <inheritdoc />
    public event BecameDirtyEventHandler? BecameDirty;
}