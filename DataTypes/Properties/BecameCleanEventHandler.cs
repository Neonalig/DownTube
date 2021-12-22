#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents the method that will handle the BecameEvent <see langword="event"/> on a <see cref="ISave"/> instance, and provide the relevant event arguments.
/// </summary>
/// <param name="Sender">The <see langword="event"/> raiser.</param>
/// <param name="E">The raised <see langword="event"/> arguments.</param>
/// <seealso cref="BecameCleanEventArgs"/>
public delegate void BecameCleanEventHandler( ISave? Sender, BecameCleanEventArgs E );