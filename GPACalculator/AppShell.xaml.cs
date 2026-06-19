namespace GPACalculator;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Регистрируем маршрут для страницы профиля студента (с параметром name)
        Routing.RegisterRoute("StudentProfilePage", typeof(StudentProfilePage));
    }
}