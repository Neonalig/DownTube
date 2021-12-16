using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="FileInfo"/> to <see cref="string"/>.
/// </summary>
public class FileInfoToStringConverter : FileSystemInfoToStringConverter<FileInfo> {
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "CatchAllClause")]
    public override FileInfo? GetT( string Path ) {
        try {
            switch ( Path ) {
                case { } P when !string.IsNullOrEmpty(P):
                    return new FileInfo(P);
            }
        } catch { }
        return null;
    }
}