using GPACalculator.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GPACalculator.Services
{
    // Реализация сервиса хранения данных.
    // Мы используем Preferences - это простое хранилище "ключ-значение" в MAUI.
    public class DataStorageService : IDataStorageService
    {
        // "Ключи" для хранения. Это как имена файлов в нашей "коробке" Preferences.
        private const string SemestersKey = "semesters_data";
        private const string SettingsKey = "settings_data";

        public void SaveSemesters(List<Semester> semesters)
        {
            // Preferences не умеет хранить сложные объекты (списки, классы).
            // Поэтому мы превращаем наш список в строку JSON (текст).
            // JSON - это просто формат текста, который легко прочитать и человеку, и компьютеру.
            string json = JsonSerializer.Serialize(semesters);

            // Сохраняем строку в Preferences под ключом SemestersKey
            Preferences.Set(SemestersKey, json);
        }

        public List<Semester> LoadSemesters()
        {
            // Читаем строку из Preferences. Если её там нет, вернем пустую строку.
            string json = Preferences.Get(SemestersKey, "");

            // Если строка пустая, значит, мы ещё ничего не сохраняли. Вернем пустой список.
            if (string.IsNullOrWhiteSpace(json))
                return new List<Semester>();

            // Превращаем строку JSON обратно в список объектов Semester
            return JsonSerializer.Deserialize<List<Semester>>(json);
        }

        public void SaveSettings(Settings settings)
        {
            string json = JsonSerializer.Serialize(settings);
            Preferences.Set(SettingsKey, json);
        }

        public Settings LoadSettings()
        {
            string json = Preferences.Get(SettingsKey, "");
            if (string.IsNullOrWhiteSpace(json))
                return new Settings(); // Вернем настройки по умолчанию

            return JsonSerializer.Deserialize<Settings>(json);
        }
    }
}
