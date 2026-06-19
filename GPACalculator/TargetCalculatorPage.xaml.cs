using GPACalculator.Services;
using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class TargetCalculatorPage : ContentPage
{
    // Конструктор получает ViewModel через DI
    public TargetCalculatorPage(TargetCalculatorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}