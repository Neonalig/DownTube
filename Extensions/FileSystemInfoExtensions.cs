#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Security;

using DownTube.Engine;

using Newtonsoft.Json;

#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="FileSystemInfo"/>.
/// </summary>
public static class FileSystemInfoExtensions {

    /// <summary>
    /// Attempts to convert the string value into a <see cref="FileInfo"/> instance, returning <see langword="true"/> if successful.
    /// </summary>
    /// <param name="Path">The path to convert</param>
    /// <param name="File">The resultant instance.</param>
    /// <returns><see langword="true"/> if conversion was successful.</returns>
    public static bool TryGetFile( this string Path, out FileInfo File ) {
        try {
            File = new FileInfo(Path);
            return true;
            // ReSharper disable once CatchAllClause
        } catch {
            File = null!;
            return false;
        }
    }

    /// <summary>
    /// Attempts to convert the string value into a <see cref="DirectoryInfo"/> instance, returning <see langword="true"/> if successful.
    /// </summary>
    /// <param name="Path">The path to convert</param>
    /// <param name="Dir">The resultant instance.</param>
    /// <returns><see langword="true"/> if conversion was successful.</returns>
    public static bool TryGetDirectory( this string Path, out DirectoryInfo Dir ) {
        try {
            Dir = new DirectoryInfo(Path);
            return true;
            // ReSharper disable once CatchAllClause
        } catch {
            Dir = null!;
            return false;
        }
    }

    /// <summary>
    /// Forces a 'try' method, returning the value if successful, or throwing an <see cref="ArgumentNullException"/> if not.
    /// </summary>
    /// <typeparam name="T">The output data type.</typeparam>
    /// <param name="Success">Whether the try method was successful.</param>
    /// <param name="Value">The <see langword="out"/> value from the try method.</param>
    /// <returns>The passed <paramref name="Value"/>.</returns>
    /// <exception cref="ArgumentNullException">The try method returned <see langword="null"/>.</exception>
    public static T Force<T>( [DoesNotReturnIf(false)] this bool Success, T Value ) => Success ? Value : throw new ArgumentNullException(nameof(Value));


    /// <summary>
    /// Forces a 'try' method, returning the value if successful, or throwing an <see cref="ArgumentNullException"/> if not.
    /// </summary>
    /// <typeparam name="T">The output data type.</typeparam>
    /// <param name="Result">The result t force.</param>
    /// <returns>The passed <paramref name="Result.Value"/>.</returns>
    /// <exception cref="ArgumentNullException"><see cref="Result{T}.Value"/> was <see langword="null"/>.</exception>
    public static T Force<T>( this Result<T?> Result ) => Result.Value ?? throw new ArgumentNullException(nameof(Result));

    /// <summary>
    /// Gets a <see cref="FileInfo"/> instance pointing to the given <paramref name="Path"/>.
    /// </summary>
    /// <param name="Path">The path to the file.</param>
    /// <returns>A new <see cref="FileInfo"/> instance.</returns>
    public static Result<FileInfo> GetFile( this string Path ) {
        try {
            return new FileInfo(Path);
        } catch ( SecurityException SecEx ) {
            return SecEx;
        } catch ( UnauthorizedAccessException UnAuthEx ) {
            return UnAuthEx;
        } catch ( ArgumentException ArgEx ) {
            return ArgEx;
        } catch ( PathTooLongException LongPathEx ) {
            return LongPathEx;
        } catch ( NotSupportedException NoSuppEx ) {
            return NoSuppEx;
        }
    }

    /// <summary>
    /// Gets a <see cref="DirectoryInfo"/> instance pointing to the given <paramref name="Path"/>.
    /// </summary>
    /// <param name="Path">The path to the file.</param>
    /// <returns>A new <see cref="DirectoryInfo"/> instance.</returns>
    public static Result<DirectoryInfo> GetDirectory( this string Path ) {
        try {
            return new DirectoryInfo(Path);
        } catch ( SecurityException SecEx ) {
            return SecEx;
        } catch ( ArgumentNullException ArgNullEx ) {
            return ArgNullEx;
        } catch ( ArgumentException ArgEx ) {
            return ArgEx;
        } catch ( PathTooLongException LongPathEx ) {
            return LongPathEx;
        }
    }

    /// <summary>
    /// Gets a <see cref="DirectoryInfo"/> instance pointing to the given <paramref name="SpecialFolder"/>.
    /// </summary>
    /// <param name="SpecialFolder">The special folder to point the instance towards.</param>
    /// <returns>A new <see cref="DirectoryInfo"/> instance.</returns>
    /// <exception cref="ArgumentException"><paramref name="SpecialFolder" /> is not a member of <see cref="Environment.SpecialFolder" />.</exception>
    /// <exception cref="PlatformNotSupportedException">The current platform is not supported.</exception>
    /// <exception cref="ArgumentNullException">The try method returned <see langword="null"/>.</exception>
    public static DirectoryInfo GetDirectory( this Environment.SpecialFolder SpecialFolder ) => TryGetDirectory(Environment.GetFolderPath(SpecialFolder), out DirectoryInfo DI).Force(DI);

    /// <summary>
    /// Creates a <see cref="FileInfo"/> instance pointing to a file in the given <paramref name="Directory"/>, and creates the file if it does not already exist.
    /// </summary>
    /// <param name="Directory">The parent directory.</param>
    /// <param name="FileName">The name of the file to create.</param>
    /// <param name="Create">Whether to create the file if it doesn't exist already.</param>
    /// <returns>A new <see cref="FileInfo"/> instance relative to the <paramref name="Directory"/>.</returns>
    /// <exception cref="ArgumentException">.NET Framework and .NET Core versions older than 2.1: <paramref name="Directory" /> or <paramref name="FileName" /> contains one or more of the invalid characters defined in <see cref="Path.GetInvalidPathChars" />.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="Directory" /> or <paramref name="FileName" /> is <see langword="null" />.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The fully qualified path and file name exceed the system-defined maximum length.</exception>
    public static FileInfo CreateSubfile( this DirectoryInfo Directory, string FileName, bool Create = true ) {
        FileInfo? F = Path.Combine(Directory.FullName.Replace('/', '\\').TrimEnd('\\') + '\\', FileName).GetFile().Value;
        switch ( F ) {
            case { } FI when Create && !FI.Exists:
                FI.Create().Dispose();
                return FI;
        }
        return F ?? throw new ArgumentNullException(nameof(FileName));
    }

    /// <summary>
    /// Creates a <see cref="Uri"/> instance pointing to <see cref="FileSystemInfo.FullName"/>.
    /// </summary>
    /// <param name="FSI">The path to point towards.</param>
    /// <returns>A new <see cref="Uri"/> instance.</returns>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The fully qualified path and file name exceed the system-defined maximum length.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="FSI" /> is <see langword="null" />.</exception>
    /// <exception cref="UriFormatException">Note: In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="FormatException" />, instead.
    /// <paramref name="FSI" /> is empty.  
    ///  -or-  
    ///  The scheme specified in <paramref name="FSI" /> is not correctly formed. See <see cref="String" />).  
    ///  -or-  
    ///  <paramref name="FSI" /> contains too many slashes.  
    ///  -or-  
    ///  The password specified in <paramref name="FSI" /> is not valid.  
    ///  -or-  
    ///  The host name specified in <paramref name="FSI" /> is not valid.  
    ///  -or-  
    ///  The file name specified in <paramref name="FSI" /> is not valid.  
    ///  -or-  
    ///  The user name specified in <paramref name="FSI" /> is not valid.  
    ///  -or-  
    ///  The host or authority name specified in <paramref name="FSI" /> cannot be terminated by backslashes.  
    ///  -or-  
    ///  The port number specified in <paramref name="FSI" /> is not valid or cannot be parsed.  
    ///  -or-  
    ///  The length of <paramref name="FSI" /> exceeds 65519 characters.  
    ///  -or-  
    ///  The length of the scheme specified in <paramref name="FSI" /> exceeds 1023 characters.  
    ///  -or-  
    ///  There is an invalid character sequence in <paramref name="FSI" />.  
    ///  -or-  
    ///  The MS-DOS path specified in <paramref name="FSI" /> must start with c:\.</exception>
    public static Uri GetUri( this FileSystemInfo FSI ) => new Uri(FSI.FullName);

    /// <summary> The path to the application executable. </summary>
    public static readonly FileInfo App = new FileInfo(Assembly.GetExecutingAssembly().Location);

    /// <summary> The folder containing the application executable. </summary>
    public static readonly DirectoryInfo AppDir = App.Directory!;

    /// <summary> The logical desktop directory. </summary>
    public static readonly DirectoryInfo Desktop = Environment.SpecialFolder.Desktop.GetDirectory();

    /// <summary>
    /// The default JsonSerialiser used when one isn't specified.
    /// </summary>
    public static readonly JsonSerializer DefaultJsonSerialiser = new JsonSerializer {
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// Serialises the given <see langword="object"/> into the <paramref name="Destination"/> file as json data.
    /// </summary>
    /// <param name="Obj">The object to serialise.</param>
    /// <param name="Destination">The destination file.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    /// <exception cref="UnauthorizedAccessException">The path specified when creating an instance of the <see cref="FileInfo" /> object is read-only or is a directory.</exception>
    /// <exception cref="DirectoryNotFoundException">The path specified when creating an instance of the <see cref="FileInfo" /> object is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="ArgumentException">The <paramref name="Destination"/> <see cref="FileStream"/> is not writable.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Destination"/> <see cref="FileStream"/> is <see langword="null" />.</exception>
    public static void Serialise( this object Obj, FileInfo Destination, JsonSerializer? Serialiser = null) {
        using (FileStream FS = Destination.OpenWrite() ) {
            using (StreamWriter SW = new StreamWriter(FS) ) {
                using (JsonTextWriter JTW = new JsonTextWriter(SW) ) {
                    (Serialiser ?? DefaultJsonSerialiser).Serialize(JTW, Obj);
                }
            }
        }
    }

    /// <summary>
    /// Serialises the given data into the <paramref name="Destination"/> file as json data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="Val">The data to serialise.</param>
    /// <param name="Destination">The destination file.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    /// <exception cref="UnauthorizedAccessException">The path specified when creating an instance of the <see cref="FileInfo" /> object is read-only or is a directory.</exception>
    /// <exception cref="DirectoryNotFoundException">The path specified when creating an instance of the <see cref="FileInfo" /> object is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="ArgumentException">The <paramref name="Destination"/> <see cref="FileStream"/> is not writable.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="Destination"/> <see cref="FileStream"/> is <see langword="null" />.</exception>
    public static void Serialise<T>( this T Val, FileInfo Destination, JsonSerializer? Serialiser = null ) {
        using ( FileStream FS = Destination.OpenWrite() ) {
            using ( StreamWriter SW = new StreamWriter(FS) ) {
                using ( JsonTextWriter JTW = new JsonTextWriter(SW) ) {
                    (Serialiser ?? DefaultJsonSerialiser).Serialize(JTW, Val);
                }
            }
        }
    }

    /// <summary>
    /// Deserialises the json data stored in the <paramref name="Location"/> file, and constructs a new object instance with the deserialised data.
    /// </summary>
    /// <typeparam name="T">The data type to deserialise into.</typeparam>
    /// <param name="Location">The file to read from.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    /// <returns>A new instance of <typeparamref name="T"/> as provided by <see cref="JsonSerializer.Deserialize{T}(JsonReader)"/></returns>
    public static Result<T> Deserialise<T>( this FileInfo Location, JsonSerializer? Serialiser = null ) {
        try {
            using ( FileStream FS = Location.OpenRead() ) {
                using ( StreamReader SR = new StreamReader(FS) ) {
                    using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                        return (Serialiser ?? DefaultJsonSerialiser).Deserialize<T>(JTR);
                    }
                }
            }
        } catch ( UnauthorizedAccessException UnAuthEx ) {
            return UnAuthEx;
        } catch ( DirectoryNotFoundException NotFoundEx ) {
            return NotFoundEx;
        } catch ( IOException IOEx ) {
            return IOEx;
        } catch ( ArgumentException ArgEx ) {
            return ArgEx;
        }
    }

    /// <summary>
    /// Deserialises the json data stored in the <paramref name="Location"/> file, and constructs a new object instance with the deserialised data.
    /// </summary>
    /// <typeparam name="T">The data type to deserialise into.</typeparam>
    /// <param name="Location">The file to read from.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    /// <param name="Token">The cancellation token.</param>
    /// <returns>A new instance of <typeparamref name="T"/> as provided by <see cref="JsonSerializer.Deserialize{T}(JsonReader)"/></returns>
    /// <exception cref="IOException">The file is already open.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="ArgumentException"><paramref name="Location"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="Location"/> is <see langword="null" />.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The fully qualified path and file name exceed the system-defined maximum length.</exception>
    /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
    public static async Task<T?> DeserialiseAsync<T>( this FileInfo Location, JsonSerializer? Serialiser = null, CancellationToken Token = default ) {
        string Text = await File.ReadAllTextAsync(Location.FullName, Token);
        using (StreamReader SR = new StreamReader(Text) ) {
            using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                return (Serialiser ?? DefaultJsonSerialiser).Deserialize<T>(JTR);
            }
        }
    }

    /// <summary>
    /// Gets a <see cref="bool"/> indicating whether the file exists or not.
    /// </summary>
    /// <param name="FI">The file to check.</param>
    /// <returns><see langword="true"/> if the file currently exists.</returns>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The fully qualified path and file name exceed the system-defined maximum length.</exception>
    public static bool GetExists( this FileInfo? FI ) => FI is not null && File.Exists(FI.FullName);

    /// <summary>
    /// Gets a <see cref="bool"/> indicating whether the directory exists or not.
    /// </summary>
    /// <param name="DI">The directory to check.</param>
    /// <returns><see langword="true"/> if the directory currently exists.</returns>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="PathTooLongException">The fully qualified path exceeds the system-defined maximum length.</exception>
    public static bool GetExists( this DirectoryInfo? DI ) => DI is not null && Directory.Exists(DI.FullName);

    /// <summary>
    /// Resolves the specified directory (expanding symbolic links and correcting capitalisation).
    /// <para/><example>
    /// <b>Example:</b>
    /// '<c>c:/windows\system32</c>' becomes '<c>C:\Windows\System32\</c>'
    /// </example>
    /// </summary>
    /// <remarks>As per the WIN32 standard, all slashes become backslashes ('\'), and the path will end with a backslash.</remarks>
    /// <param name="DI">The directory.</param>
    /// <returns>A fully resolved <see cref="DirectoryInfo"/> instance.</returns>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static DirectoryInfo Resolve( this DirectoryInfo DI ) => new DirectoryInfo(PInvoke.GetFinalPathName(DI.FullName) + '\\');

    /// <summary>
    /// Resolves the specified file (expanding symbolic links and correcting capitalisation).
    /// <para/><example>
    /// <b>Example:</b>
    /// '<c>c:/windows\system32/calc.exe</c>' becomes '<c>C:\Windows\System32\calc.exe</c>'
    /// </example>
    /// </summary>
    /// <param name="FI">The file.</param>
    /// <returns>A fully resolved <see cref="FileInfo"/> instance.</returns>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static FileInfo Resolve( this FileInfo FI ) => new FileInfo(PInvoke.GetFinalPathName(FI.FullName));
}