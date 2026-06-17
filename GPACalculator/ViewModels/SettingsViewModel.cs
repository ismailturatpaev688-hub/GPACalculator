using GPACalculator.Models;
using GPACalculator.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GPACalculator.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IDataStorageService _storageService;
        private Settings _settings;

        private string _studentName;
        private string _targetGpaText;
        private string _statusMessage;

        public string StudentName
        {
            get => _studentName;
            set
            {
                if (_studentName != value)
                {
                    _studentName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TargetGpaText
        {
            get => _targetGpaText;
            set
            {
                if (_targetGpaText != value)
                {
                    _targetGpaText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel(IDataStorageService storageService)
        {
            _storageService = storageService;
            LoadSettingsCommand = new Command(ExecuteLoadSettings);
            SaveSettingsCommand = new Command(ExecuteSaveSettings);
        }

        private void ExecuteLoadSettings()
        {
            _settings = _storageService.LoadSettings();
            StudentName = _settings.StudentName;
            TargetGpaText = _settings.TargetGpa.ToString("F2");
        }

        private void ExecuteSaveSettings()
        {
            if (string.IsNullOrWhiteSpace(StudentName))
            {
                StatusMessage = "Имя не может быть пустым!";
                return;
            }

            if (!double.TryParse(TargetGpaText, out double targetGpa) || targetGpa < 0 || targetGpa > 5)
            {
                StatusMessage = "Целевой GPA должен быть числом от 0 до 5!";
                return;
            }

            _settings.StudentName = StudentName;
            _settings.TargetGpa = targetGpa;
            _storageService.SaveSettings(_settings);
            StatusMessage = "Настройки сохранены!";
        }
    }
}
