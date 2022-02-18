#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace DownTube.Extensions;

public static class DelegateExtensions {

    /// <summary>
    /// Attempts to invoke the given action.
    /// </summary>
    /// <param name="Act">The action to invoke.</param>
    /// <returns><see langword="true"/> if no exceptions were thrown.</returns>
    public static bool Try( this Action Act ) {
        try {
            Act();
            return true;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Attempts to invoke the given action.
    /// </summary>
    /// <param name="Act">The action to invoke.</param>
    /// <param name="Result">The result from the action.</param>
    /// <returns><see langword="true"/> if no exceptions were thrown.</returns>
    public static bool Try<T>( this Func<T> Act, out T Result ) {
        try {
            Result = Act();
            return true;
        } catch {
            Result = default!;
            return false;
        }
    }

    /// <summary>
    /// Attempts to invoke the given action.
    /// </summary>
    /// <param name="Act">The action to invoke.</param>
    /// <param name="Input">The required action parameter.</param>
    /// <param name="Result">The result from the action.</param>
    /// <returns><see langword="true"/> if no exceptions were thrown.</returns>
    public static bool Try<TIn, TOut>( this Func<TIn, TOut> Act, TIn Input, out TOut Result ) {
        try {
            Result = Act(Input);
            return true;
        } catch {
            Result = default!;
            return false;
        }
    }
}