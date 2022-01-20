#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.Windows.Data;
using System.Windows.Media;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="bool"/> to <see cref="SolidColorBrush"/>.
/// </summary>
[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
public class BoolToSolidColorBrushConverter : BoolToValueConverter<SolidColorBrush> { }