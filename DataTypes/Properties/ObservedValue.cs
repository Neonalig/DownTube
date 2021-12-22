#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using DownTube.DataTypes.Properties;

using Newtonsoft.Json;

#endregion

namespace DownTube.DataTypes;

/// <summary>
/// Represents a simple value type which supports saving/loading.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class ObservedValue<T> : SimpleSave<T> {

    /// <summary>
    /// Constructs an instance of the <see cref="ObservedValue{T}"/> <see langword="class"/> with an initial value of <c><see langword="default"/>(<typeparamref name="T"/>)</c>.
    /// </summary>
    [JsonConstructor] public ObservedValue() : this(default) { }

    /// <summary>
    /// Constructs an instance of the <see cref="ObservedValue{T}"/> <see langword="class"/>.
    /// </summary>
    /// <param name="Value">The initial value.</param>
    public ObservedValue( T? Value = default ) {
        this.Value = Value;
        Saved = Value;
    }

    [JsonProperty("Value", Order = 1)] T? _Value;

    /// <summary>
    /// The current value.
    /// </summary>
    [JsonIgnore] public T? Value {
        get => _Value;
        set {
            bool WasDirty = IsDirty;
            _Value = value;
            if (WasDirty) {
                if ( !IsDirty ) { //Was and now isn't...  ∴ Just became clean
                    OnBecameClean();
                }
            } else if (IsDirty) { //Wasn't and now is...  ∴ Just became dirty
                OnBecameDirty();
            }
        }
    }

    /// <summary>
    /// The last saved value.
    /// </summary>
    public T? Saved { get; private set; }

    /// <inheritdoc />
    public override bool IsDirty => IsDifferent(Value, Saved);

    /// <inheritdoc />
    public override void Save() => Saved = Value;

    /// <inheritdoc />
    public override void Revert() => Value = Saved;

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion between <paramref name="Observed"/> and the type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="Observed">The <see cref="ObservedValue{T}"/> to retrieve the value from.</param>
    public static implicit operator T?(ObservedValue<T> Observed) => Observed.Value;

    /// <summary>
    /// Performs an <see langword="explicit"/> conversion between <paramref name="Value"/> and a new <see cref="ObservedValue{T}"/> instance.
    /// </summary>
    /// <param name="Value">The initial value to set the <see cref="ObservedValue{T}"/> to.</param>
    public static explicit operator ObservedValue<T>( T? Value ) => new ObservedValue<T>(Value);

    /// <inheritdoc />
    public override string ToString() => IsDirty switch {
        true => $"{{ Observed[{typeof(T).GetTypeName()}](Dirty):'{Value}'//'{Saved}' }}",
        _    => $"{{ Observed[{typeof(T).GetTypeName()}](Clean):'{Value}' }}"
    };
}