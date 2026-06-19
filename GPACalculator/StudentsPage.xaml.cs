using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StudentsPage : ContentPage
{
    // Конструктор получает ViewModel
    public StudentsPage(StudentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Привязываем ViewModel к странице
    }
}