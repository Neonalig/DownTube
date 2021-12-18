#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

using DownTube.DataTypes;

using ReactiveUI;

#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="DependencyObject"/> and <see cref="DependencyProperty"/>.
/// </summary>
[SuppressMessage("ReSharper", "ExceptionNotDocumented")]
[SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
public static class DependencyExtensions {

    /// <inheritdoc cref="DependencyObject.GetValue(DependencyProperty)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="Prop">The dependency property to get the value from.</param>
    public static T GetVal<T>( this DependencyObject DepObj, DependencyProperty Prop ) => (T)DepObj.GetValue(Prop);

    /// <inheritdoc cref="DependencyObject.GetValue(DependencyProperty)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="PropKey">The dependency property to get the value from.</param>
    public static T GetVal<T>( this DependencyObject DepObj, DependencyPropertyKey PropKey ) => (T)DepObj.GetValue(PropKey.DependencyProperty);

    /// <inheritdoc cref="DependencyObject.SetValue(DependencyProperty, object)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="Prop">The dependency property to set the value on.</param>
    /// <param name="Value">The value to set the property to.</param>
    public static void SetVal<T>( this DependencyObject DepObj, DependencyProperty Prop, T Value ) => DepObj.SetValue(Prop, Value);

    /// <inheritdoc cref="DependencyObject.SetValue(DependencyProperty, object)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="PropKey">The dependency property to set the value on.</param>
    /// <param name="Value">The value to set the property to.</param>
    public static void SetVal<T>( this DependencyObject DepObj, DependencyPropertyKey PropKey, T Value ) => DepObj.SetValue(PropKey, Value);

    /// <summary>
    /// Invokes the <see cref="INotifyAndRaisePropertyChanged.OnPropertyChanged"/> <see langword="event"/> callbacks.
    /// </summary>
    /// <param name="NotifyAndRaise">The object to raise the <see langword="event"/> on.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static Result InvokeOnPropertyChanged( this INotifyAndRaisePropertyChanged NotifyAndRaise, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.CatchNull();
        NotifyAndRaise.OnPropertyChanged(PropertyName);
        return Result.Success;
    }

    /// <summary>
    /// Invokes the <see cref="INotifyPropertyChanged.PropertyChanged"/> <see langword="event"/> callbacks.
    /// </summary>
    /// <param name="Notify">The object to raise the <see langword="event"/> on.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static Result InvokeOnPropertyChanged( this INotifyPropertyChanged Notify, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.CatchNull();
        return Notify.Raise(nameof(INotifyPropertyChanged.PropertyChanged), new PropertyChangedEventArgs(PropertyName));
    }

    /// <summary>
    /// Invokes the <see cref="DependencyObject"/><c>.NotifyPropertyChange</c> method.
    /// </summary>
    /// <param name="DepObj">The dependency object to raise the <see langword="event"/> on.</param>
    /// <param name="E">The property changed event arguments.</param>
    public static Result InvokeOnPropertyChanged( this DependencyObject DepObj, DependencyPropertyChangedEventArgs E ) => DepObj.Raise("NotifyPropertyChange", E);

    /// <summary>
    /// Invokes the <see cref="ReactiveObject"/><c>.RaisePropertyChanged</c> method.
    /// </summary>
    /// <param name="ReacObj">The reactive object to raise the <see langword="event"/> on.</param>
    /// <param name="PropertyName">The name of the changed property.</param>
    public static void InvokeOnPropertyChanged( this ReactiveObject ReacObj, [CallerMemberName] string? PropertyName = null ) => ReacObj.RaisePropertyChanged(PropertyName);

    /// <summary>
    /// Sets the value to the new value if changed, and invokes the <see cref="INotifyAndRaisePropertyChanged.PropertyChanged"/> method.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="NotifyAndRaise">The object to raise the <see langword="event"/> on.</param>
    /// <param name="Value">The value to change.</param>
    /// <param name="NewValue">The new value to set.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static void SetAndRaise<T>( this INotifyAndRaisePropertyChanged NotifyAndRaise, [NotNullIfNotNull("NewValue")] ref T? Value, T? NewValue, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.CatchNull();
        if ( Value is null ? NewValue is not null : NewValue is null | !Value.Equals(NewValue) ) {
            Value = NewValue;
            NotifyAndRaise.OnPropertyChanged(PropertyName);
        }
    }

    /// <summary>
    /// Sets the value to the new value if changed, and invokes the <see cref="INotifyPropertyChanged.PropertyChanged"/> method.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="Notify">The object to raise the <see langword="event"/> on.</param>
    /// <param name="Value">The value to change.</param>
    /// <param name="NewValue">The new value to set.</param>
    /// <param name="PropertyName">The name of the property.</param>
    public static Result SetAndRaise<T>( this INotifyPropertyChanged Notify, [NotNullIfNotNull("NewValue")] ref T? Value, T? NewValue, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.CatchNull();
        if ( Value is null ? NewValue is not null : NewValue is null | !Value.Equals(NewValue) ) {
            Value = NewValue;
            return Notify.Raise(nameof(INotifyPropertyChanged.PropertyChanged), new PropertyChangedEventArgs(PropertyName));
        }
        return Result.Success; //No value change is still a success
    }

    /// <summary>
    /// Sets the value to the new value if changed, and invokes the <see cref="ReactiveObject.PropertyChanged"/> and <see cref="ReactiveObject.PropertyChanging"/> methods.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="ReacObj">The reactive object to raise the <see langword="event"/> on.</param>
    /// <param name="Value">The value to change.</param>
    /// <param name="NewValue">The new value to set.</param>
    /// <param name="PropertyName">The name of the changed property.</param>
    public static void SetAndRaise<T>( this ReactiveObject ReacObj, ref T Value, T NewValue, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.CatchNull();
        if ( Value is null ? NewValue is not null : NewValue is null | !Value.Equals(NewValue) ) {
            ReacObj.RaisePropertyChanging(PropertyName);
            Value = NewValue;
            ReacObj.RaisePropertyChanged(PropertyName);
        }
    }

    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    /// <param name="Source">The source.</param>
    /// <param name="EventName">Name of the event.</param>
    /// <param name="EventArgs">The <see cref="TEventArgs"/> instance containing the event data.</param>
    /// <returns>The invocation result.</returns>
    [SuppressMessage("ReSharper", "CatchAllClause")]
    public static Result Raise<TEventArgs>( this object Source, string EventName, TEventArgs EventArgs ) where TEventArgs : notnull {
        try {
            if ( Source.GetType().GetField(EventName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(Source) is MulticastDelegate MD ) {
                try {
                    Delegate[] Handlers = MD.GetInvocationList();

                    object[] Args = { Source, EventArgs };

                    foreach ( Delegate Handler in Handlers ) {
                        Handler.Method.Invoke(Handler.Target, Args);
                    }

                    return Result.Success;
                } catch ( MemberAccessException MembAccEx) {
                    return MembAccEx;
                } catch ( Exception Ex ) {
                    return Ex;
                }
            }
            return KnownError.GetNullArgError($"{Source}.{EventName}").GetResult();
        } catch ( TargetException TargEx ) {
            return TargEx;
        } catch (NotSupportedException NoSuppEx ) {
            return NoSuppEx;
        } catch (FieldAccessException FldAccEx ) {
            return FldAccEx;
        } catch (ArgumentException ArgEx ) {
            return ArgEx;
        }
    }

    /// <inheritdoc cref="Raise{TEventArgs}(object, string, TEventArgs)"/>
    /// <param name="Sender">The sender.</param>
    /// <param name="EventName">Name of the event.</param>
    /// <param name="EventArgs">The <see cref="TEventArgs"/> instance containing the event data.</param>
    public static Result Raise<TSender, TEventArgs>( this TSender Sender, string EventName, TEventArgs EventArgs ) where TSender : notnull where TEventArgs : notnull => Raise(Source: Sender, EventName, EventArgs);
}