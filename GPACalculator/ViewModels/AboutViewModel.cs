using System;
using System.Collections.Generic;
using System.Text;

namespace GPACalculator.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public string AppVersion => "1.0.0";
        public string Developers => "Иванов Иван, Петров Петр";
        public string Group => "Группа ПИ-21";
        public string Description => "Калькулятор GPA для студентов. " +
                                     "Позволяет отслеживать успеваемость, " +
                                     "прогнозировать оценки и анализировать прогресс.";
    }
}
