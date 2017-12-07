using CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAS
{
    public class Audio
    {
        #region Globals

        private MMDeviceEnumerator _deviceEnumerator;
        private PolicyConfigClient _client;

        #endregion

        #region Properties

        #endregion

        #region Constructor

        public Audio()
        {
            _deviceEnumerator = new MMDeviceEnumerator();
            _client = new PolicyConfigClient();
        }

        #endregion

        #region Methods

        public List<MMDevice> GetDevices()
        {
            MMDeviceCollection devices = _deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, EDeviceState.DEVICE_STATE_ACTIVE);

            List<MMDevice> result = new List<MMDevice>();
            for (int i = 0; i < devices.Count; i++)
                result.Add(devices[i]);

            return result;
        }

        public MMDevice GetDefaultDevice()
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
        }

        public void SetDefaultDevice(string deviceId, ERole role)
        {
            _client.SetDefaultEndpoint(deviceId, role);
        }


        #endregion
    }
}
