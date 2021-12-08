using System.IO;
using System.Reflection;

using Newtonsoft.Json;

namespace DownTube.Extensions; 

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
    public static T Force<T>( this bool Success, T Value ) => Success ? Value : throw new ArgumentNullException(nameof(Value));

    /// <summary>
    /// Gets a <see cref="FileInfo"/> instance pointing to the given <paramref name="Path"/>.
    /// </summary>
    /// <param name="Path">The path to the file.</param>
    /// <returns>A new <see cref="FileInfo"/> instance.</returns>
    public static FileInfo GetFile( this string Path ) => TryGetFile(Path, out FileInfo FI).Force(FI);

    /// <summary>
    /// Gets a <see cref="DirectoryInfo"/> instance pointing to the given <paramref name="Path"/>.
    /// </summary>
    /// <param name="Path">The path to the file.</param>
    /// <returns>A new <see cref="DirectoryInfo"/> instance.</returns>
    public static DirectoryInfo GetDirectory( this string Path ) => TryGetDirectory(Path, out DirectoryInfo DI).Force(DI);

    /// <summary>
    /// Gets a <see cref="DirectoryInfo"/> instance pointing to the given <paramref name="SpecialFolder"/>.
    /// </summary>
    /// <param name="SpecialFolder">The special folder to point the instance towards.</param>
    /// <returns>A new <see cref="DirectoryInfo"/> instance.</returns>
    public static DirectoryInfo GetDirectory( this Environment.SpecialFolder SpecialFolder ) => TryGetDirectory(Environment.GetFolderPath(SpecialFolder), out DirectoryInfo DI).Force(DI);

    /// <summary>
    /// Creates a <see cref="FileInfo"/> instance pointing to a file in the given <paramref name="Directory"/>, and creates the file if it does not already exist.
    /// </summary>
    /// <param name="Directory">The parent directory.</param>
    /// <param name="FileName">The name of the file to create.</param>
    /// <param name="Create">Whether to create the file if it doesn't exist already.</param>
    /// <returns>A new <see cref="FileInfo"/> instance relative to the <paramref name="Directory"/>.</returns>
    public static FileInfo CreateSubfile( this DirectoryInfo Directory, string FileName, bool Create = true ) {
        FileInfo F = Path.Combine(Directory.FullName.Replace('/', '\\').TrimEnd('\\') + '\\', FileName).GetFile();
        if ( Create && !F.Exists ) { F.Create().Dispose(); }
        return F;
    }

    /// <summary>
    /// Creates a <see cref="Uri"/> instance pointing to <see cref="FileSystemInfo.FullName"/>.
    /// </summary>
    /// <param name="FSI">The path to point towards.</param>
    /// <returns>A new <see cref="Uri"/> instance.</returns>
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
    /// Serialises the given object into the <paramref name="Destination"/> file as json data.
    /// </summary>
    /// <param name="Obj">The object to serialise.</param>
    /// <param name="Destination">The destination file.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
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
    /// Deserialises the json data stored in the <paramref name="Location"/> file, and constructs a new object instance with the deserialised data.
    /// </summary>
    /// <param name="Location">The file to read from.</param>
    /// <param name="Serialiser">The serialiser to use. If <see langword="null"/>, <see cref="DefaultJsonSerialiser"/> is used instead.</param>
    /// <returns>A new instance of <typeparamref name="T"/> as provided by <see cref="JsonSerializer.Deserialize{T}(JsonReader)"/></returns>
    public static T? Deserialise<T>( this FileInfo Location, JsonSerializer? Serialiser = null ) {
        using ( FileStream FS = Location.OpenRead() ) {
            using ( StreamReader SR = new StreamReader(FS) ) {
                using ( JsonTextReader JTR = new JsonTextReader(SR) ) {
                    return (Serialiser ?? DefaultJsonSerialiser).Deserialize<T>(JTR);
                }
            }
        }
    }

}