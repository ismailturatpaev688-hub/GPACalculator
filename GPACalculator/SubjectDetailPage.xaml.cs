using GPACalculator.Models;
using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class SubjectDetailPage : ContentPage
{
    public SubjectDetailPage(SubjectDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Метод для передачи предмета на страницу
    public void SetSubject(Subject subject)
    {
        if (BindingContext is SubjectDetailViewModel vm)
        {
            vm.LoadSubject(subject);
        }
    }
}