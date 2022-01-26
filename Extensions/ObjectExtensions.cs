#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="object"/>.
/// </summary>
public static class ObjectExtensions {

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
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
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
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
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
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
    /// <param name="Res">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
    public static T CatchNull<T>( [NotNull] this Result<T>? Res, [CallerArgumentExpression("Res")] string? Expression = null ) {
        if ( Res is { Value: { } Val } ) {
            return Val;
        }
        // ReSharper disable ExceptionNotDocumented
        throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
        // ReSharper restore ExceptionNotDocumented
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
    public static T CatchNull<T>( [NotNull] this T? Obj, [CallerArgumentExpression("Obj")] string? Expression = null ) {
        if ( Obj is null ) {
            // ReSharper disable ExceptionNotDocumented
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
            // ReSharper restore ExceptionNotDocumented
        }
        return Obj;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TS">The type of the <see langword="struct"/>.</typeparam>
    /// <param name="Obj">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="Obj"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
    public static TS CatchNull<TS>( [NotNull] this TS? Obj, [CallerArgumentExpression("Obj")] string? Expression = null ) where TS : struct{
        if ( Obj is null ) {
            // ReSharper disable ExceptionNotDocumented
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
            // ReSharper restore ExceptionNotDocumented
        }
        return Obj.Value;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="AttemptResult">If <see langword="false" />, <paramref name="Result"/> is assumed <see langword="null"/>.</param>
    /// <param name="Result">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="Result"/> was <see langword="null"/>.</exception>
    [return: NotNullIfNotNull("Result")]
    [DebuggerStepThrough, DebuggerHidden]
    public static T CatchNull<T>( this bool AttemptResult, T? Result, [CallerArgumentExpression("AttemptResult")] string? Expression = null ) {
        if ( !AttemptResult || Result is null ) {
            // ReSharper disable ExceptionNotDocumented
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
            // ReSharper restore ExceptionNotDocumented
        }
        return Result;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="AttemptResult">If <see langword="false" />, <paramref name="Result"/> is assumed <see langword="null"/>.</param>
    /// <param name="Result">The object to test.</param>
    /// <param name="Expression">The name of the object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="Result"/> was <see langword="null"/>.</exception>
    [DebuggerStepThrough, DebuggerHidden]
    public static void ThrowIfNull<T>( this bool AttemptResult, [NotNull] T? Result, [CallerArgumentExpression("AttemptResult")] string? Expression = null ) {
        if ( !AttemptResult || Result is null ) {
            // ReSharper disable ExceptionNotDocumented
            throw new ArgumentNullException(Expression, $"Argument {Expression} was null.");
            // ReSharper restore ExceptionNotDocumented
        }
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
    /// <exception cref="OverflowException"><paramref name="Input" /> represents a number that is out of the range of <typeparamref name="TOut"/>.</exception>
    /// <exception cref="FormatException"><typeparamref name="TIn"/> is not in a format recognised by <typeparamref name="TOut"/>.</exception>
    [return: NotNullIfNotNull("Input")]
    public static TOut? Cast<TIn, TOut>( this TIn? Input, [CallerArgumentExpression("Input")] string? ParameterName = null ) {
        switch ( Input ) {
            case null:
                return default;
            case TOut Output:
                return Output;
            default:
                // ReSharper disable ExceptionNotDocumentedOptional
                if ( Convert.ChangeType(Input, typeof(TOut)) is TOut OutputConv ) {
                    // ReSharper restore ExceptionNotDocumentedOptional
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
    [return: NotNullIfNotNull("Input")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public static TOut? Cast<TOut>( this object? Input, [CallerArgumentExpression("Input")] string? ParameterName = null ) {
        switch ( Input ) {
            case null:
                return default;
            case TOut Output:
                return Output;
            default:
                // ReSharper disable ExceptionNotDocumentedOptional
                if ( Convert.ChangeType(Input, typeof(TOut)) is TOut OutputConv ) {
                    // ReSharper restore ExceptionNotDocumentedOptional
                    return OutputConv;
                }
                throw new InvalidCastException($"{ParameterName} of type {Input.GetType().Name} is unable to be cast to type {typeof(TOut)}");
        }
    }

    /// <summary>
    /// Invokes a function when the <paramref name="InputResult"/> value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <typeparam name="TOut">The type of the output.</typeparam>
    /// <param name="InputResult">The input value. Can be <see langword="null"/>.</param>
    /// <param name="WhenNotNull">The function to invoke when <paramref name="InputResult"/> is <b>not</b> <see langword="null"/>.</param>
    /// <param name="WhenNull">The value to return when <paramref name="InputResult"/> is <see langword="null"/>.</param>
    /// <returns>The output value of <paramref name="WhenNotNull"/> or <paramref name="WhenNull"/> depending on the given <paramref name="InputResult"/>.</returns>
    /// <exception cref="Exception">The <paramref name="WhenNotNull"/> <see langword="delegate"/> callback throws an exception.</exception>
    public static TOut WithValue<TIn, TOut>( this Result<TIn>? InputResult, Func<TIn, TOut> WhenNotNull, TOut WhenNull ) => InputResult is { WasSuccess: true } In ? WhenNotNull(In.Value!) : WhenNull;

    /// <summary>
    /// Invokes a function when the <paramref name="Input"/> value is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <typeparam name="TOut">The type of the output.</typeparam>
    /// <param name="Input">The input value. Can be <see langword="null"/>.</param>
    /// <param name="WhenNotNull">The function to invoke when <paramref name="Input"/> is <b>not</b> <see langword="null"/>.</param>
    /// <param name="WhenNull">The value to return when <paramref name="Input"/> is <see langword="null"/>.</param>
    /// <returns>The output value of <paramref name="WhenNotNull"/> or <paramref name="WhenNull"/> depending on the given <paramref name="Input"/>.</returns>
    /// <exception cref="Exception">The <paramref name="WhenNotNull"/> <see langword="delegate"/> callback throws an exception.</exception>
    public static TOut WithValue<TIn, TOut>( this TIn? Input, Func<TIn, TOut> WhenNotNull, TOut WhenNull ) => Input is { } In ? WhenNotNull(In) : WhenNull;

    /// <summary>
    /// Forces the try attempt into a value, or <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Success">Whether the attempt was successful.</param>
    /// <param name="Value">The attempt's value.</param>
    /// <returns>The attempt's return <paramref name="Value"/> or <see langword="null"/>.</returns>
    public static T? OrNull<T>( this bool Success, T? Value ) where T : struct => Success ? Value : default;

    /// <summary>
    /// Forces the try attempt into a value, or <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Success">Whether the attempt was successful.</param>
    /// <param name="Value">The attempt's value.</param>
    /// <returns>The attempt's return <paramref name="Value"/> or <see langword="null"/>.</returns>
    public static T? OrNull<T>( this bool Success, T? Value ) where T : class => Success ? Value : null;

}