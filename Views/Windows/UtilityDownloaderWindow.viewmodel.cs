using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using DownTube.Views.Controls;

using PropertyChanged;

using SharpVectors.Converters;

namespace DownTube.Views.Windows;

public class UtilityDownloaderWindow_ViewModel : Window_ViewModel<UtilityDownloaderWindow> {
    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;

    public bool UpdateDialogVisible { get; set; } = false;

    public double InstallProgress { get; set; } = -1;

    /// <summary>
    /// Gets the legal blurb for the current <see cref="DownloadUtilityType"/>.
    /// </summary>
    /// <value>
    /// The legal blurb.
    /// </value>
    [DependsOn(nameof(Utility))]
    public UserControl LegalBlurb => Utility switch {
        DownloadUtilityType.FFmpeg    => new UtilityDownloaderWindow_FFmpegLegal(),
        DownloadUtilityType.YoutubeDL => new UtilityDownloaderWindow_YoutubeDLLegal(),
        _                             => throw new EnumValueOutOfRangeException<DownloadUtilityType>(Utility)
    };

    /// <summary>
    /// Gets the legal blurb for the current <see cref="DownloadUtilityType"/>.
    /// </summary>
    /// <value>
    /// The legal blurb.
    /// </value>
    [DependsOn(nameof(Utility))]
    public string UtilitySummary => Utility switch {
        DownloadUtilityType.FFmpeg    => "A complete, cross-platform solution to record, convert and stream audio and video.",
        DownloadUtilityType.YoutubeDL => "Download videos from youtube.com or other video platforms.",
        _                             => throw new EnumValueOutOfRangeException<DownloadUtilityType>(Utility)
    };

    /// <summary>
    /// Gets or sets the utility which will be downloaded.
    /// </summary>
    /// <value>
    /// The utility which will be downloaded.
    /// </value>
    public DownloadUtilityType Utility { get; set; } = DownloadUtilityType.FFmpeg;
}

[ValueConversion(typeof(DownloadUtilityType), typeof(DrawingImage))]
public class DownloadUtilityTypeToDrawingImageConverter : DownloadUtilityTypeToTypeConverter<DrawingImage> {

}

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
            _ => throw new EnumValueOutOfRangeException<DownloadUtilityType>(DUT)
        };
    }

    /// <inheritdoc />
    public object ConvertBack( object Value, Type TargetType, object Parameter, CultureInfo Culture ) => throw new NotSupportedException();
}

/// <summary>
/// An implementation of <see cref="Image"/> which supports SVG images.
/// </summary>
/// <seealso cref="Image"/>
/// <seealso cref="SharpVectors"/>
public class SvgImage : Image {

    /// <summary>
    /// Gets or sets the SVG source.
    /// </summary>
    /// <value>
    /// The SVG source.
    /// </value>
    public string SvgSource {
        get => (string)GetValue(SvgSourceProperty);
        set => SetValue(SvgSourceProperty, value);
    }

    /// <summary>Identifies the <see cref="SvgSource"/> dependency property.</summary>
    public static readonly DependencyProperty SvgSourceProperty = DependencyProperty.Register(nameof(SvgSource), typeof(string), typeof(SvgImage), new PropertyMetadata(string.Empty, OnSvgSourceChanged));

    /// <summary>
    /// Called when <see cref="SvgSource"/> is changed.
    /// </summary>
    /// <param name="D">The dependency object.</param>
    /// <param name="E">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    static void OnSvgSourceChanged( DependencyObject D, DependencyPropertyChangedEventArgs E ) {
        if ( D is SvgImage Image ) {
            SvgImageConverterExtension Converter = new SvgImageConverterExtension {
                AppName = StaticBindings.AppName
            };
            Binding Binding = new Binding {
                Converter = Converter,
                ConverterParameter = E.NewValue
            };

            _ = Image.SetBinding(SourceProperty, Binding);
        }
    }
}

public enum DownloadUtilityType {
    FFmpeg = 1,
    YoutubeDL = 2
}