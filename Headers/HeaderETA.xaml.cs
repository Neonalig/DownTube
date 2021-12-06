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
using System.Text;
using System.Windows.Threading;

#endregion

namespace DownTube.Headers {
    public partial class HeaderETA {
        readonly DispatcherTimer _DT;
        public HeaderETA() {
            InitializeComponent();
            _DT = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(1)
            };
            _DT.Tick += _DT_Tick;
            _DT.Start();
        }

        ~HeaderETA() {
            _DT.Stop();
        }

        public string? UpcomingText = null;
        int _Decimal = -1;
        void _DT_Tick( object Sender, EventArgs E ) {
            if (UpcomingText == null) {
                _Decimal++;
                if (_Decimal > 3) { _Decimal = 0; }

                MainLabel.Content = $"   Please Wait{Dot('.', _Decimal)}{Dot(' ', 3 - _Decimal)}  ";
            } else {
                MainLabel.Content = UpcomingText;
            }
        }

        internal static string Dot( char C, int Count ) {
            if (Count <= 0) { return string.Empty; }

            StringBuilder SB = new StringBuilder(3);
            SB.Append(C, Count);
            return SB.ToString();
        }
    }
}
