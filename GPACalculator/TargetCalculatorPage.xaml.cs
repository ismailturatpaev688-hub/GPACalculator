using GPACalculator.Services;
using GPACalculator.ViewModels;

namespace GPACalculator;

// Страница расчета целевого GPA
public partial class TargetCalculatorPage : ContentPage
{
    // Конструктор создает ViewModel вручную с нужными зависимостями
    public TargetCalculatorPage(MainViewModel mainViewModel, IGpaCalculator gpaCalculator)
    {
        InitializeComponent();

        // Создаем ViewModel вручную, передавая нужные зависимости
        var viewModel = new TargetCalculatorViewModel(gpaCalculator, mainViewModel);
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}