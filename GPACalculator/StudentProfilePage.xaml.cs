using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StudentProfilePage : ContentPage
{
    // Конструктор получает ViewModel
    public StudentProfilePage(StudentProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}