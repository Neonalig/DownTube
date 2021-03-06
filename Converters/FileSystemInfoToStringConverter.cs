#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Globalization;
using System.IO;

#endregion

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <typeparamref name="T"/> to <see cref="string"/>.
/// </summary>
/// <typeparam name="T">The <see cref="FileSystemInfo"/> type to convert to/from.</typeparam>
public abstract class FileSystemInfoToStringConverter<T> : ValueConverter<T, string> where T : FileSystemInfo {

    /// <inheritdoc/>
    public override bool CanForward => true;

    /// <inheritdoc/>
    public override string Forward( T From, object? Parameter = null, CultureInfo? Culture = null ) => From.FullName;

    /// <inheritdoc/>
    public override bool CanForwardWhenNull => true;

    /// <inheritdoc/>
    public override string? ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => null;

    /// <inheritdoc/>
    public override bool CanReverse => true;

    /// <inheritdoc/>
    public override T? Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => GetT(To);

    /// <inheritdoc/>
    public override bool CanReverseWhenNull => true;

    /// <inheritdoc/>
    public override T? ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => null;

    /// <summary>
    /// Constructs a <see cref="FileSystemInfo"/> instance from the given <paramref name="Path"/>.
    /// </summary>
    /// <param name="Path">The path.</param>
    /// <returns>A new <see cref="FileSystemInfo"/> instance of type <typeparamref name="T"/>, or <see langword="null"/>.</returns>
    public abstract T? GetT( string Path );
}