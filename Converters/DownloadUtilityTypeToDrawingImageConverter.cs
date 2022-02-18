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

using DownTube.Views.Windows;

#endregion

namespace DownTube.Converters;

/// <summary>
/// Converts <see cref="DownloadUtilityType"/> enum values into a representative <see cref="DrawingImage"/>.
/// </summary>
[ValueConversion(typeof(DownloadUtilityType), typeof(DrawingImage))]
public class DownloadUtilityTypeToDrawingImageConverter : DownloadUtilityTypeToTypeConverter<DrawingImage> { }