#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents the method that will handle the BecameDirty <see langword="event"/> on a <see cref="ISave"/> instance, and provide the relevant event arguments.
/// </summary>
/// <param name="Sender">The event raiser.</param>
/// <param name="E">The raised event arguments.</param>
/// <seealso cref="BecameDirtyEventArgs"/>
public delegate void BecameDirtyEventHandler( ISave? Sender, BecameDirtyEventArgs E );