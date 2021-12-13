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
    public static IEnumerable<T> Grab<T>( this IEnumerable<T> Enum, int Amount ) => Enumerable.Take(Enum, Amount);

    /// <inheritdoc cref="Enumerable.ToList{TSource}(IEnumerable{TSource})"/>
    public static List<T> AsList<T>( this IEnumerable<T> Enum ) => Enum switch {
        List<T> Ls => Ls,
        // ReSharper disable once ExceptionNotDocumentedOptional
        _           => Enum.ToList()
    };
}