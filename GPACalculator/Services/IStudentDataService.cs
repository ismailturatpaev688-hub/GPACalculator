using System.Collections.ObjectModel;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    // Сервис для управления студентами, их долгами и посещаемостью
    public interface IStudentDataService
    {
        // Общий список студентов
        ObservableCollection<Student> Students { get; }

        // Добавить нового студента
        Student AddStudent(string name);

        // Удалить студента
        void RemoveStudent(Student student);

        // Добавить долг студенту
        void AddDebt(Student student, string subjectName, string description, System.DateTime dueDate);

        // Удалить долг
        void RemoveDebt(Student student, Debt debt);

        // Добавить запись о посещаемости
        void AddAttendance(Student student, string subjectName, System.DateTime date, bool isPresent);

        // Удалить запись о посещаемости
        void RemoveAttendance(Student student, Attendance attendance);
    }
}