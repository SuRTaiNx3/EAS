using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAS.ViewModels
{
    public class AudioDeviceViewModel : BaseViewModel
    {
        #region Globals

        #endregion

        #region Properties

        private string _deviceId;
        public string DeviceId
        {
            get { return _deviceId; }
            set
            {
                _deviceId = value;
                OnPropertyChanged("DeviceId");
            }
        }

        private string _friendlyName;
        public string FriendlyName
        {
            get { return _friendlyName; }
            set
            {
                _friendlyName = value;
                OnPropertyChanged("FriendlyName");
            }
        }


        #endregion

        #region Constructor

        public AudioDeviceViewModel(){}

        public AudioDeviceViewModel(string deviceId, string friendlyName)
        {
            DeviceId = deviceId;
            FriendlyName = friendlyName;
        }

        #endregion

        #region Methods

        #endregion
    }
}
