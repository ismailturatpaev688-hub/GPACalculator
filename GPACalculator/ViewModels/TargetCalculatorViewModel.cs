using GPACalculator.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    // ViewModel для страницы расчета цели
    public class TargetCalculatorViewModel : BaseViewModel
    {
        // Сервис для расчета GPA
        private readonly IGpaCalculator _gpaCalculator;
        // Ссылка на главную ViewModel для доступа к предметам
        private readonly MainViewModel _mainViewModel;

        // Поля для хранения данных
        private string _targetGpa = "4.5";
        private string _remainingSubjects = "3";
        private string _resultText = "";
        private string _adviceText = "";
        private string _currentGpaText = "";
        private bool _hasResult;

        // Свойства для привязки к UI

        // Желаемый GPA
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

        // Количество оставшихся предметов
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

        // Текст результата расчета
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

        // Совет студенту
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

        // Текст текущего GPA
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

        // Есть ли результат для отображения
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

        // Команда для расчета
        public ICommand CalculateCommand { get; }

        // Конструктор получает сервис и главную ViewModel
        public TargetCalculatorViewModel(IGpaCalculator gpaCalculator, MainViewModel mainViewModel)
        {
            _gpaCalculator = gpaCalculator;
            _mainViewModel = mainViewModel; // Сохраняем ссылку на главную ViewModel

            CalculateCommand = new Command(ExecuteCalculate);

            // Сразу показываем текущий GPA
            UpdateCurrentGpa();
        }

        // Метод расчета необходимого среднего балла
        private void ExecuteCalculate()
        {
            // Получаем предметы из главной ViewModel
            var subjects = _mainViewModel.Subjects;

            // Проверяем что есть предметы
            if (subjects.Count == 0)
            {
                ResultText = "Сначала добавьте предметы на главной странице!";
                AdviceText = "";
                HasResult = true;
                return;
            }

            // Пытаемся прочитать введенные данные
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

            // Считаем текущий GPA
            double currentGpa = _gpaCalculator.CalculateGpa(subjects);

            // Считаем общую сумму весов текущих предметов
            double currentWeightSum = subjects.Sum(s => s.Weight);

            // Считаем сумму взвешенных оценок
            double currentWeightedSum = subjects.Sum(s => s.Grade * s.Weight);

            // Предполагаем что каждый оставшийся предмет имеет средний вес 3
            double remainingWeightSum = remaining * 3.0;

            // Общая сумма весов после всех предметов
            double totalWeightSum = currentWeightSum + remainingWeightSum;

            // Сколько всего баллов нужно набрать для целевого GPA
            double neededTotalWeightedSum = targetGpa * totalWeightSum;

            // Сколько баллов уже есть
            double currentTotalWeightedSum = currentWeightedSum;

            // Сколько баллов нужно набрать за оставшиеся предметы
            double neededRemainingWeightedSum = neededTotalWeightedSum - currentTotalWeightedSum;

            // Необходимый средний балл за оставшиеся предметы
            double neededAverageGrade = neededRemainingWeightedSum / remainingWeightSum;

            // Формируем результат
            if (neededAverageGrade > 5.0)
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                AdviceText = "Увы, это невозможно! Максимальная оценка 5.0. Попробуйте снизить целевой GPA или добавить больше предметов.";
            }
            else if (neededAverageGrade < 2.0)
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                AdviceText = "Отлично! Вы можете получить даже минимальные оценки и достичь цели. Так держать!";
            }
            else if (neededAverageGrade <= currentGpa)
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                AdviceText = $"Ваш текущий GPA ({currentGpa:F2}) уже выше необходимого. Продолжайте в том же духе!";
            }
            else
            {
                ResultText = $"Нужен средний балл: {neededAverageGrade:F2}";
                AdviceText = $"Вам нужно подтянуть успеваемость. Текущий GPA: {currentGpa:F2}. Сосредоточьтесь на предметах с большим весом!";
            }

            HasResult = true;
        }

        // Обновление текста текущего GPA
        private void UpdateCurrentGpa()
        {
            var subjects = _mainViewModel.Subjects;

            if (subjects.Count == 0)
            {
                CurrentGpaText = "Текущий GPA: нет предметов";
            }
            else
            {
                double currentGpa = _gpaCalculator.CalculateGpa(subjects);
                CurrentGpaText = $"Текущий GPA: {currentGpa:F2}";
            }
        }
    }
}
