using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GPACalculator.Models;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    // ViewModel для страницы профиля конкретного студента
    // Реализует IQueryAttributable, чтобы получать имя студента из маршрута
    public class StudentProfileViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IStudentDataService _studentDataService;
        private Student _student;

        // Поля для добавления долга
        private string _debtSubject = "";
        private string _debtDescription = "";
        private DateTime _debtDueDate = DateTime.Now.AddDays(7);

        // Поля для добавления посещаемости
        private string _attendanceSubject = "";
        private DateTime _attendanceDate = DateTime.Now;
        private bool _attendanceIsPresent = true;

        private string _statusText = "";

        public string DebtSubject
        {
            get => _debtSubject;
            set { if (_debtSubject != value) { _debtSubject = value; OnPropertyChanged(); } }
        }
        public string DebtDescription
        {
            get => _debtDescription;
            set { if (_debtDescription != value) { _debtDescription = value; OnPropertyChanged(); } }
        }
        public DateTime DebtDueDate
        {
            get => _debtDueDate;
            set { if (_debtDueDate != value) { _debtDueDate = value; OnPropertyChanged(); } }
        }

        public string AttendanceSubject
        {
            get => _attendanceSubject;
            set { if (_attendanceSubject != value) { _attendanceSubject = value; OnPropertyChanged(); } }
        }
        public DateTime AttendanceDate
        {
            get => _attendanceDate;
            set { if (_attendanceDate != value) { _attendanceDate = value; OnPropertyChanged(); } }
        }
        public bool AttendanceIsPresent
        {
            get => _attendanceIsPresent;
            set { if (_attendanceIsPresent != value) { _attendanceIsPresent = value; OnPropertyChanged(); } }
        }

        public string StatusText
        {
            get => _statusText;
            set { if (_statusText != value) { _statusText = value; OnPropertyChanged(); } }
        }

        // Сам студент (устанавливается через ApplyQueryAsync)
        public Student Student
        {
            get => _student;
            private set
            {
                if (_student != value)
                {
                    _student = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StudentName));
                    OnPropertyChanged(nameof(AverageGradeText));
                    OnPropertyChanged(nameof(AttendancePercentText));
                }
            }
        }

        // Имя студента для заголовка
        public string StudentName => Student?.Name ?? "—";
        // Средний балл студента
        public string AverageGradeText => Student != null ? $"Средний балл: {Student.AverageGrade:F2}" : "Нет данных";
        // Процент посещаемости
        public string AttendancePercentText => Student != null ? $"Посещаемость: {Student.AttendancePercent:F1}%" : "Нет данных";

        // Команды
        public ICommand AddDebtCommand { get; }
        public ICommand RemoveDebtCommand { get; }
        public ICommand AddAttendanceCommand { get; }
        public ICommand RemoveAttendanceCommand { get; }

        public StudentProfileViewModel(IStudentDataService studentDataService)
        {
            _studentDataService = studentDataService;

            AddDebtCommand = new Command(ExecuteAddDebt);
            RemoveDebtCommand = new Command<Debt>(ExecuteRemoveDebt);
            AddAttendanceCommand = new Command(ExecuteAddAttendance);
            RemoveAttendanceCommand = new Command<Attendance>(ExecuteRemoveAttendance);
        }

        // Метод из IQueryAttributable — вызывается при навигации с параметром
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("name", out object nameObj) && nameObj is string name)
            {
                // Находим студента по имени
                Student = _studentDataService.Students.FirstOrDefault(
                    s => s.Name.Equals(Uri.UnescapeDataString(name), StringComparison.OrdinalIgnoreCase));

                if (Student == null)
                    StatusText = "Студент не найден";
            }
        }

        private void ExecuteAddDebt()
        {
            if (Student == null) { StatusText = "Студент не выбран"; return; }
            if (string.IsNullOrWhiteSpace(DebtSubject))
            {
                StatusText = "Введите название предмета для долга!";
                return;
            }
            if (string.IsNullOrWhiteSpace(DebtDescription))
            {
                StatusText = "Введите описание долга!";
                return;
            }

            _studentDataService.AddDebt(Student, DebtSubject, DebtDescription, DebtDueDate);
            DebtSubject = "";
            DebtDescription = "";
            DebtDueDate = DateTime.Now.AddDays(7);
            StatusText = "Долг добавлен!";
        }

        private void ExecuteRemoveDebt(Debt debt)
        {
            if (Student == null || debt == null) return;
            _studentDataService.RemoveDebt(Student, debt);
            StatusText = "Долг удалён";
        }

        private void ExecuteAddAttendance()
        {
            if (Student == null) { StatusText = "Студент не выбран"; return; }
            if (string.IsNullOrWhiteSpace(AttendanceSubject))
            {
                StatusText = "Введите название предмета!";
                return;
            }

            _studentDataService.AddAttendance(Student, AttendanceSubject, AttendanceDate, AttendanceIsPresent);
            AttendanceSubject = "";
            StatusText = AttendanceIsPresent ? "Запись о присутствии добавлена" : "Запись об отсутствии добавлена";
        }

        private void ExecuteRemoveAttendance(Attendance attendance)
        {
            if (Student == null || attendance == null) return;
            _studentDataService.RemoveAttendance(Student, attendance);
            StatusText = "Запись удалена";
        }
    }
}