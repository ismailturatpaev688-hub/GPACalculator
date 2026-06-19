using System;

namespace GPACalculator.Models
{
    public class Debt
    {
        public string SubjectName { get; set; }    // По какому предмету долг
        public string Description { get; set; }    // Что именно нужно сдать
        public DateTime DueDate { get; set; }      // Крайний срок

        // Проверка, просрочен ли долг
        public bool IsOverdue => DateTime.Now > DueDate;

        public string DisplayText => $"{SubjectName}: {Description} (до {DueDate:dd.MM.yyyy})";
    }
}