#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

#endregion

namespace DownTube.Exceptions;

public abstract class EnumValueOOB : ArgumentException {
    protected EnumValueOOB(string Value, string ValueUnderlying, string Min, string MinUnderlying, string Max, string MaxUnderlying, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : base($"Enum value {Value} ({ValueUnderlying}) is outside of the supported range. (Expected between {Min} ({MinUnderlying}) and {Max} ({MaxUnderlying}))", ValueParamName, new IndexOutOfRangeException($"Expected {MinUnderlying}≤𝑥≤{MaxUnderlying}, but got {ValueUnderlying}.")) { }
}

public class EnumValueOutOfRangeException : EnumValueOOB {
    public EnumValueOutOfRangeException( object Value, object ValueUnderlying, object Min, object MinUnderlying, object Max, object MaxUnderlying, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : base(Value.GetString(), ValueUnderlying.GetString(), Min.GetString(), MinUnderlying.GetString(), Max.GetString(), MaxUnderlying.GetString(), ValueParamName) { }

    public EnumValueOutOfRangeException( object Value, object Min, object Max, Type EnumType, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : this(Value, EnumToValue(Value, EnumType), Min, EnumToValue(Min, EnumType), Max, EnumToValue(Max, EnumType), ValueParamName) { }

    public EnumValueOutOfRangeException( object Value, Type EnumType, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : this(Value, EnumToValue(Value, EnumType), CtoHelper(EnumType, out object MinUnder, out object Max, out object MaxUnder), MinUnder, Max, MaxUnder, ValueParamName) { }

    //internal static object ValueToEnum( object Value, Type EnumType ) => Enum.ToObject(EnumType, Value);

    internal static object EnumToValue( object Enum, Type EnumType ) => Convert.ChangeType(Enum, EnumType.GetEnumUnderlyingType());

    //public static readonly Dictionary<Type, Func<object?, object, int>> ComparisonMethods = new Dictionary<Type, Func<object?, object, int>>();

    public static readonly Dictionary<Type, (object Min, object MinUnderlying, object Max, object MaxUnderlying)> MinMaxPairs = new Dictionary<Type, (object, object, object, object)>();
    [SuppressMessage("ReSharper", "UseNullableAnnotationInsteadOfAttribute")]
    public static void GetMinMax( Type EnumType, out object Min, out object MinUnderlying, out object Max, out object MaxUnderlying ) {
        if ( MinMaxPairs.ContainsKey(EnumType) ) {
            (object Mi, object MiU, object Ma, object MaU) = MinMaxPairs[EnumType];
            Min = Mi;
            MinUnderlying = MiU;
            Max = Ma;
            MaxUnderlying = MaU;
            return;
        }

        Type UnderType = EnumType.GetEnumUnderlyingType();
        object? Comp = typeof(Comparer<>).MakeGenericType(UnderType).GetProperty(nameof(Comparer.Default))?.GetGetMethod()?.Invoke(null, Array.Empty<object>());
        MethodInfo? CompMethod = Comp?.GetType().GetMethod(nameof(Comparer.Compare));
        CompMethod.ThrowIfNull();

        object? FMi = null, FMa = null;
        Array Arr = EnumType.GetEnumValues();
        foreach ( object Obj in Arr ) {
            if ( FMi == null || (int)CompMethod.Invoke(Comp, new [] { Obj, FMi }).CatchNull() < 0 ) {
                FMi = Obj;
            }
            if ( FMa == null || (int)CompMethod.Invoke(Comp, new[] { Obj, FMa }).CatchNull() > 0 ) {
                FMa = Obj;
            }
        }
        Min = FMi.CatchNull();
        Max = FMa.CatchNull();
        MinUnderlying = Convert.ChangeType(Min, UnderType).CatchNull();
        MaxUnderlying = Convert.ChangeType(Max, UnderType).CatchNull();
    }

    internal static object CtoHelper( Type EnumType, out object MinUnderlying, out object Max, out object MaxUnderlying ) {
        GetMinMax(EnumType, out object Min, out MinUnderlying, out Max, out MaxUnderlying);
        return Min;
    }
}

public class EnumValueOutOfRangeException<T> : EnumValueOOB where T : struct, Enum {
    public EnumValueOutOfRangeException( T Value, object ValueUnderlying, T Min, object MinUnderlying, T Max, object MaxUnderlying, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : base(Value.GetString(), ValueUnderlying.GetString(), Min.GetString(), MinUnderlying.GetString(), Max.GetString(), MaxUnderlying.GetString(), ValueParamName) { }

    public EnumValueOutOfRangeException( T Value, T Min, T Max, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : this(Value, EnumToValue(Value), Min, EnumToValue(Min), Max, EnumToValue(Max), ValueParamName) { }

    public EnumValueOutOfRangeException( T Value, [CallerArgumentExpression("Value")] string? ValueParamName = null ) : this(Value, EnumToValue(Value), CtoHelper(out object MinUnder, out T Max, out object MaxUnder), MinUnder, Max, MaxUnder, ValueParamName) { }

    //internal static T ValueToEnum( object Value ) => (T)Enum.ToObject(UnderType, Value);
    internal static object EnumToValue( T Enum ) => Convert.ChangeType(Enum, UnderType);

    public static readonly Type UnderType = typeof(T).GetEnumUnderlyingType();

    //public static readonly Dictionary<Type, Func<object?, object, int>> ComparisonMethods = new Dictionary<Type, Func<object?, object, int>>();

    public static readonly Dictionary<Type, (T Min, object MinUnderlying, T Max, object MaxUnderlying)> MinMaxPairs = new Dictionary<Type, (T, object, T, object)>();
    [SuppressMessage("ReSharper", "UseNullableAnnotationInsteadOfAttribute")]
    public static void GetMinMax( out T Min, out object MinUnderlying, out T Max, out object MaxUnderlying ) {
        if ( MinMaxPairs.ContainsKey(typeof(T)) ) {
            (T Mi, object MiU, T Ma, object MaU) = MinMaxPairs[typeof(T)];
            Min = Mi;
            MinUnderlying = MiU;
            Max = Ma;
            MaxUnderlying = MaU;
            return;
        }

        Min = Enum.GetValues<T>().Min();
        Max = Enum.GetValues<T>().Max();
        MinUnderlying = EnumToValue(Min);
        MaxUnderlying = EnumToValue(Max);
    }

    internal static T CtoHelper( out object MinUnderlying, out T Max, out object MaxUnderlying ) {
        GetMinMax(out T Min, out MinUnderlying, out Max, out MaxUnderlying);
        return Min;
    }
}