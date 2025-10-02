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

    // Este m�todo � chamado automaticamente pelo MAUI sempre que a p�gina � exibida.
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // For�amos o ViewModel a carregar os dados
        if (BindingContext is DetalheChamadoViewModel viewModel)
        {
            viewModel.LoadDetalhesCommand.Execute(null);
        }
    }
}