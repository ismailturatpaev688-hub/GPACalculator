using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class HelpPage : ContentPage
{
    // Конструктор получает ViewModel
    public HelpPage(HelpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}