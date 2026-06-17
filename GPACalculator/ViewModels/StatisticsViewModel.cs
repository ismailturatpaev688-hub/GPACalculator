using GPACalculator.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GPACalculator.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    private readonly IDataStorageService _storageService;
    private string _averageGpaText;
    private string _bestSemesterText;
    private double _maxGpa;

    public string AverageGpaText
    {
        get => _averageGpaText;
        set { _averageGpaText = value; OnPropertyChanged(); }
    }

    public string BestSemesterText
    {
        get => _bestSemesterText;
        set { _bestSemesterText = value; OnPropertyChanged(); }
    }

    // Список "столбиков" для графика
    public ObservableCollection<ChartItem> ChartItems { get; } = new();

    public ICommand LoadStatisticsCommand { get; }

    public StatisticsViewModel(IDataStorageService storageService)
    {
        _storageService = storageService;
        LoadStatisticsCommand = new Command(ExecuteLoadStatistics);
    }

    private void ExecuteLoadStatistics()
    {
        var semesters = _storageService.LoadSemesters();
        ChartItems.Clear();

        if (semesters.Count == 0)
        {
            AverageGpaText = "Нет данных";
            BestSemesterText = "Нет данных";
            return;
        }

        // Находим максимальный GPA для масштабирования графика
        _maxGpa = semesters.Max(s => s.Gpa);
        if (_maxGpa == 0) _maxGpa = 1;

        // Считаем средний GPA
        double avg = semesters.Average(s => s.Gpa);
        AverageGpaText = $"Средний GPA: {avg:F2}";

        // Находим лучший семестр
        var best = semesters.OrderByDescending(s => s.Gpa).First();
        BestSemesterText = $"Лучший семестр: {best.Name} (GPA: {best.Gpa:F2})";

        // Создаем "столбики" для графика
        foreach (var sem in semesters)
        {
            ChartItems.Add(new ChartItem
            {
                Name = sem.Name,
                Gpa = sem.Gpa,
                // Высота столбика в процентах от максимального
                HeightPercent = (sem.Gpa / _maxGpa) * 100
            });
        }
    }
}

// Модель для одного столбика графика
public class ChartItem
{
    public string Name { get; set; }
    public double Gpa { get; set; }
    public double HeightPercent { get; set; }
    public double ProgressValue => Gpa / 5.0; // Добавьте это свойство

}