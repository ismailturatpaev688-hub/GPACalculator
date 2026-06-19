using System;
using System.Collections.ObjectModel;
using System.Linq;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    // Реализация сервиса работы со студентами
    public class StudentDataService : IStudentDataService
    {
        public ObservableCollection<Student> Students { get; } = new();

        public Student AddStudent(string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name)) return null;

            // Проверка на дубликат
            if (Students.Any(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return null;

            var student = new Student(name);
            Students.Add(student);
            return student;
        }

        public void RemoveStudent(Student student)
        {
            if (student != null) Students.Remove(student);
        }

        public void AddDebt(Student student, string subjectName, string description, DateTime dueDate)
        {
            if (student == null) return;
            student.Debts.Add(new Debt
            {
                SubjectName = subjectName,
                Description = description,
                DueDate = dueDate
            });
        }

        public void RemoveDebt(Student student, Debt debt)
        {
            if (student == null || debt == null) return;
            student.Debts.Remove(debt);
        }

        public void AddAttendance(Student student, string subjectName, DateTime date, bool isPresent)
        {
            if (student == null) return;
            student.Attendances.Add(new Attendance
            {
                SubjectName = subjectName,
                Date = date,
                IsPresent = isPresent
            });
        }

        public void RemoveAttendance(Student student, Attendance attendance)
        {
            if (student == null || attendance == null) return;
            student.Attendances.Remove(attendance);
        }
    }
}