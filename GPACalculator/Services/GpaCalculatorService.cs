using System.Collections.Generic;
using System.Linq;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    // Этот класс наследует интерфейс IGpaCalculator.
    // Принцип OCP: если формула изменится, мы создадим новый класс (например, EuropeanGpaCalculator),
    // а этот не трогаем.
    public class GpaCalculatorService : IGpaCalculator
    {
        // Реализуем метод расчета GPA из интерфейса
        public double CalculateGpa(IEnumerable<Subject> subjects)
        {
            // Если список пуст, возвращаем 0, чтобы избежать деления на ноль
            if (subjects == null || !subjects.Any())
                return 0;

            // Формула взвешенного среднего:
            // Сумма (Оценка * Вес) / Сумма (Весов)

            // Считаем сумму произведений оценок на их веса
            double sumOfWeightedGrades = subjects.Sum(s => s.Grade * s.Weight);

            // Считаем общую сумму весов
            double sumOfWeights = subjects.Sum(s => s.Weight);

            // Если вдруг все веса оказались нулевыми (защита от дурака)
            if (sumOfWeights == 0) return 0;

            // Возвращаем результат деления
            return sumOfWeightedGrades / sumOfWeights;
        }

        // Реализуем метод прогноза итоговой оценки
        public double PredictNeededGrade(IEnumerable<Subject> currentSubjects, double targetGpa, double lastSubjectWeight)
        {
            // Проверка деления на ноль СРАЗУ (fail-fast принцип)
            if (lastSubjectWeight == 0) return 0;

            // Математика прогноза:
            // Мы знаем, что итоговый GPA = (Сумма_текущих_баллов + Нужная_оценка * Вес_последнего) / (Сумма_текущих_весов + Вес_последнего)
            // Выражаем отсюда Нужную_оценку.

            double currentWeightedSum = currentSubjects.Sum(s => s.Grade * s.Weight);
            double currentWeightSum = currentSubjects.Sum(s => s.Weight);

            double totalWeight = currentWeightSum + lastSubjectWeight;

            // Целевая сумма баллов, которую мы должны набрать в итоге
            double targetTotalWeightedSum = targetGpa * totalWeight;

            // Сколько баллов нам не хватает до цели
            double missingWeightedSum = targetTotalWeightedSum - currentWeightedSum;

            // Сама нужная оценка
            return missingWeightedSum / lastSubjectWeight;
        }
    }
}