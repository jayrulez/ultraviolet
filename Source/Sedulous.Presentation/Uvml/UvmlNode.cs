using System;

namespace Sedulous.Presentation.Uvml
{
    /// <summary>
    /// Represents a node in a UVML template.
    /// </summary>
    public abstract class UvmlNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UvmlNode"/> class.
        /// </summary>
        internal UvmlNode() { }

        /// <summary>
        /// Instantiates the value represented by the node.
        /// </summary>
        /// <param name="frameworkContext">The Sedulous context.</param>
        /// <param name="context">The instantiation context for this node.</param>
        /// <returns>The object which was instantiated by the node.</returns>
        public abstract Object Instantiate(FrameworkContext frameworkContext, UvmlInstantiationContext context);
    }
}
