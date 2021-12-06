#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

namespace DownTube.Models {
    public readonly struct Bitrate {
        public readonly float Rate;
        public readonly bool Variable;

        public static readonly Bitrate None = new Bitrate(- 1, false);

        /// <summary> 48 Kbps </summary>
        public static readonly Bitrate C48K = new Bitrate(48, false);
        /// <summary> 96 Kbps </summary>
        public static readonly Bitrate C96K = new Bitrate(96, false);
        /// <summary> 128 Kbps </summary>
        public static readonly Bitrate C128K = new Bitrate(128, false);
        /// <summary> 192 Kbps </summary>
        public static readonly Bitrate C192K = new Bitrate(192, false);
        /// <summary> 256 Kbps </summary>
        public static readonly Bitrate C256K = new Bitrate(256, false);
        /// <summary> 384 Kbps </summary>
        public static readonly Bitrate C384K = new Bitrate(384, false);
        /// <summary> (VBR) ~50 Kbps </summary>
        public static readonly Bitrate V50K = new Bitrate(50, true);
        /// <summary> (VBR) ~70 Kbps </summary>
        public static readonly Bitrate V70K = new Bitrate(70, true);
        /// <summary> (VBR) ~160 Kbps </summary>
        public static readonly Bitrate V160K = new Bitrate(160, true);

        public Bitrate(float Rate = 256, bool Variable = false) {
            this.Rate = Rate;
            this.Variable = Variable;
        }

        public override string ToString() => $"{(Variable ? "(VBR) ~" : string.Empty)}{Rate} Kbps";
    }
}