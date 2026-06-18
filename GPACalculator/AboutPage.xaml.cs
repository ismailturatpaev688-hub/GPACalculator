using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class AboutPage : ContentPage
{
    // Конструктор получает ViewModel
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}