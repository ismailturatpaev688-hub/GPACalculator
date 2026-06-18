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

            // Сервесы (синглтоны — общие для всего приложения)
            builder.Services.AddSingleton<IGpaCalculator, GpaCalculatorService>();
            builder.Services.AddSingleton<ISubjectDataService, SubjectDataService>();

            // Главная Страница
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            // Страница Статистики
            builder.Services.AddSingleton<StatisticsViewModel>();
            builder.Services.AddSingleton<StatisticsPage>();

            // Страница о программе
            builder.Services.AddSingleton<AboutViewModel>();
            builder.Services.AddSingleton<AboutPage>();

            // Страница цель GPA
            builder.Services.AddSingleton<TargetCalculatorPage>();

            // Страница Помощь
            builder.Services.AddSingleton<HelpViewModel>();
            builder.Services.AddSingleton<HelpPage>();

            // Страница расчета по одному предмету
            builder.Services.AddSingleton<SingleSubjectCalculatorViewModel>();
            builder.Services.AddSingleton<SingleSubjectCalculatorPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}