#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

using MVVMUtils;

#endregion

namespace DownTube.Views.Windows;

/// <summary>
/// The viewmodel for any <see cref="Window"/>
/// </summary>
[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public abstract class Window_ViewModel<T> : ViewModel<T> where T : Window {


    /// <inheritdoc />
    public override void OnSetup() {
        SolidColorBrush SCB = (SolidColorBrush)View.Background;
        WindowBGBorder.Background = SCB.WithA(242);
        const byte Contrast = 8;
        WindowBGBorder.BorderBrush = SCB.With(222, (byte)(SCB.Color.R - Contrast), (byte)(SCB.Color.G - Contrast), (byte)(SCB.Color.B - Contrast));
        View.Background = new SolidColorBrush(new Color { R = 0, G = 0, B = 0, A = 0 });
    }

    /// <summary>
    /// Gets the window's background border.
    /// </summary>
    /// <value>
    /// The window's background border.
    /// </value>
    public abstract Border WindowBGBorder { get; }

    /// <summary>
    /// Gets or sets the border margin.
    /// </summary>
    /// <value>
    /// The border margin.
    /// </value>
    public Thickness BorderMargin { get; set; } = _BorderMargin_Normal;

    /// <summary> (<see langword="const"/>) Border margin. </summary>
    static readonly Thickness
        _BorderMargin_Normal = new Thickness(0, 0, 7, 7),
        _BorderMargin_Maximised = new Thickness(0, 0, 0, 0);

    /// <summary>
    /// Gets or sets the border effect.
    /// </summary>
    /// <value>
    /// The border effect.
    /// </value>
    public Effect? BorderEffect { get; set; } = _BorderEffect_Normal;

    /// <summary> (<see langword="const"/>) Border effect. </summary>
    static readonly Effect?
        _BorderEffect_Normal = new DropShadowEffect { Opacity = 0.7 },
        _BorderEffect_Maximised = null;

    /// <summary>
    /// Gets or sets the border thickness.
    /// </summary>
    /// <value>
    /// The border thickness.
    /// </value>
    public double BorderThickness { get; set; } = _BorderThickness_Normal;

    // ReSharper disable InconsistentNaming
    /// <summary> (<see langword="const"/>) Border thickness. </summary>
    const double
        _BorderThickness_Normal = 1d,
        _BorderThickness_Maximised = 0d;
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// The window state.
    /// </summary>
    WindowState _WindowState = WindowState.Normal;

    /// <summary>
    /// Gets or sets the state of the window.
    /// </summary>
    /// <value>
    /// The state of the window.
    /// </value>
    public WindowState WindowState {
        get => _WindowState;
        set {
            if ( _WindowState != value ) {
                SetProperty(ref _WindowState, value);
                switch ( value ) {
                    case WindowState.Maximized:
                        BorderMargin = _BorderMargin_Maximised;
                        BorderEffect = _BorderEffect_Maximised;
                        BorderThickness = _BorderThickness_Maximised;
                        break;
                    default:
                        BorderMargin = _BorderMargin_Normal;
                        BorderEffect = _BorderEffect_Normal;
                        BorderThickness = _BorderThickness_Normal;
                        break;
                }

            }
        }
    }
}