using CajuAjuda.Desktop.ViewModels;

namespace CajuAjuda.Desktop.Views;

public partial class MainPage : ContentPage
{
    // O ViewModel é injetado aqui, como antes
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // ======================================================
    //               NOVO MÉTODO ADICIONADO
    // ======================================================
    // Este método é chamado pelo .NET MAUI toda vez que a página
    // fica visível na tela.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Verificamos se o BindingContext é o nosso ViewModel e, se for,
        // executamos o comando para carregar os chamados.
        if (BindingContext is MainViewModel viewModel)
        {
            viewModel.LoadChamadosCommand.Execute(null);
        }
    }
}