using GPACalculator.Services;
using GPACalculator.ViewModels;
using Microsoft.Extensions.Logging;

namespace GPACalculator
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Регистрируем сервисы (синглтоны - один экземпляр на все приложение)
            builder.Services.AddSingleton<IGpaCalculator, GpaCalculatorService>();

            // Главная страница и ViewModel
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<StatisticsViewModel>(sp =>
            {
                var calculator = sp.GetRequiredService<IGpaCalculator>();
                var mainViewModel = sp.GetRequiredService<MainViewModel>();
                return new StatisticsViewModel(calculator, mainViewModel);
            });
            builder.Services.AddSingleton<StatisticsPage>();

            // Страница деталей предмета (создается новая каждый раз)
            builder.Services.AddTransient<SubjectDetailViewModel>();
            builder.Services.AddTransient<SubjectDetailPage>();

            // Страница "О программе"
            builder.Services.AddSingleton<AboutViewModel>();
            builder.Services.AddSingleton<AboutPage>();

            // НОВАЯ: Страница расчета целевого GPA
            // TargetCalculatorViewModel создается вручную с передачей MainViewModel
            builder.Services.AddSingleton<TargetCalculatorPage>();

            // НОВАЯ: Страница помощи
            builder.Services.AddSingleton<HelpViewModel>();
            builder.Services.AddSingleton<HelpPage>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
