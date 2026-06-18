using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GPACalculator.Models;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        // Сервисы
        private readonly IGpaCalculator _gpaCalculator;
        private readonly ISubjectDataService _dataService;

        // Приватные поля
        private int _countFives, _countFours, _countThrees, _countTwos;
        private string _averageGpaText, _totalGradesText, _recommendedGradeText;
        private string _bestSubjectText, _worstSubjectText, _totalWeightText;
        private double _progressFives, _progressFours, _progressThrees, _progressTwos;
        private string _textFives, _textFours, _textThrees, _textTwos;

        // Публичные свойства
        // Уведомляет об изменение элементов
        public ObservableCollection<SubjectStats> SubjectStatsList { get; } = new();

        public int CountFives { get => _countFives; set { _countFives = value; OnPropertyChanged(); } }
        public int CountFours { get => _countFours; set { _countFours = value; OnPropertyChanged(); } }
        public int CountThrees { get => _countThrees; set { _countThrees = value; OnPropertyChanged(); } }
        public int CountTwos { get => _countTwos; set { _countTwos = value; OnPropertyChanged(); } }

        public string AverageGpaText { get => _averageGpaText; set { _averageGpaText = value; OnPropertyChanged(); } }
        public string TotalGradesText { get => _totalGradesText; set { _totalGradesText = value; OnPropertyChanged(); } }
        public string RecommendedGradeText { get => _recommendedGradeText; set { _recommendedGradeText = value; OnPropertyChanged(); } }
        public string BestSubjectText { get => _bestSubjectText; set { _bestSubjectText = value; OnPropertyChanged(); } }
        public string WorstSubjectText { get => _worstSubjectText; set { _worstSubjectText = value; OnPropertyChanged(); } }
        public string TotalWeightText { get => _totalWeightText; set { _totalWeightText = value; OnPropertyChanged(); } }

        public double ProgressFives { get => _progressFives; set { _progressFives = value; OnPropertyChanged(); } }
        public double ProgressFours { get => _progressFours; set { _progressFours = value; OnPropertyChanged(); } }
        public double ProgressThrees { get => _progressThrees; set { _progressThrees = value; OnPropertyChanged(); } }
        public double ProgressTwos { get => _progressTwos; set { _progressTwos = value; OnPropertyChanged(); } }

        public string TextFives { get => _textFives; set { _textFives = value; OnPropertyChanged(); } }
        public string TextFours { get => _textFours; set { _textFours = value; OnPropertyChanged(); } }
        public string TextThrees { get => _textThrees; set { _textThrees = value; OnPropertyChanged(); } }
        public string TextTwos { get => _textTwos; set { _textTwos = value; OnPropertyChanged(); } }

        public ICommand LoadStatisticsCommand { get; }


        // Сохраняет данные и использует команду ExecuteLoadStatistics
        public StatisticsViewModel(IGpaCalculator gpaCalculator, ISubjectDataService dataService)
        {
            _gpaCalculator = gpaCalculator;
            _dataService = dataService;
            LoadStatisticsCommand = new Command(ExecuteLoadStatistics);
        }

        private void ExecuteLoadStatistics()
        {
            // Если равен нулю то выводит сообщение: Нет данных и т.д
            SubjectStatsList.Clear();
            CountFives = CountFours = CountThrees = CountTwos = 0;

            if (_dataService.Subjects.Count == 0)
            {
                AverageGpaText = "Нет данных";
                TotalGradesText = "Всего оценок: 0";
                RecommendedGradeText = "Добавьте предметы на главной странице";
                BestSubjectText = WorstSubjectText = "—";
                TotalWeightText = "Общий вес: 0";
                return;
            }

            double totalGpa = _gpaCalculator.CalculateGpa(_dataService.Subjects);
            AverageGpaText = $"Средний балл: {totalGpa:F2}";

            double totalWeight = 0;
            Subject best = null, worst = null;

            foreach (var subject in _dataService.Subjects)
            {
                // Накапливает общий вес
                totalWeight += subject.Weight;
                // Ищет максимальную оценку
                if (best == null || subject.Grade > best.Grade) best = subject;
                // Ищет минимальную оценку
                if (worst == null || subject.Grade < worst.Grade) worst = subject;

                // Считаем оценку в распределении
                foreach (var g in subject.Grades)
                {
                    int rounded = (int)Math.Round(g, MidpointRounding.AwayFromZero);
                    switch (rounded)
                    {
                        case 5: CountFives++; break;
                        case 4: CountFours++; break;
                        case 3: CountThrees++; break;
                        case 2: CountTwos++; break;
                    }
                }

                // Формат данных: имя, все оценки, средний балл
                SubjectStatsList.Add(new SubjectStats
                {
                    Name = subject.Name,
                    GradesText = subject.GradesText,
                    AverageGrade = subject.Grade,
                    Weight = subject.Weight
                });
            }

            int total = CountFives + CountFours + CountThrees + CountTwos;
            TotalGradesText = $"Всего оценок: {total}";
            TotalWeightText = $"Общая учебная нагрузка: {totalWeight} весов";

            // Почет прогресса по разным оценкам
            CalculateProgress(CountFives, total, ref _progressFives, ref _textFives);
            CalculateProgress(CountFours, total, ref _progressFours, ref _textFours);
            CalculateProgress(CountThrees, total, ref _progressThrees, ref _textThrees);
            CalculateProgress(CountTwos, total, ref _progressTwos, ref _textTwos);

            // Если модель данных изменилось, то уведомляет об этом
            OnPropertyChanged(nameof(ProgressFives)); OnPropertyChanged(nameof(TextFives));
            OnPropertyChanged(nameof(ProgressFours)); OnPropertyChanged(nameof(TextFours));
            OnPropertyChanged(nameof(ProgressThrees)); OnPropertyChanged(nameof(TextThrees));
            OnPropertyChanged(nameof(ProgressTwos)); OnPropertyChanged(nameof(TextTwos));

            BestSubjectText = $"{best.Name} ({best.Grade:F2})";
            WorstSubjectText = $"{worst.Name} ({worst.Grade:F2})";

            GenerateSmartRecommendation(totalGpa, worst);
        }

        // Калькулятор прогресса
        private void CalculateProgress(int count, int total, ref double progress, ref string text)
        {
            if (total > 0)
            {
                progress = (double)count / total;
                text = $"{count} шт. ({(int)Math.Round(progress * 100)}%)";
            }
            else
            {
                progress = 0;
                text = "0 шт. (0%)";
            }
        }

        // Оценка среднего балла от 2 до 4,75
        private void GenerateSmartRecommendation(double gpa, Subject worst)
        {
            if (gpa >= 4.75)
                RecommendedGradeText = "Вы — отличник! Так держать, вы на пути к красному диплому.";
            else if (gpa >= 4.0)
                RecommendedGradeText = $"Хороший результат! Чтобы стать отличником, подтяните '{worst.Name}'.";
            else if (gpa >= 3.0)
                RecommendedGradeText = $"Средний уровень. Сосредоточьтесь на '{worst.Name}', чтобы повысить общий балл.";
            else
                RecommendedGradeText = $"Критическая ситуация! Срочно уделите внимание предмету '{worst.Name}'.";
        }
    }

    // Модель для статистики по предмету
    public class SubjectStats
    {
        public string Name { get; set; }
        public string GradesText { get; set; }
        public double AverageGrade { get; set; }
        public double Weight { get; set; }
    }
}