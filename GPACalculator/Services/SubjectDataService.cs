using System;
using System.Collections.ObjectModel;
using System.Linq;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    public class SubjectDataService : ISubjectDataService
    {
        public ObservableCollection<Subject> Subjects { get; } = new();

        public Subject AddOrUpdate(string name, double grade, double weight)
        {
            // Удаление пробелов
            name = name.Trim();
            // Поиск существующего  предмета
            var existing = Subjects.FirstOrDefault(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)); // сравнение объектов на равенство

            // Обновление существующего предмета
            if (existing != null)
            {
                existing.AddGrade(grade);
                return existing;
            }

            // Создание нового предмета
            var subject = new Subject(name, weight);
            subject.AddGrade(grade);
            Subjects.Add(subject);
            return subject;
        }

        public Subject AddOrUpdate(string name, IEnumerable<double> grades, double weight)
        {
            name = name.Trim();
            var existing = Subjects.FirstOrDefault(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                foreach (var g in grades) existing.AddGrade(g);
                return existing;
            }

            var subject = new Subject(name, weight);
            foreach (var g in grades) subject.AddGrade(g);
            Subjects.Add(subject);
            return subject;
        }
    }
}
