using System;

namespace GPACalculator.Models
{
    // Модель долга: предмет, описание и крайний срок
    public class Debt
    {
        public string SubjectName { get; set; }    // По какому предмету долг
        public string Description { get; set; }    // Что именно нужно сдать
        public DateTime DueDate { get; set; }      // Крайний срок

        // Проверка, просрочен ли долг
        public bool IsOverdue => DateTime.Now > DueDate;

        // Красивое отображение
        public string DisplayText => $"{SubjectName}: {Description} (до {DueDate:dd.MM.yyyy})";
    }
}