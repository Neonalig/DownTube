#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Globalization;

#endregion

namespace DownTube.Converters;

public abstract class BoolToValueConverter<T> : ValueConverter<bool, T> {
	/// <summary>
	/// Gets or sets the value to return when the input is <see langword="true"/>.
	/// </summary>
	/// <value>
	/// The value returned when the input is <see langword="true"/>.
	/// </value>
	public T? True { get; set; }

	/// <summary>
	/// Gets or sets the value to return when the input is <see langword="false"/>.
	/// </summary>
	/// <value>
	/// The value returned when the input is <see langword="false"/>.
	/// </value>
	public T? False { get; set; }

	/// <summary>
	/// Gets or sets the value to return when the input is <see langword="null"/>.
	/// </summary>
	/// <value>
	/// The value returned when the input is <see langword="null"/>.
	/// </value>
	public T? Null { get; set; }

	/// <inheritdoc />
	public override bool CanForward => true;

	/// <inheritdoc />
	public override T? Forward( bool From, object? Parameter = null, CultureInfo? Culture = null ) => From ? True : False;

	/// <inheritdoc />
	public override bool CanForwardWhenNull => true;

	/// <inheritdoc />
	public override T? ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Null;

	/// <summary>
	/// The default equality comparer for the value type.
	/// </summary>
	internal static readonly EqualityComparer<T?> Eq = EqualityComparer<T?>.Default;

	/// <inheritdoc />
	public override bool CanReverse => true;

	/// <inheritdoc />
	public override bool Reverse( T To, object? Parameter = null, CultureInfo? Culture = null ) => Eq.Equals(To, True) || (Eq.Equals(To, False) ? false : throw new NotSupportedException());

	/// <inheritdoc />
	public override bool CanReverseWhenNull => true;

	/// <inheritdoc />
	public override bool ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Eq.Equals(default, True) || (Eq.Equals(default, False) ? false : throw new NotSupportedException());
}