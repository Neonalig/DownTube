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
using System.Windows;
using ControlzEx.Theming;
using DownTube.Headers;

#endregion

namespace DownTube {
    public static class Themer {
        public enum KnownTheme {
            DarkMagenta,
            DarkGreen
        }

        public static KnownTheme ActiveTheme = default;

        public static void SetTheme( KnownTheme T ) {
            ActiveTheme = T;
            switch (T) {
                case KnownTheme.DarkMagenta:
                    ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Magenta");
                    break;
                case KnownTheme.DarkGreen:
                    ThemeManager.Current.ChangeTheme(Application.Current, "Dark.Green");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(T), T, null);
            }

            foreach (Window W in Application.Current.Windows) {
                if (W is MainWindow MW) {
                    if (MW.Header.Content is HeaderLogo HL) {
                        HL.UpdateLogo();
                    }
                    return;
                }
            }
        }
    }
}
