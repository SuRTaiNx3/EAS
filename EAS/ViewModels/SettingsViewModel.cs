using EAS.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EAS.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        #region Globals

        #endregion

        #region Properties

        private bool _startWithWindows;
        public bool StartWithWindows
        {
            get { return _startWithWindows; }
            set
            {
                _startWithWindows = value;
                OnPropertyChanged("StartWithWindows");
            }
        }

        private bool _minimizeOnExit;
        public bool MinimizeOnExit
        {
            get { return _minimizeOnExit; }
            set
            {
                _minimizeOnExit = value;
                OnPropertyChanged("MinimizeOnExit");
            }
        }

        private bool _startMinimized;
        public bool StartMinimized
        {
            get { return _startMinimized; }
            set
            {
                _startMinimized = value;
                OnPropertyChanged("StartMinimized");
            }
        }

        public Action OnCloseSettingsAction { get; set; }

        #endregion

        #region Commands

        private RelayCommand _saveSettingsCommand;
        public RelayCommand SaveSettingsCommand
        {
            get
            {
                if (_saveSettingsCommand == null)
                    _saveSettingsCommand = new RelayCommand(a => Save(), b => true);
                return _saveSettingsCommand;
            }
        }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand(a => Cancel(), b => true);
                return _cancelCommand;
            }
        }

        #endregion

        #region Constructor

        public SettingsViewModel()
        {
            StartWithWindows = SettingsList.Instance.StartWithWindows;
            MinimizeOnExit = SettingsList.Instance.MinimizeOnExit;
            StartMinimized = SettingsList.Instance.StartMinimized;
        }

        #endregion

        #region Methods

        private void Save()
        {
            if (SettingsList.Instance.StartWithWindows != StartWithWindows)
                ChangeStartWithWindowsSetting(StartWithWindows);

            SettingsList.Instance.MinimizeOnExit = MinimizeOnExit;
            SettingsList.Instance.StartWithWindows = StartWithWindows;
            SettingsList.Instance.StartMinimized = StartMinimized;
            SettingsList.Save();
            OnCloseSettingsAction?.Invoke();
        }
        private void Cancel()
        {
            OnCloseSettingsAction?.Invoke();
        }

        private void ChangeStartWithWindowsSetting(bool startWithWindows)
        {
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (startWithWindows)
                    registryKey.SetValue("EAS", System.Reflection.Assembly.GetExecutingAssembly().Location);
                else
                    registryKey.DeleteValue("EAS");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion
    }
}
