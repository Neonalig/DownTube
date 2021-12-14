using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using ReactiveUI;

namespace DownTube.DataTypes;

/// <summary>
/// Represents a datatype which automatically determines when values become dirty and provides a method to save/revert any changes.
/// </summary>
/// <seealso cref="IJsonSerialisable" />
public abstract class SaveData : ReactiveObject, ISaveData {
    /// <summary>
    /// The collection of currently changed property names.
    /// </summary>
    internal readonly HashSet<string> ChangedProperties = new HashSet<string>();

    /// <summary>
    /// The collection of properties as of the last save.
    /// </summary>
    internal readonly Dictionary<string, object?> CachedProperties = new Dictionary<string, object?>();

    /// <summary>
    /// Sets the property if the new value is different to the current, saving an original copy if <c>this</c> is the first modification since the last save.
    /// </summary>
    /// <typeparam name="T">The value data type.</typeparam>
    /// <param name="Value">The value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="ValueName">The name of the value.</param>
    internal void SetProperty<T>( [NotNullIfNotNull("NewValue")] ref T? Value, T? NewValue, [CallerMemberName] string? ValueName = null ) {
        ValueName.CatchNull();
        // ReSharper disable ExceptionNotDocumentedOptional
        lock ( CachedProperties ) {
            if ( !CachedProperties.ContainsKey(ValueName) ) {
                CachedProperties.Add(ValueName, Value);
            }
        }
        // ReSharper restore ExceptionNotDocumentedOptional

        if ( Value is null ? NewValue is not null : NewValue is null || !Value.Equals(NewValue) ) {
            this.RaisePropertyChanging(ValueName);
            Value = NewValue;
            ChangedProperties.Add(ValueName);
            this.RaisePropertyChanged(ValueName);
        }
    }

    /// <inheritdoc />
    public bool IsDirty => ChangedProperties.Count > 0;

    /// <inheritdoc />
    public IEnumerable<string> GetDirty() => ChangedProperties;

    /// <summary>
    /// Determines whether the specified property is dirty.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    /// <returns>
    /// <see langword="true" /> if the property is dirty; otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="ArgumentException"><see cref="StringComparison.InvariantCultureIgnoreCase"/> is not a <see cref="StringComparison" /> value.</exception>
    public bool IsPropertyDirty( [CallerMemberName] string? PropertyName = null ) => ChangedProperties.Contains(PropertyName.CatchNull(), StringComparison.InvariantCultureIgnoreCase);

    //public abstract IEnumerable<(string PropertyName, object? Value)> GetCurrentProperties();

    /// <summary>
    /// Gets the value of the property with the given name.
    /// </summary>
    /// <param name="PropertyName">Name of the property to search for.</param>
    /// <returns>The property's current value.</returns>
    internal abstract object? GetProp( string PropertyName );

    /// <summary>
    /// Sets the value of the property with the given name.
    /// </summary>
    /// <param name="PropertyName">Name of the property to search for.</param>
    /// <param name="Value">The new value to apply to the found property.</param>
    internal abstract void SetProp( string PropertyName, object? Value );

    /// <summary>
    /// Saves the dirty changes in <see langword="this"/> instance.
    /// </summary>
    public virtual void Save() {
        lock ( CachedProperties ) {
            lock ( ChangedProperties ) {
                foreach ( string PropertyName in ChangedProperties ) { //Update the old cache
                    // ReSharper disable ExceptionNotDocumentedOptional
                    object? Value = GetProp(PropertyName);

                    if ( CachedProperties.ContainsKey(PropertyName) ) {
                        CachedProperties[PropertyName] = Value;
                    } else {
                        CachedProperties.Add(PropertyName, Value);
                    }
                    // ReSharper restore ExceptionNotDocumentedOptional
                }
                ChangedProperties.Clear(); //Clear any indication that there are unsaved changes.
            }
        }
    }

    /// <summary>
    /// Saves the dirty changes in <see langword="this"/> instance.
    /// </summary>
    public virtual void Revert() {
        lock ( CachedProperties ) {
            lock ( ChangedProperties ) {
                foreach ( string PropertyName in ChangedProperties ) { //Apply the old cache
                    // ReSharper disable ExceptionNotDocumentedOptional
                    object? Value = CachedProperties[PropertyName];
                    SetProp(PropertyName, Value);
                    // ReSharper restore ExceptionNotDocumentedOptional
                }
                ChangedProperties.Clear(); //Clear any indication that there are unsaved changes.
            }
        }
    }
}