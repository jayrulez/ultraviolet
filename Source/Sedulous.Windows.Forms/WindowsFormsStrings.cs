using System.Reflection;
using Sedulous.Core.Text;

namespace Sedulous.Windows.Forms
{
    /// <summary>
    /// Contains the Windows Forms Sedulous host's string resources.
    /// </summary>
    public static class WindowsFormsStrings
    {
        /// <summary>
        /// Initializes the WindowsFormsStrings type.
        /// </summary>
        static WindowsFormsStrings()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("Sedulous.Windows.Forms.Resources.Strings.xml"))
            {
                StringDatabase.LoadFromStream(stream);
            }
        }

        private static readonly LocalizationDatabase StringDatabase = new LocalizationDatabase();

#pragma warning disable 1591
        public static readonly StringResource FrameworkFormRequired = new StringResource(StringDatabase, "FRAMEWORK_FORM_REQUIRED");
        public static readonly StringResource PanelAlreadyEnlisted    = new StringResource(StringDatabase, "PANEL_ALREADY_ENLISTED");
        public static readonly StringResource PanelNotEnlisted        = new StringResource(StringDatabase, "PANEL_NOT_ENLISTED");
#pragma warning restore 1591
    }
}
