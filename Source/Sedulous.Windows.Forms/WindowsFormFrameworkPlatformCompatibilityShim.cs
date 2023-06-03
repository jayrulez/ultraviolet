using System.Reflection;

namespace Sedulous.Windows.Forms
{
    internal class WindowsFormFrameworkPlatformCompatibilityShim : IFrameworkPlatformCompatibilityShim
    {
        public Assembly Assembly => typeof(WindowsFormFrameworkPlatformCompatibilityShim).Assembly;
    }
}
