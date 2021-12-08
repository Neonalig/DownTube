namespace DownTube.Extensions;

public static class StringExtensions {
    public static string Truncate( this string Value, int MaxLength, string Ellipsis = "..." ) {
        int Length = Value.Length,
            EllLen = Ellipsis.Length;
        if ( Length > MaxLength ) {
            return Value[..(MaxLength - EllLen)] + Ellipsis;
        }
        return Value;
    }
}