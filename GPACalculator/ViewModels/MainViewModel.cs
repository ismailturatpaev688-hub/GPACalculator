using GPACalculator.Models;
using GPACalculator.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Сервисы
        private readonly IGpaCalculator _gpaCalculator;
        private readonly ISubjectDataService _dataService;
        private readonly IStudentDataService _studentDataService;

        // Приватные поля
        private string _newSubjectName = "";
        private string _newSubjectGrade = "";
        private string _newSubjectWeight = "";
        private string _gpaResultText = "Введите предметы и нажмите Рассчитать";
        private string _predictionText = "";
        private Student _selectedStudent;

        public string NewSubjectName
        {
            get => _newSubjectName;
            set { if (_newSubjectName != value) { _newSubjectName = value; OnPropertyChanged(); } }
        }
        public string NewSubjectGrade
        {
            get => _newSubjectGrade;
            set { if (_newSubjectGrade != value) { _newSubjectGrade = value; OnPropertyChanged(); } }
        }
        public string NewSubjectWeight
        {
            get => _newSubjectWeight;
            set { if (_newSubjectWeight != value) { _newSubjectWeight = value; OnPropertyChanged(); } }
        }
        public string GpaResultText
        {
            get => _gpaResultText;
            set { if (_gpaResultText != value) { _gpaResultText = value; OnPropertyChanged(); } }
        }
        public string PredictionText
        {
            get => _predictionText;
            set { if (_predictionText != value) { _predictionText = value; OnPropertyChanged(); } }
        }

        // Выбранный студент — к нему добавляются предметы
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;
                    OnPropertyChanged();
                    // Уведомляем UI, что Subjects изменились (они теперь от другого студента)
                    OnPropertyChanged(nameof(Subjects));
                    // Пересчитываем результат, если он был
                    if (Subjects.Count > 0) ExecuteCalculateGpa();
                    else GpaResultText = "Введите предметы и нажмите Рассчитать";
                }
            }
        }

        // Список всех студентов — для Picker
        public ObservableCollection<Student> Students => _studentDataService.Students;

        // Предметы выбранного студента (если студент не выбран — пустой список)
        public ObservableCollection<Subject> Subjects =>
            SelectedStudent?.Subjects ?? new ObservableCollection<Subject>();

        public ICommand AddSubjectCommand { get; }
        public ICommand CalculateGpaCommand { get; }
        public ICommand PredictGradeCommand { get; }

        public MainViewModel(IGpaCalculator gpaCalculator,
                             ISubjectDataService dataService,
                             IStudentDataService studentDataService)
        {
            _gpaCalculator = gpaCalculator;
            _dataService = dataService;
            _studentDataService = studentDataService;

            AddSubjectCommand = new Command(ExecuteAddSubject);
            CalculateGpaCommand = new Command(ExecuteCalculateGpa);
            PredictGradeCommand = new Command(ExecutePredictGrade);
        }

        private void ExecuteAddSubject()
        {
            // Проверяем, выбран ли студент
            if (SelectedStudent == null)
            {
                GpaResultText = "Ошибка: Сначала выберите студента!";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewSubjectName))
            {
                GpaResultText = "Ошибка: Введите название предмета!";
                return;
            }

            if (double.TryParse(NewSubjectGrade, out double grade) &&
                double.TryParse(NewSubjectWeight, out double weight))
            {
                if (grade < 2.0 || grade > 5.0)
                {
                    GpaResultText = "Ошибка: Оценка должна быть числом от 2 до 5!";
                    return;
                }
                if (weight < 1.0 || weight > 5.0)
                {
                    GpaResultText = "Ошибка: Вес предмета должен быть числом от 1 до 5!";
                    return;
                }

                // Добавляем предмет к ВЫБРАННОМУ студенту
                // Используем вспомогательный метод, который ищет предмет в Subjects студента
                AddSubjectToStudent(SelectedStudent, NewSubjectName, grade, weight);

                NewSubjectName = "";
                NewSubjectGrade = "";
                NewSubjectWeight = "";

                GpaResultText = $"Предмет добавлен студенту '{SelectedStudent.Name}'!";
            }
            else
            {
                GpaResultText = "Ошибка: Оценка и вес должны быть числами!";
            }
        }

        // Вспомогательный метод: добавляет предмет к студенту (или дописывает оценку, если предмет уже есть)
        private void AddSubjectToStudent(Student student, string name, double grade, double weight)
        {
            name = name.Trim();
            // Ищем предмет у этого студента
            var existing = student.Subjects.FirstOrDefault(s =>
                s.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                // Предмет уже есть — дописываем оценку
                existing.AddGrade(grade);
            }
            else
            {
                // Создаём новый предмет
                var subject = new Subject(name, weight);
                subject.AddGrade(grade);
                student.Subjects.Add(subject);
            }
        }

        private void ExecuteCalculateGpa()
        {
            if (SelectedStudent == null)
            {
                GpaResultText = "Сначала выберите студента!";
                return;
            }
            if (Subjects.Count == 0)
            {
                GpaResultText = $"У студента '{SelectedStudent.Name}' нет предметов.";
                return;
            }
            double gpa = _gpaCalculator.CalculateGpa(Subjects);
            GpaResultText = $"GPA студента '{SelectedStudent.Name}': {gpa:F2}";
        }

        private void ExecutePredictGrade()
        {
            if (SelectedStudent == null)
            {
                PredictionText = "Сначала выберите студента!";
                return;
            }
            if (Subjects.Count == 0)
            {
                PredictionText = $"Сначала добавьте предметы студенту '{SelectedStudent.Name}'.";
                return;
            }

            double targetGpa = 4.5;
            const double subjectWeight = 3.0;
            double currentGpa = _gpaCalculator.CalculateGpa(Subjects);

            if (currentGpa >= targetGpa)
            {
                PredictionText = $"Поздравляем! Текущий GPA студента ({currentGpa:F2}) уже выше цели ({targetGpa}). Пятёрки не нужны!";
                return;
            }

            double currentWeightedSum = Subjects.Sum(s => s.Grade * s.Weight);
            double currentWeightSum = Subjects.Sum(s => s.Weight);

            double numerator = targetGpa * currentWeightSum - currentWeightedSum;
            double denominator = (5.0 * subjectWeight) - (targetGpa * subjectWeight);

            if (denominator <= 0)
            {
                PredictionText = $"Невозможно достичь GPA {targetGpa} даже с бесконечным количеством пятёрок.";
                return;
            }

            double neededFives = numerator / denominator;
            int fivesCount = (int)System.Math.Ceiling(neededFives);

            if (fivesCount > 20)
            {
                PredictionText = $"Для GPA {targetGpa} нужно слишком много пятёрок ({fivesCount}). Попробуйте снизить цель.";
                return;
            }

            if (fivesCount == 1)
                PredictionText = $"Студенту '{SelectedStudent.Name}' нужна 1 пятёрка для достижения GPA {targetGpa}.";
            else
                PredictionText = $"Студенту '{SelectedStudent.Name}' нужно {fivesCount} пятёрок для достижения GPA {targetGpa}.";
        }
    }
}