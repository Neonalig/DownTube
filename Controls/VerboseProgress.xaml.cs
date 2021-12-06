#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using DownTube.Common;

#nullable enable

namespace DownTube.Controls {
    public partial class VerboseProgress {

        public VerboseProgress_ViewModel VM;

        public VerboseProgress() {
            InitializeComponent();
            VM = DataContext as VerboseProgress_ViewModel ?? throw new InvalidCastException();
            VM.Init(this);
        }

        public double Progress {
            get => VM.Progress;
            set {
                VM.Indeterminate = false;
                VM.Progress = value;
            }
        }

        public bool Indeterminate {
            get => VM.Indeterminate;
            set {
                VM.Progress = 0.0;
                VM.Indeterminate = value;
            }
        }
    }

    public class VerboseProgress_ViewModel : ModelViewModel<VerboseProgress> {

        public VerboseProgress_ViewModel() : this(null!, null) { }

        public VerboseProgress_ViewModel( VerboseProgress M, PropertyChangedEventHandler? PropertyChanged = null ) : base(M, PropertyChanged) {
            _Progress = 0.0;
            _ProgressText = "0.00%";
            _Indeterminate = false;
        }

        double _Progress;
        public double Progress {
            get => GetValue(ref _Progress);
            set {
                SetValue(ref _Progress, value);
                ProgressText = $"{_Progress:P2}";
            }
        }

        bool _Indeterminate;
        public bool Indeterminate {
            get => GetValue(ref _Indeterminate);
            set {
                SetValue(ref _Indeterminate, value);
                ProgressText = value ? "Loading..." : "0.00%";
            }
        }


        string _ProgressText;
        public string ProgressText {
            get => GetValue(ref _ProgressText);
            set => SetValue(ref _ProgressText, value);
        }

    }
}
