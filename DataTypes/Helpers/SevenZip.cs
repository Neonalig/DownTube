using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;

namespace DownTube.DataTypes.Helpers;

public static class SZip {
    /// <summary>
    /// Extracts the specified archive file to the given destination.
    /// </summary>
    /// <param name="File">The archive to extract.</param>
    /// <param name="Destination">The destination path.</param>
    /// <param name="CreateSubdirectory">If <see langword="true" />, a subdirectory is created in the parent folder with the same name as the file; otherwise the parent folder is used instead.</param>
    /// <returns>The folder the archive was extracted into.</returns>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static DirectoryInfo Extract( this FileInfo File, DirectoryInfo Destination, bool CreateSubdirectory = true ) {
        Debug.WriteLine($"Extracting {File.FullName}...");
        if ( CreateSubdirectory ) {
            if ( !Destination.Exists ) { Destination.Create(); }
            Destination = Destination.CreateSubdirectory(Path.GetFileNameWithoutExtension(File.Name));
            if ( !Destination.Exists ) { Destination.Create(); }
        }
        Debug.WriteLine($"Will extract to {Destination.FullName}...");
        try {
            ZipArchive Archive = ZipFile.OpenRead(File.FullName);
            Debug.WriteLine($"Extracting from {Archive}");
            Archive.ExtractToDirectory(Destination.FullName);
            Debug.WriteLine("Extracted.");
        } catch (Exception Ex) {
            Debug.WriteLine($"Exception caught: {Ex} :: {Ex.GetType().GetTypeName()} :: {Ex.Message}");
        }
        return Destination;
    }

    /// <inheritdoc cref="Extract(FileInfo, DirectoryInfo, bool)"/>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static DirectoryInfo Extract( this FileInfo File, bool CreateSubdirectory = true ) => Extract(File, File.Directory!, CreateSubdirectory);
}