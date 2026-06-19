using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GPACalculator.Models;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    public class SingleSubjectCalculatorViewModel : BaseViewModel
    {
        // Сервисы
        private readonly ISubjectDataService _dataService;
        private readonly IStudentDataService _studentDataService;
        private readonly MainViewModel _mainViewModel;

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

        // Список студентов для выбора
        public ObservableCollection<Student> Students => _studentDataService.Students;

        // Выбранный студент
        private Student _selectedStudent;
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set { if (_selectedStudent != value) { _selectedStudent = value; OnPropertyChanged(); } }
        }

        public ICommand AddGradeCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand SaveToMainCommand { get; }
        public ICommand ClearCommand { get; }

        public SingleSubjectCalculatorViewModel(ISubjectDataService dataService,
                                                IStudentDataService studentDataService,
                                                MainViewModel mainViewModel)
        {
            _dataService = dataService;
            _studentDataService = studentDataService;
            _mainViewModel = mainViewModel;

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

        // Отправляем все локальные оценки выбранному студенту
        private void ExecuteSaveToMain()
        {
            if (SelectedStudent == null)
            {
                ResultText = "Сначала выберите студента!";
                return;
            }
            if (Grades.Count == 0) { ResultText = "Нечего сохранять — добавьте оценки!"; return; }
            if (string.IsNullOrWhiteSpace(SubjectName)) { ResultText = "Введите название предмета!"; return; }
            if (!double.TryParse(Weight, out double w) || w < 1 || w > 5)
            {
                ResultText = "Вес должен быть числом от 1 до 5!";
                return;
            }

            // Добавляем все оценки к предмету выбранного студента
            var grades = Grades.Select(g => g.Grade).ToList();
            AddGradesToStudent(SelectedStudent, SubjectName, grades, w);

            ResultText = $"Сохранено студенту '{SelectedStudent.Name}'! Оценок передано: {grades.Count}";
            Grades.Clear();
        }

        // Вспомогательный метод: добавляет несколько оценок к предмету студента
        private void AddGradesToStudent(Student student, string name, System.Collections.Generic.IEnumerable<double> grades, double weight)
        {
            name = name.Trim();
            var existing = student.Subjects.FirstOrDefault(s =>
                s.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                foreach (var g in grades) existing.AddGrade(g);
            }
            else
            {
                var subject = new Subject(name, weight);
                foreach (var g in grades) subject.AddGrade(g);
                student.Subjects.Add(subject);
            }
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