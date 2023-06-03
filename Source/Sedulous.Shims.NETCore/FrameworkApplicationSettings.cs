using System;
using System.Xml.Linq;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents an Sedulous application's internal settings.
    /// </summary>
    internal class FrameworkApplicationSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkApplicationSettings"/> class.
        /// </summary>
        private FrameworkApplicationSettings()
        {
            Window = new FrameworkApplicationWindowSettings();
        }

        /// <summary>
        /// Saves the specified application settings to the specified file.
        /// </summary>
        /// <param name="path">The path to the file in which to save the application settings.</param>
        /// <param name="settings">The <see cref="FrameworkApplicationSettings"/> to serialize to the specified file.</param>
        public static void Save(String path, FrameworkApplicationSettings settings)
        {
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Settings",
                    FrameworkApplicationWindowSettings.Save(settings.Window),
                    FrameworkApplicationAudioSettings.Save(settings.Audio)
                ));
            xml.Save(path);
        }

        /// <summary>
        /// Loads a set of application settings from the specified file.
        /// </summary>
        /// <param name="path">The path to the file from which to load the application settings.</param>
        /// <returns>The <see cref="FrameworkApplicationSettings"/> which were deserialized from the specified file
        /// or <see langword="null"/> if settings could not be loaded correctly.</returns>
        public static FrameworkApplicationSettings Load(String path)
        {
            var xml = XDocument.Load(path);

            var settings = new FrameworkApplicationSettings();

            settings.Window = FrameworkApplicationWindowSettings.Load(xml.Root.Element("Window"));
            settings.Audio = FrameworkApplicationAudioSettings.Load(xml.Root.Element("Audio"));

            if (settings.Window == null && settings.Audio == null)
                return null;

            return settings;
        }

        /// <summary>
        /// Creates a set of application settings from the current application state.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <returns>The <see cref="FrameworkApplicationSettings"/> which was retrieved.</returns>
        public static FrameworkApplicationSettings FromCurrentSettings(FrameworkContext uv)
        {
            Contract.Require(uv, nameof(uv));

            var settings = new FrameworkApplicationSettings();

            settings.Window = FrameworkApplicationWindowSettings.FromCurrentSettings(uv);
            settings.Audio = FrameworkApplicationAudioSettings.FromCurrentSettings(uv);

            return settings;
        }

        /// <summary>
        /// Applies the specified settings.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public void Apply(FrameworkContext uv)
        {
            if (Window != null)
                Window.Apply(uv);

            if (Audio != null)
                Audio.Apply(uv);
        }

        /// <summary>
        /// Gets the application's window settings.
        /// </summary>
        public FrameworkApplicationWindowSettings Window
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application's audio settings.
        /// </summary>
        public FrameworkApplicationAudioSettings Audio
        {
            get;
            private set;
        }
    }
}