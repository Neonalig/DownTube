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

#endregion

namespace DownTube.Models {
    [Flags]
    public enum DASHAudioFlags {
        None = 0,
        AAC = 1,
        [Description("HE v1")]
        HEv1 = 2,
        LC = 4,
        Vorbis = 8,
        Opus = 16
    }
}