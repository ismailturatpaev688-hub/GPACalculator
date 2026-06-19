using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StudentsPage : ContentPage
{
    public StudentsPage(StudentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}