

using CajuAjuda.Desktop.ViewModels;

namespace CajuAjuda.Desktop.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel) 
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}