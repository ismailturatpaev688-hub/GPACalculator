using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GPACalculator.Models;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    // ViewModel для страницы расчета цели
    public class TargetCalculatorViewModel : BaseViewModel
    {
        // Сервис для расчета GPA
        private readonly IGpaCalculator _gpaCalculator;
        private readonly IStudentDataService _studentDataService;

        // Поля для хранения данных
        private string _targetGpa = "";
        private string _remainingSubjects = "";
        private string _resultText = "";
        private string _adviceText = "";
        private string _currentGpaText = "";
        private bool _hasResult;
        private Student _selectedStudent;

        // Свойства для привязки к UI
        public string TargetGpa
        {
            get => _targetGpa;
            set
            {
                if (_targetGpa != value)
                {
                    _targetGpa = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RemainingSubjects
        {
            get => _remainingSubjects;
            set
            {
                if (_remainingSubjects != value)
                {
                    _remainingSubjects = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ResultText
        {
            get => _resultText;
            set
            {
                if (_resultText != value)
                {
                    _resultText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AdviceText
        {
            get => _adviceText;
            set
            {
                if (_adviceText != value)
                {
                    _adviceText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentGpaText
        {
            get => _currentGpaText;
            set
            {
                if (_currentGpaText != value)
                {
                    _currentGpaText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasResult
        {
            get => _hasResult;
            set
            {
                if (_hasResult != value)
                {
                    _hasResult = value;
                    OnPropertyChanged();
                }
            }
        }

        // Список всех студентов — для Picker
        public ObservableCollection<Student> Students => _studentDataService.Students;

        // Выбранный студент
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;
                    OnPropertyChanged();
                    // Обновляем текущий GPA при смене студента
                    UpdateCurrentGpa();
                }
            }
        }

        // Команда для расчета
        public ICommand CalculateCommand { get; }

        // Конструктор получает сервис и сервис студентов
        public TargetCalculatorViewModel(IGpaCalculator gpaCalculator, IStudentDataService studentDataService)
        {
            _gpaCalculator = gpaCalculator;
            _studentDataService = studentDataService;

            CalculateCommand = new Command(ExecuteCalculate);

            // Сразу показываем текущий GPA
            UpdateCurrentGpa();
        }

        // Метод расчета необходимого среднего балла
        private void ExecuteCalculate()
        {
            // Проверяем, выбран ли студент
            if (SelectedStudent == null)
            {
                ResultText = "Сначала выберите студента!";
                AdviceText = "";
                HasResult = true;
                return;
            }

            var subjects = SelectedStudent.Subjects;

            if (subjects.Count == 0)
            {
                ResultText = $"У студента '{SelectedStudent.Name}' нет предметов!";
                AdviceText = "Добавьте предметы на главной странице.";
                HasResult = true;
                return;
            }

            if (!double.TryParse(TargetGpa, out double targetGpa) || targetGpa < 0 || targetGpa > 5)
            {
                ResultText = "Ошибка: введите корректный целевой GPA (0-5)";
                AdviceText = "";
                HasResult = true;
                return;
            }

            if (!int.TryParse(RemainingSubjects, out int remaining) || remaining <= 0)
            {
                ResultText = "Ошибка: введите количество оставшихся предметов";
                AdviceText = "";
                HasResult = true;
                return;
            }

            // Считаем текущий GPA выбранного студента
            double currentGpa = _gpaCalculator.CalculateGpa(subjects);

            // Если текущий GPA уже выше целевого
            if (currentGpa >= targetGpa)
            {
                ResultText = $"Поздравляем! Текущий GPA студента ({currentGpa:F2}) уже выше цели ({targetGpa}).";
                AdviceText = "Вам не нужно получать дополнительные оценки. Продолжайте в том же духе!";
                HasResult = true;
                return;
            }

            // Считаем общую сумму весов текущих предметов
            double currentWeightSum = subjects.Sum(s => s.Weight);

            // Считаем сумму взвешенных оценок
            double currentWeightedSum = subjects.Sum(s => s.Grade * s.Weight);

            // Предполагаем что каждый оставшийся предмет имеет средний вес 3
            const double averageRemainingWeight = 3.0;
            double remainingWeightSum = remaining * averageRemainingWeight;

            // Общая сумма весов после всех предметов
            double totalWeightSum = currentWeightSum + remainingWeightSum;

            // Необходимый средний балл за оставшиеся предметы
            double neededAverageGrade = (targetGpa * totalWeightSum - currentWeightedSum) / remainingWeightSum;

            // Расчёт количества пятёрок для достижения цели
            double numerator = targetGpa * currentWeightSum - currentWeightedSum;
            double denominator = (5.0 * averageRemainingWeight) - (targetGpa * averageRemainingWeight);

            int fivesCount = 0;
            bool canReachWithFives = false;

            if (denominator > 0)
            {
                double neededFives = numerator / denominator;
                fivesCount = (int)Math.Ceiling(neededFives);
                canReachWithFives = true;
            }

            // Формируем результат
            if (neededAverageGrade > 5.0)
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                if (canReachWithFives && fivesCount > 0)
                {
                    AdviceText = $"Это невозможно при {remaining} предметах, но если добавить ещё {fivesCount} пятёрок (вес {averageRemainingWeight}), цель будет достигнута.";
                }
                else
                {
                    AdviceText = "Увы, это невозможно! Максимальная оценка 5.0. Попробуйте снизить целевой GPA или добавить больше предметов.";
                }
            }
            else if (neededAverageGrade < 2.0)
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                AdviceText = "Отлично! Вы можете получить даже минимальные оценки и достичь цели. Так держать!";
            }
            else if (neededAverageGrade <= currentGpa)
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                AdviceText = $"Текущий GPA студента ({currentGpa:F2}) уже выше необходимого. Продолжайте в том же духе!";
            }
            else
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                // Добавляем информацию о пятёрках
                if (canReachWithFives && fivesCount > 0)
                {
                    if (fivesCount == 1)
                    {
                        AdviceText = $"Студенту '{SelectedStudent.Name}' нужна 1 пятёрка (вес {averageRemainingWeight}) для достижения цели {targetGpa}. Текущий GPA: {currentGpa:F2}.";
                    }
                    else
                    {
                        AdviceText = $"Студенту '{SelectedStudent.Name}' нужно {fivesCount} пятёрок (вес {averageRemainingWeight}) для достижения цели {targetGpa}. Текущий GPA: {currentGpa:F2}.";
                    }
                }
                else
                {
                    AdviceText = $"Студенту нужно подтянуть успеваемость. Текущий GPA: {currentGpa:F2}. Сосредоточьтесь на предметах с большим весом!";
                }
            }

            HasResult = true;
        }

        // Обновление текста текущего GPA
        private void UpdateCurrentGpa()
        {
            if (SelectedStudent == null)
            {
                CurrentGpaText = "Сначала выберите студента";
                return;
            }

            var subjects = SelectedStudent.Subjects;

            if (subjects.Count == 0)
            {
                CurrentGpaText = $"Студент '{SelectedStudent.Name}': нет предметов";
            }
            else
            {
                double currentGpa = _gpaCalculator.CalculateGpa(subjects);
                CurrentGpaText = $"Студент '{SelectedStudent.Name}': текущий GPA {currentGpa:F2}";
            }
        }
    }
}