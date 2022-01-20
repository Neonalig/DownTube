using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

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

    //void ScrollViewer_PreviewMouseWheel( object Sender, MouseWheelEventArgs E ) {
    //    ScrollViewer Scv = (ScrollViewer)Sender;
    //    Debug.WriteLine($"scroll: {E.Delta} ({Scv.VerticalOffset} -> {Scv.VerticalOffset - E.Delta})");
    //    Scv.ScrollToVerticalOffset(Scv.VerticalOffset - E.Delta);
    //    E.Handled = true;
    //}

    void ListView_PreviewMouseWheel( object Sender, MouseWheelEventArgs E ) {
        if ( ((ListView)Sender).FindVisualChildInTree<ScrollContentPresenter>(typeof(Border), typeof(ScrollViewer), typeof(Grid), typeof(Rectangle)) is { } SCP ) {
            SCP.ScrollOwner.ScrollToVerticalOffset(SCP.ScrollOwner.VerticalOffset - E.Delta);
        } else {
            Debug.WriteLine("tree perusal failed");
        }
        // System.Windows.Controls.Border
        // System.Windows.Controls.ScrollViewer
        // System.Windows.Controls.Grid
        // System.Windows.Shapes.Rectangle
        // System.Windows.Controls.ScrollContentPresenter
        //void LogChildren( DependencyObject Obj, int Lvl ) {
        //    foreach ( DependencyObject Child in Obj.FindVisualChildren() ) {
        //        Debug.WriteLine(new string('\t', Lvl) + Child.GetType());
        //        LogChildren(Child, Lvl + 1);
        //    }
        //}
        //Debug.WriteLine(Sender.GetType());
        //LogChildren((DependencyObject)Sender, 1);
        //if (Sender is ListView LV && LV.TryGetChild(out Border B) && B.TryGetChild(out ScrollViewer Scv) ) {
        //    Debug.WriteLine($"scroll: {E.Delta} ({Scv.VerticalOffset} -> {Scv.VerticalOffset - E.Delta})");
        //    Scv.ScrollToVerticalOffset(Scv.VerticalOffset - E.Delta);
        //    E.Handled = true;
        //}
    }
}
