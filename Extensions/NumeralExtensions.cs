#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace DownTube.Extensions;

[SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
public static class NumeralExtensions {

	/// <summary>
	/// Determines the minimum item in the collection.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="Enum">The enumerable to iterate.</param>
	/// <returns>The minimum item, or <see langword="null"/> if the collection is empty.</returns>
	public static T? Min<T>( this IEnumerable<T> Enum ) where T : IComparable<T> {
		T? Min = default;

		foreach ( T Item in Enum ) {
			if ( Min is null || Item.CompareTo(Min) < 0 ) {
				Min = Item;
			}
		}
		return Min;
	}

	/// <inheritdoc cref="Min{T}(IEnumerable{T})"/>
	/// <param name="Item">The initial item to check.</param>
	/// <param name="Items">The additional items to check.</param>
	public static T Min<T>( this T Item, params T[] Items ) where T : IComparable<T> => Min(Item.Append(Items)) ?? Item;

	/// <summary>
	/// Determines the maximum item in the collection.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="Enum">The enumerable to iterate.</param>
	/// <returns>The maximum item, or <see langword="null"/> if the collection is empty.</returns>
	public static T? Max<T>( this IEnumerable<T> Enum ) where T : IComparable<T> {
		T? Max = default;

		foreach ( T Item in Enum ) {
			if ( Max is null || Item.CompareTo(Max) > 0 ) {
				Max = Item;
			}
		}
		return Max;
	}

	/// <inheritdoc cref="Max{T}(IEnumerable{T})"/>
	/// <param name="Item">The initial item to check.</param>
	/// <param name="Items">The additional items to check.</param>
	public static T Max<T>( this T Item, params T[] Items ) where T : IComparable<T> => Max(Item.Append(Items)) ?? Item;
}
