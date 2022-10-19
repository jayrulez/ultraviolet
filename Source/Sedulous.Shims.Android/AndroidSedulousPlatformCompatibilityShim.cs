using System.Reflection;

namespace Sedulous
{
    internal sealed class AndroidSedulousPlatformCompatibilityShim : ISedulousPlatformCompatibilityShim
    {
        public Assembly Assembly => typeof(AndroidSedulousPlatformCompatibilityShim).Assembly;
    }
}
