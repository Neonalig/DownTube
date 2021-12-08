namespace DownTube.Extensions; 

public static class EnumerableExtensions {

    /// <summary>
    /// Selects all items of type <typeparamref name="TOut"/> from the given enumerable, where <paramref name="Func"/> doesn't return <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="Enum">The enumerable to iterate through.</param>
    /// <param name="Func">The function to invoke to retrieve an item of type <typeparamref name="TOut"/>. If the return value is <see langword="null"/>, the output is ignored.</param>
    /// <returns>A collection of type <typeparamref name="TOut"/>.</returns>
    public static IEnumerable<TOut> SelectSuchThat<TIn, TOut>( this IEnumerable<TIn> Enum, Func<TIn, TOut?> Func ) {
        foreach( TIn Item in Enum ) {
            if ( Func(Item) is { } Out ) {
                yield return Out;
            }
        }
    }

}