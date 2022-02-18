#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.Extensions;

public static class TypeExtensions {
    public static string GetTypeName( this Type Type ) => Type.GetTypeCode(Type) switch {
        TypeCode.Empty    => "<null>",
        TypeCode.Object   => Type.Name,
        TypeCode.DBNull   => nameof(DBNull),
        TypeCode.Boolean  => "bool",
        TypeCode.Char     => "char",
        TypeCode.SByte    => "sbyte",
        TypeCode.Byte     => "byte",
        TypeCode.Int16    => "short",
        TypeCode.UInt16   => "ushort",
        TypeCode.Int32    => "int",
        TypeCode.UInt32   => "uint",
        TypeCode.Int64    => "long",
        TypeCode.UInt64   => "ulong",
        TypeCode.Single   => "float",
        TypeCode.Double   => "double",
        TypeCode.Decimal  => "decimal",
        TypeCode.DateTime => nameof(DateTime),
        TypeCode.String   => "string",
        _                 => throw new NotImplementedException()
    };
}