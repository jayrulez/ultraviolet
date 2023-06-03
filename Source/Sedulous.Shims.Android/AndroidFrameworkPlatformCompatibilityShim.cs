using System.Reflection;

namespace Sedulous
{
    internal sealed class AndroidFrameworkPlatformCompatibilityShim : IFrameworkPlatformCompatibilityShim
    {
        public Assembly Assembly => typeof(AndroidFrameworkPlatformCompatibilityShim).Assembly;
    }
}
