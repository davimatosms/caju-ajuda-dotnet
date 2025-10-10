using CajuAjuda.Desktop.ViewModels;

namespace CajuAjuda.Desktop.Views;

public partial class MainPage : ContentPage
{
    // O ViewModel � injetado aqui, como antes
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // ======================================================
    //               NOVO M�TODO ADICIONADO
    // ======================================================
    // Este m�todo � chamado pelo .NET MAUI toda vez que a p�gina
    // fica vis�vel na tela.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Verificamos se o BindingContext � o nosso ViewModel e, se for,
        // executamos o comando para carregar os chamados.
        if (BindingContext is MainViewModel viewModel)
        {
            viewModel.LoadChamadosCommand.Execute(null);
        }
    }
}