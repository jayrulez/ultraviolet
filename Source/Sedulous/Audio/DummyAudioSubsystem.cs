using System;
using System.Collections.Generic;
using System.Linq;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="IAudioSubsystem"/>.
    /// </summary>
    public sealed class DummyAudioSubsystem : FrameworkResource, IAudioSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyAudioSubsystem"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public DummyAudioSubsystem(FrameworkContext context)
            : base(context)
        {

        }

        /// <inheritdoc/>
        public IEnumerable<IAudioDevice> EnumerateAudioDevices()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return Enumerable.Empty<IAudioDevice>();
        }

        /// <inheritdoc/>
        public IAudioDevice FindAudioDeviceByName(String name)
        {
            return null;
        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            Updating?.Invoke(this, time);
        }

        /// <inheritdoc/>
        public void Suspend()
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public void Resume()
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public IAudioDevice PlaybackDevice
        {
            get { return null; }
            set { }
        }

        /// <inheritdoc/>
        public AudioCapabilities Capabilities { get; } = new DummyAudioCapabilities();

        /// <inheritdoc/>
        public Single AudioMasterVolume
        {
            get { return 1f; }
            set { }
        }

        /// <inheritdoc/>
        public Single SongsMasterVolume
        {
            get { return 1f; }
            set { }
        }

        /// <inheritdoc/>
        public Single SoundEffectsMasterVolume
        {
            get { return 1f; }
            set { }
        }

        /// <inheritdoc/>
        public Boolean AudioMuted
        {
            get { return false; }
            set { }
        }

        /// <inheritdoc/>
        public Boolean SongsMuted
        {
            get { return false; }
            set { }
        }

        /// <inheritdoc/>
        public Boolean SoundEffectsMuted
        {
            get { return false; }
            set { }
        }

        /// <inheritdoc/>
        public event FrameworkSubsystemUpdateEventHandler Updating;
    }
}
