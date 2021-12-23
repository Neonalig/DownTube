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
using System.Windows;

using DownTube.DataTypes;

#endregion

namespace DownTube.Engine;

/// <summary>
/// Manages writing to/reading from properties.
/// </summary>
[SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public static class Props {
    /// <summary>
    /// Initialises the <see cref="Props"/> class.
    /// </summary>
    static Props() {
        Application.Current.MainWindow.Closing += ( _, _ ) => Save(true);

        FFmpegPath = new NamedObservedValue<FileInfo?>(nameof(FFmpegPath));
        YoutubeDLPath = new NamedObservedValue<FileInfo?>(nameof(YoutubeDLPath));
        OutputFolder = new NamedObservedValue<DirectoryInfo?>(nameof(OutputFolder));
        TimesDownloaded = new NamedObservedValue<int>(nameof(TimesDownloaded));
        IgnoredVersions = new NamedObservedValueCollection<Version>(nameof(IgnoredVersions));
        LastCheckDate = new NamedObservedValue<DateOnly?>(nameof(LastCheckDate));
        UpdateFrequency = new NamedObservedValue<UpdateCheckFrequency>(nameof(UpdateFrequency));

        Properties = new ReadOnlyCollection<INamedSave>(
            new List<INamedSave> {
                FFmpegPath,
                YoutubeDLPath,
                OutputFolder,
                TimesDownloaded,
                IgnoredVersions,
                LastCheckDate,
                UpdateFrequency
            });

        Read();
    }

    /// <summary>
    /// Raises the <see cref="SavedPropertyChanged"/> <see langword="event"/>.
    /// </summary>
    /// <param name="Property">The <see langword="event"/> raiser.</param>
    /// <param name="PropertyName">The raised <see langword="event"/> argument.</param>
    public static void OnSavedPropertyChanged( ISave Property, string? PropertyName ) => SavedPropertyChanged?.Invoke(Property, new SavedPropertyChangedEventArgs(PropertyName));

    /// <summary>
    /// Raises the <see cref="SavedPropertyChanged"/> <see langword="event"/>.
    /// </summary>
    /// <param name="NamedProperty">The <see langword="event"/> raiser.</param>
    public static void OnSavedPropertyChanged( INamedSave NamedProperty ) => SavedPropertyChanged?.Invoke(NamedProperty, new SavedPropertyChangedEventArgs(NamedProperty.PropertyName));

    /// <summary>
    /// Raised when a saved property value is changed.
    /// </summary>
    public static event SavedPropertyChangedEventHandler? SavedPropertyChanged;

    /// <summary>
    /// Saves all unsaved changes in the observed <see cref="Properties"/> collection.
    /// </summary>
    /// <param name="SaveToFile">Whether the properties should also be written to the <see cref="SettingsFile"/>. Properties will be saved regardless if the <see cref="SettingsFile"/> does not currently exist at the time of saving.</param>
    public static void Save( bool SaveToFile = true ) {
        bool Any = false;
        foreach ( INamedSave Property in Properties ) {
            if ( Property.IsDirty ) {
                Any = true;
                Property.Save();
                OnSavedPropertyChanged(Property);
            }
        }
        if ( SaveToFile && (Any || !SettingsFile.Exists) ) {
            Write();
        }
    }

    /// <summary>
    /// Reverts all unsaved changes in the observed <see cref="Properties"/> collection.
    /// </summary>
    public static void Revert() {
        foreach ( INamedSave Property in Properties ) {
            if ( Property.IsDirty ) {
                Property.Revert();
                OnSavedPropertyChanged(Property);
            }
        }
    }

    /// <summary>
    /// The location of the '<c>Settings.json</c>' file that property values are saved/loaded from.
    /// </summary>
    public static readonly FileInfo SettingsFile = FileSystemInfoExtensions.AppDir.CreateSubfile("Settings.json");

    /// <summary>
    /// Exports all <b>saved</b> property values to the <see cref="SettingsFile"/> as JSON data.
    /// </summary>
    public static void Write() => Export().Serialise(SettingsFile);

    /// <summary>
    /// Imports all valid JSON property values from the <see cref="SettingsFile"/> and saves the changes.
    /// </summary>
    public static void Read() {
        if ( SettingsFile.Deserialise<ExportData>().Out(out ExportData Data, out Result<ExportData> Reason) ) {
            Import(Data);
        } else {
            Reason.LogWithHeader($"Failed reading settings from {SettingsFile.FullName}");
        }
    }

    /// <summary>
    /// Exports all <b>saved</b> property values.
    /// </summary>
    /// <returns>The exported data.</returns>
    public static ExportData Export() => new ExportData(
        FFmpegPath.Saved?.FullName,
        YoutubeDLPath.Saved?.FullName,
        OutputFolder.Saved?.FullName,
        TimesDownloaded.Saved,
        IgnoredVersions.Saved,
        LastCheckDate.Saved?.DayNumber,
        UpdateFrequency.Saved
    );

    /// <summary>
    /// Imports the given data then saves the changes.
    /// </summary>
    /// <param name="Data">The data to import.</param>
    public static void Import( ExportData Data ) {
        FFmpegPath.Value = Data.FFmpegPath.GetFile().Value;
        FFmpegPath.Save();

        YoutubeDLPath.Value = Data.YoutubeDLPath.GetFile().Value;
        YoutubeDLPath.Save();

        OutputFolder.Value = Data.OutputFolder.GetDirectory().Value;
        OutputFolder.Save();

        TimesDownloaded.Value = Data.TimesDownloaded;
        TimesDownloaded.Save();

        //Below is a check to truncate the 'IgnoredVersions' collection by removing all version <= the current, as the user will never be prompted to 'update' to an older or current version (therefore there is no reason for the user to ignore that update anymore.)
        IgnoredVersions.Value = Data.IgnoredVersions?.Where(IN => IN > StaticBindings.AppVersion).ToArray() ?? Array.Empty<Version>();
        IgnoredVersions.Save();

        LastCheckDate.Value = Data.LastCheckDate switch {
            { } LCD => DateOnly.FromDayNumber(LCD),
            _       => null
        };
        LastCheckDate.Save();

        UpdateFrequency.Value = Data.UpdateFrequency ?? UpdateCheckFrequency.Daily;
        UpdateFrequency.Save();
    }

    /// <summary>
    /// The datatype responsible for containing all underlying property values.
    /// </summary>
    /// <param name="FFmpegPath">The file path to the 'ffmpeg.exe' executable.</param>
    /// <param name="YoutubeDLPath">The file path to the 'youtube-dl.exe' executable.</param>
    /// <param name="OutputFolder">The path to the output directory.</param>
    /// <param name="TimesDownloaded">The amount of times songs/videos have been downloaded since application epoch.</param>
    /// <param name="IgnoredVersions">The collection of ignored update versions.</param>
    public record ExportData( string? FFmpegPath, string? YoutubeDLPath, string? OutputFolder, int TimesDownloaded, Version[]? IgnoredVersions, int? LastCheckDate, UpdateCheckFrequency? UpdateFrequency );

    /// <summary>
    /// A collection of observed properties.
    /// </summary>
    public static readonly IReadOnlyList<INamedSave> Properties;

    /// <summary>
    /// The file path to the 'ffmpeg.exe' executable.
    /// </summary>
    public static readonly NamedObservedValue<FileInfo?> FFmpegPath;

    /// <summary>
    /// The file path to the 'youtube-dl.exe' executable.
    /// </summary>
    public static readonly NamedObservedValue<FileInfo?> YoutubeDLPath;

    /// <summary>
    /// The path to the output directory.
    /// </summary>
    public static readonly NamedObservedValue<DirectoryInfo?> OutputFolder;

    /// <summary>
    /// The amount of times songs/videos have been downloaded since application epoch.
    /// </summary>
    public static readonly NamedObservedValue<int> TimesDownloaded;

    /// <summary>
    /// The collection of ignored update versions.
    /// </summary>
    public static readonly NamedObservedValueCollection<Version> IgnoredVersions;

    /// <summary>
    /// The last date <see cref="UpdateChecker.CheckForUpdates(bool, Action{UpdateChecker.UpdateSearchResult}?)"/> was ran.
    /// </summary>
    public static readonly NamedObservedValue<DateOnly?> LastCheckDate;

    /// <summary>
    /// The update check frequency.
    /// </summary>
    public static readonly NamedObservedValue<UpdateCheckFrequency> UpdateFrequency;

    /// <summary>
    /// The current date today (in UTC).
    /// </summary>
    /// <remarks>Since the program is only a small, simple application, we assume that the day the program starts is consistent throughout the entire runtime (it is unlikely the user will keep the application running non-stop for days on end.)</remarks>
    internal static readonly DateTime Today = DateTime.UtcNow;

    //internal static TimeSpan TimeSinceLastUpdateCheck => (Today - LastCheckDate.Value.ToDateTime(TimeOnly.MinValue));

    public static bool CanCheckForUpdates() => LastCheckDate.Value switch {
        { } LCD => UpdateFrequency.Value switch {
            //Always check for updates
            UpdateCheckFrequency.Always => true,
            //Check for updates every day
            UpdateCheckFrequency.Daily => LCD.Day != Today.Day,
            //Check for updates every sunday 
            UpdateCheckFrequency.Weekly => LCD.DayOfWeek      != Today.DayOfWeek
                                           && Today.DayOfWeek == DayOfWeek.Sunday,
            //Check for updates on the first of every month
            UpdateCheckFrequency.Monthly => LCD.Day      != Today.Day
                                            && Today.Day == 1,
            //Never check for updates
            UpdateCheckFrequency.Never => false,
            //Unexpected UpdateCheckFrequency value
            _ => throw new EnumValueOutOfRangeException<UpdateCheckFrequency>(UpdateFrequency.Value)
        },
        _ => true
    };
}

/// <summary>
/// Possible values for the frequency in which <see cref="UpdateChecker.CheckForUpdates(bool, Action{UpdateChecker.UpdateSearchResult}?)"/> can be ran.
/// </summary>
public enum UpdateCheckFrequency {
    /// <summary>
    /// Every time the application opens.
    /// </summary>
    Always,
    /// <summary>
    /// At most once per day.
    /// </summary>
    Daily,
    /// <summary>
    /// At most once a week.
    /// </summary>
    Weekly,
    /// <summary>
    /// At most once a month.
    /// </summary>
    Monthly,
    /// <summary>
    /// The application will never look for updates.
    /// </summary>
    Never
}