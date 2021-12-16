namespace DownTube.DataTypes.Common;

/// <summary>
/// Represents the result of a process.
/// </summary>
public interface IResult {
    /// <summary>
    /// Whether the result was successful.
    /// </summary>
    bool WasSuccess { get; }

    /// <summary>
    /// The error code of the result.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    ///     <term>Code</term>
    ///     <description>Meaning</description>
    /// </listheader>
    /// <item>
    ///     <term>0x0000</term>
    ///     <description><c>Success.</c></description>
    /// </item>
    /// <item>
    ///     <term>0x0001</term>
    ///     <description><c>An unexpected error occurred.</c></description>
    /// </item>
    /// <item>
    ///     <term>0x0002+</term>
    ///     <description>Consult the calling library or read <see cref="Message"/> for more information.</description>
    /// </item>
    /// </list></remarks>
    int ErrorCode { get; }
    
    /// <summary>
    /// The message relating to the <see cref="ErrorCode"/>.
    /// </summary>
    string Message { get; }
}