﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PlaybackSoundSwitch.Device
{
    /// <summary>
    /// MM Device
    /// </summary>
    public class MMDevice : IDisposable
    {
        #region Variables
        private readonly IMMDevice deviceInterface;
        private PropertyStore _propertyStore;
        //private AudioMeterInformation audioMeterInformation;
        //private AudioEndpointVolume audioEndpointVolume;
        //private AudioSessionManager audioSessionManager;
        //private DeviceTopology deviceTopology;
        #endregion

        #region Guids
        // ReSharper disable InconsistentNaming
        private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
        private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");
        private static Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");
        private static Guid IDD_IDeviceTopology = new Guid("2A07407E-6497-4A18-9787-32F79BD0D98F");
        // ReSharper restore InconsistentNaming
        #endregion

        #region Init
        /// <summary>
        /// Initializes the device's property store.
        /// </summary>
        /// <param name="stgmAccess">The storage-access mode to open store for.</param>
        /// <remarks>Administrative client is required for Write and ReadWrite modes.</remarks>
        public void GetPropertyInformation(StorageAccessMode stgmAccess = StorageAccessMode.Read)
        {
            Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(stgmAccess, out var propstore));
            _propertyStore = new PropertyStore(propstore);
        }

        //private AudioClient GetAudioClient()
        //{
        //    Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioClient, ClsCtx.ALL, IntPtr.Zero, out var result));
        //    return new AudioClient(result as IAudioClient);
        //}

        //private void GetAudioMeterInformation()
        //{
        //    Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioMeterInformation, ClsCtx.ALL, IntPtr.Zero, out var result));
        //    audioMeterInformation = new AudioMeterInformation(result as IAudioMeterInformation);
        //}

        //private void GetAudioEndpointVolume()
        //{
        //    Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IID_IAudioEndpointVolume, ClsCtx.ALL, IntPtr.Zero, out var result));
        //    audioEndpointVolume = new AudioEndpointVolume(result as IAudioEndpointVolume);
        //}

        //private void GetAudioSessionManager()
        //{
        //    Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IAudioSessionManager, ClsCtx.ALL, IntPtr.Zero, out var result));
        //    audioSessionManager = new AudioSessionManager(result as IAudioSessionManager);
        //}

        //private void GetDeviceTopology()
        //{
        //    Marshal.ThrowExceptionForHR(deviceInterface.Activate(ref IDD_IDeviceTopology, ClsCtx.ALL, IntPtr.Zero, out var result));
        //    deviceTopology = new DeviceTopology(result as IDeviceTopology);
        //}

        #endregion

        #region Properties

        ///// <summary>
        ///// Audio Client
        ///// Makes a new one each call to allow caller to manage when to dispose
        ///// n.b. should probably not be a property anymore
        ///// </summary>
        //public AudioClient AudioClient => GetAudioClient();

        ///// <summary>
        ///// Audio Meter Information
        ///// </summary>
        //public AudioMeterInformation AudioMeterInformation
        //{
        //    get
        //    {
        //        if (audioMeterInformation == null)
        //            GetAudioMeterInformation();

        //        return audioMeterInformation;
        //    }
        //}

        ///// <summary>
        ///// Audio Endpoint Volume
        ///// </summary>
        //public AudioEndpointVolume AudioEndpointVolume
        //{
        //    get
        //    {
        //        if (audioEndpointVolume == null)
        //            GetAudioEndpointVolume();

        //        return audioEndpointVolume;
        //    }
        //}

        ///// <summary>
        ///// AudioSessionManager instance
        ///// </summary>
        //public AudioSessionManager AudioSessionManager
        //{
        //    get
        //    {
        //        if (audioSessionManager == null)
        //        {
        //            GetAudioSessionManager();
        //        }
        //        return audioSessionManager;
        //    }
        //}

        ///// <summary>
        ///// DeviceTopology instance
        ///// </summary>
        //public DeviceTopology DeviceTopology
        //{
        //    get
        //    {
        //        if (deviceTopology == null)
        //        {
        //            GetDeviceTopology();
        //        }
        //        return deviceTopology;
        //    }
        //}

        /// <summary>
        /// Properties
        /// </summary>
        public PropertyStore Properties
        {
            get
            {
                if (_propertyStore == null)
                    GetPropertyInformation();
                return _propertyStore;
            }
        }

        /// <summary>
        /// Friendly name for the endpoint
        /// </summary>
        public string FriendlyName
        {
            get
            {
                if (_propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (_propertyStore.Contains(PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME))
                {
                    return (string)_propertyStore[PropertyKeys.PKEY_DEVICE_FRIENDLY_NAME].Value;
                }
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Friendly name of device
        /// </summary>
        public string DeviceFriendlyName
        {
            get
            {
                if (_propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (_propertyStore.Contains(PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME))
                {
                    return (string)_propertyStore[PropertyKeys.PKEY_DEVICE_INTERFACE_FRIENDLY_NAME].Value;
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// Icon path of device
        /// </summary>
        public string IconPath
        {
            get
            {
                if (_propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (_propertyStore.Contains(PropertyKeys.PKEY_DEVICE_ICON))
                {
                    return (string)_propertyStore[PropertyKeys.PKEY_DEVICE_ICON].Value;
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Device Instance Id of Device
        /// </summary>
        public string InstanceId
        {
            get
            {
                if (_propertyStore == null)
                {
                    GetPropertyInformation();
                }
                if (_propertyStore.Contains(PropertyKeys.PKEY_Device_InstanceId))
                {
                    return (string)_propertyStore[PropertyKeys.PKEY_Device_InstanceId].Value;
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Device ID
        /// </summary>
        public string ID
        {
            get
            {
                Marshal.ThrowExceptionForHR(deviceInterface.GetId(out var result));
                return result;
            }
        }

        /// <summary>
        /// Data Flow
        /// </summary>
        public EDataFlow DataFlow
        {
            get
            {
                var ep = deviceInterface as IMMEndpoint;
                ep.GetDataFlow(out var result);
                return result;
            }
        }

        /// <summary>
        /// Device State
        /// </summary>
        public EDeviceState State
        {
            get
            {
                Marshal.ThrowExceptionForHR(deviceInterface.GetState(out var result));
                return result;
            }
        }

        #endregion

        #region Constructor
        internal MMDevice(IMMDevice realDevice)
        {
            deviceInterface = realDevice;
        }
        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return FriendlyName;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //this.audioEndpointVolume?.Dispose();
            //this.audioSessionManager?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MMDevice()
        {
            Dispose();
        }
    }
}
