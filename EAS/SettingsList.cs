using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAS
{
    public class SettingsList
    {
        #region Globals

        private const string SettingsFileName = "settings.json";
        private static string SettingsDirectory = string.Empty;
        private static string SettingsFileFullPath = string.Empty;

        #endregion

        #region Properties

        public static SettingsList Instance { get; set; }


        public bool StartWithWindows { get; set; }

        public bool MinimizeOnExit { get; set; }

        public bool StartMinimized { get; set; }

        #endregion

        #region Constructor

        public SettingsList()
        {
            Instance = this;
        }

        #endregion

        #region Methods

        public static void Save()
        {
            // Check existens
            if (!Directory.Exists(SettingsDirectory))
                Directory.CreateDirectory(SettingsDirectory);

            if (!File.Exists(SettingsFileFullPath))
                File.Create(SettingsFileFullPath).Close();

            string json = JsonConvert.SerializeObject(Instance, Formatting.Indented);
            File.WriteAllText(SettingsFileFullPath, json);
        }

        public static void Load()
        {
            try
            {
                // Get full path
                string appdataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                SettingsDirectory = Path.Combine(appdataLocalPath, "EAS");
                SettingsFileFullPath = Path.Combine(SettingsDirectory, SettingsFileName);

                string json = File.ReadAllText(SettingsFileFullPath);
                Instance = JsonConvert.DeserializeObject<SettingsList>(json);
            }
            catch (Exception ex)
            {
                Instance = new SettingsList();
                Instance.MinimizeOnExit = true;
                Instance.StartWithWindows = false;
                Instance.StartMinimized = false;
            }
        }

        #endregion
    }
}
