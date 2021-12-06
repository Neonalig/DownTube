#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System.Windows;

#endregion

namespace DownTube.Headers {
    public partial class HeaderLogo {
        public HeaderLogo() {
            InitializeComponent();
            //UpdateLogo();
        }

        public enum LogoStyle {
            Normal,
            Magenta,
            Green
        }

        public void UpdateLogo() {
            switch (Themer.ActiveTheme) {
                case Themer.KnownTheme.DarkMagenta:
                    MainLogo.Visibility = Visibility.Collapsed;
                    GreenLogo.Visibility = Visibility.Collapsed;
                    MagentaLogo.Visibility = Visibility.Visible;
                    break;
                case Themer.KnownTheme.DarkGreen:
                    MainLogo.Visibility = Visibility.Collapsed;
                    GreenLogo.Visibility = Visibility.Visible;
                    MagentaLogo.Visibility = Visibility.Collapsed;
                    break;
                default:
                    MainLogo.Visibility = Visibility.Visible;
                    GreenLogo.Visibility = Visibility.Collapsed;
                    MagentaLogo.Visibility = Visibility.Collapsed;
                    break;
                    //throw new ArgumentOutOfRangeException();
            }
        }
    }
}
