using System.IO;

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
