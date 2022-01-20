using System.Windows;

using MVVMUtils;

namespace DownTube.Views.Windows;
/// <summary>
/// Interaction logic for UtilityDownloaderWindow.xaml
/// </summary>
public partial class UtilityDownloaderWindow : IView<UtilityDownloaderWindow_ViewModel> {
    public UtilityDownloaderWindow() {
        InitializeComponent();

        VM = new UtilityDownloaderWindow_ViewModel();
        DataContext = VM;

        VM.Setup(this);
    }

    /// <inheritdoc />
    public UtilityDownloaderWindow_ViewModel VM { get; }

    void ReturnToMain() {
        MainWindow.Instance.Show();
        Close();
    }

    void CancelButton_Click( object Sender, RoutedEventArgs E ) => ReturnToMain();

    void InstallButton_Click( object Sender, RoutedEventArgs E ) => VM.UpdateDialogVisible = true;

    void ManualInstall_OnClick( object Sender, RoutedEventArgs E ) { }

    void AutomaticInstall_OnClick( object Sender, RoutedEventArgs E ) { }
}
