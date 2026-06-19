using System.Collections.Generic;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    public interface IGpaCalculator
    {
        // Метод для расчета текущего среднего балла (GPA)
        double CalculateGpa(IEnumerable<Subject> subjects);

        // Метод для прогноза: какую оценку нужно получить за последний предмет.
        double PredictNeededGrade(IEnumerable<Subject> currentSubjects, double targetGpa, double lastSubjectWeight);
    }
}