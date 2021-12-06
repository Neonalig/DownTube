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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;
using DownTube.Common;

#endregion

namespace DownTube.Controls {
    public partial class StatusControl {
        public StatusControl_ViewModel VM;

        public Action<bool, double> OnProgressChanged;
        public Action<TaskbarItemProgressState, double> OnTaskbarProgressChanged;

        public StatusControl() {
            InitializeComponent();
            VM = DataContext as StatusControl_ViewModel ?? throw new InvalidCastException();
            VM.Init(this);
        }

        public void Init( Action<bool, double> OnProgressChanged, Action<TaskbarItemProgressState, double> OnTaskbarProgressChanged ) {
            this.OnProgressChanged = OnProgressChanged;
            this.OnTaskbarProgressChanged = OnTaskbarProgressChanged;
        }

        public void Set( Status NewStatus ) {
            VM.Status = NewStatus;

            Dispatcher.Invoke(() => {
                switch (NewStatus) {
                    case Status.Loading:
                        OnProgressChanged.Invoke(true, 0.0);
                        OnTaskbarProgressChanged.Invoke(TaskbarItemProgressState.Error, 0.0);
                        break;
                    case Status.Error:
                        OnProgressChanged.Invoke(false, 0.0);
                        OnTaskbarProgressChanged.Invoke(TaskbarItemProgressState.Error, 0.0);
                        break;
                    case Status.Success:
                        OnProgressChanged.Invoke(false, 1.0);
                        OnTaskbarProgressChanged.Invoke(TaskbarItemProgressState.Normal, 1.0);
                        break;
                    default:
                        OnProgressChanged.Invoke(false, 0.0);
                        OnTaskbarProgressChanged.Invoke(TaskbarItemProgressState.None, 0.0);
                        break;
                }
            });
        }

        public void Set( bool Indeterminate ) => Set(Indeterminate ? Status.Loading : Status.None);

        public void Set(double Progress) {
            VM.Status = Status.None;

            //Debug.WriteLine("\tSetting Progress to: " + Progress);
            Dispatcher.Invoke(() => {
                //Debug.WriteLine("\t\tD");
                OnProgressChanged.Invoke(false, Progress);
                OnTaskbarProgressChanged.Invoke(TaskbarItemProgressState.Normal, Progress);
            });
        }

        //public Status Status {
        //    get => VM.Status;
        //    set => VM.Status = value;
        //}
    }

    public class StatusControl_ViewModel : ModelViewModel<StatusControl> {
        public StatusControl_ViewModel() : this(null!, null) { }

        public StatusControl_ViewModel( StatusControl M, PropertyChangedEventHandler? PropertyChanged = null ) : base(M, PropertyChanged) => _Status = Status.None;

        Status _Status;
        public Status Status {
            get => GetValue(ref _Status);
            set => SetValue(ref _Status, value);
        }

        public Visibility StatusLoading => Status == Status.Loading ? Visibility.Visible : Visibility.Collapsed;
        public Visibility StatusSuccess => Status == Status.Success ? Visibility.Visible : Visibility.Collapsed;
        public Visibility StatusError   => Status == Status.Error   ? Visibility.Visible : Visibility.Collapsed;
    }

    public enum Status {
        None,
        Loading,
        Success,
        Error
    }
}
