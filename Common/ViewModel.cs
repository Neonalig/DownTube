#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

#endregion

namespace DownTube.Common {
    public abstract class ViewModel : INotifyPropertyChanged {

        protected ViewModel( PropertyChangedEventHandler? PropertyChanged = null ) {
            this.PropertyChanged = PropertyChanged ?? (( _, __ ) => { });
        }

        public T GetValue<T>( ref T Base ) => Base;

        public void SetValue<T>( ref T Base, T Value, [CallerMemberName] string BasePropertyName = "" ) {
            Base = Value;
            OnPropertyChanged(BasePropertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged( [CallerMemberName] string PropertyName = "" ) => PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
    }

    public abstract class ModelViewModel<T> : ViewModel where T : Control {
        public T M;

        protected ModelViewModel( T M, PropertyChangedEventHandler? PropertyChanged = null ) : base(PropertyChanged) => this.M = M;

        public void Init( T M ) {
            this.M = M;
            OnInit();
        }

        public virtual void OnInit() { }
    }

    public abstract class ViewModelViewModel<T> : ViewModel where T : ViewModel {
        public T VM;

        protected ViewModelViewModel( T VM, PropertyChangedEventHandler? PropertyChanged = null ) : base(PropertyChanged) => this.VM = VM;

        public void Init( T VM ) {
            this.VM = VM;
            OnInit();
        }

        public virtual void OnInit() { }
    }
}