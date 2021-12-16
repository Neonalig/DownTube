using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

using Ardalis.SmartEnum;

using DownTube.Engine;

namespace DownTube;

/// <summary>
/// Represents an error known at compile-time.
/// </summary>
public sealed class KnownError : SmartEnum<KnownError> {

    #region Enum Fields

    //--- General ---//

    /// <summary> <b>0x0000</b> - WasSuccess. </summary>
    public static readonly KnownError Success = new KnownError(nameof(Success), 0x0000, "WasSuccess.");

    /// <summary> <b>0x0001</b> - An unexpected error occurred. </summary>
    public static readonly KnownError Unexpected = new KnownError(nameof(Unexpected), 0x0001, "An unexpected error occurred.");

    /// <summary> <b>0x0002</b> - An argument was <see langword="null"/> that was not expected to be <see langword="null"/>. </summary>
    public static readonly KnownError NullArg = new KnownError(nameof(NullArg), 0x0002, "An argument was null that was not expected to be null.");

    //--- DownloadEngine ---//

    /// <summary> <b>0x0003</b> - <see cref="Props.YoutubeDLPath"/> property was <see langword="null"/>. Have you made sure to download youtube-dl.exe and set it's location on the Settings page? </summary>
    public static readonly KnownError NoYoutubeDL = new KnownError(nameof(NoYoutubeDL), 0x0003, "YoutubeDLPath property was null. Have you made sure to download youtube-dl.exe and set it's location on the Settings page?");

    /// <summary> <b>0x0004</b> - <see cref="Props.FFmpegPath"/> property was <see langword="null"/>. Have you made sure to download ffmpeg.exe and set it's location on the Settings page? </summary>
    public static readonly KnownError NoFFmpeg = new KnownError(nameof(NoFFmpeg), 0x0004, "FFmpegPath property was null. Have you made sure to download ffmpeg.exe and set it's location on the Settings page?");

    /// <summary> <b>0x0005</b> - <see cref="Props.OutputFolder"/> property was <see langword="null"/>. Have you made sure to set the download folder to a valid destination on the Settings page? </summary>
    public static readonly KnownError NoOutputFolder = new KnownError(nameof(NoOutputFolder), 0x0005, "OutputFolder property was null. Have you made sure to set the download folder to a valid destination on the Settings page?");


    //--- Reflection ---//

    /// <summary> <b>0x0006</b> - The <see langword="object"/> was not of an expected type. </summary>
    public static readonly KnownError UnexpectedType = new KnownError(nameof(UnexpectedType), 0x0006, "The object was not of an expected type.");

    #endregion

    #region Dynamic Errors

    /// <summary>
    /// Constructs a dynamic variant of the <see cref="NullArg"/> error related to the specific <paramref name="MemberName"/>.
    /// </summary>
    /// <param name="MemberName">The name of the member.</param>
    /// <returns>A dynamic <see cref="NullArg"/> variant.</returns>
    public static KnownError GetNullArgError( [CallerMemberName] string? MemberName = null ) => new KnownError(nameof(NullArg), 0x0002, $"The '{MemberName}' argument was null when it was expected to not be null.");


    /// <summary>
    /// Constructs a dynamic variant of the <see cref="NullArg"/> error related to the specific <paramref name="ArgName"/>.
    /// </summary>
    /// <param name="Obj">The argument to retrieve the name from.</param>
    /// <param name="ArgName">The name of the argument.</param>
    /// <returns>A dynamic <see cref="NullArg"/> variant.</returns>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used in CallerArgumentExpression attribute.")]
    public static KnownError GetNullArgError( object? Obj, [CallerArgumentExpression("Obj")] string? ArgName = null ) => new KnownError(nameof(NullArg), 0x0002, $"'{ArgName}' was null when it was expected to not be null.");

    /// <summary>
    /// Constructs a dynamic variant of the <see cref="UnexpectedType"/> error related to the specific <see cref="object"/>.
    /// </summary>
    /// <param name="ObjName">The name of the object.</param>
    /// <param name="ActualType">The (unexpected) type of the actual object.</param>
    /// <param name="WantedType">The expected type.</param>
    /// <returns>A dynamic <see cref="UnexpectedType"/> variant.</returns>
    public static KnownError GetUnexpectedTypeError( string ObjName, Type ActualType, Type WantedType ) => new KnownError(nameof(UnexpectedType), 0x0006, $"{ObjName} ({ActualType.GetTypeName()}) was not of the expected type {WantedType.GetTypeName()}.");

    /// <inheritdoc cref="GetUnexpectedTypeError"/>
    /// <param name="Obj">The object.</param>
    /// <param name="WantedType">The expected type.</param>
    /// <param name="ObjName">The name of the object.</param>
    public static KnownError GetUnexpectedTypeError( object Obj, Type WantedType, [CallerArgumentExpression("Obj")] string? ObjName = null ) => GetUnexpectedTypeError(ObjName ?? string.Empty, Obj.GetType(), WantedType);

    /// <inheritdoc cref="GetUnexpectedTypeError"/>
    /// <typeparam name="TWanted">The expected type.</typeparam>
    /// <param name="Obj">The object.</param>
    /// <param name="ObjName">The name of the object.</param>
    public static KnownError GetUnexpectedTypeError<TWanted>( object Obj, [CallerArgumentExpression("Obj")] string? ObjName = null ) => GetUnexpectedTypeError(ObjName ?? string.Empty, Obj.GetType(), typeof(TWanted));

    /// <inheritdoc cref="GetUnexpectedTypeError"/>
    /// <typeparam name="TActual">The (unexpected) type of the actual object.</typeparam>
    /// <typeparam name="TWanted">The expected type.</typeparam>
    /// <param name="Obj">The object.</param>
    /// <param name="ObjName">The name of the object.</param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used in CallerArgumentExpression attribute.")]
    public static KnownError GetUnexpectedTypeError<TActual, TWanted>( TActual Obj, [CallerArgumentExpression("Obj")] string? ObjName = null ) => GetUnexpectedTypeError(ObjName ?? string.Empty, typeof(TActual), typeof(TWanted));

    /// <inheritdoc cref="GetUnexpectedTypeError"/>
    /// <param name="ObjName">The name of the object.</param>
    /// <param name="ActualType">The (unexpected) type of the actual object.</param>
    /// <param name="WantedTypes">The collection of expected types.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented"),
     SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static KnownError GetUnexpectedTypeError( string ObjName, Type ActualType, Type[] WantedTypes ) {
        StringBuilder SB = new StringBuilder();
        int L = WantedTypes.Length;
        for ( int I = 0; I < L; I++ ) {
            SB.Append(WantedTypes[I].GetTypeName());
            SB.Append(I == L - 1 ? " or " : ", ");
        }

        return new KnownError(nameof(UnexpectedType), 0x0006, $"{ObjName} ({ActualType.GetTypeName()}) was not any of the expected types ({SB}).");
    }

    /// <inheritdoc cref="GetUnexpectedTypeError(string,System.Type,System.Type[])"/>
    /// <param name="Obj">The actual object.</param>
    /// <param name="WantedTypes">The collection of expected types.</param>
    /// <param name="ObjName">The name of the object.</param>
    public static KnownError GetUnexpectedTypeError( object Obj, Type[] WantedTypes, [CallerArgumentExpression("Obj")] string? ObjName = null ) => GetUnexpectedTypeError(ObjName ?? string.Empty, Obj.GetType(), WantedTypes);

    /// <inheritdoc cref="GetUnexpectedTypeError(string,System.Type,System.Type[])"/>
    /// <typeparam name="TActual">The (unexpected) type of the actual object.</typeparam>
    /// <param name="Obj">The actual object.</param>
    /// <param name="WantedTypes">The collection of expected types.</param>
    /// <param name="ObjName">The name of the object.</param>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used in CallerArgumentExpression attribute.")]
    public static KnownError GetUnexpectedTypeError<TActual>( TActual Obj, Type[] WantedTypes, [CallerArgumentExpression("Obj")] string? ObjName = null ) => GetUnexpectedTypeError(ObjName ?? string.Empty, typeof(TActual), WantedTypes);

    #endregion

    #region Boilerplate

    /// <inheritdoc />
    /// <param name="Name">The name of the enum property.</param>
    /// <param name="ErrorCode">The value (and error code) of the enum property.</param>
    /// <param name="Message">The error message of the enum property.</param>
    KnownError( string Name, int ErrorCode, string Message ) : base(Name, ErrorCode) => this.Message = Message;

    /// <summary>
    /// The diagnostics message related to the error.
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// Gets the error code for the error.
    /// </summary>
    /// <value>
    /// The error code.
    /// </value>
    public int ErrorCode => Value;

    /// <summary>
    /// Performs an explicit conversion from <see cref="KnownError"/> to <see cref="Result"/>.
    /// </summary>
    /// <param name="Err">The error to convert.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static explicit operator Result( KnownError Err ) => new Result(Err.Value == 0x0000, Err.Value, Err.Message);

    /// <summary>
    /// Gets the equivalent <see cref="Result"/> instance.
    /// </summary>
    /// <returns>A new <see cref="Result"/> instance based on <see cref="ErrorCode"/> and <see cref="Message"/>.</returns>
    public Result GetResult() => (Result)this;

    /// <summary>
    /// Gets the equivalent <see cref="Result{T}"/> instance.
    /// </summary>
    /// <remarks>If this error type is known successful, <see cref="Result{T}.Value"/> will be equivalent to <see langword="default"/>(<typeparamref name="T"/>); otherwise <see langword="null"/> will be used instead.</remarks>
    /// <typeparam name="T">The result return type.</typeparam>
    /// <returns>A new <see cref="Result{T}"/> instance based on <see cref="ErrorCode"/> and <see cref="Message"/>.</returns>
    public Result<T> GetResult<T>() => (Result<T>)(Result)this;

    #endregion

}