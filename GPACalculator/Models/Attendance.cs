using System;

namespace GPACalculator.Models
{
    // Модель посещаемости: предмет, дата, был ли студент
    public class Attendance
    {
        public string SubjectName { get; set; }   // По какому предмету
        public DateTime Date { get; set; }        // Дата занятия
        public bool IsPresent { get; set; }       // true — присутствовал, false — отсутствовал

        // Красивое отображение
        public string DisplayText => $"{SubjectName} — {Date:dd.MM.yyyy}: {(IsPresent ? "✓ был" : "✗ не был")}";
    }
}