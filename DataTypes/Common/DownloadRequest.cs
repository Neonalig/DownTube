using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

using DownTube.DataTypes.Helpers;

using Octokit;

using ReactiveUI;

namespace DownTube.DataTypes.Common;

/// <summary>
/// Represents a request to download a file from the internet.
/// </summary>
/// <seealso cref="ReactiveObject" />
[SuppressMessage("ReSharper", "ExceptionNotDocumented")]
[SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
public class DownloadRequest : ReactiveObject {

    #region Static Properties

    /// <summary>
    /// The amount of ongoing <see cref="DownloadRequest"/> instances. Used to determine when the <see cref="DownloadClient"/> can be disposed.
    /// </summary>
    static int _OngoingAmount = 0;

    /// <summary>
    /// The download client.
    /// </summary>
    static HttpClient? _DownloadClient;

    /// <summary>
    /// Gets the download client.
    /// </summary>
    /// <value>
    /// The download client.
    /// </value>
    public static HttpClient DownloadClient {
        get {
            _DownloadClient ??= new HttpClient();
            return _DownloadClient;
        }
        private set => _DownloadClient = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the destination file path.
    /// </summary>
    /// <value>
    /// The destination path.
    /// </value>
    public FileInfo Destination { get; set; }

    /// <summary>
    /// Gets the size of the download (in bytes).
    /// </summary>
    /// <value>
    /// The size of the download (in bytes).
    /// </value>
    public long Size { get; private set; } = - 1;

    long _Position = - 1;

    /// <summary>
    /// Gets the current download position.
    /// </summary>
    /// <value>
    /// The current download position.
    /// </value>
    public long Position {
        get => _Position;
        private set { 
            if ( _Position != value ) {
                _Position = value;
                OnProgressUpdated(this, Progress);
            }
        }
    }

    /// <summary>
    /// Gets the current download progress.
    /// </summary>
    /// <value>
    /// The current download progress.
    /// </value>
    public double Progress => Ongoing ? (double)Position / Size : 0;

    /// <summary>
    /// The size of the download buffer (in bytes).
    /// </summary>
    long _BufferSize = 16384;

    /// <summary>
    /// Gets or sets the size of the download memory buffer (in bytes).
    /// </summary>
    /// <value>
    /// The size of the download buffer (in bytes).
    /// </value>
    /// <exception cref="UnexpectedPropertyChangeException" accessor="set">A property was changed when a request was currently <see cref="Ongoing"/>.</exception>
    public long BufferSize {
        get => _BufferSize;
        set {
            if ( Ongoing ) {
                throw new UnexpectedPropertyChangeException();
            }
            _BufferSize = value;
        }
    }

    /// <summary>
    /// Gets the download URL.
    /// </summary>
    /// <value>
    /// The download URL.
    /// </value>
    public string DownloadUrl { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="DownloadRequest"/> is ongoing.
    /// </summary>
    /// <value>
    ///   <see langword="true" /> if ongoing; otherwise, <see langword="false" />.
    /// </value>
    public bool Ongoing { get; private set; }

    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    /// <value>
    /// The cancellation token.
    /// </value>
    public CancellationTokenSource TokenSource { get; }

    #endregion

    /// <summary>
    /// Initialises a new instance of the <see cref="DownloadRequest"/> class.
    /// </summary>
    /// <param name="DownloadUrl">The download URL.</param>
    /// <param name="Destination">The destination.</param>
    /// <param name="TokenSource">The cancellation token.</param>
    public DownloadRequest( string DownloadUrl, FileInfo Destination, CancellationTokenSource? TokenSource = null ) {
        this.DownloadUrl = DownloadUrl;
        this.Destination = Destination;
        this.TokenSource = TokenSource ?? new CancellationTokenSource();
        _OngoingAmount++;
    }

    #region Public Methods

    /// <summary>
    /// Starts the download in a new thread.
    /// </summary>
    public void Start() => Task.Run(async () => await DownloadFileInternalAsync(), TokenSource.Token);

    /// <summary>
    /// Asynchronously starts the download in the current thread.
    /// </summary>
    public async Task StartAsync() => await DownloadFileInternalAsync();

    /// <summary>
    /// Cancels the download.
    /// </summary>
    public void Cancel() {
        TokenSource.Cancel();
        Destination.Delete();
    }

    #endregion

    #region Events

    #region DownloadStarted

    /// <summary>
    /// Occurs when a download request is actually started.
    /// </summary>
    /// <param name="Request">The request.</param>
    public delegate void DownloadStartedEventArgs( DownloadRequest Request );

    /// <summary>
    /// Occurs when a download request is started.
    /// </summary>
    public event DownloadStartedEventArgs? DownloadStarted;

    /// <summary>
    /// Called when a download request is started.
    /// </summary>
    /// <param name="Request">The request.</param>
    protected virtual void OnDownloadStarted( DownloadRequest Request ) => DownloadStarted?.Invoke(Request);

    #endregion

    #region ProgressUpdated

    /// <summary>
    /// Raised when the download progress is updated (i.e. when a buffer is filled and flushed)
    /// </summary>
    /// <param name="Request">The download request.</param>
    /// <param name="Progress">The progress.</param>
    public delegate void ProgressUpdatedEventArgs( DownloadRequest Request, double Progress );

    /// <summary>
    /// Occurs when the download progress is updated (i.e. when a buffer is filled and flushed)
    /// </summary>
    public event ProgressUpdatedEventArgs? ProgressUpdated;

    protected virtual void OnProgressUpdated( DownloadRequest Request, double D ) => ProgressUpdated?.Invoke(Request, D);

    #endregion

    #region DownloadComplete

    /// <summary>
    /// Raised when a download request completes.
    /// </summary>
    /// <param name="Request">The request.</param>
    /// <param name="Destination">The destination.</param>
    public delegate void DownloadCompleteEventArgs( DownloadRequest Request, FileInfo Destination );

    /// <summary>
    /// Occurs when a download request is completed.
    /// </summary>
    public event DownloadCompleteEventArgs? DownloadComplete;

    /// <summary>
    /// Raised when a download request completes.
    /// </summary>
    /// <param name="Request">The request.</param>
    /// <param name="Destination">The destination.</param>
    protected virtual void OnDownloadComplete( DownloadRequest Request, FileInfo Destination ) => DownloadComplete?.Invoke(Request, Destination);

    #endregion

    #endregion

    /// <summary>
    /// Asynchronously downloads the file, invoking relevant callbacks throughout.
    /// </summary>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    internal async Task DownloadFileInternalAsync() {
        Ongoing = true;
        byte[] BufferArray = new byte[BufferSize];
        Memory<Byte> Buffer = new Memory<byte>(BufferArray);

        HttpResponseMessage Msg = await DownloadClient.GetAsync(DownloadUrl, TokenSource.Token);

        await using ( FileStream FS = Destination.Create() ) {
            await using ( Stream Str = await Msg.Content.ReadAsStreamAsync(TokenSource.Token) ) {
                OnDownloadStarted(this);
                Size = Str.Length;
                while ( Str.Position + BufferSize < Size ) {
                    await Str.ReadAsync(Buffer, TokenSource.Token);
                    await FS.WriteAsync(Buffer, TokenSource.Token);
                    Position = Str.Position;
                }
                int R = (int)(Size - Str.Position); //Remainder bytes that don't fill a full buffer.
                if ( R > 0 ) {
                    Memory<byte> Remainder = new byte[R];
                    await Str.ReadAsync(Remainder, TokenSource.Token);
                    await FS.WriteAsync(Remainder, TokenSource.Token);
                    Position = Size;
                }
            }
        }
        OnDownloadComplete(this, Destination);
        Ongoing = false;
    }

    /// <summary>
    /// Asynchronously downloads a file.
    /// </summary>
    /// <param name="Url">The URL to the file.</param>
    /// <param name="Dest">The destination file path.</param>
    /// <param name="Buffer">The memory download buffer.</param>
    /// <param name="DownloadStarted">Raised when the download is started.</param>
    /// <param name="ProgressUpdated">Raised when the download progress updates.</param>
    /// <param name="DownloadComplete">Raised when the download completes.</param>
    /// <param name="Token">The cancellation token.</param>
    public static async Task DownloadFileAsync( string Url, FileInfo Dest, Memory<byte> Buffer, DownloadStartedEventArgs DownloadStarted, ProgressUpdatedEventArgs ProgressUpdated, DownloadCompleteEventArgs DownloadComplete, CancellationTokenSource? Token = null ) {
        DownloadRequest Req = new DownloadRequest(Url, Dest, Token ?? new CancellationTokenSource()) {
            DownloadStarted = DownloadStarted,
            ProgressUpdated = ProgressUpdated,
            DownloadComplete = DownloadComplete
        };
        await Req.StartAsync();
    }

    /// <summary>
    /// Asynchronously downloads a file.
    /// </summary>
    /// <param name="Url">The URL to the file.</param>
    /// <param name="Dest">The destination file path.</param>
    /// <param name="Buffer">The memory download buffer.</param>
    /// <param name="Token">The cancellation token.</param>
    public static async Task DownloadFileAsync( string Url, FileInfo Dest, Memory<byte> Buffer, CancellationToken Token = default ) {
        HttpResponseMessage Msg = await DownloadClient.GetAsync(Url, Token);

        await using ( FileStream FS = Dest.Create() ) {
            await using ( Stream Str = await Msg.Content.ReadAsStreamAsync(Token) ) {
                long L = Str.Length;
                int B = Buffer.Length;
                while ( Str.Position + B < L ) {
                    await Str.ReadAsync(Buffer, Token);
                    await FS.WriteAsync(Buffer, Token);
                }
                int R = (int)(L - Str.Position); //Remainder bytes that don't fill a full buffer.
                if ( R > 0 ) {
                    Memory<byte> Remainder = new byte[R];
                    await Str.ReadAsync(Remainder, Token);
                    await FS.WriteAsync(Remainder, Token);
                }
            }
        }
    }

    public static async Task DownloadRelease( Release Release, DirectoryInfo Destination, DownloadStartedEventArgs DownloadStarted, ProgressUpdatedEventArgs ProgressUpdated, DownloadCompleteEventArgs DownloadComplete, int BufferSize = 16384, bool CreateSubdirectory = false, CancellationTokenSource? Token = null ) {
        Token ??= new CancellationTokenSource();

        //Search through all assets for the '*.zip' file and assume the first is what we want.
        foreach ( ReleaseAsset Asset in Release.Assets ) {
            string AN = Asset.Name;
            if ( !AN.ToLowerInvariant().EndsWith(".zip") ) { continue; }

            //Create a pointer file such as {Destination}\{Asset.Name} (i.e. C:\Users\Release.zip)
            FileInfo FileDest = Destination.CreateSubfile(AN, false);
            //Also create the subdirectory if requested (with the same name as the asset, but without the extension at the end)
            if ( CreateSubdirectory ) {
                Destination = Destination.CreateSubdirectory(Path.GetFileNameWithoutExtension(AN));
            }

            //Download the asset to the pointer file
            string DownloadUrl = Asset.BrowserDownloadUrl;
            Memory<byte> Buffer = new byte[BufferSize];
            await DownloadFileAsync(DownloadUrl, FileDest, Buffer, DownloadStarted, ProgressUpdated, DownloadComplete, Token);

            //Once downloaded, extract the archive to {Destination}
            //We don't need to create a subdirectory if requested as that was already done prior
            FileDest.Extract(false);

            //Delete the original .zip file
            FileDest.Delete();

            //Profit :D
            break;
        }
    }

    ~DownloadRequest() {
        _OngoingAmount--;
        if (_DownloadClient is { } Cl && _OngoingAmount <= 0 ) {
            _OngoingAmount = 0;
            Cl.Dispose();
        }
    }
}

/// <summary>
/// Represents an exception thrown when a property is changed at an unexpected time (i.e. when a <see cref="DownloadRequest"/> has an ongoing download and you change the destination file).
/// </summary>
/// <seealso cref="ArgumentException" />
public class UnexpectedPropertyChangeException : ArgumentException {
    /// <summary>
    /// Initialises a new instance of the <see cref="UnexpectedPropertyChangeException"/> class.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    public UnexpectedPropertyChangeException( [CallerMemberName] string? PropertyName = null ) : base($"{PropertyName} was changed at an unexpected time.", PropertyName) { }
}