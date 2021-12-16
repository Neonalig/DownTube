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

#endregion

namespace DownTube.Extensions;

/// <summary>
/// Extension methods and shorthand for <see cref="DependencyObject"/> and <see cref="DependencyProperty"/>.
/// </summary>
public static class DependencyExtensions {

    /// <inheritdoc cref="DependencyObject.GetValue(DependencyProperty)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="Prop">The dependency property to get the value from.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static T GetVal<T>( this DependencyObject DepObj, DependencyProperty Prop ) => (T)DepObj.GetValue(Prop);

    /// <inheritdoc cref="DependencyObject.GetValue(DependencyProperty)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="PropKey">The dependency property to get the value from.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static T GetVal<T>( this DependencyObject DepObj, DependencyPropertyKey PropKey ) => (T)DepObj.GetValue(PropKey.DependencyProperty);

    /// <inheritdoc cref="DependencyObject.SetValue(DependencyProperty, object)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="Prop">The dependency property to set the value on.</param>
    /// <param name="Value">The value to set the property to.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static void SetVal<T>( this DependencyObject DepObj, DependencyProperty Prop, T Value ) => DepObj.SetValue(Prop, Value);

    /// <inheritdoc cref="DependencyObject.SetValue(DependencyProperty, object)"/>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="DepObj">The dependency object to get the property from.</param>
    /// <param name="PropKey">The dependency property to set the value on.</param>
    /// <param name="Value">The value to set the property to.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public static void SetVal<T>( this DependencyObject DepObj, DependencyPropertyKey PropKey, T Value ) => DepObj.SetValue(PropKey, Value);

    /// <summary>
    /// Invokes the <see cref="INotifyAndRaisePropertyChanged.OnPropertyChanged"/> <see langword="event"/> callbacks.
    /// </summary>
    /// <param name="NotifyAndRaise">The object to raise the <see langword="event"/> on.</param>
    /// <param name="PropertyName">The name of the property.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
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
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public static Result InvokeOnPropertyChanged( this INotifyPropertyChanged Notify, [CallerMemberName] string? PropertyName = null ) {
        PropertyName.CatchNull();
        return Notify.Raise(nameof(INotifyPropertyChanged.PropertyChanged), new PropertyChangedEventArgs(PropertyName));
    }

    /// <summary>
    /// Invokes the <see cref="DependencyObject"/><c>.NotifyPropertyChange</c> method.
    /// </summary>
    /// <param name="DepObj">The dependency object to raise the <see langword="event"/> on.</param>
    /// <param name="E">The property changed event arguments.</param>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public static Result InvokeOnPropertyChanged( this DependencyObject DepObj, DependencyPropertyChangedEventArgs E ) => DepObj.Raise("NotifyPropertyChange", E);

    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    /// <param name="Source">The source.</param>
    /// <param name="EventName">Name of the event.</param>
    /// <param name="EventArgs">The <see cref="TEventArgs"/> instance containing the event data.</param>
    /// <returns></returns>
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
                    // ReSharper disable once CatchAllClause
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
}