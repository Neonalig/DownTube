#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using DownTube.DataTypes;

#endregion

namespace DownTube.Converters;

/// <inheritdoc cref="MVVMUtils.ValueConverter{TFrom, TTo}"/>
/// <typeparam name="TSelf">The deriving <see langword="class"/> type.</typeparam>
/// <typeparam name="TFrom">The value to convert data from.</typeparam>
/// <typeparam name="TTo">The value to convert data into.</typeparam>
public abstract class ValueConverter<TSelf, TFrom, TTo> : MVVMUtils.ValueConverter<TFrom, TTo>, IInstanced<TSelf> where TSelf : ValueConverter<TSelf, TFrom, TTo>, new() {

	/// <summary>
	/// Initialises a new instance of the <see cref="ValueConverter{TSelf, TFrom, TTo}"/> class.
	/// </summary>
	// ReSharper disable once EmptyConstructor //<-- Ensures deriving classes *must* implement a parameterless constructor
	protected ValueConverter() { }

	/// <summary>
	/// The lazy instance constructor.
	/// </summary>
	static readonly Lazy<TSelf> _LazyInstance = new Lazy<TSelf>(() => new TSelf());

	/// <inheritdoc />
	public static TSelf Instance => _LazyInstance.Value;
}