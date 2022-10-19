using System;
using System.Xml.Linq;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents an Sedulous activity's internal settings.
    /// </summary>
    internal class SedulousApplicationSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousApplicationSettings"/> class.
        /// </summary>
        private SedulousApplicationSettings()
        {

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
                    SedulousApplicationAudioSettings.Save(settings.Audio)
                ));
            xml.Save(path);
        }

        /// <summary>
        /// Loads a set of application settings from the specified file.
        /// </summary>
        /// <param name="path">The path to the file from which to load the application settings.</param>
        /// <returns>The <see cref="SedulousApplicationSettings"/> which were deserialized from the specified file.</returns>
        public static SedulousApplicationSettings Load(String path)
        {
            var xml = XDocument.Load(path);

            var settings = new SedulousApplicationSettings();

            settings.Audio = SedulousApplicationAudioSettings.Load(xml.Root.Element("Audio"));

            if (settings.Audio == null)
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

            settings.Audio = SedulousApplicationAudioSettings.FromCurrentSettings(uv);

            return settings;
        }

        /// <summary>
        /// Applies the specified settings.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public void Apply(SedulousContext uv)
        {
            if (Audio != null)
                Audio.Apply(uv);
        }

        /// <summary>
        /// Gets the activity's audio settings.
        /// </summary>
        public SedulousApplicationAudioSettings Audio
        {
            get;
            private set;
        }
    }
}