using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DownTube.Extensions;

public static class ObjectExtensions {

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was null.</exception>
    public static void ThrowIfNull( [NotNull] this object? Obj, [CallerArgumentExpression("Obj")] string? Expression = null )  {
        if ( Obj is null ) {
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was null.</exception>
    public static void ThrowIfNull<T>( [NotNull] this T? Obj, [CallerArgumentExpression("Obj")] string? Expression = null ) {
        if ( Obj is null ) {
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was null.</exception>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    public static object CatchNull( [NotNull] this object? Obj, [CallerArgumentExpression("Obj")] string? Expression = null ) {
        if ( Obj is null ) {
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
        }
        return Obj;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was null.</exception>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    public static T CatchNull<T>( [NotNull] this T? Obj, [CallerArgumentExpression("Obj")] string? Expression = null ) {
        if ( Obj is null ) {
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
        }
        return Obj;
    }

    /// <summary>
    /// Attempts to retrieve the string value from the <paramref name="Obj"/>.
    /// If the object is of type <see cref="string"/>, the raw value is returned.
    /// Otherwise, <see cref="object.ToString()"/> is invoked and the value returned.
    /// Finally, if <paramref name="Obj"/> is <see langword="null"/>, then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="Obj">The object to retrieve a string value from.</param>
    /// <returns>The <see cref="string"/> representation of <paramref name="Obj"/>.</returns>
    [return: NotNullIfNotNull("Obj")]
    public static string? GetString( this object? Obj ) => Obj switch {
        string Str => Str,
        { } O      => O.ToString(),
        _          => null
    };

    /// <summary>
    /// Casts the given <paramref name="Input"/> to type <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TIn">The input value type.</typeparam>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="Input">The input value to cast.</param>
    /// <param name="ParameterName">The name of the input value.</param>
    /// <returns>A cast value of type <typeparamref name="TOut"/>.</returns>
    /// <exception cref="InvalidCastException"><paramref name="Input"/> of type <typeparamref name="TIn"/> is unable to be cast to type <typeparamref name="TOut"/>.</exception>
    [return: NotNullIfNotNull("Input")]
    public static TOut? Cast<TIn, TOut>( this TIn? Input, [CallerArgumentExpression("Input")] string? ParameterName = null ) {
        switch ( Input ) {
            case null:
                return default;
            case TOut Output:
                return Output;
            default:
                if ( Convert.ChangeType(Input, typeof(TOut)) is TOut OutputConv ) {
                    return OutputConv;
                }
                throw new InvalidCastException($"{ParameterName} of type {typeof(TIn)} is unable to be cast to type {typeof(TOut)}");
        }
    }

    /// <summary>
    /// Casts the given <paramref name="Input"/> to type <typeparamref name="TOut"/>.
    /// </summary>
    /// <typeparam name="TOut">The output value type.</typeparam>
    /// <param name="Input">The input value to cast.</param>
    /// <param name="ParameterName">The name of the input value.</param>
    /// <returns>A cast value of type <typeparamref name="TOut"/>.</returns>
    /// <exception cref="InvalidCastException"><paramref name="Input"/> is unable to be cast to type <typeparamref name="TOut"/>.</exception>
    [return: NotNullIfNotNull("Input")]
    public static TOut? Cast<TOut>( this object? Input, [CallerArgumentExpression("Input")] string? ParameterName = null ) {
        switch ( Input ) {
            case null:
                return default;
            case TOut Output:
                return Output;
            default:
                if ( Convert.ChangeType(Input, typeof(TOut)) is TOut OutputConv ) {
                    return OutputConv;
                }
                throw new InvalidCastException($"{ParameterName} of type {Input?.GetType().Name ?? "<NULL>"} is unable to be cast to type {typeof(TOut)}");
        }
    }

}