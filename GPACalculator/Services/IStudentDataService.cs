using System.Collections.ObjectModel;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    public interface IStudentDataService
    {
        ObservableCollection<Student> Students { get; }

        // Функции добавления и удаления например: студента
        Student AddStudent(string name);
        void RemoveStudent(Student student);

        void AddDebt(Student student, string subjectName, string description, System.DateTime dueDate);
        void RemoveDebt(Student student, Debt debt);

        void AddAttendance(Student student, string subjectName, System.DateTime date, bool isPresent);
        void RemoveAttendance(Student student, Attendance attendance);
    }
}