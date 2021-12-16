using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="DirectoryInfo"/> to <see cref="string"/>.
/// </summary>
public class DirectoryInfoToStringConverter : FileSystemInfoToStringConverter<DirectoryInfo> {
    /// <inheritdoc />
    [SuppressMessage("ReSharper", "CatchAllClause")]
    public override DirectoryInfo? GetT( string Path ) {
        try {
            switch ( Path ) {
                case { } P when !string.IsNullOrEmpty(P):
                    return new DirectoryInfo(P);
            }
        } catch { }
        return null;
    }
}