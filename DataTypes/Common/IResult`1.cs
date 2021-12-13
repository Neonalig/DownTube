namespace DownTube.DataTypes.Common; 

/// <summary>
/// Represents the result of a process.
/// </summary>
public interface IResult<T> : IResult where T : IResult<T> {
    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <see cref="Result"/> to <see cref="bool"/>.
    /// </summary>
    /// <param name="Result">The result.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static abstract implicit operator bool( T Result );

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <see cref="Boolean"/> to <see cref="Result"/>.
    /// </summary>
    /// <param name="Success">If set to <see langword="true" />, the result is intended as successful (<c>0x0000</c>); otherwise the result is intended as an unexpected error (<c>0x0001</c>).</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static abstract implicit operator T( bool Success );
}