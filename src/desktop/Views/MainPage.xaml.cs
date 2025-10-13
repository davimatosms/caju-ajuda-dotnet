using CajuAjuda.Desktop.ViewModels;

// Certifique-se de que não há 'using CommunityToolkit.Maui.Views;' aqui.
namespace CajuAjuda.Desktop.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainViewModel viewModel && viewModel.LoadDataCommand.CanExecute(null))
            {
                viewModel.LoadDataCommand.Execute(null);
            }
        }
    }
}