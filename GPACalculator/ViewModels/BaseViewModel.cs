using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GPACalculator.ViewModels
{
    // Базовый класс для всех наших ViewModel.
    // Он реализует интерфейс INotifyPropertyChanged.
    // Этот интерфейс — стандартный механизм в C#, который говорит экрану: 
    // "Эй, слушай меня! Если я изменю какое-то свойство, я тебе сообщу".
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Это событие, которое срабатывает, когда свойство меняется.
        // Экран (XAML) подписывается на это событие, чтобы знать, когда обновить текст на экране.
        public event PropertyChangedEventHandler PropertyChanged;

        // Метод, который вызывает это событие.
        // [CallerMemberName] - это очень крутая фишка C#. 
        // Она автоматически подставляет имя свойства, из которого вызвали этот метод.
        // Например, если мы вызвали OnPropertyChanged() внутри сеттера свойства NewSubjectName,
        // то параметр propertyName автоматически станет равен строке "NewSubjectName".
        // Это избавляет нас от опечаток в названиях свойств.
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // Если есть кто-то, кто слушает это событие (а экран всегда слушает),
            // мы вызываем событие и передаем имя изменившегося свойства.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
