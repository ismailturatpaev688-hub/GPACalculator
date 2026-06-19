using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GPACalculator.Models;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    // IQueryAttributable позволяет классу обрабатывать данные которые передаются при навигации
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

        public string StatusText
        {
            get => _statusText;
            set { if (_statusText != value) { _statusText = value; OnPropertyChanged(); } }
        }

        // Сам студент
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

        // Вывод параметров проверка на пустое значение 
        public string StudentName => Student?.Name ?? "—";
        public string AverageGradeText => Student != null ? $"Средний балл: {Student.AverageGrade:F2}" : "Нет данных";
        public string AttendancePercentText => Student != null ? $"Посещаемость: {Student.AttendancePercent:F1}%" : "Нет данных";

        // Команды
        public ICommand AddDebtCommand { get; }
        public ICommand RemoveDebtCommand { get; }
        // Команда посещаемости
        public ICommand AddAttendanceCommand { get; }
        public ICommand RemoveAttendanceCommand { get; }

        public StudentProfileViewModel(IStudentDataService studentDataService)
        {
            _studentDataService = studentDataService;

            AddDebtCommand = new Command(ExecuteAddDebt);
            RemoveDebtCommand = new Command<Debt>(ExecuteRemoveDebt);
            // Команда принимает параметр object, который мы преобразуем в bool
            AddAttendanceCommand = new Command<object>(ExecuteAddAttendance);
            RemoveAttendanceCommand = new Command<Attendance>(ExecuteRemoveAttendance);
        }
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

        // Команда принимает параметр: "True" или "False"
        private void ExecuteAddAttendance(object parameter)
        {
            if (Student == null) { StatusText = "Студент не выбран"; return; }
            if (string.IsNullOrWhiteSpace(AttendanceSubject))
            {
                StatusText = "Введите название предмета!";
                return;
            }

            // Преобразуем параметр в bool
            bool isPresent = false;
            if (parameter is string strParam)
            {
                bool.TryParse(strParam, out isPresent);
            }
            else if (parameter is bool boolParam)
            {
                isPresent = boolParam;
            }

            _studentDataService.AddAttendance(Student, AttendanceSubject, AttendanceDate, isPresent);
            AttendanceSubject = "";
            StatusText = isPresent ? "Запись о присутствии добавлена" : "Запись об отсутствии добавлена";
        }

        private void ExecuteRemoveAttendance(Attendance attendance)
        {
            if (Student == null || attendance == null) return;
            _studentDataService.RemoveAttendance(Student, attendance);
            StatusText = "Запись удалена";
        }
    }
}