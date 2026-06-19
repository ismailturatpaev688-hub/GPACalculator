using GPACalculator.Models;
using GPACalculator.Services;
using System;
using System.Collections.Generic;
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

        // ===== Поля для БЫСТРОГО добавления (один предмет + одна оценка) =====
        private string _newSubjectName = "";
        private string _newSubjectGrade = "";
        private string _newSubjectWeight = "";

        // ===== Поля для ПАКЕТНОГО добавления (несколько оценок к одному предмету) =====
        private string _batchSubjectName = "";
        private string _batchWeight = "";
        private string _batchNewGrade = "";

        // ===== Общие поля =====
        private string _gpaResultText = "Введите предметы и нажмите Рассчитать";
        private Student _selectedStudent;

        // Локальный список оценок для пакетного режима (до сохранения)
        public ObservableCollection<GradeEntry> BatchGrades { get; } = new();

        // --- Свойства для быстрого добавления ---
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

        // --- Свойства для пакетного добавления ---
        public string BatchSubjectName
        {
            get => _batchSubjectName;
            set { if (_batchSubjectName != value) { _batchSubjectName = value; OnPropertyChanged(); } }
        }
        public string BatchWeight
        {
            get => _batchWeight;
            set { if (_batchWeight != value) { _batchWeight = value; OnPropertyChanged(); } }
        }
        public string BatchNewGrade
        {
            get => _batchNewGrade;
            set { if (_batchNewGrade != value) { _batchNewGrade = value; OnPropertyChanged(); } }
        }

        // --- Общие свойства ---
        public string GpaResultText
        {
            get => _gpaResultText;
            set { if (_gpaResultText != value) { _gpaResultText = value; OnPropertyChanged(); } }
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

        // Предметы выбранного студента
        public ObservableCollection<Subject> Subjects =>
            SelectedStudent?.Subjects ?? new ObservableCollection<Subject>();

        // ===== Команды =====
        // Быстрое добавление
        public ICommand AddSubjectCommand { get; }
        // Пакетное добавление
        public ICommand AddBatchGradeCommand { get; }
        public ICommand SaveBatchCommand { get; }
        public ICommand ClearBatchCommand { get; }
        // Расчёт
        public ICommand CalculateGpaCommand { get; }

        public MainViewModel(IGpaCalculator gpaCalculator,
                             ISubjectDataService dataService,
                             IStudentDataService studentDataService)
        {
            _gpaCalculator = gpaCalculator;
            _dataService = dataService;
            _studentDataService = studentDataService;

            // Быстрое добавление
            AddSubjectCommand = new Command(ExecuteAddSubject);
            // Пакетное добавление
            AddBatchGradeCommand = new Command(ExecuteAddBatchGrade);
            SaveBatchCommand = new Command(ExecuteSaveBatch);
            ClearBatchCommand = new Command(ExecuteClearBatch);
            // Расчёт
            CalculateGpaCommand = new Command(ExecuteCalculateGpa);
        }

        // ========== БЫСТРОЕ ДОБАВЛЕНИЕ ==========

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
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

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

        // ========== ПАКЕТНОЕ ДОБАВЛЕНИЕ ==========

        // Добавление одной оценки в локальный список
        private void ExecuteAddBatchGrade()
        {
            if (double.TryParse(BatchNewGrade, out double grade))
            {
                if (grade >= 2 && grade <= 5)
                {
                    BatchGrades.Add(new GradeEntry { Grade = grade, DisplayText = grade.ToString("F1") });
                    BatchNewGrade = "";
                }
                else
                {
                    GpaResultText = "Ошибка: оценка должна быть от 2 до 5";
                }
            }
            else
            {
                GpaResultText = "Ошибка: введите корректное число";
            }
        }

        // Сохранение всех локальных оценок выбранному студенту
        private void ExecuteSaveBatch()
        {
            if (SelectedStudent == null)
            {
                GpaResultText = "Сначала выберите студента!";
                return;
            }
            if (BatchGrades.Count == 0)
            {
                GpaResultText = "Нечего сохранять — добавьте хотя бы одну оценку!";
                return;
            }
            if (string.IsNullOrWhiteSpace(BatchSubjectName))
            {
                GpaResultText = "Введите название предмета!";
                return;
            }
            if (!double.TryParse(BatchWeight, out double w) || w < 1 || w > 5)
            {
                GpaResultText = "Вес должен быть числом от 1 до 5!";
                return;
            }

            // Добавляем все оценки к предмету выбранного студента
            var grades = new List<double>();
            foreach (var g in BatchGrades) grades.Add(g.Grade);
            AddGradesToStudent(SelectedStudent, BatchSubjectName, grades, w);

            GpaResultText = $"Сохранено студенту '{SelectedStudent.Name}'! Оценок передано: {grades.Count}";

            // Очищаем локальный список и поля
            ExecuteClearBatch();
        }

        // Вспомогательный метод: добавляет несколько оценок к предмету студента
        private void AddGradesToStudent(Student student, string name, IEnumerable<double> grades, double weight)
        {
            name = name.Trim();
            var existing = student.Subjects.FirstOrDefault(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

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

        // Очистка локального списка оценок
        private void ExecuteClearBatch()
        {
            BatchGrades.Clear();
            BatchSubjectName = "";
            BatchNewGrade = "";
            BatchWeight = "";
        }

        // ========== РАСЧЁТ GPA ==========

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
    }

    // Модель для одной оценки (используется в пакетном режиме)
    public class GradeEntry
    {
        public double Grade { get; set; }
        public string DisplayText { get; set; }
    }
}