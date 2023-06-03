using System;
using Sedulous.Audio;
using Sedulous.BASS.Messages;
using Sedulous.Core;
using Sedulous.Core.Messages;
using static Sedulous.BASS.Native.BASSNative;

namespace Sedulous.BASS.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the <see cref="SoundEffectPlayer"/> class.
    /// </summary>
    public sealed class BASSSoundEffectPlayer : SoundEffectPlayer,
        IMessageSubscriber<FrameworkMessageID>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BASSSoundEffectPlayer"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public BASSSoundEffectPlayer(FrameworkContext context)
            : base(context)
        {
            context.Messages.Subscribe(this, BASSMessages.BASSDeviceChanged);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageID>.ReceiveMessage(FrameworkMessageID type, MessageData data)
        {
            if (type == BASSMessages.BASSDeviceChanged)
            {
                StopInternal();

                if (BASSUtil.IsValidHandle(sample))
                {
                    var deviceID = ((BASSDeviceChangedMessageData)data).DeviceID;
                    if (!BASS_ChannelSetDevice(sample, deviceID))
                        throw new BASSException();
                }
                return;
            }
        }

        /// <inheritdoc/>
        public override void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public override Boolean Play(SoundEffect soundEffect, Boolean loop = false)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return PlayInternal(soundEffect, 1f, 0f, 0f, loop);
        }

        /// <inheritdoc/>
        public override Boolean Play(SoundEffect soundEffect, Single volume, Single pitch, Single pan, Boolean loop = false)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return PlayInternal(soundEffect, volume, pitch, pan, loop);
        }

        /// <inheritdoc/>
        public override void Stop()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (BASSUtil.IsValidHandle(channel) && !BASS_ChannelStop(channel))
                throw new BASSException();

            StopInternal();
        }

        /// <inheritdoc/>
        public override void Pause()
        {
            Contract.EnsureNotDisposed(this, Disposed);
            
            if (State == PlaybackState.Playing)
            {
                if (!BASS_ChannelPause(channel))
                    throw new BASSException();
            }
        }

        /// <inheritdoc/>
        public override void Resume()
        {
            Contract.EnsureNotDisposed(this, Disposed);
            
            if (State == PlaybackState.Paused)
            {
                if (!BASS_ChannelPlay(channel, false))
                    throw new BASSException();
            }
        }

        /// <inheritdoc/>
        public override void SlideVolume(Single volume, TimeSpan time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

            BASSUtil.SlideVolume(channel, volume, time);
        }

        /// <inheritdoc/>
        public override void SlidePitch(Single pitch, TimeSpan time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

            /* No-op */
        }

        /// <inheritdoc/>
        public override void SlidePan(Single pan, TimeSpan time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

            BASSUtil.SlidePan(channel, pan, time);
        }

        /// <inheritdoc/>
        public override PlaybackState State
        {
            get
            {
                if (BASSUtil.IsValidHandle(channel))
                {
                    switch (BASS_ChannelIsActive(channel))
                    {
                        case BASS_ACTIVE_STALLED:
                            return PlaybackState.Playing;

                        case BASS_ACTIVE_PLAYING:
                            return PlaybackState.Playing;

                        case BASS_ACTIVE_STOPPED:
                            return PlaybackState.Stopped;

                        case BASS_ACTIVE_PAUSED:
                            return PlaybackState.Stopped;
                    }
                }
                return PlaybackState.Stopped;
            }
        }

        /// <inheritdoc/>
        public override Boolean IsPlaying
        {
            get => State == PlaybackState.Playing;
        }

        /// <inheritdoc/>
        public override Boolean IsLooping
        {
            get => IsHandleValid() ? BASSUtil.GetIsLooping(channel) : false;
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

                BASSUtil.SetIsLooping(channel, value);
            }
        }

        /// <inheritdoc/>
        public override TimeSpan Position
        {
            get
            {
                if (!IsHandleValid())
                    return TimeSpan.Zero;

                if (!BASSUtil.IsValidHandle(stream))
                {
                    var position = BASSUtil.GetPositionInSeconds(channel);
                    return TimeSpan.FromSeconds(position);
                }
                else
                {
                    var position = BASS_ChannelBytes2Seconds(channel, (ulong)sampleDataPosition);
                    if (position < 0)
                        throw new BASSException();

                    return TimeSpan.FromSeconds(position);
                }
            }
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

                if (value.TotalSeconds < 0 || value > Duration)
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (BASSUtil.IsValidHandle(stream))
                {
                    var position = BASS_ChannelSeconds2Bytes(channel, value.TotalSeconds);
                    if (!BASSUtil.IsValidValue(position))
                        throw new BASSException();

                    sampleDataPosition = (int)position;
                }
                else
                {
                    BASSUtil.SetPositionInSeconds(channel, value.TotalSeconds);
                }
            }
        }

        /// <inheritdoc/>
        public override TimeSpan Duration
        {
            get => IsHandleValid() ? BASSUtil.GetDurationAsTimeSpan(channel) : TimeSpan.Zero;
        }

        /// <inheritdoc/>
        public override Single Volume
        {
            get => IsHandleValid() ? BASSUtil.GetVolume(channel) : 1f;
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

                BASSUtil.SetVolume(channel, MathUtil.Clamp(value, 0f, 1f));
            }
        }

        /// <inheritdoc/>
        public override Single Pitch
        {
            get { return 0f; }
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);
            }
        }

        /// <inheritdoc/>
        public override Single Pan
        {
            get => IsHandleValid() ? BASSUtil.GetPan(channel) : 0f;
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(BASSStrings.NotCurrentlyValid);

                BASSUtil.SetPan(channel, MathUtil.Clamp(value, -1f, 1f));
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (FrameworkContext != null && !FrameworkContext.Disposed)
                StopInternal();

            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Gets a value indicating whether the channel is in a valid state.
        /// </summary>
        /// <returns>true if the channel is in a valid state; otherwise, false.</returns>
        private Boolean IsHandleValid()
        {
            if (playing != null && playing.Disposed)
            {
                StopInternal();
            }
            return State != PlaybackState.Stopped;
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        private Boolean PlayInternal(SoundEffect soundEffect, Single volume, Single pitch, Single pan, Boolean loop = false)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            // Stop any sound that's already playing.
            Stop();

            // Retrieve the sample data from the sound effect.
            FrameworkContext.ValidateResource(soundEffect);
            var bassfx = (BASSSoundEffect)soundEffect;
            var sample = bassfx.GetSampleInfo(out _, out _);

            // Get a channel on which to play the sample.
            channel = BASS_SampleGetChannel(sample, true);
            if (!BASSUtil.IsValidHandle(channel))
            {
                var error = BASS_ErrorGetCode();
                if (error == BASS_ERROR_NOCHAN)
                    return false;

                throw new BASSException(error);
            }

            // Set the channel's attributes.
            BASSUtil.SetIsLooping(channel, loop);
            BASSUtil.SetVolume(channel, MathUtil.Clamp(volume, 0f, 1f));
            BASSUtil.SetPan(channel, MathUtil.Clamp(pan, -1f, 1f));

            // Play the channel.
            if (!BASS_ChannelPlay(channel, true))
                throw new BASSException();

            this.playing = soundEffect;

            return true;
        }

        /// <summary>
        /// Releases any BASS resources being held by the player.
        /// </summary>
        private Boolean StopInternal()
        {
            if (stream != 0)
            {
                if (!BASS_StreamFree(stream))
                    throw new BASSException();

                stream = 0;
            }

            if (sample != 0)
            {
                if (!BASS_SampleFree(sample))
                    throw new BASSException();

                sample = 0;
            }

            channel = 0;
            playing = null;

            return true;
        }

        // The currently-playing BASS resources.
        private Int32 sampleDataPosition;
        private UInt32 sample;
        private UInt32 stream;
        private UInt32 channel;
        private SoundEffect playing;
    }
}
