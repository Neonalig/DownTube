#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.IO;

#endregion

namespace DownTube.Extensions;

public static class ProcessExtensions {

    /// <summary>
    /// Constructs a new <see cref="Process"/> pointing to the <c>explorer.exe</c> executable.
    /// </summary>
    /// <remarks>This will not start the process. Invoke <see cref="Process.Start()"/> when ready.</remarks>
    /// <value>
    /// A new explorer <see cref="Process"/>.
    /// </value>
    public static Process GetExplorerProcess() => new Process {
        StartInfo = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"))
    };

}
