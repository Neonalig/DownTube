#region Copyright (C) 2017-2022  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.Globalization;
using System.Windows;
using System.Windows.Data;

using DownTube.Views.Windows;

namespace DownTube.Converters;

/// <summary>
/// Provides value conversions from <see cref="DownloadUtilityType"/> to <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The resultant value type.</typeparam>
/// <seealso cref="DownloadUtilityType"/>
public abstract class DownloadUtilityTypeToTypeConverter<T> : DependencyObject, IValueConverter where T : notnull {

    /// <summary>
    /// Gets or sets the value returned for <see cref="DownloadUtilityType.FFmpeg"/>.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public T FFmpeg {
        get => (T)GetValue(FFmpegProperty);
        set => SetValue(FFmpegProperty, value);
    }

    /// <summary>Identifies the <see cref="FFmpeg"/> dependency property.</summary>
    public static readonly DependencyProperty FFmpegProperty = DependencyProperty.Register(nameof(FFmpeg), typeof(T), typeof(DownloadUtilityTypeToTypeConverter<T>), new PropertyMetadata(default(T)!));

    /// <summary>
    /// Gets or sets the value returned for <see cref="DownloadUtilityType.YoutubeDL"/>.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public T YoutubeDL {
        get => (T)GetValue(YoutubeDLProperty);
        set => SetValue(YoutubeDLProperty, value);
    }

    /// <summary>Identifies the <see cref="YoutubeDL"/> dependency property.</summary>
    public static readonly DependencyProperty YoutubeDLProperty = DependencyProperty.Register(nameof(YoutubeDL), typeof(T), typeof(DownloadUtilityTypeToTypeConverter<T>), new PropertyMetadata(default(T)!));

    /// <inheritdoc />
    public object Convert( object Value, Type TargetType, object Parameter, CultureInfo Culture ) {
        DownloadUtilityType DUT = (DownloadUtilityType)Value;
        Debug.WriteLine($"Converted {Value} to {DUT} ({FFmpeg}/{YoutubeDL})");
        return DUT switch {
            DownloadUtilityType.FFmpeg    => FFmpeg,
            DownloadUtilityType.YoutubeDL => YoutubeDL,
            _                             => throw new EnumValueOutOfRangeException<DownloadUtilityType>(DUT)
        };
    }

    /// <inheritdoc />
    public object ConvertBack( object Value, Type TargetType, object Parameter, CultureInfo Culture ) => throw new NotSupportedException();
}