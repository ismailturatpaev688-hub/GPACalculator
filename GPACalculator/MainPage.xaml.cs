using GPACalculator.ViewModels;

namespace GPACalculator
{
    public partial class MainPage : ContentPage
    {
        // Конструктор страницы. Сюда тоже внедряется ViewModel (принцип DIP).
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            // Устанавливаем ViewModel как "Контекст данных" для этой страницы.
            // Теперь все {Binding} в XAML будут искать свойства именно в этом viewModel.
            BindingContext = viewModel;
        }
    }
}
