#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Notifies clients that a property value has changed. Also provides a method to invoke the event programmatically.
/// </summary>
public interface INotifyAndRaisePropertyChanged : INotifyPropertyChanged {
    /// <summary>
    /// Called when a property is changed.
    /// </summary>
    /// <param name="PropertyName">Name of the property.</param>
    /// <exception cref="Exception">A <see langword="delegate"/> callback throws an exception.</exception>
    [NotifyPropertyChangedInvocator]
    void OnPropertyChanged( [CallerMemberName] string? PropertyName = null );
}