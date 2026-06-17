using GPACalculator.Models;
using GPACalculator.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    // Теперь мы наследуемся от нашего собственного BaseViewModel.
    // Мы убрали слово "partial", потому что нам больше не нужна генерация кода.
    public class MainViewModel : BaseViewModel
    {
        // Это наш "сервис" (математика). Принцип DIP остается в силе.
        private readonly IGpaCalculator _gpaCalculator;

        // --- ПРИВАТНЫЕ ПОЛЯ (BACKING FIELDS) ---
        // Это переменные, где реально хранятся данные. Они приватные, 
        // потому что экран не должен иметь к ним прямой доступ.
        private string _newSubjectName = "";
        private string _newSubjectGrade = "";
        private string _newSubjectWeight = "";
        private string _gpaResultText = "Введите предметы и нажмите Рассчитать";
        private string _predictionText = "";

        // --- ПУБЛИЧНЫЕ СВОЙСТВА (PROPERTIES) ---
        // Именно к этим свойствам привязывается экран через {Binding} в XAML.
        // Раньше это делал атрибут [ObservableProperty]. Теперь мы пишем это вручную.

        public string NewSubjectName
        {
            // get просто возвращает значение из приватной переменной
            get => _newSubjectName;

            // set вызывается, когда мы в коде пишем NewSubjectName = "Новое значение".
            set
            {
                // Проверяем, действительно ли значение изменилось.
                // Это нужно, чтобы не дергать экран лишний раз, если значение то же самое.
                if (_newSubjectName != value)
                {
                    _newSubjectName = value; // Сохраняем новое значение
                    OnPropertyChanged();     // Кричим экрану: "NewSubjectName изменился, обнови текст!"
                }
            }
        }

        public string NewSubjectGrade
        {
            get => _newSubjectGrade;
            set
            {
                if (_newSubjectGrade != value)
                {
                    _newSubjectGrade = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NewSubjectWeight
        {
            get => _newSubjectWeight;
            set
            {
                if (_newSubjectWeight != value)
                {
                    _newSubjectWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public string GpaResultText
        {
            get => _gpaResultText;
            set
            {
                if (_gpaResultText != value)
                {
                    _gpaResultText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PredictionText
        {
            get => _predictionText;
            set
            {
                if (_predictionText != value)
                {
                    _predictionText = value;
                    OnPropertyChanged();
                }
            }
        }

        // Это наш список предметов. 
        // ObservableCollection - это стандартный класс C#, он сам умеет уведомлять экран,
        // если мы добавили (Add) или удалили (Remove) элемент. Его переделывать не нужно.
        public ObservableCollection<Subject> Subjects { get; } = new();

        // --- КОМАНДЫ (COMMANDS) ---
        // В XAML кнопки привязываются к этим свойствам типа ICommand.
        // Раньше это делал атрибут [RelayCommand]. Теперь мы используем встроенный в MAUI класс Command.

        public ICommand AddSubjectCommand { get; }
        public ICommand CalculateGpaCommand { get; }
        public ICommand PredictGradeCommand { get; }

        // Конструктор. Сюда "внедряется" наш калькулятор.
        public MainViewModel(IGpaCalculator gpaCalculator)
        {
            _gpaCalculator = gpaCalculator;

            // Инициализируем команды. 
            // new Command(Метод) означает: "Когда нажмут кнопку, вызови этот метод".
            // Мы передаем сюда названия методов, которые написаны ниже.
            AddSubjectCommand = new Command(ExecuteAddSubject);
            CalculateGpaCommand = new Command(ExecuteCalculateGpa);
            PredictGradeCommand = new Command(ExecutePredictGrade);
        }

        // --- МЕТОДЫ КОМАНД ---
        // Это методы, которые реально выполняют действия. 
        // Они приватные, потому что вызываются только через команды (кнопки).
        // Я добавил префикс "Execute", чтобы отличать их от самих свойств-команд.

        private void ExecuteAddSubject()
        {
            // Простая проверка: если пользователь не ввел имя, ругаемся и выходим
            if (string.IsNullOrWhiteSpace(NewSubjectName))
            {
                GpaResultText = "Ошибка: Введите название предмета!";
                return;
            }

            // Пытаемся превратить текст из полей ввода в числа (double)
            if (double.TryParse(NewSubjectGrade, out double grade) &&
                double.TryParse(NewSubjectWeight, out double weight))
            {
                // Если всё хорошо, создаем новый предмет и добавляем в список
                var newSubject = new Subject(NewSubjectName, grade, weight);
                Subjects.Add(newSubject);

                // Очищаем поля ввода. 
                // Обратите внимание: мы присваиваем значения СВОЙСТВАМ (с большой буквы),
                // а не приватным полям. Это важно, чтобы сработал сеттер и вызвался OnPropertyChanged!
                NewSubjectName = "";
                NewSubjectGrade = "";
                NewSubjectWeight = "";

                GpaResultText = "Предмет добавлен!";
            }
            else
            {
                // Если ввели буквы вместо цифр
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

            // Вызываем наш сервис.
            double gpa = _gpaCalculator.CalculateGpa(Subjects);

            // Форматируем число до 2 знаков после запятой (F2)
            GpaResultText = $"Ваш текущий GPA: {gpa:F2}";
        }

        private void ExecutePredictGrade()
        {
            if (Subjects.Count == 0)
            {
                PredictionText = "Сначала добавьте хотя бы один предмет.";
                return;
            }

            // Допустим, студент хочет итоговый GPA 4.5, а вес последнего предмета пусть будет 3
            double targetGpa = 4.5;
            double lastWeight = 3.0;

            double neededGrade = _gpaCalculator.PredictNeededGrade(Subjects, targetGpa, lastWeight);

            if (neededGrade > 5.0)
            {
                PredictionText = $"Увы, для GPA {targetGpa} нужна оценка {neededGrade:F2}, а максимум 5.0. Не тянете!";
            }
            else if (neededGrade < 2.0)
            {
                PredictionText = $"Вам достаточно получить {neededGrade:F2}, чтобы иметь GPA {targetGpa}. Вы молодец!";
            }
            else
            {
                PredictionText = $"Для GPA {targetGpa} вам нужно получить {neededGrade:F2} за предмет (вес {lastWeight}).";
            }
        }
    }
}
