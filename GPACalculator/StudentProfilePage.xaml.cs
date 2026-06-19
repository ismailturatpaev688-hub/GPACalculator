using GPACalculator.ViewModels;

namespace GPACalculator;

public partial class StudentProfilePage : ContentPage
{
    public StudentProfilePage(StudentProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Обработчик для кнопки "Был" — устанавливает IsPresent = true перед выполнением команды
    private void OnMarkPresent(object sender, EventArgs e)
    {
        if (BindingContext is StudentProfileViewModel vm)
            vm.AttendanceIsPresent = true;
    }

    // Обработчик для кнопки "Не был" — устанавливает IsPresent = false
    private void OnMarkAbsent(object sender, EventArgs e)
    {
        if (BindingContext is StudentProfileViewModel vm)
            vm.AttendanceIsPresent = false;
    }
}