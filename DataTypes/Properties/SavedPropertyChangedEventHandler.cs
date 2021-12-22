#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using DownTube.Engine;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents the method that will handle the SavedPropertyChanged <see langword="event"/> on a <see cref="Props"/> instance, and provide the relevant event arguments.
/// </summary>
/// <param name="Sender">The <see langword="event"/> raiser.</param>
/// <param name="E">The raised <see langword="event"/> arguments.</param>
/// <seealso cref="SavedPropertyChangedEventArgs"/>
public delegate void SavedPropertyChangedEventHandler( ISave? Sender, SavedPropertyChangedEventArgs E );