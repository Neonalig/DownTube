using static DownTube.DataTypes.IProcedure;

namespace DownTube.DataTypes;

public class Procedure : IProcedure, IProcedureStatusMessageHandler, IProcedureExceptionHandler {

    /// <summary>
    /// Initialises a new instance of the <see cref="Procedure"/> class.
    /// </summary>
    /// <param name="Methods">The collection of methods to invoke throughout the procedure's lifetime.</param>
    public Procedure( params ProcedureMethod[] Methods ) => this.Methods = Methods;

    /// <summary>
    /// The collection of methods to invoke throughout the procedure's lifetime.
    /// </summary>
    public ProcedureMethod[] Methods;

    /// <summary>
    /// A runtime asynchronous procedure method.
    /// </summary>
    /// <param name="StatusHandler">The status handler.</param>
    /// <param name="FailFast">The exception handler.</param>
    /// <returns>An awaitable task.</returns>
    public delegate Task ProcedureMethod( IProcedureStatusMessageHandler StatusHandler, IProcedureExceptionHandler FailFast );

    #region Events

    /// <summary>
    /// Occurs when a <see cref="IProcedureStartMessage"/> is received from an ongoing <see cref="IProcedure"/>.
    /// </summary>
    public event ProcedureStartEventHandler? ProcedureStart;

    /// <summary>
    /// Occurs when a <see cref="IProcedureProgressMessage"/> is received from an ongoing <see cref="IProcedure"/>.
    /// </summary>
    public event ProcedureProgressEventHandler? ProcedureProgress;

    /// <summary>
    /// Occurs when a <see cref="IProcedureFinishMessage"/> is received from an ongoing <see cref="IProcedure"/>.
    /// </summary>
    public event ProcedureFinishEventHandler? ProcedureFinish;

    /// <summary>
    /// Occurs when a <see cref="IProcedureStatusMessage"/> is received from an ongoing <see cref="IProcedure"/>.
    /// </summary>
    /// <remarks>Subscriptions to the <see langword="event"/> will be raised every time a message deriving from <see cref="IProcedureStatusMessage"/> is posted, such as when <see cref="ProcedureStart"/>, <see cref="ProcedureProgress"/> and <see cref="ProcedureFinish"/> are raised.</remarks>
    /// <seealso cref="ProcedureStart"/>
    /// <seealso cref="ProcedureProgress"/>
    /// <seealso cref="ProcedureFinish"/>
    public event ProcedureStatusEventHandler? ProcedureStatus;

    /// <summary>
    /// Occurs when an exception is thrown during procedure runtime.
    /// </summary>
    public event ProcedureExceptionEventHandler? ProcedureException;

    /// <summary>
    /// Represents the method that will handle the <see cref="ProcedureException"/> <see langword="event"/> on a <see cref="IProcedure"/> instance, and provide the relevant event arguments.
    /// </summary>
    /// <param name="Sender">The <see langword="event"/> raiser.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    /// <seealso cref="ProcedureExceptionEventArgs"/>
    public delegate void ProcedureExceptionEventHandler( IProcedure? Sender, ProcedureExceptionEventArgs E );

    /// <summary>
    /// Provides additional data for the <see cref="ProcedureExceptionEventHandler"/>.
    /// </summary>
    /// <seealso cref="ProcedureExceptionEventHandler"/>
    public class ProcedureExceptionEventArgs : EventArgs {
        /// <summary>
        /// Constructs an instance of the <see cref="ProcedureExceptionEventArgs"/> <see langword="class"/>.
        /// </summary>
        /// <param name="Value">The collection of thrown exceptions.</param>
        public ProcedureExceptionEventArgs( IReadOnlyList<Exception> Value ) => this.Value = Value;

        /// <summary>
        /// The collection of thrown exceptions.
        /// </summary>
        public IReadOnlyList<Exception> Value { get; set; }
    }

    #region Invocators

    /// <summary>
    /// Raises the <see cref="ProcedureStart" /> <see langword="event"/>.
    /// </summary>
    /// <param name="E">The <see cref="IProcedure.ProcedureStartEventArgs"/> instance containing the <see langword="event"/> data.</param>
    protected virtual void OnProcedureStart( ProcedureStartEventArgs E ) {
        ProcedureStart?.Invoke(this, E);
        OnProcedureStatus(E);
    }

    /// <summary>
    /// Raises the <see cref="ProcedureProgress" /> <see langword="event"/>.
    /// </summary>
    /// <param name="E">The <see cref="IProcedure.ProcedureProgressEventArgs"/> instance containing the <see langword="event"/> data.</param>
    protected virtual void OnProcedureProgress( ProcedureProgressEventArgs E ) {
        ProcedureProgress?.Invoke(this, E);
        OnProcedureStatus(E);
    }

    /// <summary>
    /// Raises the <see cref="ProcedureFinish" /> <see langword="event"/>.
    /// </summary>
    /// <param name="E">The <see cref="IProcedure.ProcedureFinishEventArgs"/> instance containing the <see langword="event"/> data.</param>
    protected virtual void OnProcedureFinish( ProcedureFinishEventArgs E ) {
        ProcedureFinish?.Invoke(this, E);
        OnProcedureStatus(E);
    }

    /// <summary>
    /// Raises the <see cref="ProcedureStatus" /> <see langword="event"/>.
    /// </summary>
    /// <param name="E">The <see cref="IProcedure.ProcedureStatusEventArgs"/> instance containing the <see langword="event"/> data.</param>
    protected virtual void OnProcedureStatus( ProcedureStatusEventArgs E ) => ProcedureStatus?.Invoke(this, E);

    /// <summary>
    /// Raises the <see cref="ProcedureStatus" /> <see langword="event"/>.
    /// </summary>
    /// <param name="E">The <see cref="IProcedure.ProcedureStatusEventArgs"/> instance containing the <see langword="event"/> data.</param>
    protected virtual void OnProcedureStatus( IProcedureStatusEventArgs E ) => ProcedureStatus?.Invoke(this, E);

    /// <summary>
    /// Raises the <see cref="ProcedureException" /> <see langword="event"/>.
    /// </summary>
    /// <param name="E">The <see cref="ProcedureExceptionEventArgs"/> instance containing the <see langword="event"/> data.</param>
    protected virtual void OnProcedureException( ProcedureExceptionEventArgs E ) => ProcedureException?.Invoke(this, E);

    #endregion

    #endregion

    #region IProcedure Implementation

    /// <inheritdoc />
    public async Task Invoke() {
        foreach ( ProcedureMethod Method in Methods ) {
            await Method(this, this);
            if ( ExceptionStack.Count > 0 ) {
                Debug.WriteLine($"An exception was thrown in the procedure method stack. '{string.Join("', '", ExceptionStack)}'");
                //ExceptionStack.AsList().AsReadOnly()
                OnProcedureException(new ProcedureExceptionEventArgs(new ReadOnlyList<Exception>(ExceptionStack)));
            }
        }
    }

    #endregion

    #region IProcedureStatusMessageHandler Implementation

    /// <inheritdoc />
    public void PostStatus( IProcedureStartMessage Message ) => OnProcedureStart(new ProcedureStartEventArgs(Message));

    /// <inheritdoc />
    public void PostStatus( IProcedureProgressMessage Message ) => OnProcedureProgress(new ProcedureProgressEventArgs(Message));

    /// <inheritdoc />
    public void PostStatus( IProcedureFinishMessage Message ) => OnProcedureFinish(new ProcedureFinishEventArgs(Message));

    /// <inheritdoc />
    public void PostStatus( IProcedureStatusMessage Message ) => OnProcedureStatus(new ProcedureStatusEventArgs(Message));

    #endregion

    #region IProcedureExceptionHandler Implementation

    /// <inheritdoc />
    public void Throw( Exception Exception ) => ExceptionStack.Push(Exception);

    /// <summary>
    /// The collection of thrown <see cref="IProcedure"/> <see cref="Exception">exceptions</see>.
    /// </summary>
    public Stack<Exception> ExceptionStack = new Stack<Exception>();

    #endregion
}

/// <summary>
/// Handles runtime <see cref="Exception">exceptions</see> thrown via an ongoing <see cref="IProcedure"/>.
/// </summary>
public interface IProcedureExceptionHandler {
    /// <summary>
    /// Throws the specified exception on the procedure, safely aborting the thread at the earliest convenience.
    /// </summary>
    /// <param name="Exception">The exception.</param>
    void Throw( Exception Exception );
}

/// <summary>
/// Represents a system that can post and handle <see cref="IProcedureStatusMessage">IProcedureStatusMessages</see>.
/// </summary>
public interface IProcedureStatusMessageHandler {

    /// <summary>
    /// Posts the status message.
    /// </summary>
    /// <param name="Message">The procedure start message.</param>
    void PostStatus( IProcedureStartMessage Message );

    /// <summary>
    /// Posts the status message.
    /// </summary>
    /// <param name="Message">The procedure progress message.</param>
    void PostStatus( IProcedureProgressMessage Message );

    /// <summary>
    /// Posts the status message.
    /// </summary>
    /// <param name="Message">The procedure finish message.</param>
    void PostStatus( IProcedureFinishMessage Message );

    /// <summary>
    /// Posts the status message.
    /// </summary>
    /// <param name="Message">The procedure status message.</param>
    void PostStatus( IProcedureStatusMessage Message );

}

//public abstract class CustomProcedure : IProcedure { //TODO: Implement <--
//    /// <inheritdoc />
//    public abstract Task Invoke();
//}

/// <summary>
/// Represents an asynchronous procedure which can be run on additional threads.
/// </summary>
public interface IProcedure {

    /// <summary>
    /// Invokes the procedure runtime.
    /// </summary>
    /// <returns>An awaitable task.</returns>
    Task Invoke();

    #region Events

    #region ProcedureStart

    /// <summary>
    /// Represents the method that will handle the ProcedureStart <see langword="event"/> on a <see cref="IProcedure"/> instance, and provide the relevant event arguments.
    /// </summary>
    /// <param name="Sender">The <see langword="event"/> raiser.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    /// <seealso cref="ProcedureStartEventArgs"/>
    public delegate void ProcedureStartEventHandler( IProcedure? Sender, ProcedureStartEventArgs E );

    /// <summary>
    /// Provides additional data for the <see cref="ProcedureStartEventHandler"/>.
    /// </summary>
    /// <seealso cref="ProcedureStartEventHandler"/>
    public class ProcedureStartEventArgs : EventArgs, IProcedureStatusEventArgs {
        /// <summary>
        /// Constructs an instance of the <see cref="ProcedureStartEventArgs"/> <see langword="class"/>.
        /// </summary>
        /// <param name="Value">The status message.</param>
        public ProcedureStartEventArgs( IProcedureStartMessage Value ) => this.Value = Value;

        /// <summary>
        /// The status message.
        /// </summary>
        public IProcedureStartMessage Value { get; set; }

        /// <inheritdoc />
        IProcedureStatusMessage IProcedureStatusEventArgs.Message => Value;
    }

    #endregion

    #region ProcedureProgress

    /// <summary>
    /// Represents the method that will handle the ProcedureProgress <see langword="event"/> on a <see cref="IProcedure"/> instance, and provide the relevant event arguments.
    /// </summary>
    /// <param name="Sender">The <see langword="event"/> raiser.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    /// <seealso cref="ProcedureProgressEventArgs"/>
    public delegate void ProcedureProgressEventHandler( IProcedure? Sender, ProcedureProgressEventArgs E );

    /// <summary>
    /// Provides additional data for the <see cref="ProcedureProgressEventHandler"/>.
    /// </summary>
    /// <seealso cref="ProcedureProgressEventHandler"/>
    public class ProcedureProgressEventArgs : EventArgs, IProcedureStatusEventArgs {
        /// <summary>
        /// Constructs an instance of the <see cref="ProcedureProgressEventArgs"/> <see langword="class"/>.
        /// </summary>
        /// <param name="Value">The status message.</param>
        public ProcedureProgressEventArgs( IProcedureProgressMessage Value ) => this.Value = Value;

        /// <summary>
        /// The status message.
        /// </summary>
        public IProcedureProgressMessage Value { get; set; }

        /// <inheritdoc />
        IProcedureStatusMessage IProcedureStatusEventArgs.Message => Value;
    }

    #endregion

    #region ProcedureFinish

    /// <summary>
    /// Represents the method that will handle the ProcedureFinish <see langword="event"/> on a <see cref="IProcedure"/> instance, and provide the relevant event arguments.
    /// </summary>
    /// <param name="Sender">The <see langword="event"/> raiser.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    /// <seealso cref="ProcedureFinishEventArgs"/>
    public delegate void ProcedureFinishEventHandler( IProcedure? Sender, ProcedureFinishEventArgs E );

    /// <summary>
    /// Provides additional data for the <see cref="ProcedureFinishEventHandler"/>.
    /// </summary>
    /// <seealso cref="ProcedureFinishEventHandler"/>
    public class ProcedureFinishEventArgs : EventArgs, IProcedureStatusEventArgs {
        /// <summary>
        /// Constructs an instance of the <see cref="ProcedureFinishEventArgs"/> <see langword="class"/>.
        /// </summary>
        /// <param name="Value">The status message.</param>
        public ProcedureFinishEventArgs( IProcedureFinishMessage Value ) => this.Value = Value;

        /// <summary>
        /// The status message.
        /// </summary>
        public IProcedureFinishMessage Value { get; set; }

        /// <inheritdoc />
        IProcedureStatusMessage IProcedureStatusEventArgs.Message => Value;
    }

    #endregion

    #region ProcedureStatus

    /// <summary>
    /// Represents the method that will handle the ProcedureStatus <see langword="event"/> on a <see cref="IProcedure"/> instance, and provide the relevant event arguments.
    /// </summary>
    /// <param name="Sender">The <see langword="event"/> raiser.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    /// <seealso cref="ProcedureStatusEventArgs"/>
    /// <seealso cref="IProcedureStatusEventArgs"/>
    public delegate void ProcedureStatusEventHandler( IProcedure? Sender, IProcedureStatusEventArgs E );

    /// <summary>
    /// Represents the event arguments for a <see cref="IProcedureStatusMessage"/> <see langword="event"/> handler.
    /// </summary>
    public interface IProcedureStatusEventArgs {

        /// <summary>
        /// Gets the status message.
        /// </summary>
        /// <value>
        /// The status message.
        /// </value>
        IProcedureStatusMessage Message { get; }
    }

    /// <summary>
    /// Provides additional data for the <see cref="ProcedureStatusEventHandler"/>.
    /// </summary>
    /// <seealso cref="ProcedureStatusEventHandler"/>
    public class ProcedureStatusEventArgs : EventArgs, IProcedureStatusEventArgs {
        /// <summary>
        /// Constructs an instance of the <see cref="ProcedureStatusEventArgs"/> <see langword="class"/>.
        /// </summary>
        /// <param name="Value">The status message.</param>
        public ProcedureStatusEventArgs( IProcedureStatusMessage Value ) => this.Value = Value;

        /// <summary>
        /// The status message.
        /// </summary>
        public IProcedureStatusMessage Value { get; set; }

        /// <inheritdoc />
        IProcedureStatusMessage IProcedureStatusEventArgs.Message => Value;
    }

    #endregion

    #endregion

}

/// <summary>
/// Represents a status update from an ongoing <see cref="Procedure"/>.
/// </summary>
public interface IProcedureStatusMessage { }

/// <summary>
/// Represents the starting status message from a <see cref="Procedure"/>.
/// </summary>
/// <seealso cref="IProcedureStatusMessage"/>
public interface IProcedureStartMessage : IProcedureStatusMessage { }

/// <summary>
/// Represents the ongoing progress status message from a working <see cref="Procedure"/>.
/// </summary>
/// <seealso cref="IProcedureStatusMessage"/>
public interface IProcedureProgressMessage : IProcedureStatusMessage { }

/// <summary>
/// Represents the finishing status message from an ongoing <see cref="Procedure"/>.
/// </summary>
/// <seealso cref="IProcedureStatusMessage"/>
public interface IProcedureFinishMessage : IProcedureStatusMessage { }

public class ReadOnlyList<T> : IReadOnlyList<T> {
    readonly T[] _Items;

    #region Constructors

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <remarks>The internal collection will be equivalent to <see cref="Array.Empty{T}"/>.</remarks>
    public ReadOnlyList() => _Items = Array.Empty<T>();

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="Items">The items collection.</param>
    public ReadOnlyList( IList<T> Items ) {
        int L = Items.Count;
        _Items = new T[L];
        for ( int I = 0; I < Items.Count; I++ ) {
            _Items[I] = Items[I];
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="Items">The items collection.</param>
    public ReadOnlyList( ICollection<T> Items ) {
        int L = Items.Count;
        _Items = new T[L];
        int I = 0;
        foreach ( T Item in Items ) {
            _Items[I] = Item;
            I++;
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="ReadOnlyItems">The items collection.</param>
    public ReadOnlyList( IReadOnlyList<T> ReadOnlyItems ) {
        int L = ReadOnlyItems.Count;
        _Items = new T[L];
        for ( int I = 0; I < ReadOnlyItems.Count; I++ ) {
            _Items[I] = ReadOnlyItems[I];
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="ReadOnlyItems">The items collection.</param>
    public ReadOnlyList( IReadOnlyCollection<T> ReadOnlyItems ) {
        int L = ReadOnlyItems.Count;
        _Items = new T[L];
        int I = 0;
        foreach ( T Item in ReadOnlyItems ) {
            _Items[I] = Item;
            I++;
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="ItemsList">The items collection.</param>
    public ReadOnlyList( List<T> ItemsList ) : this(Items: ItemsList) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="ItemsArray">The items collection.</param>
    public ReadOnlyList( T[] ItemsArray ) : this(Items: ItemsArray) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="ReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="Items">The items collection.</param>
    public ReadOnlyList( IEnumerable<T> Items ) : this(new List<T>(Items)) { }

    #endregion

    #region IReadOnlyList<T> Implementation

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() {
        foreach ( T Item in _Items ) {
            yield return Item;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => _Items.Length;

    /// <inheritdoc />
    public T this[ int Index ] => _Items[Index];

    #endregion

    /// <inheritdoc />
    public override string? ToString() => _Items.ToString();

}