#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="IResult"/>.
/// </summary>
public static class ResultExtensions {

    /// <summary>
    /// Gets a simple log message with the specified result and header information.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="Result">The result.</param>
    /// <param name="Header">The header message.</param>
    /// <returns>A log message.</returns>
    public static string GetLogWithHeader<T>( this T Result, string Header ) where T : IResult => $"[{Result.ErrorCode}] {Header}\n\t{Result.Message.Replace("\n", "\n\t")}";

    /// <summary>
    /// Logs a simple log message with the specified result and header information.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="Result">The result.</param>
    /// <param name="Header">The header message.</param>
    /// <param name="Category">The log category.</param>
    [Conditional("DEBUG")]
    public static void LogWithHeader<T>( this T Result, string Header, string Category ) where T : IResult => Debug.WriteLine(GetLogWithHeader(Result, Header), Category);

    /// <summary>
    /// Logs a simple log message with the specified result and header information.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="Result">The result.</param>
    /// <param name="Header">The header message.</param>
    [Conditional("DEBUG")]
    public static void LogWithHeader<T>( this T Result, string Header ) where T : IResult => LogWithHeader(Result, Header, Result.WasSuccess ? string.Empty : "WARNING");

}
