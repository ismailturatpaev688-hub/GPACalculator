namespace GPACalculator.Models
{
    // Класс Subject (Предмет) представляет собой один предмет в нашем списке.
    // Принцип SRP: этот класс отвечает ТОЛЬКО за хранение данных о предмете.
    public class Subject
    {
        // Название предмета (например, "Математика")
        public string Name { get; set; }

        // Оценка, которую мы хотим получить или уже получили (от 2 до 5)
        public double Grade { get; set; }

        // Вес предмета (кредиты или важность). 
        // Например, курсовая весит 3, а обычная лекция 1.
        public double Weight { get; set; }

        // Конструктор — это способ создать объект с уже заполненными данными.
        public Subject(string name, double grade, double weight)
        {
            Name = name;
            Grade = grade;
            Weight = weight;
        }
    }
}