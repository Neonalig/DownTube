using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using ReactiveUI;

namespace DownTube.DataTypes;

/// <summary>
/// Represents a datatype which automatically determines when values become dirty and provides a method to save/revert any changes.
/// </summary>
/// <seealso cref="IJsonSerialisable" />
internal class SaveData : ReactiveObject, ISaveData {
    /// <summary>
    /// The collection of currently changed property names.
    /// </summary>
    internal readonly HashSet<string> ChangedProperties = new HashSet<string>();

    /// <summary>
    /// The collection of properties as of the last save.
    /// </summary>
    internal readonly Dictionary<string, object?> Properties = new Dictionary<string, object?>();

    /// <summary>
    /// Sets the property if the new value is different to the current, saving an original copy if <c>this</c> is the first modification since the last save.
    /// </summary>
    /// <typeparam name="T">The value data type.</typeparam>
    /// <param name="Value">The value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="ValueName">The name of the value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="NewValue"/> was <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="Dictionary{TKey, TValue}" />.</exception>
    internal void SetProperty<T>( [NotNullIfNotNull("NewValue")] ref T? Value, T? NewValue, [CallerMemberName] string? ValueName = null ) {
        ValueName.CatchNull();
        if ( !Properties.ContainsKey(ValueName) ) {
            Properties.Add(ValueName, Value);
        }

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
    /// <exception cref="ArgumentNullException"><paramref name="PropertyName"/> was null.</exception>
    /// <exception cref="ArgumentException"><see cref="StringComparison.InvariantCultureIgnoreCase"/> is not a <see cref="StringComparison" /> value.</exception>
    public bool IsPropertyDirty( [CallerMemberName] string? PropertyName = null ) => ChangedProperties.Contains(PropertyName.CatchNull(), StringComparison.InvariantCultureIgnoreCase);
}