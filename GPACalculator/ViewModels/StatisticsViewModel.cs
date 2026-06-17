using GPACalculator.Models;
using GPACalculator.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GPACalculator.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    private readonly IGpaCalculator _gpaCalculator;
    private readonly ObservableCollection<Subject> _subjects;

    // Счетчики оценок
    private int _countFives;
    private int _countFours;
    private int _countThrees;
    private int _countTwos;

    // Текстовые представления
    private string _averageGpaText;
    private string _totalGradesText;
    private string _recommendedGradeText;

    // Список предметов со средними баллами
    public ObservableCollection<SubjectStats> SubjectStatsList { get; } = new();

    // Счетчики оценок
    public int CountFives
    {
        get => _countFives;
        set { _countFives = value; OnPropertyChanged(); }
    }

    public int CountFours
    {
        get => _countFours;
        set { _countFours = value; OnPropertyChanged(); }
    }

    public int CountThrees
    {
        get => _countThrees;
        set { _countThrees = value; OnPropertyChanged(); }
    }

    public int CountTwos
    {
        get => _countTwos;
        set { _countTwos = value; OnPropertyChanged(); }
    }

    public string AverageGpaText
    {
        get => _averageGpaText;
        set { _averageGpaText = value; OnPropertyChanged(); }
    }

    public string TotalGradesText
    {
        get => _totalGradesText;
        set { _totalGradesText = value; OnPropertyChanged(); }
    }

    public string RecommendedGradeText
    {
        get => _recommendedGradeText;
        set { _recommendedGradeText = value; OnPropertyChanged(); }
    }

    public ICommand LoadStatisticsCommand { get; }

    // Конструктор принимает сервис и список предметов с главной страницы
    public StatisticsViewModel(IGpaCalculator gpaCalculator, MainViewModel mainViewModel)
    {
        _gpaCalculator = gpaCalculator;
        _subjects = mainViewModel.Subjects;
        LoadStatisticsCommand = new Command(ExecuteLoadStatistics);
    }

    private void ExecuteLoadStatistics()
    {
        // Очищаем старые данные
        SubjectStatsList.Clear();
        CountFives = 0;
        CountFours = 0;
        CountThrees = 0;
        CountTwos = 0;

        if (_subjects.Count == 0)
        {
            AverageGpaText = "Нет данных";
            TotalGradesText = "Всего оценок: 0";
            RecommendedGradeText = "Добавьте предметы на главной странице";
            return;
        }

        // Считаем общий GPA
        double totalGpa = _gpaCalculator.CalculateGpa(_subjects);
        AverageGpaText = $"Средний балл: {totalGpa:F2}";

        // Считаем распределение оценок
        foreach (var subject in _subjects)
        {
            // Округляем оценку до целого для категоризации
            int grade = (int)Math.Round(subject.Grade);

            switch (grade)
            {
                case 5: CountFives++; break;
                case 4: CountFours++; break;
                case 3: CountThrees++; break;
                case 2: CountTwos++; break;
            }

            // Добавляем статистику по предмету
            SubjectStatsList.Add(new SubjectStats
            {
                Name = subject.Name,
                Grade = subject.Grade,
                Weight = subject.Weight,
                // Формируем строку с распределением оценок (пока заглушка)
                GradeDistribution = $"{subject.Grade:F1}"
            });
        }

        // Общее количество оценок
        int total = CountFives + CountFours + CountThrees + CountTwos;
        TotalGradesText = $"Всего оценок: {total}";

        // Рекомендуемая оценка (простая логика)
        if (totalGpa >= 4.5)
            RecommendedGradeText = "Рекомендуемая оценка: Продолжайте в том же духе!";
        else if (totalGpa >= 3.5)
            RecommendedGradeText = "Рекомендуемая оценка: Подтяните слабые предметы";
        else if (totalGpa >= 2.5)
            RecommendedGradeText = "Рекомендуемая оценка: Нужно серьезно заняться учебой";
        else
            RecommendedGradeText = "Рекомендуемая оценка: Критическая ситуация!";
    }
}

// Модель для статистики по предмету
public class SubjectStats
{
    public string Name { get; set; }
    public double Grade { get; set; }
    public double Weight { get; set; }
    public string GradeDistribution { get; set; }
}