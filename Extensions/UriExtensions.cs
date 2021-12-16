#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="Uri"/>.
/// </summary>
public static class UriExtensions {

    // ReSharper disable once ExceptionNotDocumentedOptional //ArgumentNullException is not thrown.
    /// <summary>
    /// Gets the raw <see cref="string"/> representation of the given <see cref="Uri"/>.
    /// </summary>
    /// <param name="U">The URI.</param>
    /// <returns>An unescaped <see cref="string"/> representation of <paramref name="U"/>.</returns>
    /// <exception cref="InvalidOperationException">This instance represents a relative URI, and this property is valid only for absolute URIs.</exception>
    public static string GetRaw( this Uri U ) => Uri.UnescapeDataString(U.AbsolutePath);

    // ReSharper disable once ExceptionNotDocumentedOptional //ArgumentNullException is not thrown.
    /// <summary>
    /// Gets the raw path.
    /// </summary>
    /// <param name="U">The URI.</param>
    /// <returns>An unescaped <see cref="string"/> representation of <paramref name="U"/>.</returns>
    /// <exception cref="InvalidOperationException">This instance represents a relative URI, and this property is valid only for absolute URIs.</exception>
    public static string GetRawPath( this Uri U ) => Uri.UnescapeDataString(U.AbsolutePath).Replace('/', '\\');
}