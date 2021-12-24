using System.Diagnostics.CodeAnalysis;

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
