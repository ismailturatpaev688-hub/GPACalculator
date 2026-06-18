using System.Collections.Generic;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    // Интерфейс калькулятора. 
    // Принцип DIP: ViewModel будет зависеть от этого интерфейса, а не от конкретного класса.
    public interface IGpaCalculator
    {
        // Метод для расчета текущего среднего балла (GPA)
        double CalculateGpa(IEnumerable<Subject> subjects);

        // Метод для прогноза: какую оценку нужно получить за последний предмет, 
        // чтобы итоговый GPA был нужным (например, 5.0).
        double PredictNeededGrade(IEnumerable<Subject> currentSubjects, double targetGpa, double lastSubjectWeight);
    }
}