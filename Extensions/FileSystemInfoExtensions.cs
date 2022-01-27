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

using DownTube.DataTypes;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using PInvoke = DownTube.Engine.PInvoke;

#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="FileSystemInfo"/>.
/// </summary>
public static class FileSystemInfoExtensions {

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
    public static Result<FileInfo> GetFile( this string? Path ) {
        if ( Path is null ) {
            return KnownError.NullArg.GetResult<FileInfo>();
        }
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
    public static Result<DirectoryInfo> GetDirectory( this string? Path ) {
        if ( Path is null ) {
            return KnownError.NullArg.GetResult<DirectoryInfo>();
        }
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
    public static Result<DirectoryInfo> GetDirectory( this Environment.SpecialFolder SpecialFolder ) => GetDirectory(Environment.GetFolderPath(SpecialFolder));

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
    public static readonly DirectoryInfo Desktop = Environment.SpecialFolder.Desktop.GetDirectory().Value!;

    /// <summary> The logical downloads directory. </summary>
    public static readonly DirectoryInfo Downloads = KnownFolder.Downloads.Path.GetDirectory().Value!;

    /// <summary>
    /// The default JsonSerialiser used when one isn't specified.
    /// </summary>
    public static readonly JsonSerializer DefaultJsonSerialiser = new JsonSerializer {
        Formatting = Formatting.Indented,
        Converters = {
            new StringEnumConverter()
        }
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
    public static void Serialise( this object Obj, FileInfo Destination, JsonSerializer? Serialiser = null ) {
        using ( FileStream FS = Destination.OpenWrite() ) {
            using ( StreamWriter SW = new StreamWriter(FS) ) {
                using ( JsonTextWriter JTW = new JsonTextWriter(SW) ) {
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
        using ( FileStream FS = Destination.Exists
                   ? File.Open(Destination.FullName, FileMode.Truncate, FileAccess.Write)
                   : Destination.Create() ) {
            using ( StreamWriter SW = new StreamWriter(FS) ) {
                using ( JsonTextWriter JTW = new JsonTextWriter(SW) ) {
                    (Serialiser ?? DefaultJsonSerialiser).Serialize(JTW, Val);
                }
            }
        }
    }

    /// <summary>
    /// Serialises the given data into json data stored in a <see cref="string"/>.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="Val">The data to serialise.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    public static string Serialise<T>( this T Val, JsonSerializer? Serialiser = null ) {
        using ( StringWriter SW = new StringWriter() ) {
            using ( JsonTextWriter JTW = new JsonTextWriter(SW) ) {
                (Serialiser ?? DefaultJsonSerialiser).Serialize(JTW, Val);
                return SW.ToString();
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
        } catch ( JsonSerializationException JSerEx ) {
            return JSerEx;
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
        using ( StreamReader SR = new StreamReader(Text) ) {
            using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                return (Serialiser ?? DefaultJsonSerialiser).Deserialize<T>(JTR);
            }
        }
    }

    /// <summary>
    /// Deserialises the given json data into a new instance.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    /// <param name="Str">The data to deserialise.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    public static Result<T> Deserialise<T>( this string Str, JsonSerializer? Serialiser = null ) {
        try {
            using ( StringReader SR = new StringReader(Str) ) {
                using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                    T? Return = (Serialiser ?? DefaultJsonSerialiser).Deserialize<T>(JTR);
                    if ( Return is null ) {
                        return KnownError.GetNullArgError(nameof(Return)).GetResult<T>();
                    }
                    return Return;
                }
            }
        } catch ( Exception Ex ) {
            return Ex;
        }
    }

    /// <summary>
    /// Deserialises the given json data into a new instance.
    /// </summary>
    /// <param name="Str">The data to deserialise.</param>
    /// <param name="Tp">The data type.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    public static object? Deserialise( this string Str, Type Tp, JsonSerializer? Serialiser = null ) {
        using ( StringReader SR = new StringReader(Str) ) {
            using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                return (Serialiser ?? DefaultJsonSerialiser).Deserialize(JTR, Tp);
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

    /// <summary>
    /// Moves the given file to a new location, overwriting any files if one already exists.
    /// </summary>
    /// <remarks>If the destination directory does not exist, it will be created.</remarks>
    /// <param name="File">The file to move.</param>
    /// <param name="Dest">The destination. If the destination already exists, it will be overwritten.</param>
    /// <exception cref="IOException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentNullException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="SecurityException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="UnauthorizedAccessException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="FileNotFoundException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="DirectoryNotFoundException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="PathTooLongException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    public static void MoveTo( this FileInfo File, FileInfo Dest ) {
        if ( Dest.Directory is { Exists: false } Dir ) {
            Dir.Create();
        }
        if ( Dest.Exists ) {
            File.MoveTo(Dest.FullName, true);
        } else {
            File.MoveTo(Dest.FullName);
        }
    }

    /// <summary>
    /// Moves the given file to a new location, overwriting any files if one already exists.
    /// </summary>
    /// <remarks>If the destination directory does not exist, it will be created.</remarks>
    /// <param name="File">The file to move.</param>
    /// <param name="Dir">The destination directory. If the directory does not exist, it will be created.</param>
    /// <returns>The new location of the moved file.</returns>
    /// <exception cref="IOException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentNullException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="SecurityException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="UnauthorizedAccessException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="FileNotFoundException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="DirectoryNotFoundException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="PathTooLongException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    public static FileInfo MoveTo( this FileInfo File, DirectoryInfo Dir ) {
        if ( !Dir.Exists ) {
            Dir.Create();
        }
        FileInfo Dest = new FileInfo(Path.Combine(Dir.FullName, File.Name));
        MoveTo(File, Dest);
        return Dest;
    }

    /// <summary>
    /// Moves the given directory to a new location. If the destination already exists, it will become a subdirectory.
    /// </summary>
    /// <remarks>The destination folder name cannot be identical to the original folder name.</remarks>
    /// <param name="Location">The location.</param>
    /// <param name="Destination">The destination.</param>
    /// <exception cref="ArgumentNullException"><inheritdoc cref="DirectoryInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentException"><inheritdoc cref="DirectoryInfo.MoveTo(string)"/></exception>
    /// <exception cref="IOException"><inheritdoc cref="DirectoryInfo.MoveTo(string)"/></exception>
    /// <exception cref="SecurityException"><inheritdoc cref="DirectoryInfo.MoveTo(string)"/></exception>
    /// <exception cref="DirectoryNotFoundException"><inheritdoc cref="DirectoryInfo.MoveTo(string)"/></exception>
    public static void MoveTo( this DirectoryInfo Location, DirectoryInfo Destination ) => Location.MoveTo(Destination.FullName);

    /// <summary>
    /// Renames the specified file.
    /// </summary>
    /// <param name="File">The file to rename.</param>
    /// <param name="NewName">The new name.</param>
    /// <param name="ChangeExtension">If <see langword="true" />, the extension will be overwritten; otherwise the extension will remain the same.</param>
    /// <exception cref="IOException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentNullException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="ArgumentException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="SecurityException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="UnauthorizedAccessException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="FileNotFoundException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="DirectoryNotFoundException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="PathTooLongException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    /// <exception cref="NotSupportedException"><inheritdoc cref="FileInfo.MoveTo(string)"/></exception>
    public static void Rename( this FileInfo File, string NewName, bool ChangeExtension ) {
        FileInfo Dest = new FileInfo(
            ChangeExtension switch {
                true => Path.Combine(File.DirectoryName ?? string.Empty, NewName),
                _    => Path.Combine(File.DirectoryName ?? string.Empty, Path.GetFileNameWithoutExtension(NewName)) + File.Extension
            });
        MoveTo(File, Dest);
    }

    /// <summary>
    /// Renames the specified directory.
    /// </summary>
    /// <param name="Dir">The directory to rename.</param>
    /// <param name="NewName">The new name.</param>
    /// <exception cref="IOException"><inheritdoc cref="Directory.Move(string, string)"/></exception>
    /// <exception cref="UnauthorizedAccessException"><inheritdoc cref="Directory.Move(string, string)"/></exception>
    /// <exception cref="ArgumentException"><inheritdoc cref="Directory.Move(string, string)"/></exception>
    /// <exception cref="ArgumentNullException"><inheritdoc cref="Directory.Move(string, string)"/></exception>
    /// <exception cref="PathTooLongException"><inheritdoc cref="Directory.Move(string, string)"/></exception>
    /// <exception cref="DirectoryNotFoundException"><inheritdoc cref="Directory.Move(string, string)"/></exception>
    public static void Rename( this DirectoryInfo Dir, string NewName ) => Directory.Move(Dir.FullName, Path.Combine(Dir.Parent?.Name ?? Dir.Root.Name, NewName));

    /// <summary>
    /// Gets the children directories.
    /// </summary>
    /// <param name="Dir">The parent directory.</param>
    /// <param name="WildCard">The search pattern. Leave '<c>*</c>' for all.</param>
    /// <param name="Recurse">If <see langword="true" />, grandchildren are checked as well; otherwise only the immediate children are checked.</param>
    /// <returns>The found files with the common ancestor <paramref name="Dir"/>.</returns>
    public static DirectoryInfo[] GetChildrenDirectories( this DirectoryInfo Dir, string WildCard = "*", bool Recurse = true ) => Dir.GetDirectories(WildCard, Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Gets the children files.
    /// </summary>
    /// <param name="Dir">The parent directory.</param>
    /// <param name="WildCard">The search pattern. Leave '<c>*</c>' for all.</param>
    /// <param name="Recurse">If <see langword="true" />, grandchildren are checked as well; otherwise only the immediate children are checked.</param>
    /// <returns>The found files with the common ancestor <paramref name="Dir"/>.</returns>
    public static FileInfo[] GetChildrenFiles( this DirectoryInfo Dir, string WildCard = "*", bool Recurse = true ) => Dir.GetFiles(WildCard, Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Gets the children files/folders.
    /// </summary>
    /// <param name="Dir">The parent directory.</param>
    /// <param name="WildCard">The search pattern. Leave '<c>*</c>' for all.</param>
    /// <param name="Recurse">If <see langword="true" />, grandchildren are checked as well; otherwise only the immediate children are checked.</param>
    /// <returns>The found files/folders with the common ancestor <paramref name="Dir"/>.</returns>
    public static IEnumerable<FileSystemInfo> GetChildren( this DirectoryInfo Dir, string WildCard = "*", bool Recurse = true ) {
        foreach ( DirectoryInfo ChildDir in GetChildrenDirectories(Dir, WildCard, Recurse) ) {
            yield return ChildDir;
            if ( Recurse ) {
                foreach (FileInfo ChildOfChildFile in ChildDir.GetFiles(WildCard) ) {
                    yield return ChildOfChildFile;
                }
            }
        }
        foreach ( FileInfo ChildFile in Dir.GetFiles(WildCard) ) {
            yield return ChildFile;
        }
    }

    /// <summary>
    /// Selects the path in file explorer.
    /// </summary>
    /// <param name="Path">The path to select.</param>
    /// <param name="P">The created process.</param>
    /// <returns><see langword="true"/> if the process was successfully started; otherwise <see langword="false"/>.</returns>
    public static bool SelectInExplorer( this FileSystemInfo Path, out Process P ) {
        P = ProcessExtensions.GetExplorerProcess();
        P.StartInfo.Arguments = $"/select,\"{Path.FullName}\"";
        return P.Start();
    }

    /// <summary>
    /// Selects the path in file explorer.
    /// </summary>
    /// <param name="Path">The path to select.</param>
    /// <param name="P">The created process.</param>
    /// <param name="Highlight">If <see langword="true"/>, the parent folder opened, and the <paramref name="Path"/> is highlighted as a selection; otherwise the <paramref name="Path"/> is opened as the root itself.</param>
    /// <returns><see langword="true"/> if the process was successfully started; otherwise <see langword="false"/>.</returns>
    public static bool SelectInExplorer( this DirectoryInfo Path, out Process P, bool Highlight = true ) {
        P = ProcessExtensions.GetExplorerProcess();
        P.StartInfo.Arguments = Highlight ? $"/select,\"{Path.FullName}\"" : $"\"{Path.FullName}\"";
        return P.Start();
    }


    /// <summary>
    /// Smartly moves the directory to the given destination, moving just the relative files/folders if the destination already exists.
    /// </summary>
    /// <param name="Dir">The directory.</param>
    /// <param name="Destination">The destination.</param>
    public static void SmartMoveTo( this DirectoryInfo Dir, string Destination ) => SmartMoveTo(Dir, new DirectoryInfo(Destination));

    /// <summary>
    /// Smartly moves the directory to the given destination, moving just the relative files/folders if the destination already exists.
    /// </summary>
    /// <param name="Dir">The directory.</param>
    /// <param name="Destination">The destination.</param>
    public static void SmartMoveTo( this DirectoryInfo Dir, DirectoryInfo Destination ) {
        if ( Destination.Exists ) {
            string DirNm = Dir.FullName, DestNm = Destination.FullName;
            foreach ( FileSystemInfo Child in Dir.GetChildren() ) {
                switch ( Child ) {
                    case FileInfo FI:
                        FI.MoveTo(Path.Combine(DestNm, Path.GetRelativePath(DirNm, FI.FullName)), true);
                        break;
                    case DirectoryInfo DI:
                        string NewDI = Path.Combine(DestNm, Path.GetRelativePath(DirNm, DI.FullName));
                        if ( Directory.Exists(NewDI) ) {
                            Directory.Delete(NewDI, true);
                        }
                        DI.MoveTo(NewDI);
                        break;
                }
            }
        } else {
            Dir.MoveTo(Destination);
        }
    }
}