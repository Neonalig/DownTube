using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

using MVVMUtils;

namespace DownTube.Views.Windows; 

/// <summary>
/// Interaction logic for UpdateWindow.xaml
/// </summary>
public partial class UpdateWindow : IView<UpdateWindow_ViewModel> {
    public UpdateWindow() {
        InitializeComponent();

        VM = new UpdateWindow_ViewModel();
        DataContext = VM;

        VM.Setup(this);
    }

    /// <inheritdoc />
    public UpdateWindow_ViewModel VM { get; }

    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void AutomaticInstall_OnClick(object Sender, RoutedEventArgs E ) {

    }
    
    /// <summary>
    /// Occurs when the OnClick <see langword="event"/> is raised.
    /// </summary>
    /// <param name="Sender">The source of the <see langword="event"/>.</param>
    /// <param name="E">The raised <see langword="event"/> arguments.</param>
    void ManualInstall_OnClick( object Sender, RoutedEventArgs E ) {

    }
}

public class UpdateWindow_ViewModel : Window_ViewModel<UpdateWindow> {
    /// <summary>
    /// Gets or sets the latest version available.
    /// </summary>
    /// <value>
    /// The latest version available from GitHub.
    /// </value>
    [SuppressMessage("ReSharper", "ExceptionNotDocumentedOptional")]
    public Version LatestVersion { get; set; } = new Version(0, 1, 1, 0);

    /// <inheritdoc />
    public override Border WindowBGBorder => View.MainBorder;
}