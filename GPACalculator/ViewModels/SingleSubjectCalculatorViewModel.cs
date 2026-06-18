using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    public class SingleSubjectCalculatorViewModel : BaseViewModel
    {
        // Сервис
        private readonly ISubjectDataService _dataService;
        // Приватные поля
        private string _subjectName = "";
        private string _newGrade = "";
        private string _weight = ""; 
        private string _resultText = "Добавьте оценки для расчета";
        private double _averageGrade;

        public ObservableCollection<GradeEntry> Grades { get; } = new();

        public string SubjectName
        {
            get => _subjectName;
            set { if (_subjectName != value) { _subjectName = value; OnPropertyChanged(); } }
        }
        public string NewGrade
        {
            get => _newGrade;
            set { if (_newGrade != value) { _newGrade = value; OnPropertyChanged(); } }
        }
        public string Weight
        {
            get => _weight;
            set { if (_weight != value) { _weight = value; OnPropertyChanged(); } }
        }
        public string ResultText
        {
            get => _resultText;
            set { if (_resultText != value) { _resultText = value; OnPropertyChanged(); } }
        }
        public double AverageGrade
        {
            get => _averageGrade;
            set { if (_averageGrade != value) { _averageGrade = value; OnPropertyChanged(); } }
        }

        // Команды: Добавить, расчитать, сихронизация, очистить
        public ICommand AddGradeCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand SaveToMainCommand { get; }
        public ICommand ClearCommand { get; }

        public SingleSubjectCalculatorViewModel(ISubjectDataService dataService)
        {
            _dataService = dataService;

            AddGradeCommand = new Command(ExecuteAddGrade);
            CalculateCommand = new Command(ExecuteCalculate);
            SaveToMainCommand = new Command(ExecuteSaveToMain);
            ClearCommand = new Command(ExecuteClear);
        }

        private void ExecuteAddGrade()
        {
            if (double.TryParse(NewGrade, out double grade))
            {
                if (grade >= 2 && grade <= 5)
                {
                    Grades.Add(new GradeEntry { Grade = grade, DisplayText = grade.ToString("F1") });
                    NewGrade = "";
                    ResultText = $"Локально оценок: {Grades.Count}";
                }
                else ResultText = "Ошибка: оценка должна быть от 2 до 5";
            }
            else ResultText = "Ошибка: введите корректное число";
        }

        private void ExecuteCalculate()
        {
            if (Grades.Count == 0) { ResultText = "Добавьте хотя бы одну оценку!"; return; }

            AverageGrade = Grades.Sum(g => g.Grade) / Grades.Count;
            string gradeText = AverageGrade switch
            {
                >= 4.5 => "Отлично!",
                >= 3.5 => "Хорошо",
                >= 2.5 => "Удовлетворительно",
                _ => "Нужно подтянуть!"
            };
            ResultText = $"Средний балл по \"{SubjectName}\": {AverageGrade:F2}\n{gradeText}";
        }

        // Отправляем все локальные оценки в общее хранилище
        private void ExecuteSaveToMain()
        {
            if (Grades.Count == 0) { ResultText = "Нечего сохранять — добавьте оценки!"; return; }
            if (string.IsNullOrWhiteSpace(SubjectName)) { ResultText = "Введите название предмета!"; return; }
            if (!double.TryParse(Weight, out double w) || w < 1 || w > 5)
            {
                ResultText = "Вес должен быть числом от 1 до 5!";
                return;
            }

            // Передаём все оценки в сервис. Он сам объединит с существующим предметом, если такой есть.
            var grades = Grades.Select(g => g.Grade).ToList();
            _dataService.AddOrUpdate(SubjectName, grades, w);

            ResultText = $"Сохранено в общий список! Оценок передано: {grades.Count}";

            // Очищаем локальный список, чтобы при повторном сохранении не задвоить
            Grades.Clear();
        }

        private void ExecuteClear()
        {
            Grades.Clear();
            SubjectName = "";
            NewGrade = "";
            Weight = "";
            ResultText = "Добавьте оценки для расчета";
            AverageGrade = 0;
        }
    }

    public class GradeEntry
    {
        public double Grade { get; set; }
        public string DisplayText { get; set; }
    }
}