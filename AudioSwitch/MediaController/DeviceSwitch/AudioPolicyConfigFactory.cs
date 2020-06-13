﻿using PlaybackSoundSwitch.ComObjects;

namespace PlaybackSoundSwitch.DeviceSwitch
{
    internal sealed class AudioPolicyConfigFactory
    {
        public static IAudioPolicyConfigFactory Create()
        {
            var iid = typeof(IAudioPolicyConfigFactory).GUID;
            object factory;
            ComBase.RoGetActivationFactory("Windows.Media.Internal.AudioPolicyConfig", ref iid, out factory);
            return (IAudioPolicyConfigFactory)factory;
        }
    }
}