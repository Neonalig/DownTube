#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;

#endregion

namespace DownTube.Extensions;

public static class StringExtensions {

    /// <summary>
    /// Navigates to the website using the user's default browser.
    /// </summary>
    /// <param name="URL">The URL.</param>
    /// <seealso cref="NavigateToWebsite(Uri)"/>
    public static void NavigateToWebsite( this string URL ) => _ = Process.Start(new ProcessStartInfo(URL) { UseShellExecute = true });

    /// <summary>
    /// Navigates to the website using the user's default browser.
    /// </summary>
    /// <param name="URL">The URL.</param>
    /// <seealso cref="NavigateToWebsite(string)"/>
    public static void NavigateToWebsite( this Uri URL ) => NavigateToWebsite(Uri.UnescapeDataString(URL.AbsoluteUri));

    /// <summary>
    /// Truncates the specified text to the maximum length, adding ellipsis if exceeded.
    /// </summary>
    /// <param name="Value">The text to truncate.</param>
    /// <param name="MaxLength">The maximum possible length (including the <paramref name="Ellipsis"/>).</param>
    /// <param name="Ellipsis">The ellipsis text.</param>
    /// <returns>The (possibly) truncated text.</returns>
    public static string Truncate( this string Value, int MaxLength, string Ellipsis = "..." ) {
        int Length = Value.Length,
            EllLen = Ellipsis.Length;
        if ( Length > MaxLength ) {
            return Value[..(MaxLength - EllLen)] + Ellipsis;
        }
        return Value;
    }

    /// <summary>
    /// Gets the URI.
    /// </summary>
    /// <remarks>
    /// Possible result exceptions:
    /// <list type="bullet">
    /// <item>
    ///     <term><see cref="ArgumentNullException"/></term>
    ///     <description><paramref name="Value" /> is <see langword="null" /></description>
    /// </item>
    /// <item>
    ///     <term><see cref="FormatException"/></term>
    ///     <description>See <see cref="UriFormatException"/> below.</description>
    /// </item>
    /// <item>
    ///     <term><see cref="UriFormatException"/></term>
    ///     <description>Note: In .NET for Windows Store apps or Portable Class Libraries, <see cref="FormatException" /> is thrown instead.
    ///  <br/><paramref name="Value" /> is empty.
    ///  <br/>-or-
    ///  <br/>The scheme specified in <paramref name="Value" /> is not correctly formed. See <see cref="String" />.
    ///  <br/>-or-
    ///  <br/><paramref name="Value" /> contains too many slashes.
    ///  <br/>-or-
    ///  <br/>The password specified in <paramref name="Value" /> is not valid.
    ///  <br/>-or-
    ///  <br/>The host name specified in <paramref name="Value" /> is not valid.
    ///  <br/>-or-
    ///  <br/>The file name specified in <paramref name="Value" /> is not valid.
    ///  <br/>-or-
    ///  <br/>The user name specified in <paramref name="Value" /> is not valid.
    ///  <br/>-or-
    ///  <br/>The host or authority name specified in <paramref name="Value" /> cannot be terminated by backslashes.
    ///  <br/>-or-
    ///  <br/>The port number specified in <paramref name="Value" /> is not valid or cannot be parsed.
    ///  <br/>-or-
    ///  <br/>The length of <paramref name="Value" /> exceeds 65519 characters.
    ///  <br/>-or-
    ///  <br/>The length of the scheme specified in <paramref name="Value" /> exceeds 1023 characters.
    ///  <br/>-or-
    ///  <br/>There is an invalid character sequence in <paramref name="Value" />.
    ///  <br/>-or-
    ///  <br/>The MS-DOS path specified in <paramref name="Value" /> must start with c:\\.</description>
    /// </item>
    /// </list></remarks>
    /// <param name="Value">The value.</param>
    /// <returns>A new <see cref="Uri"/> instance.</returns>
    public static Result<Uri> GetUri( this string? Value ) {
        try {
            return new Uri(Value ?? string.Empty);
        } catch (ArgumentNullException ArgNullEx) {
            return ArgNullEx;
        } catch (UriFormatException UriFormEx ) {
            return UriFormEx;
        } catch ( FormatException FormEx ) {
            return FormEx;
        }
    }

    /// <summary>
    /// Determines whether the given value is <see langword="null"/> or <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="Value">The value to check.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="Value"/> is <see langword="null"/> or <see cref="string.Empty"/>; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsNullOrEmpty( [NotNullWhen(false)] this string? Value ) => string.IsNullOrEmpty(Value);

    /// <summary>
    /// Determines whether the given value is <see langword="null"/>, <see cref="string.Empty"/>, or filled with only whitespace characters.
    /// </summary>
    /// <param name="Value">The value to check.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="Value"/> is <see langword="null"/>, <see cref="string.Empty"/> or whitespace; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsNullOrWhiteSpace( [NotNullWhen(false)] this string? Value ) => string.IsNullOrWhiteSpace(Value);
}