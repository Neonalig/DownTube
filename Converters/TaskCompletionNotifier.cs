#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System.ComponentModel;

namespace DownTube.Converters;

// Watches a task and raises property-changed notifications when the task completes.
// See https://stackoverflow.com/a/15007717/11519246 for more info
public sealed class TaskCompletionNotifier<TResult> : INotifyPropertyChanged {
    public TaskCompletionNotifier( Task<TResult> Task ) {
        this.Task = Task;
        if ( !Task.IsCompleted ) {
            TaskScheduler Scheduler = (SynchronizationContext.Current == null) ? TaskScheduler.Current : TaskScheduler.FromCurrentSynchronizationContext();
            Task.ContinueWith(
                T => {
                    PropertyChangedEventHandler? PropertyChanged = this.PropertyChanged;
                    if ( PropertyChanged != null ) {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
                        if ( T.IsCanceled ) {
                            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
                        } else if ( T.IsFaulted ) {
                            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                            //PropertyChanged(this, new PropertyChangedEventArgs("ErrorMessage"));
                        } else {
                            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
                            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Result)));
                        }
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                Scheduler);
        }
    }

    // Gets the task being watched. This property never changes and is never <c>null</c>.
    public Task<TResult> Task { get; }

    // Gets the result of the task. Returns the default value of TResult if the task has not completed successfully.
    public TResult Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default!;

    // Gets whether the task has completed.
    public bool IsCompleted => Task.IsCompleted;

    // Gets whether the task has completed successfully.
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

    // Gets whether the task has been cancelled.
    public bool IsCanceled => Task.IsCanceled;

    // Gets whether the task has faulted.
    public bool IsFaulted => Task.IsFaulted;

    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
}