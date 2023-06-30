using System;
using System.Collections.Generic;
using Sedulous.Audio;

namespace Sedulous
{
    /// <summary>
    /// Initializes a new instance of the IAudioSubsystem implementation.
    /// </summary>
    /// <param name="context">The Framework context.</param>
    /// <param name="configuration">The Framework configuration settings for the current context.</param>
    public delegate IAudioSubsystem FrameworkAudioFactory(FrameworkContext context, FrameworkConfiguration configuration);

    /// <summary>
    /// Represents the Framework's audio subsystem.
    /// </summary>
    public interface IAudioSubsystem : IFrameworkSubsystem
    {
        /// <summary>
        /// Produces an enumeration of the system's currently installed audio devices.
        /// </summary>
        /// <returns>A collection which contains the system's currently installed audio devices.</returns>
        IEnumerable<IAudioDevice> EnumerateAudioDevices();

        /// <summary>
        /// Searches the list of known audio devices for a valid device with the specified name.
        /// </summary>
        /// <param name="name">The name of the audio device for which to search.</param>
        /// <returns>The first valid audio device with the specified name, if it is found; otherwise, <see langword="null"/>.</returns>
        IAudioDevice FindAudioDeviceByName(String name);

        /// <summary>
        /// Suspends all audio output.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes audio output after a call to <see cref="Suspend"/>.
        /// </summary>
        void Resume();

        /// <summary>
        /// Gets or sets the device which is used for audio playback.
        /// </summary>
        IAudioDevice PlaybackDevice { get; set; }

        /// <summary>
        /// Gets a <see cref="AudioCapabilities"/> object which exposes the capabilities of the current audio device.
        /// </summary>
        AudioCapabilities Capabilities { get; }

        /// <summary>
        /// Gets or sets the master volume for all audio output.
        /// </summary>
        Single AudioMasterVolume { get; set; }

        /// <summary>
        /// Gets or sets the master volume for songs.
        /// </summary>
        Single SongsMasterVolume { get; set; }

        /// <summary>
        /// Gets or sets the master volume for sound effects.
        /// </summary>
        Single SoundEffectsMasterVolume { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all audio output is globally muted.
        /// </summary>
        Boolean AudioMuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether songs are globally muted.
        /// </summary>
        Boolean SongsMuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sound effects are globally muted.
        /// </summary>
        Boolean SoundEffectsMuted { get; set; }
    }
}
