#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

// ReSharper disable LoopCanBeConvertedToQuery
namespace DownTube.DataTypes.Numerals;

public static class NumeralExtensions {

    public static TResult Sum<TResult>( this IEnumerable<INumber> Values ) {
        TResult Base = default!;
        foreach ( INumber Value in Values ) {
            if ( Base == null ) {
                Base = Value.InnerValue;
            } else {
                Base += Value.InnerValue;
            }
        }
        return Base;
    }

    public static TResult Sum<T, TResult>( this IEnumerable<T> Values ) where T : TResult, IAdd<TResult, TResult, TResult> where TResult : IAdd<TResult, TResult, TResult> {
        TResult Base = default!;
        foreach ( T Value in Values ) {
            if (Base == null ) {
                Base = Value;
            } else {
                Base += Value;
            }
        }
        return Base;
    }

    public static TResult Product<T, TResult>( this IEnumerable<T> Values ) where T : TResult, INumber<T> where TResult : INumber<TResult> {
        TResult Base = default!;
        foreach ( T Value in Values ) {
            if ( Base == null ) {
                Base = Value;
            } else {
                Base *= Value;
            }
        }
        return Base;
    }

    public static T Add<T>( this T Value, params T[] Values ) where T : IAdd<T, T, T> {
        T Base = Value;
        foreach ( T Val in Values ) {
            Base += Val;
        }
        return Base;
    }

    public static T Subtract<T>( this T Value, params T[] Values ) where T : ISubtract<T, T, T> {
        T Base = Value;
        foreach ( T Val in Values ) {
            Base -= Val;
        }
        return Base;
    }

    public static T Multiply<T>( this T Value, params T[] Values ) where T : IMultiply<T, T, T> {
        T Base = Value;
        foreach ( T Val in Values ) {
            Base *= Val;
        }
        return Base;
    }

    public static T Divide<T>( this T Value, params T[] Values ) where T : IDivide<T, T, T> {
        T Base = Value;
        foreach ( T Val in Values ) {
            Base /= Val;
        }
        return Base;
    }

    public static SByteNumber  ToSByte( this INumber Number )  => new SByteNumber((sbyte)Number.InnerValue);
    public static Int16Number  ToInt16( this INumber Number )  => new Int16Number((short)Number.InnerValue);
    public static Int32Number  ToInt32( this INumber Number )  => new Int32Number((int)Number.InnerValue);
    public static Int64Number  ToInt64( this INumber Number )  => new Int64Number((long)Number.InnerValue);
    public static ByteNumber   ToByte( this INumber Number )   => new ByteNumber((byte)Number.InnerValue);
    public static UInt16Number ToUInt16( this INumber Number, bool CatchNegative = true ) => new UInt16Number(!CatchNegative || Number.InnerValue >= 0 ? (ushort)Number.InnerValue : (ushort)0);
    public static UInt32Number ToUInt32( this INumber Number, bool CatchNegative = true ) => new UInt32Number(!CatchNegative || Number.InnerValue >= 0 ? (uint)Number.InnerValue : 0u);
    public static UInt64Number ToUInt64( this INumber Number, bool CatchNegative = true ) => new UInt64Number(!CatchNegative || Number.InnerValue >= 0 ? (ulong)Number.InnerValue : 0ul);

}