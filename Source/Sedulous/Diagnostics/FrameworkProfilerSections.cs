using System;

namespace Sedulous
{
    /// <summary>
    /// Contains identifiers for the Sedulous Framework's profiler sections.
    /// </summary>
    public static class FrameworkProfilerSections
    {        
        /// <summary>
        /// Identifies the profiler section that starts at the beginning of a frame and ends at the end of a frame.
        /// </summary>
        public const String Frame = "UV_Frame";

        /// <summary>
        /// Identifies the profiler section that starts at the beginning of <see cref="FrameworkContext.Draw(FrameworkTime)"/> and
        /// ends and the end of that method.
        /// </summary>
        public const String Draw = "UV_Draw";

        /// <summary>
        /// Identifies the profiler section that starts at the beginning of <see cref="FrameworkContext.Update(FrameworkTime)"/> and
        /// ends and the end of that method.
        /// </summary>
        public const String Update = "UV_Update";
    }
}
