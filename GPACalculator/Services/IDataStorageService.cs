using System;
using System.Text;
using GPACalculator.Models;
using System.Collections.Generic;

namespace GPACalculator.Services
{
    // Интерфейс для хранения данных.
    // Принцип DIP: ViewModel будут зависеть от этого интерфейса, а не от конкретного способа хранения.
    // Если завтра мы захотим сохранять в базу данных, мы просто создадим новый класс,
    // а ViewModel трогать не придется.
    public interface IDataStorageService
    {
        // Сохранить список семестров (каждый семестр — это список предметов + название)
        void SaveSemesters(List<Semester> semesters);

        // Загрузить список семестров
        List<Semester> LoadSemesters();

        // Сохранить настройки (целевой GPA и т.д.)
        void SaveSettings(Settings settings);

        // Загрузить настройки
        Settings LoadSettings();
    }
}
