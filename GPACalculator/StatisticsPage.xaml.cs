using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StatisticsPage : ContentPage
{
    public StatisticsPage(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StatisticsViewModel vm)
        {
            vm.LoadStatisticsCommand.Execute(null);
        }
    }
}