#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;

#endregion

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions between <see cref="Version"/> and <see cref="string"/>.
/// </summary>
[ValueConversion(typeof(Version), typeof(string))]
public class VersionToStringConverter : ValueConverter<VersionToStringConverter, Version, string> {
    /// <summary>
    /// Gets or sets the prefix.
    /// </summary>
    /// <value>
    /// The prefix.
    /// </value>
    public string Prefix { get; set; } = "v";

    /// <summary>
    /// Gets or sets the suffix.
    /// </summary>
    /// <value>
    /// The suffix.
    /// </value>
    public string Suffix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Version.Major"/> is displayed.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Version.Major"/> is displayed; otherwise, <see langword="false" />.
    /// </value>
    public VersionPartDisplay Major { get; set; } = VersionPartDisplay.Display;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Version.Minor"/> is displayed.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Version.Minor"/> is displayed; otherwise, <see langword="false" />.
    /// </value>
    public VersionPartDisplay Minor { get; set; } = VersionPartDisplay.Display;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Version.Build"/> is displayed.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Version.Build"/> is displayed; otherwise, <see langword="false" />.
    /// </value>
    public VersionPartDisplay Build { get; set; } = VersionPartDisplay.Display;

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="Version.Revision"/> is displayed.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Version.Revision"/> is displayed; otherwise, <see langword="false" />.
    /// </value>
    public VersionPartDisplay Revision { get; set; } = VersionPartDisplay.DisplayIfNonZero;

    /// <inheritdoc />
    public override bool CanReverse => true;

    /// <summary>
    /// Appends the version part to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="SB">The string builder.</param>
    /// <param name="VersionPart">The version part.</param>
    /// <param name="Display">The display type.</param>
    /// <param name="AppendDecimal">If <see langword="true"/>, a decimal will be appended; otherwise no decimal will be displayed.</param>
    internal static void Append(StringBuilder SB, int VersionPart, VersionPartDisplay Display, bool AppendDecimal = true ) {
        switch (Display) {
            case VersionPartDisplay.Display:
            case VersionPartDisplay.DisplayIfNonZero when VersionPart != 0:
                SB.Append(VersionPart);
                if ( AppendDecimal ) {
                    SB.Append('.');
                }
                return;
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public override string Forward( Version From, object? Parameter = null, CultureInfo? Culture = null ) {
        StringBuilder SB = new StringBuilder();
        SB.Append(Prefix);
        Append(SB, From.Major, Major, true);
        Append(SB, From.Minor, Minor, true);
        Append(SB, From.Build, Build, true);
        Append(SB, From.Revision, Revision, false);
        //while ( SB[^1] == '.' ) {
        //    SB.Remove(0, SB.Length - 1);
        //}
        //SB.Append(Suffix);
        return SB.ToString().TrimEnd('.') + Suffix;
    }

    /// <summary>
    /// The regex responsible for determining <see cref="Version"/> data from a <see cref="string"/>.
    /// </summary>
    public static readonly Regex VersionRegex = new Regex(".+(?<Useful>(?:[0-9].?)+)");
    //(?<Prefix>.+?)?(?:(?<Major>[0-9]+)\\.)?(?:(?<Minor>[0-9]+)\\.)?(?:(?<Build>[0-9]+)\\.)?(?:(?<Revision>[0-9]+))?(?<Suffix>.+)?

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public override Version? Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) {
        try {
            if ( VersionRegex.Match(To) is { Success: true } Mt ) {
                string[] Sectors = Mt.Value.Split('.');
                int Mj = - 2, Mi = - 2, Bu = - 2, Re = - 2;
                foreach ( string Sector in Sectors ) {
                    if ( int.TryParse(Sector, out int Sint) ) {
                        if ( Mj == - 2 ) {
                            Mj = Sint;
                            continue;
                        }
                        if ( Mi == - 2 ) {
                            Mi = Sint;
                            continue;
                        }
                        if ( Bu == - 2 ) {
                            Bu = Sint;
                            continue;
                        }
                        if ( Re == - 2 ) {
                            Re = Sint;
                            continue;
                        }
                        break;
                    }
                }

                static int Fix( int Val ) => Val < 0 ? 0 : Val;

                return Mj switch {
                    - 2 => null,
                    _ => Mi switch {
                        - 2 => new Version(Fix(Mj), 0, 0, 0),
                        _ => Bu switch {
                            - 2 => new Version(Fix(Mj), Fix(Mi), 0, 0),
                            _ => Re switch {
                                - 2 => new Version(Fix(Mj), Fix(Mi), Fix(Bu), 0),
                                _   => new Version(Fix(Mj), Fix(Mi), Fix(Bu), Fix(Re))
                            }
                        }
                    }
                };
            }
            return null;
        } catch ( RegexMatchTimeoutException ) { }
        return null;
    }
}

/// <summary>
/// Controls how a part of a <see cref="Version"/> is displayed.
/// </summary>
public enum VersionPartDisplay {
    /// <summary>
    /// The part is hidden.
    /// </summary>
    Hide,
    /// <summary>
    /// The part is displayed.
    /// </summary>
    Display,
    /// <summary>
    /// The part is displayed only if not equal to zero.
    /// </summary>
    DisplayIfNonZero
}