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

#endregion

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