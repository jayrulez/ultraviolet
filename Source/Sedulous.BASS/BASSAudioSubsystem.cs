﻿using System;
using System.Collections.Generic;
using Sedulous.Audio;
using Sedulous.BASS.Audio;
using Sedulous.BASS.Messages;
using Sedulous.BASS.Native;
using Sedulous.Core;
using Sedulous.Core.Messages;
using static Sedulous.BASS.Native.BASSNative;

namespace Sedulous.BASS
{
    /// <summary>
    /// Represents the BASS implementation of the Sedulous audio subsystem.
    /// </summary>
    public sealed unsafe class BASSAudioSubsystem : FrameworkResource, IAudioSubsystem, IMessageSubscriber<FrameworkMessageID>
    {
        /// <summary>
        /// Initializes a new instance of the BASSSedulousAudio class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public BASSAudioSubsystem(FrameworkContext context)
            : base(context)
        {
            this.Capabilities = new BASSAudioCapabilities();

            if (context.Platform == FrameworkPlatform.Windows || context.Platform == FrameworkPlatform.macOS)
            {
                if (!BASS_SetConfig(BASS_CONFIG_DEV_DEFAULT, 1))
                {
                    var setConfigError = BASS_ErrorGetCode();
                    if (setConfigError != BASS_ERROR_NOTAVAIL && setConfigError != BASS_ERROR_ILLPARAM)
                        throw new BASSException(setConfigError);
                }
            }

            var device = -1;
            var freq = 44100u;
            if (!BASS_Init(device, freq, 0, IntPtr.Zero, IntPtr.Zero))
                throw new BASSException();

            UpdateAudioDevices();
            PlaybackDevice = GetDefaultDevice();

            context.Messages.Subscribe(this, FrameworkMessages.ApplicationSuspending);
            context.Messages.Subscribe(this, FrameworkMessages.ApplicationResumed);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageID>.ReceiveMessage(FrameworkMessageID type, MessageData data)
        {
            if (type == FrameworkMessages.ApplicationSuspending)
            {
                if (!suspended)
                {
                    awaitingResume = true;
                    Suspend();
                }
                return;
            }

            if (type == FrameworkMessages.ApplicationResumed)
            {
                if (awaitingResume)
                {
                    awaitingResume = false;
                    Resume();
                }
                return;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IAudioDevice> EnumerateAudioDevices()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return knownAudioDevices;
        }

        /// <inheritdoc/>
        public IAudioDevice FindAudioDeviceByName(String name)
        {
            Contract.Require(name, nameof(name));
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var device in knownAudioDevices)
            {
                if (device.IsValid && String.Equals(name, device.Name, StringComparison.Ordinal))
                    return device;
            }

            return null;
        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            UpdateAudioDevices();

            OnUpdating(time);
        }

        /// <inheritdoc/>
        public void Suspend()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (!BASS_Pause())
                throw new BASSException();

            suspended = true;
        }

        /// <inheritdoc/>
        public void Resume()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (!BASS_Start())
                throw new BASSException();

            suspended = false;
        }

        /// <inheritdoc/>
        public IAudioDevice PlaybackDevice
        {
            get => playbackDevice;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var val = value ?? GetDefaultDevice();
                if (val != PlaybackDevice)
                {
                    if (val == null)
                    {
                        if (!BASS_Free())
                            throw new BASSException();

                        if (!BASS_SetDevice(0))
                            throw new BASSException();

                        playbackDevice = null;
                    }
                    else
                    {
                        if (val is BASSAudioDevice device)
                        {
                            FrameworkContext.ValidateResource(device);

                            var oldDevice = (playbackDevice == null) ? 0u : BASS_GetDevice();

                            if (!BASS_Init((int)device.ID, 44100u, 0, IntPtr.Zero, IntPtr.Zero))
                            {
                                var error = BASS_ErrorGetCode();
                                if (error != BASS_ERROR_ALREADY)
                                    throw new BASSException(error);
                            }

                            if (FrameworkContext != null && !FrameworkContext.Disposed)
                            {
                                var data = FrameworkContext.Messages.CreateMessageData<BASSDeviceChangedMessageData>();
                                data.DeviceID = device.ID;
                                FrameworkContext.Messages.PublishImmediate(BASSMessages.BASSDeviceChanged, data);
                            }

                            if (oldDevice > 0)
                            {
                                if (!BASS_SetDevice(oldDevice))
                                    throw new BASSException();

                                if (!BASS_Free())
                                    throw new BASSException();

                                if (!BASS_SetDevice(device.ID))
                                    throw new BASSException();
                            }

                            playbackDevice = device;
                        }
                        else throw new ArgumentException(nameof(value));
                    }
                }
            }
        }

        /// <inheritdoc/>
        public AudioCapabilities Capabilities { get; }

        /// <inheritdoc/>
        public Single AudioMasterVolume 
        {
            get => audioMasterVolume;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var audioMasterVolumeClamped = MathUtil.Clamp(value, 0f, 1f);
                if (audioMasterVolumeClamped != audioMasterVolume)
                {
                    audioMasterVolume = audioMasterVolumeClamped;

                    UpdateSampleVolume();
                    UpdateStreamVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Single SongsMasterVolume
        {
            get => songsMasterVolume;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var songsMasterVolumeClamped = MathUtil.Clamp(value, 0f, 1f);
                if (songsMasterVolumeClamped != songsMasterVolume)
                {
                    songsMasterVolume = songsMasterVolumeClamped;

                    UpdateStreamVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Single SoundEffectsMasterVolume
        {
            get => soundEffectsMasterVolume;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var soundEffectsMasterVolumeClamped = MathUtil.Clamp(value, 0f, 1f);
                if (soundEffectsMasterVolumeClamped != soundEffectsMasterVolume)
                {
                    soundEffectsMasterVolume = soundEffectsMasterVolumeClamped;

                    UpdateSampleVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Boolean AudioMuted 
        {
            get => audioMuted;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (audioMuted != value)
                {
                    audioMuted = value;

                    UpdateStreamVolume();
                    UpdateSampleVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Boolean SongsMuted
        {
            get => songsMuted;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (songsMuted != value)
                {
                    songsMuted = value;

                    UpdateStreamVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Boolean SoundEffectsMuted
        {
            get => soundEffectsMuted;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (soundEffectsMuted != value)
                {
                    soundEffectsMuted = value;

                    UpdateSampleVolume();
                }
            }
        }

        /// <inheritdoc/>
        public event FrameworkSubsystemUpdateEventHandler Updating;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (!BASS_Free())
                throw new BASSException();

            if (disposing && !FrameworkContext.Disposed)
            {
                FrameworkContext.Messages.Unsubscribe(this);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the Updating event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        private void OnUpdating(FrameworkTime time) =>
            Updating?.Invoke(this, time);

        /// <summary>
        /// Updates the global volume of streams to match the subsystem's current settings.
        /// </summary>
        private void UpdateStreamVolume()
        {
            var volumeStream = (audioMuted || songsMuted) ? 0 : (uint)(10000 * audioMasterVolume * songsMasterVolume);
            if (!BASS_SetConfig(BASS_CONFIG_GVOL_STREAM, volumeStream))
                throw new BASSException();
        }

        /// <summary>
        /// Updates the global volume of samples to match the subsystem's current settings.
        /// </summary>
        private void UpdateSampleVolume()
        {
            var volumeSample = (audioMuted || soundEffectsMuted) ? 0 : (uint)(10000 * audioMasterVolume * soundEffectsMasterVolume);
            if (!BASS_SetConfig(BASS_CONFIG_GVOL_SAMPLE, volumeSample))
                throw new BASSException();
        }

        /// <summary>
        /// Updates the list of audio devices.
        /// </summary>
        private void UpdateAudioDevices()
        {
            BASS_DEVICEINFO info;
            for (int i = 1; BASS_GetDeviceInfo((uint)i, &info); i++)
            {
                var isEnabled = (info.flags & BASS_DEVICE_ENABLED) == BASS_DEVICE_ENABLED;
                var isDefault = (info.flags & BASS_DEVICE_DEFAULT) == BASS_DEVICE_DEFAULT;

                var ix = (i - 1);
                if (ix >= knownAudioDevices.Count)
                {
                    var marshalledInfo = info.ToMarshalledStruct();
                    knownAudioDevices.Add(new BASSAudioDevice(FrameworkContext, (uint)i, marshalledInfo.name));
                }

                var device = knownAudioDevices[ix];
                device.IsValid = isEnabled;
                device.IsDefault = isDefault;
            }

            if (playbackDevice != null && !playbackDevice.IsValid)
                this.PlaybackDevice = GetDefaultDevice();
        }

        /// <summary>
        /// Gets the default audio device.
        /// </summary>
        private BASSAudioDevice GetDefaultDevice()
        {
            foreach (var device in knownAudioDevices)
            {
                if (device.IsValid && device.IsDefault)
                    return device;
            }
            return null;
        }

        // Property values.
        private Single audioMasterVolume = 1.0f;
        private Single songsMasterVolume = 1.0f;
        private Single soundEffectsMasterVolume = 1.0f;
        private Boolean audioMuted;
        private Boolean songsMuted;
        private Boolean soundEffectsMuted;

        // State values.
        private Boolean suspended;
        private Boolean awaitingResume;

        // Audio device cache.
        private IAudioDevice playbackDevice;
        private List<BASSAudioDevice> knownAudioDevices = 
            new List<BASSAudioDevice>();
    }
}
