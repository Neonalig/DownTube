#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

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
}