using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StudentProfilePage : ContentPage
{
    public StudentProfilePage(StudentProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}