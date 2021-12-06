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
    public enum DASHVideoFlags {
        None = 0,
        HDR = 1,
        HFR = 2,
        [Description("H.264")]
        H264 = 4,
        VP8 = 8,
        VP9 = 16,
        [Description("VP9.2")]
        VP92 = 32,
        AV1 = 64,
        Baseline = 128,
        Main = 256,
        High = 512,
        [Description("L1.1")]
        L11 = 1024,
        [Description("L2.1")]
        L21 = 2048,
        [Description("L3.0")]
        L30 = 4096,
        [Description("L3.1")]
        L31 = 8192,
        [Description("L3.2")]
        L32 = 16384,
        [Description("L4.0")]
        L40 = 32768,
        [Description("L4.2")]
        L42 = 65535
    }
}