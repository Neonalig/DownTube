#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.IO;

#endregion

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