using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GPACalculator.ViewModels
{
    // Он реализует интерфейс INotifyPropertyChanged - этот интерфейс говорит программе, что изменилось
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Оно даёт знать, когда обновить текст
        // Добавили "?" чтобы разрешить null для события
        public event PropertyChangedEventHandler? PropertyChanged;

        // Она автоматически подставляет имя свойства, из которого вызвали этот метод.
        // Это избавляет нас от опечаток в названиях свойств.
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // Мы вызываем событие и передаем имя изменившегося свойства.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
