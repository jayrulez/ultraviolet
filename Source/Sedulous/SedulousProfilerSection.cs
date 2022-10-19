using System;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a section of code which is being measured by a profiler.
    /// </summary>
    public struct SedulousProfilerSection : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousProfilerSection"/> structure.
        /// </summary>
        /// <param name="name"></param>
        internal SedulousProfilerSection(String name)
        {
            Contract.Require(name, nameof(name));

            this.name = name;
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        public void Dispose()
        {
            SedulousProfiler.EndSection(name);
        }

        /// <summary>
        /// Gets the name of the profiler section.
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        // Property values.
        private readonly String name;
    }
}
