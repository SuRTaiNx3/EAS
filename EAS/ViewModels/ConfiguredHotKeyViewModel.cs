using CoreAudioApi;
using EAS.Common;
using EAS.Enums;
using MvvmValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace EAS.ViewModels
{
    public class ConfiguredHotKeyViewModel : BaseViewModel
    {
        #region Globals

        #endregion

        #region Properties

        public bool IsNew { get; set; } = false;


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _nameTemp = value;
                OnPropertyChanged("Name");
            }
        }

        private AudioDeviceViewModel _audioDevice;
        public AudioDeviceViewModel AudioDevice
        {
            get { return _audioDevice; }
            set
            {
                _audioDevice = value;
                _selectedDevice = MainViewModel.Instance.Devices?.FirstOrDefault(d => d.ID == value.DeviceId);
                OnPropertyChanged("AudioDevice");
            }
        }

        private Modes _mode;
        public Modes Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _modeTemp = value;
                OnPropertyChanged("Mode");
            }
        }

        private HotKey _shortcut;
        public HotKey Shortcut
        {
            get { return _shortcut; }
            set
            {
                _shortcut = value;
                _shortcutTemp = new HotKey(value.Key, value.KeyModifiers, null, false);
                OnPropertyChanged("Shortcut");
            }
        }



        // Temporary properties

        private MMDevice _selectedDevice;
        [JsonIgnore]
        public MMDevice SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
                Validator.Validate(nameof(SelectedDevice));
            }
        }

        private string _nameTemp;
        [JsonIgnore]
        public string NameTemp
        {
            get { return _nameTemp; }
            set
            {
                _nameTemp = value;
                OnPropertyChanged("NameTemp");
                Validator.Validate(nameof(NameTemp));
            }
        }

        private Modes _modeTemp;
        [JsonIgnore]
        public Modes ModeTemp
        {
            get { return _modeTemp; }
            set
            {
                _modeTemp = value;
                OnPropertyChanged("ModeTemp");
            }
        }

        private HotKey _shortcutTemp;
        [JsonIgnore]
        public HotKey ShortcutTemp
        {
            get { return _shortcutTemp; }
            set
            {
                _shortcutTemp = value;
                OnPropertyChanged("ShortcutTemp");
                Validator.Validate(nameof(ShortcutTemp));
            }
        }


        [JsonIgnore]
        public Action<ConfiguredHotKeyViewModel> OnEditHotKeyAction { get; set; }

        [JsonIgnore]
        public Action<ConfiguredHotKeyViewModel> OnCancelEditHotKeyAction { get; set; }

        [JsonIgnore]
        public Action<ConfiguredHotKeyViewModel> OnSaveHotKeyAction { get; set; }

        [JsonIgnore]
        public Action<ConfiguredHotKeyViewModel> OnDeleteHotKeyAction { get; set; }

        [JsonIgnore]
        public Action<HotKey> OnHotKeyAppliedAction { get; set; }

        #endregion

        #region Commands

        private RelayCommand _applyConfiguredHotKeyCommand;
        public RelayCommand ApplyConfiguredHotKeyCommand
        {
            get
            {
                if (_applyConfiguredHotKeyCommand == null)
                    _applyConfiguredHotKeyCommand = new RelayCommand(a => ApplyConfiguredHotKey(), b => true);
                return _applyConfiguredHotKeyCommand;
            }
        }

        private RelayCommand _editConfiguredHotKeyCommand;
        public RelayCommand EditConfiguredHotKeyCommand
        {
            get
            {
                if (_editConfiguredHotKeyCommand == null)
                    _editConfiguredHotKeyCommand = new RelayCommand(a => EditConfiguredHotKey(), b => true);
                return _editConfiguredHotKeyCommand;
            }
        }

        private RelayCommand _closeEditConfiguredHotKeyCommand;
        public RelayCommand CloseEditConfiguredHotKeyCommand
        {
            get
            {
                if (_closeEditConfiguredHotKeyCommand == null)
                    _closeEditConfiguredHotKeyCommand = new RelayCommand(a => CloseEditConfiguredHotKey(), b => true);
                return _closeEditConfiguredHotKeyCommand;
            }
        }

        private RelayCommand _saveHotKeyCommand;
        public RelayCommand SaveHotKeyCommand
        {
            get
            {
                if (_saveHotKeyCommand == null)
                    _saveHotKeyCommand = new RelayCommand(a => SaveConfiguredHotKey(), b => !HasErrors);
                return _saveHotKeyCommand;
            }
        }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand(a => CloseEditConfiguredHotKey(), b => true);
                return _cancelCommand;
            }
        }

        private RelayCommand _deleteConfiguredHotKeyCommand;
        public RelayCommand DeleteConfiguredHotKeyCommand
        {
            get
            {
                if (_deleteConfiguredHotKeyCommand == null)
                    _deleteConfiguredHotKeyCommand = new RelayCommand(a => DeleteConfiguredHotKey(), b => true);
                return _deleteConfiguredHotKeyCommand;
            }
        }


        #endregion

        #region Constructor

        public ConfiguredHotKeyViewModel()
        {
            Validator.AddRule(nameof(NameTemp), 
                () => RuleResult.Assert(!string.IsNullOrEmpty(NameTemp), "Name is required"));

            Validator.AddRule(nameof(ShortcutTemp), 
                () => RuleResult.Assert(
                    ShortcutTemp != null && 
                    (ShortcutTemp.Key != Key.None || ShortcutTemp.KeyModifiers != KeyModifier.None), "HotKey is required"));

            Validator.AddRule(nameof(SelectedDevice),
                () => RuleResult.Assert(SelectedDevice != null, "HotKey is required"));
        }

        #endregion

        #region Methods

        private void EditConfiguredHotKey()
        {
            OnEditHotKeyAction?.Invoke(this);            
        }

        private void ApplyConfiguredHotKey()
        {
            OnHotKeyAppliedAction?.Invoke(Shortcut);
        }

        private void CloseEditConfiguredHotKey()
        {
            OnCancelEditHotKeyAction?.Invoke(this);
        }

        private void SaveConfiguredHotKey()
        {
            if (!Validator.ValidateAll().IsValid)
                return;

            Name = NameTemp;
            Mode = ModeTemp;
            Shortcut?.Unregister();
            Shortcut?.Dispose();
            Shortcut = ShortcutTemp;
            Shortcut.Register();
            AudioDevice = new AudioDeviceViewModel()
            {
                DeviceId = SelectedDevice.ID,
                FriendlyName = SelectedDevice.FriendlyName
            };

            OnSaveHotKeyAction(this);
            CloseEditConfiguredHotKey();
        }

        public void HotKeyRecorded(Key key, KeyModifier modifier)
        {
            ShortcutTemp = new HotKey(key, modifier, null, false);
        }

        private void DeleteConfiguredHotKey()
        {
            if (MessageBox.Show("Really delete?", "Delete HotKey", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                OnDeleteHotKeyAction?.Invoke(this);
        }

        #endregion
    }
}
