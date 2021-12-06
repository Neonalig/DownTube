#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

using System.IO;
using System.Reflection;

#nullable enable

namespace DownTube.Common {
    public static class SysStruct {
        public static readonly FileInfo App = new FileInfo(Assembly.GetExecutingAssembly().Location);

        public static readonly DirectoryInfo AppDir = App.Directory!;

        public static readonly DirectoryInfo DriveRoot = AppDir.Root;

        public static readonly DirectoryInfo YouTubeDLDir = new DirectoryInfo(Path.Combine(AppDir.FullName, "YoutubeDL"));

        public static readonly FileInfo YouTubeDL = new FileInfo(Path.Combine(YouTubeDLDir.FullName, "youtube-dl.exe"));

        public static readonly FileInfo FFmpeg = new FileInfo(Path.Combine(YouTubeDLDir.FullName, "ffmpeg.exe"));
    }
}
