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
using System.Runtime.CompilerServices;

using DownTube.Engine;

using ReactiveUI;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a datatype which automatically determines when values become dirty and provides a method to save/revert any changes.
/// </summary>
/// <seealso cref="IJsonSerialisable" />
[SuppressMessage("ReSharper", "ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "LoopCanBePartlyConvertedToQuery")]
public abstract class SaveData<T> : ReactiveObject, ISaveData<T> where T : ISavedProperty {

    /// <summary>
    /// Initialises the <see cref="Properties"/> collection and relevant callbacks.
    /// </summary>
    [MemberNotNull(nameof(Properties))]
    public virtual void Setup() {
        Properties = new ObservableCollection<T>(GetInitialProperties());
        foreach ( T Property in Properties ) {
            Property.PropertyChanging += P => SavedPropertyChanging((ISavedProperty)P);
            Property.PropertyChanged += ( P, O, N ) => SavedPropertyChanged((ISavedProperty)P, O, N);
        }
    }

    /// <summary>
    /// Saves the dirty changes in <see langword="this"/> instance.
    /// </summary>
    public virtual void Save() {
        lock ( Properties ) {
            foreach ( T Property in Properties ) {
                Property.Save();
            }
        }
    }

    /// <summary>
    /// Saves the dirty changes in <see langword="this"/> instance.
    /// </summary>
    public virtual void Revert() {
        lock ( Properties ) {
            foreach ( T Property in Properties ) {
                Property.Revert();
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    public bool IsDirty {
        get {
            lock ( Properties ) {
                foreach ( T Property in Properties ) {
                    if ( Property.IsDirty ) {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Sets the property value, and raises any relevant <see langword="event"/>(s).
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public void SetProperty( object? Value, [CallerMemberName] string? PropertyName = null ) {
        T Property = GetProperty(PropertyName);
        Property.Value = Value;
    }

    /// <summary>
    /// Sets the property value, and raises any relevant <see langword="event"/>(s).
    /// </summary>
    /// <typeparam name="TX">The property value type.</typeparam>
    /// <param name="Value">The value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public void SetProperty<TX>( TX? Value, [CallerMemberName] string? PropertyName = null ) {
        ISavedProperty<TX> Property = GetProperty<TX>(PropertyName);
        Property.Value = Value;
    }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    /// <returns>The found property.</returns>
    /// <exception cref="PropertyNotFoundException">No property could be found with the name '<paramref name="PropertyName"/>'.</exception>
    public T GetProperty( [CallerMemberName] string? PropertyName = null ) {
        PropertyName.ThrowIfNull();
        lock ( Properties ) {
            foreach ( T Property in Properties ) {
                if ( Property.PropertyName == PropertyName ) {
                    return Property;
                }
            }
            throw new PropertyNotFoundException(PropertyName);
        }
    }

    /// <summary>
    /// Gets the property value.
    /// </summary>
    /// <typeparam name="TX">The property value type.</typeparam>
    /// <param name="PropertyName">The name of the property.</param>
    /// <returns>The found property.</returns>
    /// <exception cref="PropertyNotFoundException{T}">No property could be found with the name '<paramref name="PropertyName"/>', or the property value was not of type <typeparamref name="TX"/>.</exception>
    public ISavedProperty<TX> GetProperty<TX>( [CallerMemberName] string? PropertyName = null ) {
        PropertyName.ThrowIfNull();
        lock ( Properties ) {
            foreach ( T Property in Properties ) {
                if ( Property.PropertyName == PropertyName
                    && Property is ISavedProperty<TX> Prop ) {
                    return Prop;
                }
            }
            throw new PropertyNotFoundException<TX>(PropertyName);
        }
    }

    /// <summary>
    /// Gets the collection of monitored properties.
    /// </summary>
    /// <value>
    /// The currently monitored properties.
    /// </value>
    [PropertyChanged.DoNotNotify] public abstract ObservableCollection<T> Properties { get; set; }

    /// <summary>
    /// Gets the initial properties.
    /// </summary>
    /// <returns>The initial properties to monitor.</returns>
    public abstract IEnumerable<T> GetInitialProperties();

    /// <inheritdoc />
    public IEnumerable<T> GetDirty() {
        lock ( Properties ) {
            foreach ( T Property in Properties ) {
                if ( Property.IsDirty ) {
                    yield return Property;
                }
            }
        }
    }

    /// <summary>
    /// Occurs when a property is about to change.
    /// </summary>
    public event SavedPropertyChangingEventArgs SavedPropertyChanging = delegate { };

    /// <summary>
    /// Occurs when a property is changed.
    /// </summary>
    public event SavedPropertyChangedEventArgs SavedPropertyChanged = delegate { };

    /// <summary>
    /// Called when a property is about to change.
    /// </summary>
    /// <param name="Property">The changing property.</param>
    protected virtual void OnSavedPropertyChanging( ISavedProperty Property ) => SavedPropertyChanging.Invoke(Property);

    /// <summary>
    /// Called when property was just changed.
    /// </summary>
    /// <param name="Property">The changed property.</param>
    /// <param name="OldValue">The old property value.</param>
    /// <param name="NewValue">The new property value.</param>
    [PropertyChanged.SuppressPropertyChangedWarnings] protected virtual void OnSavedPropertyChanged( ISavedProperty Property, object? OldValue, object? NewValue ) => SavedPropertyChanged.Invoke(Property, OldValue, NewValue);
}

/// <summary>
/// Raised when a property value is about to change.
/// </summary>
/// <param name="Property">The changing property.</param>
public delegate void SavedPropertyChangingEventArgs( ISavedProperty Property );

/// <summary>
/// Raised after a property value was changed.
/// </summary>
/// <param name="Property">The changed property.</param>
public delegate void SavedPropertyChangedEventArgs( ISavedProperty Property, object? OldValue, object? NewValue );

/// <summary>
/// Exception thrown when a <see cref="ISavedProperty"/> could not be found with the given name.
/// </summary>
public class PropertyNotFoundException : KeyNotFoundException {
    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyNotFoundException"/> class.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    public PropertyNotFoundException([CallerMemberName] string? PropertyName = null) : base($"The property '{PropertyName}' could not be found.") { }
}

/// <summary>
/// Exception thrown when a <see cref="ISavedProperty{T}"/> could not be found with the given name.
/// </summary>
/// <typeparam name="T">The property value type.</typeparam>
public class PropertyNotFoundException<T> : PropertyNotFoundException {
    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyNotFoundException"/> class.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    public PropertyNotFoundException( [CallerMemberName] string? PropertyName = null ) : base($"The property '{PropertyName}' could not be found, or was found but the value was not of type {typeof(T).GetTypeName()}.") { }
}