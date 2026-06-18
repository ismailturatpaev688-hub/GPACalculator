using GPACalculator.Models;
using GPACalculator.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Сервисы
        private readonly IGpaCalculator _gpaCalculator;
        private readonly ISubjectDataService _dataService;

        // Приватные поля
        private string _newSubjectName = "";
        private string _newSubjectGrade = "";
        private string _newSubjectWeight = "";
        private string _gpaResultText = "Введите предметы и нажмите Рассчитать";
        private string _predictionText = "";

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

        // Берём коллекцию напрямую из сервиса — она общая для всех страниц
        public ObservableCollection<Subject> Subjects => _dataService.Subjects;

        public ICommand AddSubjectCommand { get; }
        public ICommand CalculateGpaCommand { get; }
        public ICommand PredictGradeCommand { get; }

        public MainViewModel(IGpaCalculator gpaCalculator, ISubjectDataService dataService)
        {
            _gpaCalculator = gpaCalculator;
            _dataService = dataService;

            AddSubjectCommand = new Command(ExecuteAddSubject);
            CalculateGpaCommand = new Command(ExecuteCalculateGpa);
            PredictGradeCommand = new Command(ExecutePredictGrade);
        }

        private void ExecuteAddSubject()
        {
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

                // Сервис сам проверит: есть ли такой предмет. Если есть — допишет оценку.
                _dataService.AddOrUpdate(NewSubjectName, grade, weight);

                NewSubjectName = "";
                NewSubjectGrade = "";
                NewSubjectWeight = "";

                GpaResultText = "Предмет добавлен!";
            }
            else
            {
                GpaResultText = "Ошибка: Оценка и вес должны быть числами!";
            }
        }

        private void ExecuteCalculateGpa()
        {
            if (Subjects.Count == 0)
            {
                GpaResultText = "Список пуст! Добавьте предметы.";
                return;
            }
            double gpa = _gpaCalculator.CalculateGpa(Subjects);
            GpaResultText = $"Ваш текущий GPA: {gpa:F2}";
        }

        private void ExecutePredictGrade()
        {
            if (Subjects.Count == 0)
            {
                PredictionText = "Сначала добавьте хотя бы один предмет.";
                return;
            }
            double targetGpa = 4.5;
            double lastWeight = 3.0;
            double neededGrade = _gpaCalculator.PredictNeededGrade(Subjects, targetGpa, lastWeight);

            if (neededGrade > 5.0)
                PredictionText = $"Увы, для GPA {targetGpa} нужна оценка {neededGrade:F2}, а максимум 5.0!";
            else if (neededGrade < 2.0)
                PredictionText = $"Вам достаточно получить {neededGrade:F2}, чтобы иметь GPA {targetGpa}. Вы молодец!";
            else
                PredictionText = $"Для GPA {targetGpa} вам нужно получить {neededGrade:F2} за предмет (вес {lastWeight}).";
        }
    }
}