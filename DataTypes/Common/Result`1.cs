using System.Diagnostics.CodeAnalysis;

namespace DownTube.DataTypes.Common; 

/// <summary>
/// Represents the result of a process, along with the return value.
/// </summary>
public readonly struct Result<T> : IResult<Result<T>> {
    /// <inheritdoc />
    public bool Success { get; }

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
    [MemberNotNullWhen(true, nameof(Success))] public T? Value { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="Value">The return value of the result.</param>
    /// <param name="Success">If set to <see langword="true" />, the result is intended as successful (<c>0x0000</c>); otherwise the result is intended as an unexpected error (<c>0x0001</c>).</param>
    /// <param name="ErrorCode">The error code.</param>
    /// <param name="Message">The message.</param>
    public Result( T? Value, bool Success, int ErrorCode, string Message ) {
        this.Value = Value;
        this.Success = Success;
        this.ErrorCode = ErrorCode;
        this.Message = Message;
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="Value">The return value of the result. If <see langword="null" />, the result is intended as an unexpected error (<c>0x0001</c>); otherwise the result is intended as successful (<c>0x0000</c>).</param>
    public Result( T? Value = default ) {
        this.Value = Value;
        Success = Value is not null;
        if ( Success ) {
            ErrorCode = 0x0000;
            Message = "Success.";
        } else {
            ErrorCode = 0x0001;
            Message = "An unexpected error occurred.";
        }
    }

    /// <inheritdoc />
    public static implicit operator bool( Result<T> Result ) => Result.Success;

    /// <inheritdoc />
    public static implicit operator Result<T>( bool Success ) => new Result<T>(Success ? default : (T?)(dynamic)null!);

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <typeparamref name="T"/> to <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator Result<T>( T? Value ) => new Result<T>(Value);
}