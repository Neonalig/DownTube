﻿#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="bool"/> to <see cref="SolidColorBrush"/>.
/// </summary>
[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
public class BoolToSolidColorBrushConverter : BoolToValueConverter<SolidColorBrush> { }