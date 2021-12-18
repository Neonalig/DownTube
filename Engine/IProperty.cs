using System.ComponentModel;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace DownTube.Engine;

/// <summary>
/// Represents a property which automatically raises <see cref="PropertyChangedEventArgs"/> and <see cref="PropertyChangingEventArgs"/> events where appropriate.
/// </summary>
/// <typeparam name="T">The property value type.</typeparam>
public interface IProperty<T> : IProperty {
    //T Value { get; }

    /// <summary>
    /// Gets the name of the property.
    /// </summary>
    /// <value>
    /// The name of the property.
    /// </value>
    [JsonProperty("Name", Order = 0), JsonRequired] string PropertyName { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>The current value of the property.</returns>
    new T? GetValue();

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="NewValue">The new value.</param>
    void SetValue(T? NewValue);

    /// <summary>
    /// Occurs when the property is changed.
    /// </summary>
    new event PropertyChangedEventArgs<T> PropertyChanged;

    /// <summary>
    /// Invokes the <see cref="PropertyChanged"/> <see langword="event"/> handlers.
    /// </summary>
    /// <param name="OldValue">The old value.</param>
    /// <param name="NewValue">The new value.</param>
    void OnPropertyChanged( T? OldValue, T? NewValue );
}

public interface IProperty {
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>The current value of the property.</returns>
    object? GetValue();

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="NewValue">The new value.</param>
    void SetValue( object? NewValue );

    /// <summary>
    /// Invokes the <see cref="PropertyChanging"/> <see langword="event"/> handlers.
    /// </summary>
    void OnPropertyChanging();

    /// <summary>
    /// Invokes the <see cref="PropertyChanged"/> <see langword="event"/> handlers.
    /// </summary>
    void OnPropertyChanged(object? OldValue, object? NewValue);

    /// <summary>
    /// Occurs when the property is about to be changed.
    /// </summary>
    event PropertyChangingEventArgs PropertyChanging;

    /// <summary>
    /// Occurs when the property is changed.
    /// </summary>
    event PropertyChangedEventArgs PropertyChanged;
}

public delegate void PropertyChangingEventArgs( string PropertyName );
public delegate void PropertyChangedEventArgs( string PropertyName, object? OldValue, object? NewValue );
public delegate void PropertyChangedEventArgs<in T>( string PropertyName, T? OldValue, T? NewValue );

/// <summary>
/// Simple property which automates the raising of <see cref="PropertyChanged"/> and <see cref="PropertyChanging"/> events where appropriate.
/// </summary>
/// <typeparam name="T">The property value type.</typeparam>
public class Property<T> : IProperty<T> {
    /// <summary>
    /// The property value.
    /// </summary>
    T? _Value;


    /// <summary>
    /// Initialises a new instance of the <see cref="Property{T}"/> class.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="PropertyChanging">The <see langword="event"/> to raise when the property's value is about to be changed.</param>
    /// <param name="PropertyChanged">The <see langword="event"/> to raise when the property's value was changed.</param>
    /// <param name="PropertyName">The name of the property.</param>
    /// <exception cref="ArgumentNullException"><see cref="PropertyName"/> was <see langword="null"/>.</exception>
    protected Property( T? Value, PropertyChangingEventArgs PropertyChanging, PropertyChangedEventArgs<T> PropertyChanged, string PropertyName ) {
        _Value = Value;
        this.PropertyName = PropertyName ?? throw new ArgumentNullException(nameof(PropertyName));
        this.PropertyChanging = PropertyChanging;
        this.PropertyChanged = PropertyChanged;
    }

    /// <summary>
    /// Gets a new property instance.
    /// </summary>
    /// <param name="Sender">The sender.</param>
    /// <param name="Value">The value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    /// <returns>A new property instance.</returns>
    public static Property<T> GetProperty(IPropertyChangeNotifier Sender, T? Value, [CallerMemberName] string? PropertyName = null ) =>
        new Property<T>(
            Value: Value,
            PropertyChanging: P => {
                if ( Sender.SupportsOnPropertyChanging ) {
                    Sender.OnPropertyChanging(P);
                }
            },
            PropertyChanged: (P, O, N) => {
                if ( Sender.SupportsOnPropertyChanged ) {
                    Sender.OnPropertyChanged(P, O, N);
                }
            },
            PropertyName: PropertyName.CatchNull()
            );

    /// <inheritdoc />
    public string PropertyName { get; }

    /// <inheritdoc />
    public T? GetValue() => _Value;

    /// <inheritdoc />
    public void SetValue( T? NewValue ) {
        if ( _Value is null ? NewValue is not null : NewValue is null || !_Value.Equals(NewValue) ) {
            PropertyChanging.Invoke(PropertyName);
            T? OldValue = _Value;
            _Value = NewValue;
            PropertyChanged.Invoke(PropertyName, OldValue, NewValue);
        }
    }

    /// <inheritdoc />
    void IProperty.OnPropertyChanged( object? OldValue, object? NewValue ) => PropertyChanged(PropertyName, (T?)OldValue, (T?)NewValue);

    /// <inheritdoc />
    public event PropertyChangingEventArgs PropertyChanging;

    /// <inheritdoc />
    public event PropertyChangedEventArgs<T> PropertyChanged;

    /// <inheritdoc />
    public void OnPropertyChanged( T? OldValue, T? NewValue ) => PropertyChanged.Invoke(PropertyName, OldValue, NewValue);

    /// <inheritdoc />
    object? IProperty.GetValue() => GetValue();

    /// <inheritdoc />
    void IProperty.SetValue( object? NewValue ) => SetValue((T?)NewValue);

    /// <inheritdoc />
    public void OnPropertyChanging() => PropertyChanging.Invoke(PropertyName);

    /// <summary>
    /// The collection of anonymous delegates formed for the <see cref="IProperty.PropertyChanged"/> callback.
    /// </summary>
    readonly Dictionary<string, PropertyChangedEventArgs<T>> _PropChangedAnonDelegates = new Dictionary<string, PropertyChangedEventArgs<T>>();

    /// <inheritdoc />
    event PropertyChangedEventArgs IProperty.PropertyChanged {
        add {
            void Dlg( string Prop, T? Old, T? New ) => value.Invoke(Prop, Old, New);
            _PropChangedAnonDelegates.Add(value.Method.Name, Dlg);
            PropertyChanged += Dlg;
        }
        remove => PropertyChanged -= _PropChangedAnonDelegates[value.Method.Name];
    }
}

/// <summary>
/// Represents an <see langword="object"/> which supports 'PropertyChanging' and/or 'PropertyChanged' <see langword="event"/>s.
/// </summary>
public interface IPropertyChangeNotifier {
    /// <summary>
    /// Gets a value indicating whether the notifier supports raising a 'PropertyChanging' <see langword="event"/>.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="OnPropertyChanging(string)"/> can be invoked; otherwise, <see langword="false" />.
    /// </value>
    bool SupportsOnPropertyChanging { get; }

    /// <summary>
    /// Raises the relevant 'PropertyChanging' <see langword="event"/>.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    void OnPropertyChanging( string PropertyName );

    /// <summary>
    /// Gets a value indicating whether the notifier supports raising a 'PropertyChanged' <see langword="event"/>.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if <see cref="OnPropertyChanged(string, object?, object?)"/> can be invoked; otherwise, <see langword="false" />.
    /// </value>
    bool SupportsOnPropertyChanged { get; }

    /// <summary>
    /// Raises the relevant 'PropertyChanged' <see langword="event"/>.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    /// <param name="OldValue">The value before any changes.</param>
    /// <param name="NewValue">The new value after any changes.</param>
    void OnPropertyChanged( string PropertyName, object? OldValue, object? NewValue );

    /// <summary>
    /// Sets the property and raises any required notifications.
    /// </summary>
    /// <remarks>Equality checks will not commence.</remarks>
    /// <typeparam name="T">The property value type.</typeparam>
    /// <param name="Notifier">The notifier.</param>
    /// <param name="Value">The value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static void SetAndRaise<T>( IPropertyChangeNotifier Notifier, ref T? Value, T? NewValue, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.ThrowIfNull();
        T? OldValue = Value;
        if ( Notifier.SupportsOnPropertyChanging ) {
            Notifier.OnPropertyChanging(PropertyName);
        }
        Value = NewValue;
        if ( Notifier.SupportsOnPropertyChanged ) {
            Notifier.OnPropertyChanged(PropertyName, OldValue, NewValue);
        }
    }

    /// <summary>
    /// Sets the property and raises any required notifications.
    /// </summary>
    /// <remarks>Equality checks will not commence.</remarks>
    /// <param name="Notifier">The notifier.</param>
    /// <param name="Value">The value.</param>
    /// <param name="NewValue">The new value.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static void SetAndRaise( IPropertyChangeNotifier Notifier, ref object? Value, object? NewValue, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.ThrowIfNull();
        object? OldValue = Value;
        if ( Notifier.SupportsOnPropertyChanging ) {
            Notifier.OnPropertyChanging(PropertyName);
        }
        Value = NewValue;
        if ( Notifier.SupportsOnPropertyChanged ) {
            Notifier.OnPropertyChanged(PropertyName, OldValue, NewValue);
        }
    }
}

public sealed class PropertyChanging_PropertyChangeNotifier<T> : IPropertyChangeNotifier where T : INotifyPropertyChanging {

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyChanging_PropertyChangeNotifier{T}"/> class.
    /// </summary>
    /// <param name="Base">The base.</param>
    public PropertyChanging_PropertyChangeNotifier( T Base ) => this._Base = Base;

    readonly T _Base;

    /// <inheritdoc />
    public bool SupportsOnPropertyChanging => true;

    /// <inheritdoc />
    public void OnPropertyChanging( string PropertyName ) => _Base.Raise(nameof(INotifyPropertyChanging.PropertyChanging), new System.ComponentModel.PropertyChangingEventArgs(PropertyName));

    /// <inheritdoc />
    public bool SupportsOnPropertyChanged => false;

    /// <inheritdoc />
    public void OnPropertyChanged( string PropertyName, object? OldValue, object? NewValue ) => throw new NotSupportedException();

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <see cref="T"/> to <see cref="PropertyChanging_PropertyChangeNotifier{T}"/>.
    /// </summary>
    /// <param name="Base">The base.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator PropertyChanging_PropertyChangeNotifier<T>( T Base ) => new PropertyChanging_PropertyChangeNotifier<T>(Base);
}

public sealed class PropertyChanged_PropertyChangeNotifier<T> : IPropertyChangeNotifier where T : INotifyPropertyChanged {

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyChanged_PropertyChangeNotifier{T}"/> class.
    /// </summary>
    /// <param name="Base">The base.</param>
    public PropertyChanged_PropertyChangeNotifier( T Base ) => this._Base = Base;

    readonly T _Base;

    /// <inheritdoc />
    public bool SupportsOnPropertyChanging => false;

    /// <inheritdoc />
    public void OnPropertyChanging( string PropertyName ) => throw new NotSupportedException();

    /// <inheritdoc />
    public bool SupportsOnPropertyChanged => true;

    /// <inheritdoc />
    [NotifyPropertyChangedInvocator]
    public void OnPropertyChanged( string PropertyName, object? OldValue, object? NewValue ) => _Base.Raise(nameof(INotifyPropertyChanged.PropertyChanged), new System.ComponentModel.PropertyChangedEventArgs(PropertyName));

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <see cref="T"/> to <see cref="PropertyChanged_PropertyChangeNotifier{T}"/>.
    /// </summary>
    /// <param name="Base">The base.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator PropertyChanged_PropertyChangeNotifier<T>( T Base ) => new PropertyChanged_PropertyChangeNotifier<T>(Base);
}

public sealed class PropertyChangingChanged_PropertyChangeNotifier<T> : IPropertyChangeNotifier where T : INotifyPropertyChanging, INotifyPropertyChanged {

    /// <summary>
    /// Initialises a new instance of the <see cref="PropertyChangingChanged_PropertyChangeNotifier{T}"/> class.
    /// </summary>
    /// <param name="Base">The base.</param>
    public PropertyChangingChanged_PropertyChangeNotifier( T Base ) => this._Base = Base;

    readonly T _Base;

    /// <inheritdoc />
    public bool SupportsOnPropertyChanging => true;

    /// <inheritdoc />
    public void OnPropertyChanging( string PropertyName ) => _Base.Raise(nameof(INotifyPropertyChanged.PropertyChanged), new System.ComponentModel.PropertyChangingEventArgs(PropertyName));

    /// <inheritdoc />
    public bool SupportsOnPropertyChanged => true;

    /// <inheritdoc />
    [NotifyPropertyChangedInvocator]
    public void OnPropertyChanged( string PropertyName, object? OldValue, object? NewValue ) => _Base.Raise(nameof(INotifyPropertyChanged.PropertyChanged), new System.ComponentModel.PropertyChangedEventArgs(PropertyName));

    /// <summary>
    /// Performs an <see langword="implicit"/> conversion from <see cref="T"/> to <see cref="PropertyChanged_PropertyChangeNotifier{T}"/>.
    /// </summary>
    /// <param name="Base">The base.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator PropertyChangingChanged_PropertyChangeNotifier<T>( T Base ) => new PropertyChangingChanged_PropertyChangeNotifier<T>(Base);
}

public sealed class SavedProperty<T> : Property<T>, ISavedProperty<T> {
    /// <inheritdoc />
    public SavedProperty( T? Value, PropertyChangingEventArgs PropertyChanging, PropertyChangedEventArgs<T> PropertyChanged, string PropertyName ) : base(Value, PropertyChanging, PropertyChanged, PropertyName) => Saved = Value;

    /// <summary>
    /// Initialises a new instance of the <see cref="SavedProperty{T}"/> class.
    /// </summary>
    /// <remarks>Constructor is intended only for use by <see cref="Newtonsoft.Json"/>.</remarks>
    /// <param name="Name">The property name.</param>
    /// <param name="Value">The value.</param>
    [JsonConstructor]
    public SavedProperty( string? Name, T? Value ) : this(Value, delegate { }, delegate { }, Name ?? string.Empty) { }

    /// <inheritdoc />
    [JsonIgnore] public bool IsDirty => Value is null ? Saved is not null : Saved is null || !Value.Equals(Saved);

    /// <inheritdoc />
    [JsonIgnore] object? ISavedProperty.Saved {
        get => Saved;
        set => Saved = (T?)value;
    }

    /// <inheritdoc />
    [JsonIgnore] object? ISavedProperty.Value {
        get => Value;
        set => Value = (T?)value;
    }

    /// <inheritdoc />
    public void Save() => Saved = Value;

    /// <inheritdoc />
    public void Revert() => Value = Saved;

    /// <inheritdoc />
    [JsonIgnore] public T? Saved { get; set; }

    /// <inheritdoc />
    [JsonProperty(Order = 1), JsonRequired] public T? Value {
        get => GetValue();
        set => SetValue(value);
    }
}

public interface ISavedProperty {
    /// <summary>
    /// Gets a value indicating whether the property is dirty.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the property is dirty; otherwise, <see langword="false" />.
    /// </value>
    bool IsDirty { get; }

    /// <summary>
    /// Gets or sets the saved value.
    /// </summary>
    /// <value>
    /// The saved value.
    /// </value>
    object? Saved { get; set; }

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    /// <value>
    /// The current value.
    /// </value>
    object? Value { get; set; }

    /// <summary>
    /// Saves the new value.
    /// </summary>
    void Save();

    /// <summary>
    /// Reverts back to the old value.
    /// </summary>
    void Revert();
}

public interface ISavedProperty<T> : ISavedProperty {
    /// <inheritdoc cref="ISavedProperty.Saved"/>
    new T? Saved { get; set; }

    /// <inheritdoc cref="ISavedProperty.Value"/>
    new T? Value { get; set; }
}