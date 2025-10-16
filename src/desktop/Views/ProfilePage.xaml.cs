using CajuAjuda.Desktop.ViewModels;

namespace CajuAjuda.Desktop.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}