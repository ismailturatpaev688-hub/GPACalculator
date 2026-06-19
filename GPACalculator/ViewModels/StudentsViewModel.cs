using System.Collections.ObjectModel;
using System.Windows.Input;
using GPACalculator.Models;
using GPACalculator.Services;

namespace GPACalculator.ViewModels
{
    // ViewModel для страницы списка студентов
    public class StudentsViewModel : BaseViewModel
    {
        private readonly IStudentDataService _studentDataService;
        private string _newStudentName = "";
        private string _statusText = "Добавьте первого студента";

        public string NewStudentName
        {
            get => _newStudentName;
            set { if (_newStudentName != value) { _newStudentName = value; OnPropertyChanged(); } }
        }

        public string StatusText
        {
            get => _statusText;
            set { if (_statusText != value) { _statusText = value; OnPropertyChanged(); } }
        }

        // Список студентов — берём напрямую из сервиса
        public ObservableCollection<Student> Students => _studentDataService.Students;

        public ICommand AddStudentCommand { get; }
        public ICommand DeleteStudentCommand { get; }
        public ICommand OpenProfileCommand { get; }

        public StudentsViewModel(IStudentDataService studentDataService)
        {
            _studentDataService = studentDataService;
            AddStudentCommand = new Command(ExecuteAddStudent);
            DeleteStudentCommand = new Command<Student>(ExecuteDeleteStudent);
            // Команда открытия профиля: передаём студента, навигация делается через Shell
            OpenProfileCommand = new Command<Student>(ExecuteOpenProfile);
        }

        private void ExecuteAddStudent()
        {
            if (string.IsNullOrWhiteSpace(NewStudentName))
            {
                StatusText = "Ошибка: введите имя студента!";
                return;
            }

            var student = _studentDataService.AddStudent(NewStudentName);
            if (student == null)
            {
                StatusText = "Ошибка: студент с таким именем уже существует!";
                return;
            }

            NewStudentName = "";
            StatusText = $"Студент '{student.Name}' добавлен!";
        }

        private void ExecuteDeleteStudent(Student student)
        {
            if (student == null) return;
            _studentDataService.RemoveStudent(student);
            StatusText = $"Студент удалён. Осталось: {Students.Count}";
        }

        // Навигация на профиль студента через Shell
        private async void ExecuteOpenProfile(Student student)
        {
            if (student == null) return;
            // Передаём имя студента как параметр маршрута
            await Shell.Current.GoToAsync($"StudentProfilePage?name={Uri.EscapeDataString(student.Name)}");
        }
    }
}