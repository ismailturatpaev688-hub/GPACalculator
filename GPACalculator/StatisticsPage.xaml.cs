using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StatisticsPage : ContentPage
{
    // Конструктор получает ViewModel
    public StatisticsPage(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }

    protected override void OnAppearing()
    {
        // Вызов базового класса
        base.OnAppearing();
        // Проверка типа
        if (BindingContext is StatisticsViewModel vm)
        {
            // Если тип совпадает, то загружаются данные
            vm.LoadStatisticsCommand.Execute(null);
        }
    }
}