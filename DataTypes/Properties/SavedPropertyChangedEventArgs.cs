#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Runtime.CompilerServices;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Provides additional data for the <see cref="SavedPropertyChangedEventHandler"/>.
/// </summary>
/// <seealso cref="SavedPropertyChangedEventHandler"/>
public class SavedPropertyChangedEventArgs : EventArgs {
    /// <summary>
    /// Constructs an instance of the <see cref="SavedPropertyChangedEventArgs"/> <see langword="class"/>.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    public SavedPropertyChangedEventArgs( [CallerMemberName] string? PropertyName = null ) => this.PropertyName = PropertyName.CatchNull();

    /// <summary>
    /// The name of the property.
    /// </summary>
    public string PropertyName { get; set; }
}