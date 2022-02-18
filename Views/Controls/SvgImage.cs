#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using SharpVectors.Converters;

#endregion

namespace DownTube.Views.Controls;

/// <summary>
/// An implementation of <see cref="Image"/> which supports SVG images.
/// </summary>
/// <seealso cref="Image"/>
/// <seealso cref="SharpVectors"/>
public class SvgImage : Image {

    /// <summary>
    /// Gets or sets the SVG source.
    /// </summary>
    /// <value>
    /// The SVG source.
    /// </value>
    public string SvgSource {
        get => (string)GetValue(SvgSourceProperty);
        set => SetValue(SvgSourceProperty, value);
    }

    /// <summary>Identifies the <see cref="SvgSource"/> dependency property.</summary>
    public static readonly DependencyProperty SvgSourceProperty = DependencyProperty.Register(nameof(SvgSource), typeof(string), typeof(SvgImage), new PropertyMetadata(string.Empty, OnSvgSourceChanged));

    /// <summary>
    /// Called when <see cref="SvgSource"/> is changed.
    /// </summary>
    /// <param name="D">The dependency object.</param>
    /// <param name="E">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    static void OnSvgSourceChanged( DependencyObject D, DependencyPropertyChangedEventArgs E ) {
        if ( D is SvgImage Image ) {
            SvgImageConverterExtension Converter = new SvgImageConverterExtension {
                AppName = StaticBindings.AppName
            };
            Binding Binding = new Binding {
                Converter = Converter,
                ConverterParameter = E.NewValue
            };

            _ = Image.SetBinding(SourceProperty, Binding);
        }
    }
}