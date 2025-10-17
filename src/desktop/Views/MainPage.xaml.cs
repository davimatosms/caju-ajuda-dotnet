using CajuAjuda.Desktop.ViewModels;

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

            // Carrega os dados quando a página aparece
            if (BindingContext is MainViewModel viewModel)
            {
                viewModel.LoadDataCommand.Execute(null);
            }
        }

        // Eventos de clique para as abas (evita problemas de conversão de tipo)
        private void OnTab0Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel viewModel)
            {
                viewModel.SelectTabCommand.Execute(0);
            }
        }

        private void OnTab1Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel viewModel)
            {
                viewModel.SelectTabCommand.Execute(1);
            }
        }

        private void OnTab2Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MainViewModel viewModel)
            {
                viewModel.SelectTabCommand.Execute(2);
            }
        }
    }
}
