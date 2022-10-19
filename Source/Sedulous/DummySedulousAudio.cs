using System;
using System.Collections.Generic;
using System.Linq;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="ISedulousAudio"/>.
    /// </summary>
    public sealed class DummySedulousAudio : SedulousResource, ISedulousAudio
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummySedulousAudio"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public DummySedulousAudio(SedulousContext uv)
            : base(uv)
        {

        }

        /// <inheritdoc/>
        public IEnumerable<ISedulousAudioDevice> EnumerateAudioDevices()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return Enumerable.Empty<ISedulousAudioDevice>();
        }

        /// <inheritdoc/>
        public ISedulousAudioDevice FindAudioDeviceByName(String name)
        {
            return null;
        }

        /// <inheritdoc/>
        public void Update(SedulousTime time)
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
        public ISedulousAudioDevice PlaybackDevice
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
        public event SedulousSubsystemUpdateEventHandler Updating;
    }
}
