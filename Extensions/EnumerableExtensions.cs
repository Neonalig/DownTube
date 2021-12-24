#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

using System.Collections.ObjectModel;

namespace DownTube.Extensions;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/> types.
/// </summary>
public static class EnumerableExtensions {

    /// <summary>
    /// Selects all items of type <typeparamref name="TOut"/> from the given enumerable, where <paramref name="Func"/> doesn't return <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="Enum">The enumerable to iterate through.</param>
    /// <param name="Func">The function to invoke to retrieve an item of type <typeparamref name="TOut"/>. If the return value is <see langword="null"/>, the output is ignored.</param>
    /// <returns>A collection of type <typeparamref name="TOut"/>.</returns>
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public static IEnumerable<TOut> SelectSuchThat<TIn, TOut>( this IEnumerable<TIn> Enum, Func<TIn, TOut?> Func ) {
        foreach( TIn Item in Enum ) {
            if ( Func(Item) is { } Out ) {
                yield return Out;
            }
        }
    }

    /// <summary>
    /// Determines whether the <paramref name="Enum"/> contains the specific string.
    /// </summary>
    /// <param name="Enum">The enumerable to iterate.</param>
    /// <param name="Search">The string to search for.</param>
    /// <param name="ComparisonType">The type of string comparison.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="Enum"/> contains <paramref name="Search"/>; otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="ComparisonType" /> is not a <see cref="StringComparison" /> value.</exception>
    public static bool Contains(this IEnumerable<string> Enum, string Search, StringComparison ComparisonType ) {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach(string Item in Enum ) {
            if (Search.Equals(Item, ComparisonType) ) {
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc cref="Enumerable.Take{TSource}(IEnumerable{TSource}, int)"/>
    public static IEnumerable<T> Grab<T>( this IEnumerable<T> Enum, int Amount ) => Enum.Take( Amount);

    /// <inheritdoc cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/>
    // ReSharper disable once RedundantNullableFlowAttribute
    public static List<T> AsList<T>( this IEnumerable<T> Enum ) => Enum switch {
        List<T> Ls => Ls,
        // ReSharper disable once ExceptionNotDocumentedOptional
        _           => Enum.ToList()
    };

    /// <summary>
    /// Attempts to get the first item in the collection.
    /// </summary>
    /// <typeparam name="T">The enumerable containing type.</typeparam>
    /// <param name="Enum">The enumerable to iterate.</param>
    /// <param name="Found">The found item.</param>
    /// <returns><see langword="true"/> if an item was found; otherwise <see langword="false"/>.</returns>
    public static bool TryGetFirst<T>( this IEnumerable<T>? Enum, out T Found ) {
        if ( Enum is not null ) {
            foreach ( T Item in Enum ) {
                Found = Item;
                return true;
            }
        }
        Found = default!;
        return false;
    }

    /// <summary>
    /// Gets the first item in the collection, returning <see langword="default"/> if no items are found.
    /// </summary>
    /// <typeparam name="T">The enumerable containing type.</typeparam>
    /// <param name="Enum">The enumerable to iterate.</param>
    /// <returns>The first item in the collection, or <see langword="default"/>.</returns>
    public static T? GetFirstOrDefault<T>( this IEnumerable<T?>? Enum ) {
        if ( Enum is not null ) {
            foreach ( T? Item in Enum ) {
                return Item;
            }
        }
        return default;
    }

    /// <summary>
    /// Gets the read only equivalent of the specified dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="Dict">The dictionary.</param>
    /// <returns>A new <see cref="ReadOnlyDictionary{TKey, TValue}"/> instance with the same content as <paramref name="Dict"/>.</returns>
    public static ReadOnlyDictionary<TKey, TValue> GetReadOnly<TKey, TValue>( this IDictionary<TKey, TValue> Dict ) where TKey : notnull => new ReadOnlyDictionary<TKey, TValue>(Dict);

    /// <summary>
    /// Represents a subset of the given enumerable, starting at the specified index.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Enum">The enumerable.</param>
    /// <param name="StartIndex">The start index.</param>
    /// <returns>A subset of <paramref name="Enum"/> starting at index <paramref name="StartIndex"/> (inclusive).</returns>
    public static IEnumerable<T> Subset<T>( this IEnumerable<T> Enum, int StartIndex ) {
        int I = 0;
        foreach ( T Item in Enum ) {
            if ( I >= StartIndex ) {
                yield return Item;
            }
            I++;
        }
    }

    /// <summary>
    /// Represents a subset of the given enumerable, starting at the specified index, and returning <b>up to</b> the specified <paramref name="Length"/> (may return early if the collection is smaller).
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Enum">The enumerable.</param>
    /// <param name="StartIndex">The start index.</param>
    /// <param name="Length">The maximum amount of items to return.</param>
    /// <returns>A subset of <paramref name="Enum"/> starting at index <paramref name="StartIndex"/> (inclusive), and ending at <c><paramref name="StartIndex"/> + <paramref name="Length"/></c> or the length of the collection, whichever comes first.</returns>
    public static IEnumerable<T> Subset<T>( this IEnumerable<T> Enum, int StartIndex, int Length ) {
        int I = 0;
        foreach ( T Item in Enum ) {
            if ( I >= StartIndex ) {
                if ( (I - StartIndex) > Length ) { yield break; }
                yield return Item;
            }
            I++;
        }
    }

    /// <summary>
    /// Appends the specified collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="EnumA">The initial collection.</param>
    /// <param name="EnumB">The collection to append after the first.</param>
    /// <returns>The concatenation of <paramref name="EnumA"/> and <paramref name="EnumB"/>. </returns>
    public static IEnumerable<T> Append<T>( this IEnumerable<T> EnumA, IEnumerable<T> EnumB ) {
        foreach ( T ItemA in EnumA ) {
            yield return ItemA;
        }
        foreach ( T ItemB in EnumB ) {
            yield return ItemB;
        }
    }

    /// <summary>
    /// Appends the specified collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Item">The initial item.</param>
    /// <param name="EnumB">The collection to append after the first.</param>
    /// <returns>The concatenation of <paramref name="Item"/> and <paramref name="EnumB"/>. </returns>
    public static IEnumerable<T> Append<T>( this T Item, IEnumerable<T> EnumB ) {
        yield return Item;
        foreach ( T ItemB in EnumB ) {
            yield return ItemB;
        }
    }

    /// <summary>
    /// Appends the specified item to the collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="EnumA">The initial collection.</param>
    /// <param name="Item">The item to append after the collection.</param>
    /// <returns>The concatenation of <paramref name="EnumA"/> and <paramref name="Item"/>. </returns>
    public static IEnumerable<T> Append<T>( this IEnumerable<T> EnumA, T Item ) {
        foreach ( T ItemA in EnumA ) {
            yield return ItemA;
        }
        yield return Item;
    }

    /// <summary>
    /// Iterates through the enumerable collection, ignoring the elements inside.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Enum">The enumerable to iterate.</param>
    public static void Iterate<T>( this IEnumerable<T?>? Enum ) {
        if ( Enum is null ) { return; }
        foreach ( T? _ in Enum ) { }
    }
    /// <summary>
    /// Iterates through the enumerable collection, ignoring the elements inside.
    /// </summary>
    /// <param name="Enum">The enumerable to iterate.</param>
    public static void Iterate( this IEnumerable? Enum ) {
        if ( Enum is null ) { return; }
        foreach ( object? _ in Enum ) { }
    }

    /// <inheritdoc cref="Iterate{T}(IEnumerable{T?}?)"/>
    public static void FireAndForget<T>( this IEnumerable<T>? Enum ) => Iterate(Enum);

    /// <inheritdoc cref="Iterate(IEnumerable?)"/>
    public static void FireAndForget( this IEnumerable? Enum ) => Iterate(Enum);
}