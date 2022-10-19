using System.Reflection;

namespace Sedulous.Windows.Forms
{
    internal class WindowsFormSedulousPlatformCompatibilityShim : ISedulousPlatformCompatibilityShim
    {
        public Assembly Assembly => typeof(WindowsFormSedulousPlatformCompatibilityShim).Assembly;
    }
}
