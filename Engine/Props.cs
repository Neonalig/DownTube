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
using System.Windows;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Manages writing to/reading from properties.
/// </summary>
public static class Props {
    /// <summary>
    /// Initialises the <see cref="Props"/> class.
    /// </summary>
    static Props() {
        //Debug.WriteLine("Static const.");
        Data = new SavedProps();
        Application.Current.MainWindow.Closing += ( _, _ ) => Data.Save();
    }

    /// <summary>
    /// The saved data.
    /// </summary>
    public static readonly SavedProps Data;

    /// <inheritdoc cref="SavedProps.FFmpegPath"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static FileInfo? FFmpegPath {
        get => Data.FFmpegPath;
        set => Data.FFmpegPath = value;
    }

    /// <inheritdoc cref="SavedProps.YoutubeDLPath"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static FileInfo? YoutubeDLPath {
        get => Data.YoutubeDLPath;
        set => Data.YoutubeDLPath = value;
    }

    /// <inheritdoc cref="SavedProps.OutputFolder"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static DirectoryInfo? OutputFolder {
        get => Data.OutputFolder;
        set => Data.OutputFolder = value;
    }

    /// <inheritdoc cref="SavedProps.TimesDownloaded"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static int TimesDownloaded {
        get => Data.TimesDownloaded;
        set => Data.TimesDownloaded = value;
    }
}