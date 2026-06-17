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

            // 1. Регистрируем наш Сервис (математику) как синглтон.
            // Синглтон означает, что на всё приложение будет создан только ОДИН экземпляр калькулятора.
            // Это экономит память и соответствует принципу DIP.
            builder.Services.AddSingleton<IGpaCalculator, GpaCalculatorService>();
            builder.Services.AddSingleton<IDataStorageService, DataStorageService>();

            // 2. Регистрируем ViewModel. 
            // Когда система будет создавать MainViewModel, она увидит, что ей нужен IGpaCalculator,
            // посмотрит в этот контейнер, найдет там GpaCalculatorService и сама передаст его в конструктор!
            builder.Services.AddSingleton<MainViewModel>();

            // 3. Регистрируем саму страницу (View), чтобы она могла принять ViewModel.
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddSingleton<HistoryViewModel>();
            builder.Services.AddSingleton<HistoryPage>();

            builder.Services.AddSingleton<SettingsViewModel>();
            builder.Services.AddSingleton<SettingsPage>();

            builder.Services.AddSingleton<StatisticsViewModel>();
            builder.Services.AddSingleton<StatisticsPage>();

            builder.Services.AddTransient<SubjectDetailViewModel>();
            builder.Services.AddTransient<SubjectDetailPage>();

            builder.Services.AddSingleton<AboutViewModel>();
            builder.Services.AddSingleton<AboutPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
