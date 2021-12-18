#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using DownTube.DataTypes;
using DownTube.Properties;

using Newtonsoft.Json;

using static DownTube.Engine.SavedProps;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Manages writing to/reading from properties.
/// </summary>
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public sealed class SavedProps : SaveData<ISavedPropertyInternalHolder> {
    /// <summary>
    /// The local properties file.
    /// </summary>
    readonly FileInfo _LocalPropsFile = FileSystemInfoExtensions.AppDir.CreateSubfile("settings.json", false);

    /// <summary>
    /// Initialises a new instance of the <see cref="SavedProps"/> class.
    /// </summary>
    public SavedProps() {
        Properties = null!;
        Setup();

        Revert();
    }

    /// <summary>
    /// Gets a value indicating whether the properties are from a local settings.json file or not.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if the properties is from a local file; otherwise, <see langword="false" />.
    /// </value>
    public bool FromLocalFile { get; private set; }

    #region Properties

    /// <summary>
    /// Gets or sets the path to the ffmpeg executable.
    /// </summary>
    /// <value>
    /// The ffmpeg path.
    /// </value>
    public FileInfo? FFmpegPath {
        get => GetProperty<FileInfo?>().Value;
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the path to the youtube-dl executable.
    /// </summary>
    /// <value>
    /// The path to the youtube-dl executable.
    /// </value>
    public FileInfo? YoutubeDLPath {
        get => GetProperty<FileInfo?>().Value;
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the output folder.
    /// </summary>
    /// <value>
    /// The output folder.
    /// </value>
    public DirectoryInfo? OutputFolder {
        get => GetProperty<DirectoryInfo?>().Value;
        set => SetProperty(value);
    }

    /// <summary>
    /// Gets or sets the number of times any song/video was been downloaded.
    /// </summary>
    /// <value>
    /// The number of times any song/video was been downloaded.
    /// </value>
    public int TimesDownloaded {
        get => GetProperty<int>().Value;
        set => SetProperty(value);
    }

    #endregion

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0066:Convert switch statement to expression")]
    public override void Save() {
        Debug.WriteLine("Saving data...");
        base.Save();
        FromLocalFile = _LocalPropsFile.GetExists();
        Debug.WriteLine($"Save local? {FromLocalFile}");

        switch ( FromLocalFile ) {
            case true:
                List<ISavedPropertyInternalHolder> Ls = new List<ISavedPropertyInternalHolder>();
                foreach ( ISavedPropertyInternalHolder SP in Properties ) {
                    if ( SP.CanOverride && SP.Value is { } Val ) {
                        switch ( Val ) {
                            case FileSystemInfo FSI:
                                Ls.Add(new SavedPropertyInternalHolder<string>(SP.PropertyName, FSI.FullName, true));
                                break;
                            default:
                                Ls.Add(SP);
                                break;
                        }
                    }
                }
                Ls.Serialise(_LocalPropsFile);
                break;
        }

        foreach ( ISavedPropertyInternalHolder SP in Properties ) {
            switch ( SP.Value ) {
                case FileSystemInfo FSI:
                    Settings.Default[SP.PropertyName] = FSI.FullName;
                    break;
                case bool:
                case byte:
                case char:
                case decimal:
                case double:
                case float:
                case int:
                case long:
                case sbyte:
                case short:
                case string:
                case System.Collections.Specialized.StringCollection:
                case DateTime:
                case System.Drawing.Color:
                case System.Drawing.Font:
                case System.Drawing.Point:
                case System.Drawing.Size:
                case Guid:
                case TimeSpan:
                case uint:
                case ulong:
                case ushort:
                    Settings.Default[SP.PropertyName] = SP.Value;
                    break;
                default:
                    Settings.Default[SP.PropertyName] = SP.Serialise();
                    break;
            }
            //switch ( SPP.PropertyName ) {
            //    case nameof(Settings.Default.FFmpegPath):
            //        Settings.Default.FFmpegPath = ((FileInfo?)SPP.Value)?.FullName;
            //        break;
            //    case nameof(Settings.Default.YoutubeDLPath):
            //        Settings.Default.YoutubeDLPath = ((FileInfo?)SPP.Value)?.FullName;
            //        break;
            //    case nameof(Settings.Default.OutputFolder):
            //        Settings.Default.OutputFolder = ((DirectoryInfo?)SPP.Value)?.FullName;
            //        break;
            //}
        }
        Settings.Default.Save();
    }

    /// <inheritdoc />
    [SuppressMessage("Style", "IDE0066:Convert switch statement to expression")]
    public override void Revert() {
        Debug.WriteLine("Reverting data...");
        base.Revert();
        FromLocalFile = _LocalPropsFile.GetExists();
        Debug.WriteLine($"Revert local? {FromLocalFile}");

        switch ( FromLocalFile ) {
            case true when _LocalPropsFile.Deserialise<List<ISavedPropertyInternalHolder>>().Value is { } Ls:
                foreach ( ISavedPropertyInternalHolder SP in Ls ) {
                    ISavedPropertyInternalHolder RealProp = GetProperty(SP.PropertyName);
                    if ( RealProp.CanOverride ) {
                        RealProp.Value = SP.Value;
                    }
                }
                break;
        }

        foreach ( ISavedPropertyInternalHolder SP in Properties ) {
            switch ( SP.Value ) {
                case FileInfo:
                    SP.Value = ((string)Settings.Default[SP.PropertyName]).GetFile().Value;
                    break;
                case DirectoryInfo:
                    SP.Value = ((string)Settings.Default[SP.PropertyName]).GetDirectory().Value;
                    break;
                case bool:
                case byte:
                case char:
                case decimal:
                case double:
                case float:
                case int:
                case long:
                case sbyte:
                case short:
                case string:
                case System.Collections.Specialized.StringCollection:
                case DateTime:
                case System.Drawing.Color:
                case System.Drawing.Font:
                case System.Drawing.Point:
                case System.Drawing.Size:
                case Guid:
                case TimeSpan:
                case uint:
                case ulong:
                case ushort:
                    SP.Value = Settings.Default[SP.PropertyName];
                    break;
                default:
                    Type? Tp = SP.Value?.GetType();
                    Tp ??= SP.GetType().GetGenericArguments().FirstOrDefault();
                    if ( Tp is null ) {
                        Debug.WriteLine($"Attempted to deserialise {SP.GetType()}//{SP.PropertyName}, but the value was null and the type was unknown.", "WARNING");
                        return;
                    }
                    SP.Value = DeserialiseHelper((string?)Settings.Default[SP.PropertyName], Tp);
                    break;
            }
        }
    }

    /// <summary>
    /// Helps with specific type deserialisation, such as <see cref="FileSystemInfo"/> types which do not support construction via empty <see cref="string"/> values.
    /// </summary>
    /// <param name="PropertyStringValue">The property's string value.</param>
    /// <param name="ExpectedType">The expected type.</param>
    /// <returns>The deserialised value.</returns>
    internal static object? DeserialiseHelper(string? PropertyStringValue, Type ExpectedType ) {
        switch ( PropertyStringValue ) {
            case { } PSV:
                if ( ExpectedType == typeof(FileInfo) ) {
                    return !string.IsNullOrEmpty(PSV) ? new FileInfo(PSV) : null;
                }
                if ( ExpectedType == typeof(DirectoryInfo) ) {
                    return !string.IsNullOrEmpty(PSV) ? new DirectoryInfo(PSV) : null;
                }
                return PSV.Deserialise(ExpectedType);
            default:
                return null;
        }
    }

    /// <inheritdoc />
    public override ObservableCollection<ISavedPropertyInternalHolder> Properties { get; set; }

    /// <inheritdoc />
    public override IEnumerable<ISavedPropertyInternalHolder> GetInitialProperties() {
        yield return new SavedPropertyInternalHolder<FileInfo?>(
            nameof(FFmpegPath),
            Settings.Default.FFmpegPath.GetFile().Value,
            true);
        yield return new SavedPropertyInternalHolder<FileInfo?>(
            nameof(YoutubeDLPath),
            Settings.Default.YoutubeDLPath.GetFile().Value,
            true);
        yield return new SavedPropertyInternalHolder<DirectoryInfo?>(
            nameof(OutputFolder),
            Settings.Default.OutputFolder.GetDirectory().Value,
            true);
        yield return new SavedPropertyInternalHolder<int?>(
            nameof(TimesDownloaded),
            Settings.Default.TimesDownloaded,
            false);
    }

    public interface ISavedPropertyInternalHolder : ISavedProperty {
        /// <summary>
        /// Gets a value indicating whether the property value can be overridden by a local file.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the property can be overridden; otherwise, <see langword="false" />.
        /// </value>
        [JsonIgnore] bool CanOverride { get; }
    }

    public interface ISavedPropertyInternalHolder<T> : ISavedPropertyInternalHolder, ISavedProperty<T> { }

    public class SavedPropertyInternalHolder : SavedProperty, ISavedPropertyInternalHolder {
        /// <inheritdoc />
        public SavedPropertyInternalHolder( object? Value, PropertyChangingEventArgs PropertyChanging, PropertyChangedEventArgs PropertyChanged, string PropertyName, bool CanOverride ) : base(Value, PropertyChanging, PropertyChanged, PropertyName) => this.CanOverride = CanOverride;

        /// <inheritdoc />
        public SavedPropertyInternalHolder( string? Name, object? Value, bool CanOverride ) : base(Name, Value) => this.CanOverride = CanOverride;

        /// <inheritdoc />
        [JsonIgnore] public bool CanOverride { get; }
    }

    public class SavedPropertyInternalHolder<T> : SavedProperty<T>, ISavedPropertyInternalHolder<T> {
        /// <inheritdoc />
        public SavedPropertyInternalHolder( T? Value, PropertyChangingEventArgs PropertyChanging, PropertyChangedEventArgs<T> PropertyChanged, string PropertyName, bool CanOverride ) : base(Value, PropertyChanging, PropertyChanged, PropertyName) => this.CanOverride = CanOverride;

        /// <inheritdoc />
        public SavedPropertyInternalHolder( string? Name, T? Value, bool CanOverride ) : base(Name, Value) => this.CanOverride = CanOverride;

        /// <inheritdoc />
        [JsonIgnore] public bool CanOverride { get; }
    }
}