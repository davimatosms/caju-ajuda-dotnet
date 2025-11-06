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

    // Carrega os detalhes quando a página aparece — compatível com Shell e navegação direta.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is DetalheChamadoViewModel viewModel)
        {
            // Só executa se o ChamadoId já estiver definido (QueryProperty via Shell)
            if (viewModel.ChamadoId != 0)
            {
                viewModel.LoadDetalhesCommand.Execute(null);
            }
        }
    }

    // 🧹 Cleanup ao sair da página
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is DetalheChamadoViewModel viewModel)
        {
            await viewModel.OnDisappearingAsync();
        }
    }
}