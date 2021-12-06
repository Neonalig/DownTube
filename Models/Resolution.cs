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

#endregion

namespace DownTube.Models {
    public readonly struct Resolution {
        public readonly byte Width, Height;

        /// <summary> No resolution (i.e. Audio Only) </summary>
        public static readonly Resolution None = new Resolution(0, 0);

        /// <summary> 4320p </summary>
        public static readonly Resolution UHD8K = new Resolution(7680, 4320);
        public static readonly Resolution P4320 = UHD8K;
        /// <summary> 2160p </summary>
        public static readonly Resolution UHD = new Resolution(3840, 2160);
        public static readonly Resolution P2160 = UHD;
        /// <summary> 1440p </summary>
        public static readonly Resolution QHD = new Resolution(2560, 1440);
        public static readonly Resolution P1440 = QHD;
        /// <summary> 1080p </summary>
        public static readonly Resolution FHD = new Resolution(1920, 1080);
        public static readonly Resolution P1080 = FHD;
        /// <summary> 720p </summary>
        public static readonly Resolution HD = new Resolution(1280, 720);
        public static readonly Resolution P720 = HD;
        /// <summary> 480p </summary>
        public static readonly Resolution SD = new Resolution(854, 480);
        /// <summary> 480p (Equivalent to SD) </summary>
        public static readonly Resolution VGA = SD;
        public static readonly Resolution P480 = SD;
        /// <summary> 360p </summary>
        public static readonly Resolution LD = new Resolution(640, 360);
        /// <summary> 360p (Equivalent to LD) </summary>
        public static readonly Resolution LDTV = LD;
        public static readonly Resolution P360 = LD;
        /// <summary> 240p </summary>
        public static readonly Resolution QVGA = new Resolution(426, 240);
        public static readonly Resolution P240 = QVGA;
        /// <summary> 144p </summary>
        public static readonly Resolution Mobile = new Resolution(256, 144);
        public static readonly Resolution P144 = Mobile;

        public Resolution(int Width = 1920, int Height = 1080) : this((byte)Width, (byte)Height) { }

        public Resolution(byte Width, byte Height) {
            this.Width = Width;
            this.Height = Height;
        }

        public Resolution(byte Height, byte RatioWidth = 16, byte RatioHeight = 9) {
            this.Height = Height;
            Width = (byte)Math.Round((float)Height / RatioHeight * RatioWidth);
        }

        public override string ToString() => $"{Height}p";
    }
}