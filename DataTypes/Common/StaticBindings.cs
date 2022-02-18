#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Reflection;
using System.Runtime.CompilerServices;

#endregion

namespace DownTube.DataTypes.Common;

public class StaticBindings {

    /// <summary>
    /// Gets the year copyright was put into effect.
    /// </summary>
    /// <value>
    /// The copyright start year.
    /// </value>
    public static int CopyrightStartYear => 2021;

    /// <summary>
    /// Gets the year the copyright is <c>void</c>.
    /// </summary>
    /// <value>
    /// The copyright end year.
    /// </value>
    public static int CopyrightEndYear { get; } = DateTime.UtcNow.Year;

    /// <summary>
    /// Gets the copyright start and end range.
    /// </summary>
    /// <value>
    /// The copyright year range.
    /// </value>
    public static string CopyrightYearRange { get; } = CopyrightEndYear != CopyrightStartYear ? $"{CopyrightStartYear} - {CopyrightEndYear}" : CopyrightStartYear.ToString();

    static readonly Assembly _ExecAss = Assembly.GetExecutingAssembly();
    static readonly AssemblyName _ExecAssName = _ExecAss.GetName();
    static readonly FileVersionInfo _ExecAssInfo = FileVersionInfo.GetVersionInfo(_ExecAss.Location);

    /// <summary>
    /// Gets the application version.
    /// </summary>
    /// <value>
    /// The application version.
    /// </value>
    public static Version AppVersion { get; } = _ExecAssName.Version ?? new Version();

    /// <summary>
    /// Gets the application company.
    /// </summary>
    /// <value>
    /// The application company.
    /// </value>
    public static string AppCompany { get; } = _ExecAssInfo.CompanyName ?? string.Empty;

    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    /// <value>
    /// The name of the application.
    /// </value>
    public static string AppName { get; } = _ExecAssName.Name ?? string.Empty;

    /// <summary>
    /// Event arguments raised when a <see langword="static"/> binding value is changed.
    /// </summary>
    /// <param name="OldValue">The old value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public delegate void StaticBindingChangedEventArgs( object? OldValue, object? NewValue, [CallerMemberName] string? PropertyName = null );

    /// <summary>
    /// Occurs when a <see langword="static"/> binding value is changed.
    /// </summary>
    public static event StaticBindingChangedEventArgs StaticBindingChanged = delegate { };

    /// <summary>
    /// Invokes the <see cref="StaticBindingChanged"/> <see langword="event"/>.
    /// </summary>
    /// <param name="OldValue">The old value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static void OnStaticBindingChanged( object? OldValue, object? NewValue, [CallerMemberName] string? PropertyName = null ) => StaticBindingChanged(OldValue, NewValue, PropertyName);

    /// <summary>
    /// Invokes the <see cref="StaticBindingChanged"/> <see langword="event"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="OldValue">The old value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static void OnStaticBindingChanged<T>( T? OldValue, T? NewValue, [CallerMemberName] string? PropertyName = null ) => StaticBindingChanged(OldValue, NewValue, PropertyName);
}