#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System;
using System.Collections.Generic;

#endregion

namespace DownTube.Common {
    public delegate void QAction();

    public delegate void QAction<in T0>( T0 Arg0 );

    public delegate void QAction<in T0, in T1>( T0 Arg0, T1 Arg1 );

    public delegate void QAction<in T0, in T1, in T2>( T0 Arg0, T1 Arg1, T2 Arg2 );

    public delegate void QAction<in T0, in T1, in T2, in T3>( T0 Arg0, T1 Arg1, T2 Arg2, T3 Arg3 );

    public sealed class QEvent : QEventBase<QAction> {
        public QEvent( params QAction[] Handles ) {
            Internal = InternalDefault;
            AddListeners(Handles);
        }

        public override QAction InternalDefault => () => { };

        public void Invoke() => Internal.Invoke();
    }

    public sealed class QEvent<T0> : QEventBase<QAction<T0>> {
        public QEvent( params QAction<T0>[] Handles ) {
            Internal = InternalDefault;
            AddListeners(Handles);
        }

        public override QAction<T0> InternalDefault => _ => { };

        public void Invoke( T0 Arg0 ) => Internal.Invoke(Arg0);
    }

    public sealed class QEvent<T0, T1> : QEventBase<QAction<T0, T1>> {
        public QEvent( params QAction<T0, T1>[] Handles ) {
            Internal = InternalDefault;
            AddListeners(Handles);
        }

        public override QAction<T0, T1> InternalDefault => ( _, __ ) => { };

        public void Invoke( T0 Arg0, T1 Arg1 ) => Internal.Invoke(Arg0, Arg1);
    }

    public sealed class QEvent<T0, T1, T2> : QEventBase<QAction<T0, T1, T2>> {
        public QEvent( params QAction<T0, T1, T2>[] Handles ) {
            Internal = InternalDefault;
            AddListeners(Handles);
        }

        public override QAction<T0, T1, T2> InternalDefault => ( _, __, ___ ) => { };

        public void Invoke( T0 Arg0, T1 Arg1, T2 Arg2 ) => Internal.Invoke(Arg0, Arg1, Arg2);
    }

    public sealed class QEvent<T0, T1, T2, T3> : QEventBase<QAction<T0, T1, T2, T3>> {
        public QEvent( params QAction<T0, T1, T2, T3>[] Handles ) {
            Internal = InternalDefault;
            AddListeners(Handles);
        }

        public override QAction<T0, T1, T2, T3> InternalDefault => ( _, __, ___, ____ ) => { };

        public void Invoke( T0 Arg0, T1 Arg1, T2 Arg2, T3 Arg3 ) => Internal.Invoke(Arg0, Arg1, Arg2, Arg3);
    }

    public abstract class QEventBase<T> where T : Delegate {
        internal T Internal;
        readonly List<T> _Handles;

        //protected QEventBase( List<T> Handles = null ) => _Handles = Handles ?? new List<T>();
        protected QEventBase() {
            _Handles = new List<T>();
            Internal = null!;
        }

        public abstract T InternalDefault { get; }

        public void AddListeners( params T[] Listeners ) => AddListeners(ListenersEnum: Listeners);

        public void AddListeners( IEnumerable<T> ListenersEnum ) {
            foreach (T Listener in ListenersEnum) { AddListener(Listener); }
        }

        //public void Invoke(params object[] Args) => Internal.DynamicInvoke(Args);

        public void AddListener( T Listener ) {
            _Handles.Add(Listener);
            Internal = (T)Delegate.Combine(Internal, Listener);
        }

        public void RemoveListeners( params T[] Listeners ) => RemoveListeners(ListenersEnum: Listeners);

        public void RemoveListeners( IEnumerable<T> ListenersEnum ) {
            foreach (T Listener in ListenersEnum) { RemoveListener(Listener); }
        }

        public void RemoveListener( T Listener ) {
            _Handles.Remove(Listener);
            Internal = Delegate.Remove(Internal, Listener) as T ?? InternalDefault;
        }

        public void RemoveAllListeners() {
            foreach (T Handle in _Handles) { Internal = Delegate.Remove(Internal, Handle) as T ?? InternalDefault; }

            _Handles.Clear();
        }
    }
}