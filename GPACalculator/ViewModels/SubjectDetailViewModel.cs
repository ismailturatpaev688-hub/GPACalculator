using GPACalculator.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    public class SubjectDetailViewModel : BaseViewModel
    {
        private Subject _subject;
        private string _name;
        private string _grade;
        private string _weight;
        private bool _isSaved;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Grade
        {
            get => _grade;
            set { _grade = value; OnPropertyChanged(); }
        }

        public string Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(); }
        }

        public bool IsSaved
        {
            get => _isSaved;
            set { _isSaved = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }

        public SubjectDetailViewModel()
        {
            SaveCommand = new Command(ExecuteSave);
        }

        // Метод для инициализации данными существующего предмета
        public void LoadSubject(Subject subject)
        {
            _subject = subject;
            Name = subject.Name;
            Grade = subject.Grade.ToString("F2");
            Weight = subject.Weight.ToString("F2");
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return;

            if (double.TryParse(Grade, out double grade) &&
                double.TryParse(Weight, out double weight))
            {
                _subject.Name = Name;
                _subject.Grade = grade;
                _subject.Weight = weight;
                IsSaved = true;
            }
        }
    }
}
