using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EAS.Extensions;
using System.Collections.Specialized;
using EAS.Common;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace EAS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Globals

        private Audio _audio;

        private const string SettingsFileName = "hotkeys.json";
        private string SettingsDirectory = string.Empty;
        private string SettingsFileFullPath = string.Empty;

        #endregion

        #region Properties

        public static MainViewModel Instance { get; set; }

        private ObservableCollection<MMDevice> _devices;
        public ObservableCollection<MMDevice> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        private ObservableCollection<ConfiguredHotKeyViewModel> _hotKeys;
        public ObservableCollection<ConfiguredHotKeyViewModel> HotKeys
        {
            get { return _hotKeys; }
            set
            {
                _hotKeys = value;
                _hotKeys.CollectionChanged += HotKeys_CollectionChanged;
                OnPropertyChanged("HotKeys");
            }
        }

        private bool _isEditDialogVisible = false;
        public bool IsEditDialogVisible
        {
            get { return _isEditDialogVisible; }
            set
            {
                _isEditDialogVisible = value;
                OnPropertyChanged("IsEditDialogVisible");
            }
        }

        private ConfiguredHotKeyViewModel _hotKeyToEdit;
        public ConfiguredHotKeyViewModel HotKeyToEdit
        {
            get { return _hotKeyToEdit; }
            set
            {
                _hotKeyToEdit = value;
                OnPropertyChanged("HotKeyToEdit");
            }
        }

        public Action OnExitApplication { get; set; }

        #endregion

        #region Commands

        private RelayCommand _addHotKeyCommand;
        public RelayCommand AddHotKeyCommand
        {
            get
            {
                if (_addHotKeyCommand == null)
                    _addHotKeyCommand = new RelayCommand(a => AddHotKey(), b => true);
                return _addHotKeyCommand;
            }
        }

        private RelayCommand _showSettingsCommand;
        public RelayCommand ShowSettingsCommand
        {
            get
            {
                if (_showSettingsCommand == null)
                    _showSettingsCommand = new RelayCommand(a => ShowSettings(), b => true);
                return _showSettingsCommand;
            }
        }

        private RelayCommand _exitApplicationCommand;
        public RelayCommand ExitApplicationCommand
        {
            get
            {
                if (_exitApplicationCommand == null)
                    _exitApplicationCommand = new RelayCommand(a => Exit(), b => true);
                return _exitApplicationCommand;
            }
        }

        #endregion


        #region Constructor

        public MainViewModel()
        {
            Instance = this;
            SettingsList.Load();
            _audio = new Audio();
            Devices = _audio.GetDevices().ToObservableCollection();

            // Get full path
            string appdataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            SettingsDirectory = Path.Combine(appdataLocalPath, "EAS");
            SettingsFileFullPath = Path.Combine(SettingsDirectory, SettingsFileName);

            LoadHotKeysFromFile();
            RegisterAllHotKeys();
            ReloadHotKeys();
        }

        #endregion

        #region Methods

        private void HotKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HotKeys");
        }

        private void OnHotKeyHandler(HotKey hotkey)
        {
            ConfiguredHotKeyViewModel vm = HotKeys.FirstOrDefault(h => h.Shortcut.Id == hotkey.Id);
            ERole role = vm.Mode == Enums.Modes.Communication ? ERole.eCommunications : ERole.eMultimedia;
            _audio.SetDefaultDevice(vm.AudioDevice.DeviceId, role);
        }

        private void EditConfiguredHotKey(ConfiguredHotKeyViewModel vm)
        {
            HotKeyToEdit = vm;
            HotKeyToEdit.Shortcut?.Unregister();
            IsEditDialogVisible = true;
        }

        private void AddHotKey()
        {
            HotKeyToEdit = new ConfiguredHotKeyViewModel();
            HotKeyToEdit.IsNew = true;
            HotKeyToEdit.OnCancelEditHotKeyAction = CloseEditDialog;
            HotKeyToEdit.OnSaveHotKeyAction = SaveHotKey;
            HotKeyToEdit.OnEditHotKeyAction = EditConfiguredHotKey;
            HotKeyToEdit.OnHotKeyAppliedAction = OnHotKeyHandler;
            HotKeyToEdit.OnDeleteHotKeyAction = DeleteHotKey;
            IsEditDialogVisible = true;
        }
        
        private void CloseEditDialog(ConfiguredHotKeyViewModel vm)
        {
            IsEditDialogVisible = false;
            HotKeyToEdit = null;
        }

        private void SaveHotKey(ConfiguredHotKeyViewModel vm)
        {
            if (vm.IsNew)
            {
                HotKeys.Add(vm);
                vm.IsNew = false;
                OnPropertyChanged("HotKeys");
            }

            ReloadHotKeys();
            SaveHotKeysToFile();
        }

        private void DeleteHotKey(ConfiguredHotKeyViewModel vm)
        {
            HotKeys.Remove(vm);
            CloseEditDialog(vm);
            SaveHotKeysToFile();
        }

        private void ReloadHotKeys()
        {
            if (HotKeys == null)
                return;

            foreach (ConfiguredHotKeyViewModel hotkey in HotKeys)
            {
                hotkey.Shortcut.Action = OnHotKeyHandler;
                hotkey.OnCancelEditHotKeyAction = CloseEditDialog;
                hotkey.OnSaveHotKeyAction = SaveHotKey;
                hotkey.OnEditHotKeyAction = EditConfiguredHotKey;
                hotkey.OnHotKeyAppliedAction = OnHotKeyHandler;
                hotkey.OnDeleteHotKeyAction = DeleteHotKey;
            }
        }

        private void RegisterAllHotKeys()
        {
            if (HotKeys == null)
                return;

            foreach (ConfiguredHotKeyViewModel hotkey in HotKeys)
                hotkey.Shortcut.Register();
        }

        public void SaveHotKeysToFile()
        {
            // Check existens
            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);

            if (!File.Exists(SettingsFileFullPath))
                File.Create(SettingsFileFullPath).Close();

            string json = JsonConvert.SerializeObject(HotKeys, Formatting.Indented);
            File.WriteAllText(SettingsFileFullPath, json);
        }

        private void LoadHotKeysFromFile()
        {
            try
            {
                string json = File.ReadAllText(SettingsFileFullPath);
                HotKeys = JsonConvert.DeserializeObject<List<ConfiguredHotKeyViewModel>>(json).ToObservableCollection();
            }
            catch (Exception ex)
            {
                HotKeys = new ObservableCollection<ConfiguredHotKeyViewModel>();
            }
        }

        private void ShowSettings()
        {
            Settings settings = new Settings();
            SettingsViewModel settingsVM = new SettingsViewModel();
            settingsVM.OnCloseSettingsAction = () => { settings.Close(); };
            settings.DataContext = settingsVM;
            settings.ShowDialog();
        }

        private void Exit()
        {
            OnExitApplication?.Invoke();
        } 

        #endregion
    }
}
