using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using JetBrains.Annotations;

namespace DownTube.Views.Controls;

/// <summary>
/// Interaction logic for FolderPicker.xaml
/// </summary>
public partial class FolderPicker : INotifyPropertyChanged {
    /// <summary>
    /// Initialises a new instance of the <see cref="FolderPicker"/> class.
    /// </summary>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public FolderPicker() {
        InitializeComponent();
        Path = DefaultPath;
        TextPath = Path.FullName;
    }

    /// <summary>
    /// Gets or sets the default path.
    /// </summary>
    /// <value>
    /// The default path.
    /// </value>
    public DirectoryInfo DefaultPath { get; set; } = FileSystemInfoExtensions.AppDir;

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
                this.InvokeOnPropertyChanged();
                if ( !Path.Exists ) {
                    Path = DefaultPath; 
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected path.
    /// </summary>
    /// <value>
    /// The selected path.
    /// </value>
    public DirectoryInfo Path { get; set; }

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
                if ( TextPath != Path.FullName ) {
                    Validate();
                    E.Handled = true;
                }
                break;
        }
    }

    /// <summary>
    /// Validates the user input, changing <see cref="Path"/> is correct, or resetting <see cref="TextPath"/> if not.
    /// </summary>
    void Validate() {
        Debug.WriteLine($"Validating {TextPath}...");
        if ( TextPath.GetDirectory().Out(out DirectoryInfo Dir) && ( !MustExist || Dir.Exists ) ) {
            Debug.WriteLine($"\tValid! ({Dir.FullName})");
            Path = Dir.Resolve();
        } else {
            Debug.WriteLine($"\tInvalid! (Resetting to {Path.FullName})");
        }
        TextPath = Path.FullName;
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