using System.Collections.Generic;
using System.Reflection;
using Sedulous.Content;

namespace Sedulous
{
    /// <summary>
    /// Represents the Sedulous Framework's content management subsystem.
    /// </summary>
    public interface IContentSubsystem : IFrameworkSubsystem
    {
        /// <summary>
        /// Registers any content importers or processors defined in the specified assembly.
        /// </summary>
        /// <param name="asm">The assembly to register.</param>
        void RegisterImportersAndProcessors(Assembly asm);

        /// <summary>
        /// Registers any content importers or processors defined in the Sedulous Core assembly or
        /// any assembly containing the implementation of one of the Sedulous context's subsystems.
        /// </summary>
        /// <param name="additionalAssemblies">A collection of assemblies to include in the registration process, 
        /// or <see langword="null"/> to only load Sedulous assemblies.</param>
        void RegisterImportersAndProcessors(IEnumerable<Assembly> additionalAssemblies = null);

        /// <summary>
        /// Gets the content manifest registry.
        /// </summary>
        ContentManifestRegistry Manifests
        {
            get;
        }

        /// <summary>
        /// Gets the content importer registry.
        /// </summary>
        ContentImporterRegistry Importers
        {
            get;
        }

        /// <summary>
        /// Gets the content processor registry.
        /// </summary>
        ContentProcessorRegistry Processors
        {
            get;
        }
    }
}
