using System;

namespace Sedulous.Presentation.Styles
{
    /// <summary>
    /// Represents a <see cref="CompositeUvssDocument"/> which may be used as a globally-applied style sheet.
    /// </summary>
    public sealed class GlobalStyleSheet : CompositeUvssDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalStyleSheet"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        private GlobalStyleSheet(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="GlobalStyleSheet"/> instance.
        /// </summary>
        /// <returns>The <see cref="GlobalStyleSheet"/> which was created.</returns>
        public static GlobalStyleSheet Create()
        {
            var uv = FrameworkContext.DemandCurrent();
            return new GlobalStyleSheet(uv);
        }

        /// <inheritdoc/>
        protected override bool OnValidating(String path, UvssDocument asset)
        {
            var upf = FrameworkContext.GetUI().GetPresentationFoundation();
            return upf.TrySetGlobalStyleSheet(this);            
        }

        /// <inheritdoc/>
        protected override void OnValidationComplete(String path, UvssDocument asset, Boolean validated)
        {
            if (validated)
                return;

            var upf = FrameworkContext.GetUI().GetPresentationFoundation();
            upf.TrySetGlobalStyleSheet(this);
        }
    }
}
