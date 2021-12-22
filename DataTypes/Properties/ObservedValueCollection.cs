using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace DownTube.DataTypes.Properties;

/// <summary>
/// Represents a simple value type collection which supports saving/loading.
/// </summary>
/// <typeparam name="T">The contained value type.</typeparam>
/// <seealso cref="SimpleSave{T}"/>
/// <seealso cref="ICollection{T}"/>
[JsonArray]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public class ObservedValueCollection<T> : SimpleSave<T>, ICollection<T> {
    [JsonIgnore] readonly ObservableCollection<T> _Coll;
    /// <summary>
    /// The collection of saved elements.
    /// </summary>
    [JsonIgnore] public /*[JsonProperty("Values", Order = 0)]*/ T[] Saved;

    /// <summary>
    /// Initialises a new instance of the <see cref="ObservedValueCollection{T}"/> class.
    /// </summary>
    [JsonConstructor] public ObservedValueCollection() {
        _Coll = new ObservableCollection<T>();
        Saved = Array.Empty<T>();
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ObservedValueCollection{T}"/> class.
    /// </summary>
    /// <param name="Collection">The collection.</param>
    public ObservedValueCollection( ObservableCollection<T> Collection ) {
        _Coll = Collection;
        Saved = _Coll.ToArray();
    }

    /// <summary>
    /// Directly sets the current collection of items.
    /// </summary>
    /// <remarks>This will not call <see cref="Save()"/>. You must do that manually when appropriate.</remarks>
    /// <value>
    /// The collection of items.
    /// </value>
    public T[] Value {
        //set {
        //    Saved = value;
        //    Revert();
        //}
        set {
            lock ( _Coll ) {
                _Coll.Clear();
                // ReSharper disable HeuristicUnreachableCode
                if ( value is null ) { return; } //Can occur due to malformed json data. If the data is 'null', just clear the current collection then return immediately afterwards.
                // ReSharper restore HeuristicUnreachableCode
                foreach( T Item in value ) {
                    _Coll.Add(Item);
                }
            }
        }
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="ObservedValueCollection{T}"/> class.
    /// </summary>
    /// <param name="Enumerable">The enumerable.</param>
    public ObservedValueCollection( IEnumerable<T> Enumerable ) : this(new ObservableCollection<T>(Enumerable)) { }

    /// <summary>
    /// Initialises a new instance of the <see cref="ObservedValueCollection{T}"/> class.
    /// </summary>
    /// <param name="Ls">The list.</param>
    public ObservedValueCollection( List<T> Ls ) : this(new ObservableCollection<T>(Ls)) { }

    /// <inheritdoc />
    public override bool IsDirty {
        get {
            int LA = _Coll.Count, LB = Saved.Length;
            if ( LA != LB ) { return true; } //If the saved and current collections are a different length, then a value has been changed (added/removed)
            for ( int I = 0; I < LA; I++ ) {
                if ( IsDifferent(_Coll[LA], Saved[LA]) ) { //If any value is different between the saved and current collection, then a value has been changed (modified)
                    return true;
                }
            } //If the lengths are the same, and the elements are the same, then no value has likely been changed.
            return false;
        }
    }

    /// <inheritdoc />
    public override void Save() => Saved = _Coll.ToArray();

    /// <inheritdoc />
    public override void Revert() {
        lock ( _Coll ) {
            _Coll.Clear();
            foreach( T Item in Saved ) {
                _Coll.Add( Item );
            }
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() {
        foreach ( T Item in _Coll ) {
            yield return Item;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add( T Item ) => _Coll.Add( Item );

    /// <inheritdoc />
    public void Clear() => _Coll.Clear();

    /// <inheritdoc />
    public bool Contains( T Item ) => _Coll.Contains(Item);

    /// <inheritdoc />
    public void CopyTo( T[] Array, int ArrayIndex ) => _Coll.CopyTo(Array, ArrayIndex);

    /// <inheritdoc />
    public bool Remove( T Item ) => _Coll.Remove(Item);

    /// <inheritdoc />
    public int Count => _Coll.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public override string ToString() => $"{{ ObservedCollection[{typeof(T).GetTypeName()}]({(IsDirty ? "Dirty" : "Clean")}):'{Count} items...' }}";

    /// <summary>
    /// Converts the collection to a valid json string.
    /// </summary>
    public void ConvertToJsonString() => Saved.Serialise();

    /// <summary>
    /// Reads the collection data from a valid json string.
    /// </summary>
    /// <param name="Json">The json data.</param>
    public void ReadFromJsonString(string Json) {
        if ( Json.Deserialise<T[]>().Out(out T[] Result)) {
            Saved = Result;
            Revert();
        }
    }
}