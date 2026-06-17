using System;
using System.Collections.Generic;
using System.Text;

namespace GPACalculator.Models
{
    // Модель семестра. Это просто "коробка", в которой лежит список предметов и название семестра.
    public class Semester
    {
        public string Name { get; set; } // Например, "1 семестр 2 курса"
        public List<Subject> Subjects { get; set; } = new();
        public double Gpa { get; set; } // Сохраняем рассчитанный GPA, чтобы не считать заново
    }
}
