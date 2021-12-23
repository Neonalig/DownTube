#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a simple named value type which supports saving/loading.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class NamedObservedValueCollection<T> : ObservedValueCollection<T>, INamedSave, IJsonSerialisable {

    /// <inheritdoc />
    [JsonProperty("Name", Order = 0)] public string PropertyName { get; }

    /// <summary>
    /// Constructs an instance of the <see cref="NamedObservedValueCollection{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="Collection">The collection.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public NamedObservedValueCollection( ObservableCollection<T> Collection, [CallerMemberName] string? PropertyName = null ) : base(Collection) => this.PropertyName = PropertyName.CatchNull();

    /// <summary>
    /// Constructs an instance of the <see cref="NamedObservedValueCollection{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="Enumerable">The enumerable.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public NamedObservedValueCollection( IEnumerable<T> Enumerable, [CallerMemberName] string? PropertyName = null ) : base(Enumerable) => this.PropertyName = PropertyName.CatchNull();

    /// <summary>
    /// Constructs an instance of the <see cref="NamedObservedValueCollection{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="Ls">The list.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public NamedObservedValueCollection( List<T> Ls, [CallerMemberName] string? PropertyName = null ) : base(Ls) => this.PropertyName = PropertyName.CatchNull();

    /// <summary>
    /// Constructs an instance of the <see cref="NamedObservedValueCollection{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    public NamedObservedValueCollection( [CallerMemberName] string? PropertyName = null ) /*: base()*/ => this.PropertyName = PropertyName.CatchNull();

    /// <summary>
    /// Prevents a default instance of the <see cref="NamedObservedValueCollection{T}"/> class from being created.
    /// </summary>
    /// <remarks>For use only by the <see cref="Newtonsoft.Json"/> (de/)serialiser.</remarks>
    [JsonConstructor] NamedObservedValueCollection() : this(null) { }

    /// <inheritdoc />
    public override string ToString() => $"{{ ObservedCollection[{typeof(T).GetTypeName()} {PropertyName}]({(IsDirty ? "Dirty" : "Clean")}):'{Count} items...' }}";

    /// <summary>
    /// The <see langword="record"/> used for json (de/)serialisation.
    /// </summary>
    internal record NamedCollectionData( string PropertyName, T[] Data );

    /// <summary>
    /// Converts the collection to a valid json string.
    /// </summary>
    public new void ConvertToJsonString() => new NamedCollectionData(PropertyName, Saved).Serialise();

    /// <summary>
    /// Reads the collection data from a valid json string.
    /// </summary>
    /// <param name="Json">The json data.</param>
    public new void ReadFromJsonString( string Json ) {
        if ( Json.Deserialise<NamedCollectionData>().Out(out NamedCollectionData Result) ) {
            if (Result.PropertyName != PropertyName ) {
                throw new NotSupportedException("Collection property names must match to show original intent. You may not deserialise different collections into each other.");
            }
            Saved = Result.Data;
            Revert();
        }
    }
}