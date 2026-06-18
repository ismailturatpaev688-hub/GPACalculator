using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class SingleSubjectCalculatorPage : ContentPage
{
	public SingleSubjectCalculatorPage(SingleSubjectCalculatorViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}