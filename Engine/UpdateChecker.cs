using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using DownTube.Converters;

using Octokit;

namespace DownTube.Engine;

/// <summary>
/// Provides methods for checking if the project is up-to-date with the latest GitHub release.
/// </summary>
public static class UpdateChecker {

    /// <summary>
    /// The <see cref="Version"/> &lt;-&gt; <see cref="string"/> converter used for the <see cref="Client"/> header and <see cref="Release.TagName"/> parsing.
    /// </summary>
    static readonly VersionToStringConverter _VerToStr = new VersionToStringConverter {
        Prefix = "v",
        Major = VersionPartDisplay.Display,
        Minor = VersionPartDisplay.Display,
        Build = VersionPartDisplay.Display,
        Revision = VersionPartDisplay.DisplayIfNonZero,
        Suffix = "-UC" //'...-UC' indicates '(U)pdate (C)hecker'
    };

    /// <summary>
    /// The GitHub client.
    /// </summary>
    static GitHubClient? _Client = null;

    /// <summary>
    /// Gets the GitHub client.
    /// </summary>
    /// <value>
    /// The GitHub client.
    /// </value>
    public static GitHubClient Client {
        get {
            _Client ??= new GitHubClient(
                new ProductHeaderValue(
                    StaticBindings.AppName,
                    _VerToStr.Forward(StaticBindings.AppVersion)
                    )
                );
            return _Client;
        }
        private set => _Client = value;
    }

    /// <summary>
    /// Contains found update information, such as it's <see cref="Version"/> and <see cref="Octokit.Release"/>.
    /// </summary>
    public record UpdateSearchResult( bool HasUpdate, Version Current, Version Newest, Release Release );

    /// <summary>
    /// Asynchronously checks for an updated release on the GitHub repository (starflash-studios/DownTube).
    /// </summary>
    /// <returns><see langword="true"/> if the latest release is a greater version than <see cref="StaticBindings.AppVersion"/></returns>
    public static async Task<Result<UpdateSearchResult>> SearchForUpdatesAsync() {
        Debug.WriteLine("Searching for updates...");
        try {
            Version Current = StaticBindings.AppVersion;
            Debug.WriteLine($"Current version is {Current}");

            Repository Repo = await Client.Repository.Get("starflash-studios", "DownTube");
            RepoUrl = Repo.HtmlUrl;
            Debug.WriteLine($"Repo is {Repo}");
            Release Latest = await Client.Repository.Release.GetLatest(Repo.Id);
            Debug.WriteLine($"Release is {Latest}");
            Version? V = _VerToStr.Reverse(Latest.TagName);
            Debug.WriteLine($"Version is {V}");
            if ( V is not null ) {
                Debug.WriteLine("Update successfully found.");
                return new Result<UpdateSearchResult>(new UpdateSearchResult(V > Current, Current, V, Latest));
            }
        } catch ( ApiException ApiEx ) {
            Debug.WriteLine($"ApiException thrown {ApiEx}");
            return ApiEx;
        }
        Debug.WriteLine("Unexpected.");
        return Result<UpdateSearchResult>.Unexpected;
    }

    /// <summary>
    /// Checks for any available updates, raising <paramref name="OnNewVersionDetected"/> if any are detected.
    /// </summary>
    /// <param name="OnNewVersionDetected">The action to raise when a new update is detected, or <see langword="null"/>.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static void CheckForUpdates( Action<UpdateSearchResult>? OnNewVersionDetected = null ) {
        Task.Run(async () => {
            UpdateSearchResult? Res = await SearchForUpdatesAsync();
            if ( Res is null ) { return; }
            HasUpdate = Res.HasUpdate;
            LatestVersion = Res.Newest;
            LatestRelease = Res.Release;
            OnNewVersionDetected?.Invoke(Res);
        });
    }

    #region Properties

    #region PropertyChanged Boilerplate

    /// <summary>
    /// Occurs when a <see langword="static"/> property value is changed.
    /// </summary>
    public static event StaticPropertyChangedEventArg? StaticPropertyChanged = delegate { };

    /// <summary>
    /// Called when a <see langword="static"/> property value is changed.
    /// </summary>
    /// <param name="OldValue">The old value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    static void OnStaticPropertyChanged( object? OldValue, object? NewValue, [CallerMemberName] string? PropertyName = null ) => StaticPropertyChanged?.Invoke(OldValue, NewValue, PropertyName);

    /// <summary>
    /// Changes the property value, raising <see cref="StaticPropertyChanged"/> if a change is detected.
    /// </summary>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="Value">The value to change.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    static void ChangeProperty<T>( [NotNullIfNotNull("NewValue")] ref T? Value, T? NewValue, [CallerMemberName] string? PropertyName = null ) {
        if (Value is null ? NewValue is not null : NewValue is null || !Value.Equals(NewValue) ) {
            T? Old = Value;
            Value = NewValue;
            OnStaticPropertyChanged(Old, NewValue, PropertyName);
        }
    }

    #endregion

    #region HasUpdate

    /// <summary>
    /// Indicates whether there is a newer GitHub release available.
    /// </summary>
    static bool _HasUpdate;

    /// <summary>
    /// Gets a value indicating whether there is a newer GitHub release available.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if there is an update available; otherwise, <see langword="false" />.
    /// </value>
    public static bool HasUpdate {
        get => _HasUpdate;
        private set => ChangeProperty(ref _HasUpdate, value);
    }

    #endregion

    #region LatestVersion

    /// <summary>
    /// The latest update version, or <see langword="null"/>.
    /// </summary>
    static Version? _LatestVersion;

    /// <summary>
    /// Gets the latest update version.
    /// </summary>
    /// <value>
    /// The latest update version, or <see langword="null"/> if the project is up-to-date / <see cref="CheckForUpdates"/> has not yet been called.
    /// </value>
    public static Version? LatestVersion {
        get => _LatestVersion;
        private set => ChangeProperty(ref _LatestVersion, value);
    }

    #endregion

    #region LatestRelease


    /// <summary>
    /// The latest release.
    /// </summary>
    static Release? _LatestRelease;

    /// <summary>
    /// Gets the latest release.
    /// </summary>
    /// <value>
    /// The latest release.
    /// </value>
    public static Release? LatestRelease {
        get => _LatestRelease;
        private set => ChangeProperty(ref _LatestRelease, value);
    }

    #endregion

    #region RepoUrl

    /// <summary>
    /// The repository's URL
    /// </summary>
    static string _RepoUrl = string.Empty;

    /// <summary>
    /// Gets the URL for the repository.
    /// </summary>
    /// <value>
    /// The repository's URL.
    /// </value>
    public static string RepoUrl {
        get => _RepoUrl;
        private set => ChangeProperty(ref _RepoUrl, value);
    }

    #endregion

    #endregion

}

/// <summary>
/// Event arguments raised when a <see langword="static"/> property value is changed.
/// </summary>
/// <param name="OldValue">The old value.</param>
/// <param name="NewValue">The new value.</param>
/// <param name="PropertyName">The name of the property.</param>
public delegate void StaticPropertyChangedEventArg( object? OldValue, object? NewValue, [CallerMemberName] string? PropertyName = null );