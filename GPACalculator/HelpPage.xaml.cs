using GPACalculator.ViewModels;

namespace GPACalculator;

// Страница помощи и инструкций
public partial class HelpPage : ContentPage
{
    // Конструктор получает ViewModel
    public HelpPage(HelpViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Устанавливаем контекст
    }
}