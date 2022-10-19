using System;
using System.Xml.Linq;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents an Sedulous application's internal settings.
    /// </summary>
    internal class SedulousApplicationSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousApplicationSettings"/> class.
        /// </summary>
        private SedulousApplicationSettings()
        {
            Window = new SedulousApplicationWindowSettings();
        }

        /// <summary>
        /// Saves the specified application settings to the specified file.
        /// </summary>
        /// <param name="path">The path to the file in which to save the application settings.</param>
        /// <param name="settings">The <see cref="SedulousApplicationSettings"/> to serialize to the specified file.</param>
        public static void Save(String path, SedulousApplicationSettings settings)
        {
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Settings",
                    SedulousApplicationWindowSettings.Save(settings.Window),
                    SedulousApplicationAudioSettings.Save(settings.Audio)
                ));
            xml.Save(path);
        }

        /// <summary>
        /// Loads a set of application settings from the specified file.
        /// </summary>
        /// <param name="path">The path to the file from which to load the application settings.</param>
        /// <returns>The <see cref="SedulousApplicationSettings"/> which were deserialized from the specified file
        /// or <see langword="null"/> if settings could not be loaded correctly.</returns>
        public static SedulousApplicationSettings Load(String path)
        {
            var xml = XDocument.Load(path);

            var settings = new SedulousApplicationSettings();

            settings.Window = SedulousApplicationWindowSettings.Load(xml.Root.Element("Window"));
            settings.Audio = SedulousApplicationAudioSettings.Load(xml.Root.Element("Audio"));

            if (settings.Window == null && settings.Audio == null)
                return null;

            return settings;
        }

        /// <summary>
        /// Creates a set of application settings from the current application state.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <returns>The <see cref="SedulousApplicationSettings"/> which was retrieved.</returns>
        public static SedulousApplicationSettings FromCurrentSettings(SedulousContext uv)
        {
            Contract.Require(uv, nameof(uv));

            var settings = new SedulousApplicationSettings();

            settings.Window = SedulousApplicationWindowSettings.FromCurrentSettings(uv);
            settings.Audio = SedulousApplicationAudioSettings.FromCurrentSettings(uv);

            return settings;
        }

        /// <summary>
        /// Applies the specified settings.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public void Apply(SedulousContext uv)
        {
            if (Window != null)
                Window.Apply(uv);

            if (Audio != null)
                Audio.Apply(uv);
        }

        /// <summary>
        /// Gets the application's window settings.
        /// </summary>
        public SedulousApplicationWindowSettings Window
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application's audio settings.
        /// </summary>
        public SedulousApplicationAudioSettings Audio
        {
            get;
            private set;
        }
    }
}