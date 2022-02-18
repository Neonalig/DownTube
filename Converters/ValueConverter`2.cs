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
public abstract class ValueConverter<TFrom, TTo> : MVVMUtils.ValueConverter<TFrom, TTo>, IInstanced<ValueConverter<TFrom, TTo>> {

	/// <summary>
	/// Initialises a new instance of the <see cref="ValueConverter{TFrom, TTo}"/> class.
	/// </summary>
	// ReSharper disable once EmptyConstructor //<-- Ensures deriving classes *must* implement a parameterless constructor
	protected ValueConverter() { }

	/// <summary>
	/// The lazy instance constructor.
	/// </summary>
	static readonly Lazy<ValueConverter<TFrom, TTo>> _LazyInstance = new Lazy<ValueConverter<TFrom, TTo>>(Activator.CreateInstance<ValueConverter<TFrom, TTo>>);

	/// <inheritdoc />
	public static ValueConverter<TFrom, TTo> Instance => _LazyInstance.Value;
}