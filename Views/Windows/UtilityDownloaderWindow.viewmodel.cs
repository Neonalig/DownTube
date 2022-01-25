using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using DownTube.Converters;
using DownTube.Views.Controls;

using JetBrains.Annotations;

using Octokit;

using PropertyChanged;

using SharpVectors.Converters;

using WPFUI.Common;

namespace DownTube.Views.Windows;

public class UtilityDownloaderWindow_ViewModel : Window_ViewModel<UtilityDownloaderWindow> {

    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;

    public bool UpdateDialogVisible { get; set; } = false;

    public bool PickerDialogVisible { get; set; } = false;

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
    /// Gets the name of the current <see cref="DownloadUtilityType"/>.
    /// </summary>
    /// <value>
    /// The name of the utility.
    /// </value>
    [DependsOn(nameof(Utility))]
    public string UtilityName => Utility switch {
        DownloadUtilityType.FFmpeg    => "FFmpeg",
        DownloadUtilityType.YoutubeDL => "youtube-dl",
        _                             => throw new EnumValueOutOfRangeException<DownloadUtilityType>(Utility)
    };

    /// <summary>
    /// Gets or sets the utility which will be downloaded.
    /// </summary>
    /// <value>
    /// The utility which will be downloaded.
    /// </value>
    public DownloadUtilityType Utility { get; set; } = DownloadUtilityType.FFmpeg;

    KnownUtilityRelease? Int_AwaitingRelease { get; set; }

    /// <summary>
    /// Gets or sets the awaiting release.
    /// </summary>
    /// <value>
    /// The awaiting release.
    /// </value>
    public KnownUtilityRelease? AwaitingRelease {
        get => Int_AwaitingRelease;
        set {
            if ( Int_AwaitingRelease == value ) { return; }
            Int_AwaitingRelease = value;

            if ( value is not null ) {
                value.AssetChosen += (_, _) => UpdateContinuationButton();
            }
        }
    }

    void UpdateContinuationButton() => AwaitingReleaseContinuationButtonAppearance =
        Int_AwaitingRelease is not null && Int_AwaitingRelease.IsAssetChosen
            ? Appearance.Primary
            : Appearance.Secondary;

    /// <summary>
    /// Gets or sets the <see cref="AwaitingRelease"/> continuation button appearance.
    /// </summary>
    /// <value>
    /// The <see cref="AwaitingRelease"/> continuation button appearance.
    /// </value>
    public Appearance AwaitingReleaseContinuationButtonAppearance { get; set; } = Appearance.Secondary;
}

/// <summary>
/// A known <see cref="Release"/>.
/// </summary>
/// <seealso cref="IReadOnlyList{T}" />
/// <seealso cref="KnownUtilityDownload"/>
public class KnownUtilityRelease : DependencyObject, IReadOnlyList<KnownUtilityDownload> {

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the release.
    /// </summary>
    /// <value>
    /// The release.
    /// </value>
    public Release Release { get; set; }

    /// <summary>
    /// The found download assets.
    /// </summary>
    internal readonly ObservableCollection<KnownUtilityDownload> Downloads;

    /// <summary>
    /// Prevents a default instance of the <see cref="KnownUtilityRelease"/> class from being created.
    /// </summary>
    public KnownUtilityRelease() : this("", null!, _ => true) {
        if ( !DesignerProperties.GetIsInDesignMode(this) ) {
            throw new NotSupportedException();
        }

        Downloads = new ObservableCollection<KnownUtilityDownload> {
            new KnownUtilityDownload("fake1", this, null!),
            new KnownUtilityDownload("fake2", this, null!),
            new KnownUtilityDownload("fake3", this, null!)
        };

        AssetChosen += ( _, _ ) => {
            IsAssetChosen = Downloads.Any(KUD => KUD.Chosen);
            Debug.WriteLine($"Some asset was (un/)chosen. Any picked? {IsAssetChosen}");
        };
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="KnownUtilityRelease"/> class.
    /// </summary>
    /// <param name="Release">The release.</param>
    /// <param name="AssetValidator">The asset validator.</param>
    public KnownUtilityRelease( Release Release, Func<string, bool> AssetValidator ) : this(Release.Name, Release, AssetValidator) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="KnownUtilityRelease"/> class.
    /// </summary>
    /// <param name="Name">The name.</param>
    /// <param name="Release">The release.</param>
    /// <param name="AssetValidator">The asset validator.</param>
    public KnownUtilityRelease( string Name, Release Release, Func<string, bool>? AssetValidator ) {
        this.Name = Name;
        this.Release = Release;

        Downloads = Release is null
            ? new ObservableCollection<KnownUtilityDownload>()
            : new ObservableCollection<KnownUtilityDownload>(
                (AssetValidator is null
                    ? Release.Assets
                    : Release.Assets.Where(A => AssetValidator(A.Name.ToLowerInvariant()))
                ).Select(A => new KnownUtilityDownload(this, A)));

        AssetChosen += ( _, _ ) => {
            IsAssetChosen = Downloads.Any(KUD => KUD.Chosen);
            Debug.WriteLine($"Some asset was (un/)chosen. Any picked? {IsAssetChosen}");
        };
    }

    /// <summary>
    /// Gets a value indicating whether any asset has been chosen.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if any asset is chosen; otherwise, <see langword="false" />.
    /// </value>
    public bool IsAssetChosen { get; set; }

    /// <summary>
    /// Gets the chosen asset.
    /// </summary>
    /// <value>
    /// The chosen asset.
    /// </value>
    public KnownUtilityDownload? ChosenAsset => Downloads.FirstOrDefault(KUD => KUD.Chosen);

    /// <inheritdoc />
    public IEnumerator<KnownUtilityDownload> GetEnumerator() => Downloads.GetEnumerator<KnownUtilityDownload>();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => Downloads.Count;

    /// <inheritdoc />
    public KnownUtilityDownload this[ int Index ] => Downloads[Index];

    #region AssetChosen Event

    /// <summary>
    /// Event arguments for the <see cref="AssetChosen"/> event (<inheritdoc cref="AssetChosen"/>).
    /// </summary>
    /// <param name="Sender">The event raiser.</param>
    /// <param name="E">The raised event arguments.</param>
    public delegate void AssetChosenEventArgs( KnownUtilityRelease Sender, KnownUtilityDownload E );

    /// <summary>
    /// Invoked when an asset is chosen for download.
    /// </summary>
    public event AssetChosenEventArgs AssetChosen;

    bool _IgnoringAC = false;
    /// <summary>
    /// Raises the <see cref="AssetChosen"/> event (<inheritdoc cref="AssetChosen"/>)
    /// </summary>
    /// <param name="E">The raised event arguments.</param>
    public void OnAssetChosen( KnownUtilityDownload E ) {
        if ( _IgnoringAC ) { return; }
        _IgnoringAC = true;

        Debug.WriteLine($"Selecting {E.FileName}");
        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (KnownUtilityDownload KUD in Downloads ) {
            if ( KUD.Chosen && KUD.FileName != E.FileName ) {
                Debug.WriteLine($"Deselecting {KUD.FileName}");
                KUD.Chosen = false;
            }
        }
        AssetChosen(this, E);
        _IgnoringAC = false;
    }

    #endregion

    /// <inheritdoc />
    public override string ToString() => $"{Name} ({Downloads.Count} items)";
}

/// <summary>
/// A known <see cref="ReleaseAsset"/>.
/// </summary>
/// <seealso cref="KnownUtilityRelease"/>
public class KnownUtilityDownload : DependencyObject, INotifyPropertyChanged {

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>
    /// The name of the file.
    /// </value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the release.
    /// </summary>
    /// <value>
    /// The release.
    /// </value>
    public KnownUtilityRelease Release { get; set; }

    /// <summary>
    /// Gets or sets the asset.
    /// </summary>
    /// <value>
    /// The asset.
    /// </value>
    public ReleaseAsset Asset { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="KnownUtilityDownload"/> is chosen.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if chosen; otherwise, <see langword="false" />.
    /// </value>
    public bool Chosen { get; set; }

    /// <summary>
    /// Prevents a default instance of the <see cref="KnownUtilityDownload"/> class from being created.
    /// </summary>
    public KnownUtilityDownload() : this("", null!, null!) {
        if ( !DesignerProperties.GetIsInDesignMode(this) ) {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="KnownUtilityDownload"/> class.
    /// </summary>
    /// <param name="Release">The release.</param>
    /// <param name="Asset">The asset.</param>
    public KnownUtilityDownload( KnownUtilityRelease Release, ReleaseAsset Asset ) : this(Asset.Name, Release, Asset) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="KnownUtilityDownload"/> class.
    /// </summary>
    /// <param name="FileName">The name of the file.</param>
    /// <param name="Release">The release.</param>
    /// <param name="Asset">The asset.</param>
    public KnownUtilityDownload( string FileName, KnownUtilityRelease Release, ReleaseAsset Asset ) {
        this.FileName = FileName;
        this.Release = Release;
        this.Asset = Asset;

        PropertyChanged += ( _, E ) => {
            switch ( E.PropertyName ) {
                case nameof(Chosen):
                    Release.OnAssetChosen(this);
                    break;
            }
        };
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Called when a property is changed.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string? PropertyName = null ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <inheritdoc />
    public override string ToString() => FileName;

    /// <summary>
    /// Gets or sets the match support.
    /// </summary>
    /// <value>
    /// The match support.
    /// </value>
    public KnownUtilityDownloadMatchType Match { get; set; }

    /// <summary>
    /// Gets a value indicating whether <see cref="Match"/> is <see cref="KnownUtilityDownloadMatchType.Supported"/>.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Match"/> <c>==</c> <see cref="KnownUtilityDownloadMatchType.Supported"/>; otherwise, <see langword="false" />.
    /// </value>
    [DependsOn(nameof(Match))]
    public bool IsSupported => Match == KnownUtilityDownloadMatchType.Supported;

    /// <summary>
    /// Gets a value indicating whether <see cref="Match"/> is <see cref="KnownUtilityDownloadMatchType.Recommended"/>.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Match"/> <c>==</c> <see cref="KnownUtilityDownloadMatchType.Recommended"/>; otherwise, <see langword="false" />.
    /// </value>
    [DependsOn(nameof(Match))]
    public bool IsRecommended => Match == KnownUtilityDownloadMatchType.Recommended;

    /// <summary>
    /// Gets a value indicating whether <see cref="Match"/> is <see cref="KnownUtilityDownloadMatchType.Unknown"/>.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Match"/> <c>==</c> <see cref="KnownUtilityDownloadMatchType.Unknown"/>; otherwise, <see langword="false" />.
    /// </value>
    /// <seealso cref="IsKnown"/>
    [DependsOn(nameof(Match))]
    public bool IsUnknown => Match == KnownUtilityDownloadMatchType.Unknown;

    /// <summary>
    /// Gets a value indicating whether <see cref="Match"/> is <b>NOT</b> <see cref="KnownUtilityDownloadMatchType.Unknown"/>.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="Match"/> <c>!=</c> <see cref="KnownUtilityDownloadMatchType.Unknown"/>; otherwise, <see langword="false" />.
    /// </value>
    /// <seealso cref="IsUnknown"/>
    [DependsOn(nameof(Match))]
    public bool IsKnown => Match != KnownUtilityDownloadMatchType.Unknown;
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
/// Provides value conversions from <see cref="bool"/> to <see cref="KnownUtilityDownloadMatchType"/>.
/// </summary>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
[ValueConversion(typeof(bool), typeof(KnownUtilityDownloadMatchType))]
public sealed class BoolToKnownUtilityDownloadMatchTypeConverter : BoolToEnumConverter<KnownUtilityDownloadMatchType> { }

/// <summary>
/// Provides value conversions from <see cref="bool"/> to <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
/// <seealso cref="ValueConverter{TFrom, TTo}"/>
//[ValueConversion(typeof(bool), typeof(TEnum))]
public abstract class BoolToEnumConverter<TEnum> : ValueConverter<bool, TEnum> where TEnum : struct, Enum {

    /// <summary>
    /// Gets or sets the enum value returned when <see langword="true"/>.
    /// </summary>
    /// <value>
    /// The value returned when <see langword="true"/>.
    /// </value>
    public TEnum True { get; set; }

    /// <summary>
    /// Gets or sets the enum value returned when <see langword="false"/>.
    /// </summary>
    /// <value>
    /// The value returned when <see langword="false"/>.
    /// </value>
    public TEnum False { get; set; }

    /// <summary>
    /// Gets or sets the enum value returned when <see langword="null"/>.
    /// </summary>
    /// <value>
    /// The value returned when <see langword="null"/>.
    /// </value>
    public TEnum Null { get; set; }

    /// <inheritdoc />
    public override bool CanReverse => false;

    public override bool CanForwardWhenNull => true;

    /// <inheritdoc />
    public override TEnum Forward( bool From, object? Parameter = null, CultureInfo? Culture = null ) => From ? True : False;

    /// <inheritdoc />
    public override TEnum ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Null;

    /// <inheritdoc />
    public override bool Reverse( TEnum To, object? Parameter = null, CultureInfo? Culture = null ) => false;
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

public enum KnownUtilityDownloadMatchType {
    /// <summary>
    /// The download is a known form that can be handled immediately.
    /// </summary>
    Supported,
    /// <summary>
    /// The download is a known file type that can feasibly be <b>attempted</b> to be handled automatically.
    /// </summary>
    Recommended,
    /// <summary>
    /// The download is an unknown type, and must be handled by the user.
    /// </summary>
    Unknown
}