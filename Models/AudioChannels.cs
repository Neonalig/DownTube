#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#nullable enable

#region Using Directives

using System.ComponentModel;

#endregion

namespace DownTube.Models {
    public enum AudioChannels {
        None,
        [Description("Mono / Dual Mono (1)")]
        Mono,
        [Description("Stereo (2)")]
        Stereo,
        [Description("Surround (5.1)")]
        Surround51,
        [Description("Surround (7.1)")]
        Surround71
    }
}