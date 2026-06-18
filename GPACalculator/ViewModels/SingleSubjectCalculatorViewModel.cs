using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    public class SingleSubjectCalculatorViewModel : BaseViewModel
    {
        private string _subjectName = "";
        private string _newGrade = "";
        private string _resultText = "Добавьте оценки для расчета";
        private double _averageGrade;

        public ObservableCollection<GradeEntry> Grades { get; } = new();

        public string SubjectName
        {
            get => _subjectName;
            set
            {
                if (_subjectName != value)
                {
                    _subjectName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NewGrade
        {
            get => _newGrade;
            set
            {
                if (_newGrade != value)
                {
                    _newGrade = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public double AverageGrade
        {
            get => _averageGrade;
            set
            {
                if (_averageGrade != value)
                {
                    _averageGrade = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddGradeCommand { get; }
        public ICommand CalculateCommand { get; }
        public ICommand ClearCommand { get; }

        public SingleSubjectCalculatorViewModel()
        {
            AddGradeCommand = new Command(ExecuteAddGrade);
            CalculateCommand = new Command(ExecuteCalculate);
            ClearCommand = new Command(ExecuteClear);
        }

        private void ExecuteAddGrade()
        {
            if (double.TryParse(NewGrade, out double grade))
            {
                if (grade >= 2 && grade <= 5)
                {
                    Grades.Add(new GradeEntry
                    {
                        Grade = grade,
                        DisplayText = grade.ToString("F1")
                    });
                    NewGrade = "";
                    ResultText = $"Добавлено оценок: {Grades.Count}";
                }
                else
                {
                    ResultText = "Ошибка: оценка должна быть от 2 до 5";
                }
            }
            else
            {
                ResultText = "Ошибка: введите корректное число";
            }
        }

        private void ExecuteCalculate()
        {
            if (Grades.Count == 0)
            {
                ResultText = "Добавьте хотя бы одну оценку!";
                return;
            }

            double sum = Grades.Sum(g => g.Grade);
            AverageGrade = sum / Grades.Count;

            string gradeText = AverageGrade switch
            {
                >= 4.5 => "Отлично! 🎉",
                >= 3.5 => "Хорошо 👍",
                >= 2.5 => "Удовлетворительно",
                _ => "Нужно подтянуть! 📚"
            };

            ResultText = $"Средний балл по \"{SubjectName}\": {AverageGrade:F2}\n{gradeText}";
        }

        private void ExecuteClear()
        {
            Grades.Clear();
            SubjectName = "";
            NewGrade = "";
            ResultText = "Добавьте оценки для расчета";
            AverageGrade = 0;
        }
    }

    // Модель для одной оценки
    public class GradeEntry
    {
        public double Grade { get; set; }
        public string DisplayText { get; set; }
    }
}