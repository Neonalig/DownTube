using System.Windows;

namespace DownTube; 

/// <summary> Interaction logic for App.xaml </summary>
public partial class App  {
    protected override void OnStartup( StartupEventArgs E ) {
        //base.OnStartup(E);
        WPFUI.Theme.Manager.SetSystemTheme();
    }
}