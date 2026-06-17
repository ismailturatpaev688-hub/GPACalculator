using System;
using System.Collections.Generic;
using System.Text;

namespace GPACalculator.Models
{
    // Модель настроек приложения
    public class Settings
    {
        public double TargetGpa { get; set; } = 4.5; // Целевой GPA по умолчанию
        public string StudentName { get; set; } = "Студент"; // Имя студента
    }
}
