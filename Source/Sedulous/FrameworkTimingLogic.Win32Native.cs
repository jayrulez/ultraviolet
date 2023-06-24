using System.Runtime.InteropServices;

namespace Sedulous
{
    partial class FrameworkTimingLogic
    {
        private static class Win32Native
        {
            [DllImport("winmm")]
            public static extern uint timeBeginPeriod(uint period);

            [DllImport("winmm")]
            public static extern uint timeEndPeriod(uint period);
        }
    }
}
