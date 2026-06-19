using GPACalculator.Services;
using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class TargetCalculatorPage : ContentPage
{
    // Конструктор получает ViewModel
    public TargetCalculatorPage(TargetCalculatorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}