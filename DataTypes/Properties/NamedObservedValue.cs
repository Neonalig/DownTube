#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Runtime.CompilerServices;

using Newtonsoft.Json;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a simple named value type which supports saving/loading.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class NamedObservedValue<T> : ObservedValue<T>, INamedSave {
    /// <inheritdoc />
    [JsonProperty("Name", Order = 0)] public string PropertyName { get; }

    /// <summary>
    /// Constructs an instance of the <see cref="NamedObservedValue{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="Value">The initial value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public NamedObservedValue( T? Value = default, [CallerMemberName] string? PropertyName = null ) : base(Value) => this.PropertyName = PropertyName.CatchNull();

    /// <summary>
    /// Constructs an instance of the <see cref="NamedObservedValue{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    public NamedObservedValue( [CallerMemberName] string? PropertyName = null ) : this(default, PropertyName) { }

    /// <summary>
    /// Prevents a default instance of the <see cref="NamedObservedValue{T}"/> class from being created.
    /// </summary>
    /// <remarks>For use only by the <see cref="Newtonsoft.Json"/> (de/)serialiser.</remarks>
    [JsonConstructor] NamedObservedValue() : this(null) { }

    /// <inheritdoc />
    public override string ToString() => IsDirty switch {
        true => $"{{ Observed[{typeof(T).GetTypeName()} {PropertyName}](Dirty):'{Value}'//'{Saved}' }}",
        _    => $"{{ Observed[{typeof(T).GetTypeName()} {PropertyName}](Clean):'{Value}' }}"
    };
}