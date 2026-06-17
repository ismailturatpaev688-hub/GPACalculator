using GPACalculator.Models;
using GPACalculator.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private readonly IDataStorageService _storageService;
        private readonly IGpaCalculator _gpaCalculator;

        // Приватные поля
        private Semester _selectedSemester;

        // Свойства
        public Semester SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                if (_selectedSemester != value)
                {
                    _selectedSemester = value;
                    OnPropertyChanged();
                    // Когда пользователь выбирает семестр в списке, мы можем что-то сделать
                    // Например, перейти на страницу деталей. Пока оставим пустым.
                }
            }
        }

        // Список семестров для отображения
        public ObservableCollection<Semester> Semesters { get; } = new();

        // Команды
        public ICommand LoadSemestersCommand { get; }
        public ICommand SaveCurrentSemesterCommand { get; }
        public ICommand DeleteSemesterCommand { get; }

        // Конструктор. Сюда внедряются ДВА сервиса (принцип DIP).
        public HistoryViewModel(IDataStorageService storageService, IGpaCalculator gpaCalculator)
        {
            _storageService = storageService;
            _gpaCalculator = gpaCalculator;

            LoadSemestersCommand = new Command(ExecuteLoadSemesters);
            SaveCurrentSemesterCommand = new Command(ExecuteSaveCurrentSemester);
            DeleteSemesterCommand = new Command<Semester>(ExecuteDeleteSemester);
        }

        // Загрузка семестров из хранилища
        private void ExecuteLoadSemesters()
        {
            Semesters.Clear();
            var loaded = _storageService.LoadSemesters();
            foreach (var sem in loaded)
            {
                Semesters.Add(sem);
            }
        }

        // Сохранение текущего списка предметов как нового семестра
        // Этот метод будет вызываться с главной страницы
        public void SaveCurrentAsSemester(string semesterName, ObservableCollection<Subject> currentSubjects)
        {
            if (string.IsNullOrWhiteSpace(semesterName) || currentSubjects.Count == 0)
                return;

            // Создаем новый семестр
            var newSemester = new Semester
            {
                Name = semesterName,
                Subjects = new System.Collections.Generic.List<Subject>(currentSubjects),
                Gpa = _gpaCalculator.CalculateGpa(currentSubjects)
            };

            // Добавляем в список и сохраняем
            Semesters.Add(newSemester);
            _storageService.SaveSemesters(new System.Collections.Generic.List<Semester>(Semesters));
        }

        private void ExecuteSaveCurrentSemester()
        {
            // Этот метод можно использовать для диалога ввода названия семестра
            // Пока оставим заглушку
        }

        // Удаление семестра
        private void ExecuteDeleteSemester(Semester semester)
        {
            if (semester == null) return;

            Semesters.Remove(semester);
            _storageService.SaveSemesters(new System.Collections.Generic.List<Semester>(Semesters));
        }
    }
}
