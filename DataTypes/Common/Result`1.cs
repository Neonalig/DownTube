#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace DownTube.DataTypes.Common; 

/// <summary>
/// Represents the result of a process, along with the return value.
/// </summary>
public readonly struct Result<T> : IResult<Result<T>> {
    /// <inheritdoc />
    public bool WasSuccess { get; }

    /// <inheritdoc />
    public int ErrorCode { get; }

    /// <inheritdoc />
    public string Message { get; }

    /// <summary>
    /// Gets the return value of the result.
    /// </summary>
    /// <value>
    /// The return value of the result.
    /// </value>
    [MemberNotNullWhen(true, nameof(WasSuccess))] public T? Value { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="Value">The return value of the result.</param>
    /// <param name="Success">If set to <see langword="true" />, the result is intended as successful (<c>0x0000</c>); otherwise the result is intended as an unexpected error (<c>0x0001</c>).</param>
    /// <param name="ErrorCode">The error code.</param>
    /// <param name="Message">The message.</param>
    public Result( T? Value, bool Success, int ErrorCode, string Message ) {
        this.Value = Value;
        WasSuccess = Success;
        this.ErrorCode = ErrorCode;
        this.Message = Message;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="Value">The return value of the result. If <see langword="null" />, the result is intended as an unexpected error (<c>0x0001</c>); otherwise the result is intended as successful (<c>0x0000</c>).</param>
    public Result( T? Value = default ) {
        this.Value = Value;
        WasSuccess = Value is not null;
        if ( WasSuccess ) {
            ErrorCode = 0x0000;
            Message = "WasSuccess.";
        } else {
            ErrorCode = 0x0001;
            Message = "An unexpected error occurred.";
        }
    }

    /// <summary>
    /// Represents a <see langword="null"/> reference of type <see cref="T"/>.
    /// </summary>
    static readonly T? _NoData = (T?)(dynamic)null!;

    /// <inheritdoc />
    public static implicit operator bool( Result<T> Result ) => Result.WasSuccess;

    // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
    /// <inheritdoc />
#pragma warning disable IDE0034 // Simplify 'default' expression
    public static implicit operator Result<T>( bool Success ) => new Result<T>(Success ? default(T) : _NoData);
#pragma warning restore IDE0034 // Simplify 'default' expression

    /// <inheritdoc />
    public static implicit operator Result<T>( Exception? Ex ) => Ex switch {
        { } E => new Result<T>(_NoData, false, E.HResult, Ex.ToString()),
        // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
#pragma warning disable IDE0034 // Simplify 'default' expression
        _     => new Result<T>(default(T), true, 0x0000, "WasSuccess")
#pragma warning restore IDE0034 // Simplify 'default' expression
    };

    /// <summary>
    /// Determines whether the result was a success, returning the <paramref name="Value"/> if successful.
    /// </summary>
    /// <param name="Value">The result value.</param>
    /// <returns><see langword="true"/> if the result was successful; otherwise <see langword="false"/>.</returns>
    public bool IsSuccess( out T Value ) {
        Value = this.Value!;
        return WasSuccess;
    }

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <typeparamref name="T"/> to <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator Result<T>( T? Value ) => new Result<T>(Value);

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <see cref="Result{T}"/> to <see cref="T"/>.
    /// </summary>
    /// <param name="Result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator T?(Result<T> Result) => Result.Value;

    /// <summary>
    /// Performs an explicit conversion from <see cref="Result{T}"/> to <see cref="Result"/>.
    /// </summary>
    /// <param name="ResultWithValue">The result with a value.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static explicit operator Result( Result<T> ResultWithValue ) => new Result(ResultWithValue.WasSuccess, ResultWithValue.ErrorCode, ResultWithValue.Message);

    /// <summary>
    /// Performs an explicit conversion from <see cref="Result"/> to <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="Result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static explicit operator Result<T>( Result Result ) => new Result<T>(Result.WasSuccess ? default(T) : _NoData, Result.WasSuccess, Result.ErrorCode, Result.Message);

    /// <summary>
    /// Outputs the value of the result.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <returns>Whether the result was a success.</returns>
    public bool Out( out T Value ) {
        Value = this.Value!;
        return WasSuccess;
    }
    /// <summary>
    /// Outputs the value of the result.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="Result">The result.</param>
    /// <returns>Whether the result was a success.</returns>
    public bool Out( out T Value, out Result<T> Result ) {
        Result = this;
        Value = this.Value!;
        return WasSuccess;
    }

    /// <inheritdoc />
    public static Result<T> Success { get; } = new Result<T>(default, true, 0x0000, "WasSuccess.");

    /// <inheritdoc />
    public static Result<T> Unexpected { get; } = new Result<T>(_NoData, false, 0x0001, "An unexpected error occurred.");

    /// <summary>
    /// Retrieves the result, throwing an <see cref="ArgumentNullException"/> if the given value is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>The original object (if not <see langword="null"/>).</returns>
    public T CatchNull() {
        if ( Value is { } Val ) {
            return Val;
        }
        // ReSharper disable ExceptionNotDocumented
        throw new ArgumentNullException(nameof(Value), $"{nameof(Value)} was null.");
        // ReSharper restore ExceptionNotDocumented
    }
}