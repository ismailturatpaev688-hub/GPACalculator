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
            const double subjectWeight = 3.0;
            double currentGpa = _gpaCalculator.CalculateGpa(Subjects);

            if (currentGpa >= targetGpa)
            {
                PredictionText = $"Поздравляем! Ваш текущий GPA ({currentGpa:F2}) уже выше цели ({targetGpa}). Пятёрки не нужны!";
                return;
            }

            // Считаем текущую взвешенную сумму и сумму весов
            double currentWeightedSum = Subjects.Sum(s => s.Grade * s.Weight);
            double currentWeightSum = Subjects.Sum(s => s.Weight);

            // Расчёта количества пятёрок
            double numerator = targetGpa * currentWeightSum - currentWeightedSum;
            double denominator = (5.0 * subjectWeight) - (targetGpa * subjectWeight);

            // Проверяем, можно ли вообще достичь цели (знаменатель должен быть положительным)
            if (denominator <= 0)
            {
                PredictionText = $"Невозможно достичь GPA {targetGpa} даже с бесконечным количеством пятёрок.";
                return;
            }

            // Считаем количество пятёрок и округляем вверх до целого
            double neededFives = numerator / denominator;
            int fivesCount = (int)Math.Ceiling(neededFives);

            // Проверяем, не слишком ли много пятёрок нужно (больше 20 — нереалистично)
            if (fivesCount > 20)
            {
                PredictionText = $"Для GPA {targetGpa} нужно слишком много пятёрок ({fivesCount}). Попробуйте снизить цель.";
                return;
            }

            // Формируем итоговое сообщение
            if (fivesCount == 1)
            {
                PredictionText = $"Вам нужна 1 пятёрка для достижения GPA {targetGpa}.";
            }
            else
            {
                PredictionText = $"Вам нужно {fivesCount} пятёрок для достижения GPA {targetGpa}.";
            }
        }
    }
}