namespace DownTube.Extensions;

public static class UriExtensions {

    public static string GetRaw( this Uri U ) => Uri.UnescapeDataString(U.ToString());
    public static string GetRawPath( this Uri U ) => Uri.UnescapeDataString(U.ToString()).Replace('/', '\\');
}