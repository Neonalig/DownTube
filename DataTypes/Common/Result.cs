#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes.Common;

/// <summary>
/// Represents the result of a process.
/// </summary>
public readonly struct Result : IResult<Result> {
    /// <inheritdoc />
    public bool WasSuccess { get; }

    /// <inheritdoc />
    public int ErrorCode { get; }

    /// <inheritdoc />
    public string Message { get; }


    /// <inheritdoc />
    public static Result Success { get; } = new Result(true);

    /// <inheritdoc />
    public static Result Unexpected { get; } = new Result(false);

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="Success">If set to <see langword="true" />, the result is intended as successful (<c>0x0000</c>); otherwise the result is intended as an unexpected error (<c>0x0001</c>).</param>
    /// <param name="ErrorCode">The error code.</param>
    /// <param name="Message">The message.</param>
    public Result( bool Success, int ErrorCode, string Message ) {
        WasSuccess = Success;
        this.ErrorCode = ErrorCode;
        this.Message = Message;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="Success">If set to <see langword="true" />, the result is intended as successful (<c>0x0000</c>); otherwise the result is intended as an unexpected error (<c>0x0001</c>).</param>
    public Result( bool Success = true ) {
        WasSuccess = Success;
        if ( Success ) {
            ErrorCode = 0x0000;
            Message = "WasSuccess.";
        } else {
            ErrorCode = 0x0001;
            Message = "An unexpected error occurred.";
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="MaybeNullValue">If <see langword="null"/>, the result is intended as an unexpected error (<c>0x0001</c>); otherwise the result is intended as successful (<c>0x0000</c>).</param>
    public Result( object? MaybeNullValue = null ) : this(MaybeNullValue is not null) { }

    /// <inheritdoc />
    public static implicit operator bool( Result Result ) => Result.WasSuccess;

    /// <inheritdoc />
    public static implicit operator Result( bool Success ) => new Result(Success);

    /// <inheritdoc />
    public static implicit operator Result( Exception? Ex ) => Ex switch {
        { } E => new Result(false, E.HResult, Ex.ToString()),
        _     => new Result(true, 0x0000, "WasSuccess")
    };
}