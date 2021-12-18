#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;
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
    /// Raised when a property value is about to be changed.
    /// </summary>
    /// <param name="E">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> instance containing the event data.</param>
    public delegate void PropChangingEvent( System.ComponentModel.PropertyChangingEventArgs E );

    /// <summary>
    /// Raised when a property value has just been changed.
    /// </summary>
    /// <param name="E">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> instance containing the event data.</param>
    public delegate void PropChangedEvent( System.ComponentModel.PropertyChangedEventArgs E );

    /// <summary>
    /// The property changing event handler.
    /// </summary>
    public static event PropChangingEvent PropertyChangingEventHandler = delegate { };

    /// <summary>
    /// The property changed event handler.
    /// </summary>
    public static event PropChangedEvent PropertyChangedEventHandler = delegate { };

    /// <summary> The saved property data instance. </summary>
    public static readonly SavedProps Data;

    /// <inheritdoc cref="SavedProps.FFmpegPath"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static FileInfo? FFmpegPath {
        get => Data.FFmpegPath;
        set {
            if ( value != Data.FFmpegPath ) {
                PropertyChangingEventHandler(new System.ComponentModel.PropertyChangingEventArgs(nameof(FFmpegPath)));
                Data.FFmpegPath = value;
                PropertyChangedEventHandler(new System.ComponentModel.PropertyChangedEventArgs(nameof(FFmpegPath)));
            }
        }
    }

    /// <inheritdoc cref="SavedProps.YoutubeDLPath"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static FileInfo? YoutubeDLPath {
        get => Data.YoutubeDLPath;
        set {
            if ( value != Data.YoutubeDLPath ) {
                PropertyChangingEventHandler(new System.ComponentModel.PropertyChangingEventArgs(nameof(YoutubeDLPath)));
                Data.YoutubeDLPath = value;
                PropertyChangedEventHandler(new System.ComponentModel.PropertyChangedEventArgs(nameof(YoutubeDLPath)));
            }
        }
    }

    /// <inheritdoc cref="SavedProps.OutputFolder"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static DirectoryInfo? OutputFolder {
        get => Data.OutputFolder;
        set {
            if ( value != Data.OutputFolder ) {
                PropertyChangingEventHandler(new System.ComponentModel.PropertyChangingEventArgs(nameof(OutputFolder)));
                Data.OutputFolder = value;
                PropertyChangedEventHandler(new System.ComponentModel.PropertyChangedEventArgs(nameof(OutputFolder)));
            }
        }
    }

    /// <inheritdoc cref="SavedProps.TimesDownloaded"/>
    [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
    public static int TimesDownloaded {
        get => Data.TimesDownloaded;
        set {
            if ( value != Data.TimesDownloaded ) {
                PropertyChangingEventHandler(new System.ComponentModel.PropertyChangingEventArgs(nameof(TimesDownloaded)));
                Data.TimesDownloaded = value;
                PropertyChangedEventHandler(new System.ComponentModel.PropertyChangedEventArgs(nameof(TimesDownloaded)));
            }
        }
    }
}