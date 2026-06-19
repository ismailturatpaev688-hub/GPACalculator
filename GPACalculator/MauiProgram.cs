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

            // Сервисы (синглтоны — общие для всего приложения)
            builder.Services.AddSingleton<IGpaCalculator, GpaCalculatorService>();
            builder.Services.AddSingleton<ISubjectDataService, SubjectDataService>();
            builder.Services.AddSingleton<IStudentDataService, StudentDataService>();

            // Главная страница (теперь включает оба режима добавления)
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            // Страница статистики
            builder.Services.AddSingleton<StatisticsViewModel>();
            builder.Services.AddSingleton<StatisticsPage>();

            // Страница "О программе"
            builder.Services.AddSingleton<AboutViewModel>();
            builder.Services.AddSingleton<AboutPage>();

            // Страница цели GPA
            builder.Services.AddSingleton<TargetCalculatorViewModel>();
            builder.Services.AddSingleton<TargetCalculatorPage>();

            // Страница помощи
            builder.Services.AddSingleton<HelpViewModel>();
            builder.Services.AddSingleton<HelpPage>();

            // Страница студентов и профиль студента
            builder.Services.AddSingleton<StudentsViewModel>();
            builder.Services.AddSingleton<StudentsPage>();
            builder.Services.AddTransient<StudentProfileViewModel>();
            builder.Services.AddTransient<StudentProfilePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}