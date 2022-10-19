using System;
using Sedulous.UI;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Contains methods for initializing the Presentation Foundation.
    /// </summary>
    internal sealed class PresentationFoundationInitializer : UIViewProviderInitializer
    {
        /// <inheritdoc/>
        public override void Initialize(SedulousContext uv, Object configuration)
        {
            var config = (PresentationFoundationConfiguration)configuration ?? new PresentationFoundationConfiguration();

            var upf = uv.GetUI().GetPresentationFoundation();
            upf.BindingExpressionCompilerAssemblyName = config.BindingExpressionCompilerAssembly;
        }
    }
}
