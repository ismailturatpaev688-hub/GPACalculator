using System.Collections.ObjectModel;
using GPACalculator.Models;

namespace GPACalculator.Services
{
    public interface ISubjectDataService
    {
        ObservableCollection<Subject> Subjects { get; }
        // Добавляет оценку, если этот предмет существует
        Subject AddOrUpdate(string name, double grade, double weight);
        // Добавляет сразу несколько оценок
        Subject AddOrUpdate(string name, IEnumerable<double> grades, double weight);
    }
}
