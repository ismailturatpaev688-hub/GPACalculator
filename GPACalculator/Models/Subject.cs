using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GPACalculator.Models
{
    public class Subject : INotifyPropertyChanged
    {
        private string _name;
        private double _weight;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // Основной конструктор
        public Subject(string name, double weight)
        {
            _name = name;
            _weight = weight;

            // Когда добавляется новая оценка, то пересчитывает оценки
            Grades.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Grade));
                OnPropertyChanged(nameof(GradesText));
            };
        }

        // Обратная совместимость
        public Subject(string name, double grade, double weight) : this(name, weight)
        {
            Grades.Add(grade);
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public double Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(); }
        }

        // Все оценки по предмету
        public ObservableCollection<double> Grades { get; } = new();

        // Вычисляемый средний балл 
        public double Grade => Grades.Count > 0 ? Grades.Average() : 0;

        // Красивая строка для UI: "4.0, 5.0, 3.5"
        public string GradesText => string.Join(", ", Grades.Select(g => g.ToString("F1")));

        public void AddGrade(double grade) => Grades.Add(grade);
    }
}