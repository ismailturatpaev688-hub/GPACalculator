namespace GPACalculator.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public string AppVersion => "1.3";
        public string Developers => "Андрей Уски, Исмаил Туратпаев";
        public string Group => "Группа 1942с";
        public string Description => "Калькулятор GPA для студентов. " +
                                     "Позволяет отслеживать успеваемость и планировать цели.";
    }
}