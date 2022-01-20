#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// A <see langword="class"/> which provides a singleton instance of itself.
/// </summary>
/// <typeparam name="T">The type of the deriving <see langword="class"/></typeparam>
/// <seealso cref="IInstanced{T}"/>
public abstract class Instanced<T> : IInstanced<T> where T : Instanced<T> {

	/// <summary>
	/// The lazy instance constructor.
	/// </summary>
	static readonly Lazy<T> _LazyInstance = new Lazy<T>(Activator.CreateInstance<T>);

	/// <inheritdoc />
	public static T Instance => _LazyInstance.Value;
}