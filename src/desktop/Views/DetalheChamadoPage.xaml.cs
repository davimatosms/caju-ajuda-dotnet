// CajuAjuda.Desktop/Views/DetalheChamadoPage.xaml.cs

using CajuAjuda.Desktop.ViewModels;

namespace CajuAjuda.Desktop.Views;

public partial class DetalheChamadoPage : ContentPage
{
    public DetalheChamadoPage(DetalheChamadoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Este método é chamado automaticamente pelo MAUI sempre que a página é exibida.
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Forçamos o ViewModel a carregar os dados
        if (BindingContext is DetalheChamadoViewModel viewModel)
        {
            viewModel.LoadDetalhesCommand.Execute(null);
        }
    }
}