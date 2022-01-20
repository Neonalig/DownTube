#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using JetBrains.Annotations;

using WPFUI.Common;

#endregion

namespace DownTube.Views.Controls;

/// <summary>
/// Interaction logic for FilePicker.xaml
/// </summary>
public partial class FilePicker : INotifyPropertyChanged {
    /// <summary>
    /// Initialises a new instance of the <see cref="FilePicker"/> class.
    /// </summary>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public FilePicker() {
        InitializeComponent();
        Path = DefaultPath;
        TextPath = Path.FullName;
        SetGlyph(Glyph, GlyphFilled);
        Loaded += ( _, _ ) => {
            TB.CaretIndex = TB.Text.Length;
            Rect Rect = TB.GetRectFromCharacterIndex(TB.CaretIndex);
            TB.ScrollToHorizontalOffset(Rect.Right);
        };
        PropertyChanged += ( S, E ) => {
            if ( S is not FilePicker FP ) { return; }
            switch ( E.PropertyName ) {
                case nameof(Glyph):
                case nameof(GlyphFilled):
                    FP.SetGlyph(Glyph, GlyphFilled);
                    break;
            }
        };
    }

    /// <summary>
    /// Sets the glyph to the specified <see cref="Icon"/> and fill.
    /// </summary>
    /// <param name="Glyph">The glyph.</param>
    /// <param name="Filled">If <see langword="true" />, the icon is filled; otherwise the icon is an outline.</param>
    public void SetGlyph( Icon Glyph = Icon.Document48, bool Filled = true ) {
        Ico.Glyph = Glyph;
        Ico.Filled = Filled;
    }

    /// <summary>
    /// Gets or sets the default path.
    /// </summary>
    /// <value>
    /// The default path.
    /// </value>
    public FileInfo DefaultPath { get; set; } = FileSystemInfoExtensions.App;

    /// <summary>
    /// Whether the path must exist to be considered valid.
    /// </summary>
    bool _MustExist = true;

    /// <summary>
    /// Gets or sets a value indicating whether the path must exist.
    /// </summary>
    /// <value> <see langword="true" /> if the path must exist to be valid; otherwise, <see langword="false" />. </value>
    public bool MustExist {
        get => _MustExist;
        set {
            if ( value != _MustExist ) {
                _MustExist = value;
                _ = this.InvokeOnPropertyChanged();
                if ( Path is null || !Path.Exists ) {
                    Path = DefaultPath;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the glyph displayed on the 'Browse' button.
    /// </summary>
    /// <value>
    /// The glyph displayed to the right of the 'Browse' text.
    /// </value>
    public Icon Glyph { get; set; } = Icon.Document48;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Glyph"/> is filled.
    /// </summary>
    /// <value> <see langword="true" /> if <see cref="Glyph"/> is filled; otherwise, <see langword="false" />. </value>
    public bool GlyphFilled { get; set; } = true;

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public FileInfo? Path {
        get => (FileInfo?)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    /// <summary>Identifies the <see cref="Path"/> dependency property.</summary>
    public static readonly DependencyProperty PathProperty = DependencyProperty.Register(nameof(Path), typeof(FileInfo), typeof(FilePicker), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the text path.
    /// </summary>
    /// <value>
    /// The text path.
    /// </value>
    string TextPath {
        get => TB.Text;
        set => TB.Text = value;
    }

    /// <summary>
    /// Occurs when the LostKeyboardFocus <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void TextBox_LostKeyboardFocus( object Sender, KeyboardFocusChangedEventArgs E ) {
        Validate();
    }

    /// <summary>
    /// Occurs when the  <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void TextBox_PreviewKeyDown( object Sender, KeyEventArgs E ) {
        switch ( E.Key ) {
            case Key.Return:
                if ( Path is null || TextPath != Path.FullName ) {
                    Validate();
                    E.Handled = true;
                }
                break;
        }
    }

    string? _VerifyPattern = null;

    /// <summary>
    /// Gets or sets the verification regex.
    /// </summary>
    /// <remarks>If <see langword="null"/> or empty, this is ignored; otherwise, if <see cref="Match.Success"/> is <see langword="false"/>, the text is considered invalid.</remarks>
    /// <value>
    /// The verification regex.
    /// </value>
    /// <seealso cref="VerifyRegex"/>
    public string? VerifyPattern {
        get => _VerifyPattern;
        set {
            if ( _VerifyPattern != value ) {
                _VerifyPattern = value;
                VerifyRegex = value is not null ? new Regex(value) : null;
            }
        }
    }

    /// <summary>
    /// Gets the verification regex.
    /// </summary>
    /// <value>
    /// The verification regex.
    /// </value>
    /// <seealso cref="VerifyPattern"/>
    public Regex? VerifyRegex { get; private set; }

    /// <summary>
    /// Validates the user input, changing <see cref="Path"/> is correct, or resetting <see cref="TextPath"/> if not.
    /// </summary>
    void Validate() {
        Debug.WriteLine($"Validating {TextPath}...");
        if ( string.IsNullOrWhiteSpace(TextPath) ) {
            TextPath = string.Empty;
            Path = null;
            Debug.WriteLine("\tEmpty!");
            return;
        }
        if ( TextPath.GetFile().Out(out FileInfo Fl) && (!MustExist || Fl.Exists) && (VerifyRegex is null || VerifyRegex.IsMatch(Fl.FullName)) ) {
            Debug.WriteLine($"\tValid! ({Fl.FullName})");
            Path = Fl.Resolve();
        } else {
            Debug.WriteLine($"\tInvalid! (Resetting to {Path?.FullName ?? string.Empty})");
        }
        TextPath = Path?.FullName ?? string.Empty;
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Called when a property is changed.
    /// </summary>
    /// <param name="PropertyName">The name of the property.</param>
    /// <exception cref="Exception">A <see langword="delegate"/> callback throws an exception.</exception>
    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged( [CallerMemberName] string? PropertyName = null ) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
}