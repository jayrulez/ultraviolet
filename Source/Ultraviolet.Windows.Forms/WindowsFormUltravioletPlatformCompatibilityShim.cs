using System.Reflection;

namespace Ultraviolet.Windows.Forms
{
    internal class WindowsFormUltravioletPlatformCompatibilityShim : IUltravioletPlatformCompatibilityShim
    {
        public Assembly Assembly => typeof(WindowsFormUltravioletPlatformCompatibilityShim).Assembly;
    }
}
