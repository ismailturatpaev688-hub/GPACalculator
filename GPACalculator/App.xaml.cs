using Microsoft.Extensions.DependencyInjection;

namespace GPACalculator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }
 
        protected override Window CreateWindow(IActivationState? activationState) // Он передаёт информацию об активации
        {
            // Создаёт экземпляр класса Window
            return new Window(new AppShell());
        }
    }
}