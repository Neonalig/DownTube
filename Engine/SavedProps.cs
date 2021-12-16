#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.IO;

using DownTube.DataTypes;
using DownTube.Properties;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Manages writing to/reading from properties.
/// </summary>
public sealed class SavedProps : SaveData {
    /// <summary>
    /// The local properties file.
    /// </summary>
    readonly FileInfo _LocalPropsFile = FileSystemInfoExtensions.AppDir.CreateSubfile("settings.json", false);

    /// <summary>
    /// Simplified record for JSON (de/)serialisation of local <c>override</c> properties.
    /// </summary>
    /// <remarks>If <see cref="_LocalPropsFile"/> exists, and JSON deserialisation of the file as this <see langword="record"/> is successful, these properties are used instead of those located at <see cref="Settings.Default"/>.</remarks>
    // ReSharper disable once ClassNeverInstantiated.Local
    record LocalProps( string FFmpegPath, string YoutubeDLPath, string OutputFolder );

    /// <summary>
    /// Initialises a new instance of the <see cref="SavedProps"/> class.
    /// </summary>
    public SavedProps() {
        if ( _LocalPropsFile.Exists
            && _LocalPropsFile.Deserialise<LocalProps>().IsSuccess(out LocalProps LP)) {
            FromLocalFile = true;

            _FFmpegPath    = LP.FFmpegPath.GetFile().Value;
            _YoutubeDLPath = LP.YoutubeDLPath.GetFile().Value;
            _OutputFolder  = LP.OutputFolder.GetDirectory().Value;
        } else {
            FromLocalFile = false;

            _FFmpegPath      = Settings.Default.FFmpegPath.GetFile().Value;
            _YoutubeDLPath   = Settings.Default.YoutubeDLPath.GetFile().Value;
            _OutputFolder    = Settings.Default.OutputFolder.GetDirectory().Value;
        }
        _TimesDownloaded = Settings.Default.TimesDownloaded;
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
    /// The ffmpeg path
    /// </summary>
    FileInfo? _FFmpegPath;

    /// <summary>
    /// Gets or sets the path to the ffmpeg executable.
    /// </summary>
    /// <value>
    /// The ffmpeg path.
    /// </value>
    public FileInfo? FFmpegPath {
        get => _FFmpegPath;
        set => SetProperty(ref _FFmpegPath, value);
    }

    /// <summary>
    /// The path to the youtube-dl executable.
    /// </summary>
    FileInfo? _YoutubeDLPath;

    /// <summary>
    /// Gets or sets the path to the youtube-dl executable.
    /// </summary>
    /// <value>
    /// The path to the youtube-dl executable.
    /// </value>
    public FileInfo? YoutubeDLPath {
        get => _YoutubeDLPath;
        set => SetProperty(ref _YoutubeDLPath, value);
    }

    /// <summary>
    /// The output folder
    /// </summary>
    DirectoryInfo? _OutputFolder;

    /// <summary>
    /// Gets or sets the output folder.
    /// </summary>
    /// <value>
    /// The output folder.
    /// </value>
    public DirectoryInfo? OutputFolder {
        get => _OutputFolder;
        set => SetProperty(ref _OutputFolder, value);
    }

    /// <summary>
    /// The number of times any song/video was been downloaded.
    /// </summary>
    int _TimesDownloaded;

    /// <summary>
    /// Gets or sets the number of times any song/video was been downloaded.
    /// </summary>
    /// <value>
    /// The number of times any song/video was been downloaded.
    /// </value>
    public int TimesDownloaded {
        get => _TimesDownloaded;
        set => SetProperty(ref _TimesDownloaded, value);
    }

    #endregion

    /// <inheritdoc />
    /// <exception cref="ArgumentException">The property with the name <paramref name="PropertyName"/> could not be found.</exception>
    internal override object? GetProp( string PropertyName ) =>
        PropertyName switch {
            nameof(FFmpegPath)      => FFmpegPath,
            nameof(YoutubeDLPath)   => YoutubeDLPath,
            nameof(OutputFolder)    => OutputFolder,
            nameof(TimesDownloaded) => TimesDownloaded,
            _                       => throw new ArgumentException($"The property with the name '{PropertyName}' could not be found.")
        };

    /// <inheritdoc />
    /// <exception cref="ArgumentException">The property with the name <paramref name="PropertyName"/> could not be found.</exception>
    internal override void SetProp( string PropertyName, object? Value ) {
        switch ( PropertyName ) {
            case nameof(FFmpegPath):
                FFmpegPath    = (FileInfo?)Value;
                break;
            case nameof(YoutubeDLPath):
                YoutubeDLPath = (FileInfo?)Value;
                break;
            case nameof(OutputFolder):
                OutputFolder  = (DirectoryInfo?)Value;
                break;
            case nameof(TimesDownloaded):
                TimesDownloaded = (int?)Value ?? 0;
                break;
            default:
                throw new ArgumentException($"The property with the name '{PropertyName}' could not be found.");
        }
    }

    /// <inheritdoc />
    [ SuppressMessage("ReSharper", "ExceptionNotDocumented")][ SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public override void Save() {
        Debug.WriteLine("Saving data...");
        base.Save();
        FromLocalFile = _LocalPropsFile.GetExists();
        Debug.WriteLine($"Save local? {FromLocalFile}");

        string
            FFmpeg    = _FFmpegPath?.FullName    ?? string.Empty,
            YoutubeDL = _YoutubeDLPath?.FullName ?? string.Empty,
            Output    = _OutputFolder?.FullName  ?? string.Empty;

        switch ( FromLocalFile ) {
            case true:
                LocalProps LP = new LocalProps(FFmpeg, YoutubeDL, Output);
                LP.Serialise(_LocalPropsFile);
                break;
            default:
                Settings.Default.FFmpegPath    = FFmpeg;
                Settings.Default.YoutubeDLPath = YoutubeDL;
                Settings.Default.OutputFolder  = Output;
                break;
        }
        Settings.Default.TimesDownloaded = TimesDownloaded;
        Settings.Default.Save();
    }
}