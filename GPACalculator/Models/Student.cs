using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GPACalculator.Models
{
    public class Student : INotifyPropertyChanged
    {
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public Student(string name)
        {
            _name = name;

            // При изменении списка предметов — пересчитываем средний балл
            Subjects.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AverageGrade));
            // При изменении посещаемости — пересчитываем процент
            Attendances.CollectionChanged += (s, e) => OnPropertyChanged(nameof(AttendancePercent));
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        // Предметы студента (каждый со своими оценками)
        public ObservableCollection<Subject> Subjects { get; } = new();

        // Долги студента
        public ObservableCollection<Debt> Debts { get; } = new();

        // Посещаемость студента
        public ObservableCollection<Attendance> Attendances { get; } = new();

        // Вычисляемый средний балл по всем предметам студента
        public double AverageGrade => Subjects.Count > 0 ? Subjects.Average(s => s.Grade) : 0;

        // Процент посещаемости
        public double AttendancePercent => Attendances.Count > 0
            ? (double)Attendances.Count(a => a.IsPresent) * 100.0 / Attendances.Count
            : 0;

        // Количество должностей
        public int DebtsCount => Debts.Count;

        public string DisplayText => $"{Name} — GPA: {AverageGrade:F2}";
    }
}