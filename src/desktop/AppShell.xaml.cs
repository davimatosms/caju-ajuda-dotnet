

using CajuAjuda.Desktop.Views; 

namespace CajuAjuda.Desktop;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

       
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}