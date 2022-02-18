#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using PropertyChanged;

#endregion

namespace DownTube.DataTypes.Collections;

/// <summary>
/// <inheritdoc cref="ObservableCollection{T}"/> Supports light sorting.
/// </summary>
/// <typeparam name="T"><inheritdoc cref="ObservableCollection{T}"/></typeparam>
/// <seealso cref="ObservableCollection{T}"/>
public class SortedObservableList<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : IComparable<T> {

    /// <summary>
    /// The internal collection.
    /// </summary>
    readonly ObservableCollection<T> _Coll = new ObservableCollection<T>();

    /// <inheritdoc cref="ObservableCollection{T}(IEnumerable{T})"/>
    /// <param name="Collection">The collection from which the elements are copied.</param>
    public SortedObservableList( IEnumerable<T> Collection ) : this() {
        foreach ( T Item in Collection ) {
            Add(Item);
        }
    }

    /// <inheritdoc cref="ObservableCollection{T}()"/>
    public SortedObservableList() {
        _Coll.CollectionChanged += (_, E) => OnCollectionChanged(E);
        _ = _Coll.AddHandler(nameof(PropertyChanged), ( ObservableCollection<T> _, PropertyChangedEventArgs E ) => OnPropertyChanged(E));
    }

    /// <inheritdoc />
    public void Add( T Item ) {
        //Debug.WriteLine($"Adding {Item}");
        int I = 0;
        foreach ( T Itm in this ) {
            if ( Item.CompareTo(Itm) <= 0 ) {
                //Debug.WriteLine($"\tInserted @ {I}");
                _Coll.Insert(I, Item);
                return;
            }
            I++;
        }
        //Debug.WriteLine($"\tAdded @ {Count}");
        _Coll.Add(Item);
    }

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
    public int IndexOf( T Item ) => _Coll.IndexOf(Item);

    /// <inheritdoc />
    /// <remarks>Equivalent to <see cref="Add(T)"/> due to sorting behaviour of the collection.</remarks>
    public void Insert( int _, T Item ) => Add(Item);

    /// <inheritdoc />
    public void RemoveAt( int Index ) => _Coll.RemoveAt(Index);

    /// <inheritdoc cref="ObservableCollection{T}.this[int]"/>
    /// <summary>
    /// Gets the <see cref="T"/> at the specified index.
    /// </summary>
    /// <remarks>The setter is not recommended, as it ignores sorting of the collection.</remarks>
    /// <param name="Index">The index.</param>
    /// <returns>The found <see cref="T"/> at the given index.</returns>
    [SuppressPropertyChangedWarnings]
    public T this[int Index] {
        get => _Coll[Index];
        set => _Coll[Index] = value;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator() => _Coll.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// Raised when a property is changed.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string? PropertyName = null ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    /// <summary>
    /// Raised when the collection is changed.
    /// </summary>
    /// <param name="E">The raised event arguments.</param>
    protected virtual void OnPropertyChanged( PropertyChangedEventArgs E ) => PropertyChanged?.Invoke(this, E);

    /// <summary>
    /// Raised when the collection is changed.
    /// </summary>
    /// <param name="E">The raised event arguments.</param>
    [SuppressPropertyChangedWarnings]
    protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs E ) => CollectionChanged?.Invoke(this, E);
}