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
        private readonly IStudentDataService _studentDataService;

        // Приватные поля
        private int _countFives, _countFours, _countThrees, _countTwos;
        private string _averageGpaText, _totalGradesText, _recommendedGradeText;
        private string _bestSubjectText, _worstSubjectText, _totalWeightText;
        private double _progressFives, _progressFours, _progressThrees, _progressTwos;
        private string _textFives, _textFours, _textThrees, _textTwos;

        // Поля для посещаемости и студентов
        private string _attendanceText;
        private double _attendanceProgress;
        private string _studentsCountText;
        private string _totalDebtsText;

        // Выбранный студент (null = смотреть всех)
        private Student _selectedStudent;

        // Публичные свойства
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

        public string AttendanceText { get => _attendanceText; set { _attendanceText = value; OnPropertyChanged(); } }
        public double AttendanceProgress { get => _attendanceProgress; set { _attendanceProgress = value; OnPropertyChanged(); } }
        public string StudentsCountText { get => _studentsCountText; set { _studentsCountText = value; OnPropertyChanged(); } }
        public string TotalDebtsText { get => _totalDebtsText; set { _totalDebtsText = value; OnPropertyChanged(); } }

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
                    OnPropertyChanged(nameof(IsStudentSelected));
                    // Пересчитываем статистику при смене студента
                    ExecuteLoadStatistics();
                }
            }
        }

        // Удобное свойство для UI: выбран ли конкретный студент
        public bool IsStudentSelected => SelectedStudent != null;

        // Команды
        public ICommand LoadStatisticsCommand { get; }
        public ICommand DeleteSubjectCommand { get; }
        public ICommand RemoveDebtCommand { get; }
        public ICommand RemoveAttendanceCommand { get; }

        public StatisticsViewModel(IGpaCalculator gpaCalculator,
                                   ISubjectDataService dataService,
                                   IStudentDataService studentDataService)
        {
            _gpaCalculator = gpaCalculator;
            _dataService = dataService;
            _studentDataService = studentDataService;
            LoadStatisticsCommand = new Command(ExecuteLoadStatistics);
            DeleteSubjectCommand = new Command<SubjectStats>(ExecuteDeleteSubject);
            RemoveDebtCommand = new Command<Debt>(ExecuteRemoveDebt);
            RemoveAttendanceCommand = new Command<Attendance>(ExecuteRemoveAttendance);
        }

        private void ExecuteLoadStatistics()
        {
            SubjectStatsList.Clear();
            CountFives = CountFours = CountThrees = CountTwos = 0;

            // Если выбран конкретный студент — работаем только с ним
            if (SelectedStudent != null)
            {
                LoadStudentStatistics(SelectedStudent);
                return;
            }

            // Иначе — общая статистика по всем студентам
            var allSubjects = _studentDataService.Students.SelectMany(s => s.Subjects).ToList();
            var allAttendances = _studentDataService.Students.SelectMany(s => s.Attendances).ToList();
            int totalDebts = _studentDataService.Students.Sum(s => s.Debts.Count);

            StudentsCountText = $"Всего студентов: {_studentDataService.Students.Count}";
            TotalDebtsText = $"Всего должностей: {totalDebts}";

            if (allSubjects.Count == 0)
            {
                AverageGpaText = "Нет данных";
                TotalGradesText = "Всего оценок: 0";
                RecommendedGradeText = "Добавьте студентов и предметы";
                BestSubjectText = WorstSubjectText = "—";
                TotalWeightText = "Общий вес: 0";
                AttendanceText = "Посещаемость: нет данных";
                AttendanceProgress = 0;
                return;
            }

            double totalGpa = _gpaCalculator.CalculateGpa(allSubjects);
            AverageGpaText = $"Средний балл: {totalGpa:F2}";

            double totalWeight = 0;
            Subject best = null, worst = null;

            foreach (var subject in allSubjects)
            {
                totalWeight += subject.Weight;
                if (best == null || subject.Grade > best.Grade) best = subject;
                if (worst == null || subject.Grade < worst.Grade) worst = subject;

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

                SubjectStatsList.Add(new SubjectStats
                {
                    Name = subject.Name,
                    GradesText = subject.GradesText,
                    AverageGrade = subject.Grade,
                    Weight = subject.Weight,
                    Subject = subject
                });
            }

            int total = CountFives + CountFours + CountThrees + CountTwos;
            TotalGradesText = $"Всего оценок: {total}";
            TotalWeightText = $"Общая учебная нагрузка: {totalWeight} весов";

            UpdateAttendanceStats(allAttendances);
            UpdateProgressBars();

            BestSubjectText = best != null ? $"{best.Name} ({best.Grade:F2})" : "—";
            WorstSubjectText = worst != null ? $"{worst.Name} ({worst.Grade:F2})" : "—";

            GenerateSmartRecommendation(totalGpa, worst);
        }

        // Загрузка статистики по конкретному студенту
        private void LoadStudentStatistics(Student student)
        {
            StudentsCountText = $"Студент: {student.Name}";
            TotalDebtsText = $"Долгов: {student.Debts.Count}";

            var subjects = student.Subjects.ToList();
            var attendances = student.Attendances.ToList();

            if (subjects.Count == 0)
            {
                AverageGpaText = "Нет предметов";
                TotalGradesText = "Всего оценок: 0";
                RecommendedGradeText = "Добавьте предметы этому студенту";
                BestSubjectText = WorstSubjectText = "—";
                TotalWeightText = "Общий вес: 0";
                AttendanceText = "Посещаемость: нет данных";
                AttendanceProgress = 0;
                return;
            }

            double gpa = _gpaCalculator.CalculateGpa(subjects);
            AverageGpaText = $"Средний балл: {gpa:F2}";

            double totalWeight = 0;
            Subject best = null, worst = null;

            foreach (var subject in subjects)
            {
                totalWeight += subject.Weight;
                if (best == null || subject.Grade > best.Grade) best = subject;
                if (worst == null || subject.Grade < worst.Grade) worst = subject;

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

                SubjectStatsList.Add(new SubjectStats
                {
                    Name = subject.Name,
                    GradesText = subject.GradesText,
                    AverageGrade = subject.Grade,
                    Weight = subject.Weight,
                    Subject = subject
                });
            }

            int total = CountFives + CountFours + CountThrees + CountTwos;
            TotalGradesText = $"Всего оценок: {total}";
            TotalWeightText = $"Учебная нагрузка: {totalWeight} весов";

            UpdateAttendanceStats(attendances);
            UpdateProgressBars();

            BestSubjectText = best != null ? $"{best.Name} ({best.Grade:F2})" : "—";
            WorstSubjectText = worst != null ? $"{worst.Name} ({worst.Grade:F2})" : "—";

            GenerateSmartRecommendation(gpa, worst);
        }

        // Обновление статистики посещаемости
        private void UpdateAttendanceStats(System.Collections.Generic.List<Attendance> attendances)
        {
            int presentCount = attendances.Count(a => a.IsPresent);
            int totalCount = attendances.Count;
            if (totalCount > 0)
            {
                double percent = (double)presentCount * 100.0 / totalCount;
                AttendanceProgress = percent / 100.0;
                AttendanceText = $"Посещаемость: {presentCount} из {totalCount} ({percent:F1}%)";
            }
            else
            {
                AttendanceProgress = 0;
                AttendanceText = "Посещаемость: нет данных";
            }
        }

        // Обновление прогресс-баров
        private void UpdateProgressBars()
        {
            int total = CountFives + CountFours + CountThrees + CountTwos;
            CalculateProgress(CountFives, total, ref _progressFives, ref _textFives);
            CalculateProgress(CountFours, total, ref _progressFours, ref _textFours);
            CalculateProgress(CountThrees, total, ref _progressThrees, ref _textThrees);
            CalculateProgress(CountTwos, total, ref _progressTwos, ref _textTwos);

            OnPropertyChanged(nameof(ProgressFives)); OnPropertyChanged(nameof(TextFives));
            OnPropertyChanged(nameof(ProgressFours)); OnPropertyChanged(nameof(TextFours));
            OnPropertyChanged(nameof(ProgressThrees)); OnPropertyChanged(nameof(TextThrees));
            OnPropertyChanged(nameof(ProgressTwos)); OnPropertyChanged(nameof(TextTwos));
        }

        // Удаление долга выбранного студента
        private void ExecuteRemoveDebt(Debt debt)
        {
            if (SelectedStudent == null || debt == null) return;
            _studentDataService.RemoveDebt(SelectedStudent, debt);
            // Пересчитываем, чтобы обновить TotalDebtsText
            ExecuteLoadStatistics();
        }

        // Удаление записи посещаемости выбранного студента
        private void ExecuteRemoveAttendance(Attendance attendance)
        {
            if (SelectedStudent == null || attendance == null) return;
            _studentDataService.RemoveAttendance(SelectedStudent, attendance);
            ExecuteLoadStatistics();
        }

        private void ExecuteDeleteSubject(SubjectStats stats)
        {
            if (stats?.Subject == null) return;

            if (SelectedStudent != null)
            {
                // Удаляем у выбранного студента
                SelectedStudent.Subjects.Remove(stats.Subject);
            }
            else
            {
                // Ищем студента, у которого есть этот предмет
                foreach (var student in _studentDataService.Students)
                {
                    if (student.Subjects.Contains(stats.Subject))
                    {
                        student.Subjects.Remove(stats.Subject);
                        break;
                    }
                }
            }

            ExecuteLoadStatistics();
        }

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

        private void GenerateSmartRecommendation(double gpa, Subject worst)
        {
            if (worst == null)
            {
                RecommendedGradeText = "Добавьте предметы для получения рекомендаций.";
                return;
            }

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
        public Subject Subject { get; set; }
    }
}