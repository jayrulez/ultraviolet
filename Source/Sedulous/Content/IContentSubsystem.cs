using System.Collections.Generic;
using System.Reflection;
using Sedulous.Content;

namespace Sedulous
{
    /// <summary>
    /// Represents the Framework's content management subsystem.
    /// </summary>
    public interface IContentSubsystem : IFrameworkSubsystem
    {
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
