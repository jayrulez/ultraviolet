using System;

namespace Sedulous
{
    /// <summary>
    /// Contains methods for profiling Sedulous applications.
    /// </summary>
    public static class FrameworkProfiler
    {
        /// <summary>
        /// Creates an <see cref="FrameworkProfilerSection"/> object which represents a profiler section
        /// and immediately begins that section.
        /// </summary>
        /// <param name="name">The name of the profiler section.</param>
        /// <returns>The <see cref="FrameworkProfilerSection"/> object which was created.</returns>
        public static FrameworkProfilerSection Section(String name)
        {
            var section = new FrameworkProfilerSection(name);
            BeginSection(name);
            return section;
        }

        /// <summary>
        /// Requests that the profiler take a snapshot that contains the entirety of the next frame.
        /// </summary>
        public static void TakeSnapshotOfNextFrame()
        {
            var impl = instance.Value;
            if (impl == null)
                throw new InvalidOperationException(FrameworkStrings.ContextDoesNotHaveProfiler);

            impl.TakeSnapshotOfNextFrame();
        }

        /// <summary>
        /// Begins a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to begin.</param>
        public static void BeginSection(String name)
        {
            var impl = instance.Value;
            if (impl != null)
                impl.BeginSection(name);
        }

        /// <summary>
        /// Ends a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to begin.</param>
        public static void EndSection(String name)
        {
            var impl = instance.Value;
            if (impl != null)
                impl.EndSection(name);
        }

        /// <summary>
        /// Begins a profiler snapshot.
        /// </summary>
        public static void BeginSnapshot()
        {
            var impl = instance.Value;
            if (impl == null)
                throw new InvalidOperationException(FrameworkStrings.ContextDoesNotHaveProfiler);

            impl.BeginSnapshot();
        }

        /// <summary>
        /// Ends the current profiler snapshot.
        /// </summary>
        public static void EndSnapshot()
        {
            var impl = instance.Value;
            if (impl == null)
                throw new InvalidOperationException(FrameworkStrings.ContextDoesNotHaveProfiler);

            impl.EndSnapshot();
        }

        /// <summary>
        /// Enables a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to enable.</param>
        /// <remarks>Only enabled sections will be included in profiler snapshots.</remarks>
        public static void EnableSection(String name)
        {
            var impl = instance.Value;
            if (impl == null)
                throw new InvalidOperationException(FrameworkStrings.ContextDoesNotHaveProfiler);

            impl.EnableSection(name);
        }

        /// <summary>
        /// Disables a profiler section.
        /// </summary>
        /// <param name="name">The name of the section to enable.</param>
        /// <remarks>Only enabled sections will be included in profiler snapshots.</remarks>
        public static void DisableSection(String name)
        {
            var impl = instance.Value;
            if (impl == null)
                throw new InvalidOperationException(FrameworkStrings.ContextDoesNotHaveProfiler);

            impl.DisableSection(name);
        }

        /// <summary>
        /// Represents the singleton profiler instance used by the current context.
        /// </summary>
        private static readonly FrameworkSingleton<FrameworkProfilerBase> instance =
            new FrameworkSingleton<FrameworkProfilerBase>(uv =>
            {
                var factory = uv.TryGetFactoryMethod<SedulousProfilerFactory>();
                if (factory != null)
                {
                    return factory(uv);
                }
                return null;
            });
    }
}
