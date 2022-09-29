using System.Reflection;

namespace Ultraviolet
{
    internal sealed class AndroidUltravioletPlatformCompatibilityShim : IUltravioletPlatformCompatibilityShim
    {
        public Assembly Assembly => typeof(AndroidUltravioletPlatformCompatibilityShim).Assembly;
    }
}
