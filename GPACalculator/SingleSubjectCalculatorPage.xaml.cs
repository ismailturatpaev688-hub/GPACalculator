using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class SingleSubjectCalculatorPage : ContentPage
{
    // Конструктор получает ViewModel
    public SingleSubjectCalculatorPage(SingleSubjectCalculatorViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}