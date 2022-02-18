#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Provides data for an <see langword="event"/> that signals when an <see cref="ISave"/> instance just became clean.
/// </summary>
/// <seealso cref="BecameCleanEventHandler"/>
public class BecameCleanEventArgs : EventArgs { }