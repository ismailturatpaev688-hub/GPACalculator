using GPACalculator.ViewModels;

namespace GPACalculator
{
    public partial class MainPage : ContentPage
    {
        // Конструктор получает ViewModel
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel; // Привязываем ViewModel к странице
        }
    }
}
