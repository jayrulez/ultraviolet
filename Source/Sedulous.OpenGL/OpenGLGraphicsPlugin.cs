using Sedulous.Core;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Graphics;
using Sedulous.OpenGL.Graphics;
using Sedulous.OpenGL.Graphics.Graphics2D;
using Sedulous.OpenGL.Bindings;
using Sedulous.Content;

namespace Sedulous.OpenGL
{
    /// <summary>
    /// Represents an Sedulous plugin which registers OpenGL as the graphics subsystem implementation.
    /// </summary>
    public class OpenGLGraphicsPlugin : FrameworkPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLGraphicsPlugin"/> class.
        /// </summary>
        /// <param name="configuration">The graphics configuration settings.</param>
        public OpenGLGraphicsPlugin(OpenGLGraphicsConfiguration configuration = null)
        {
            this.configuration = configuration ?? OpenGLGraphicsConfiguration.Default;
        }

        /// <inheritdoc/>
        public override void Register(FrameworkConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            configuration.GraphicsConfiguration = this.configuration;

            base.Register(configuration);
        }

        /// <inheritdoc/>
        public override void Configure(FrameworkContext context, FrameworkFactory factory)
        {

            // Core classes
            factory.SetFactoryMethod<GeometryStreamFactory>((uv) => new OpenGLGeometryStream(uv));
            factory.SetFactoryMethod<VertexBufferFactory>((uv, vdecl, vcount) => new OpenGLVertexBuffer(uv, vdecl, vcount, GL.GL_STATIC_DRAW));
            factory.SetFactoryMethod<IndexBufferFactory>((uv, itype, icount) => new OpenGLIndexBuffer(uv, itype, icount, GL.GL_STATIC_DRAW));
            factory.SetFactoryMethod<DynamicVertexBufferFactory>((uv, vdecl, vcount) => new OpenGLVertexBuffer(uv, vdecl, vcount, GL.GL_DYNAMIC_DRAW));
            factory.SetFactoryMethod<DynamicIndexBufferFactory>((uv, itype, icount) => new OpenGLIndexBuffer(uv, itype, icount, GL.GL_DYNAMIC_DRAW));
            factory.SetFactoryMethod<Texture2DFromRawDataFactory>((uv, pixels, width, height, bytesPerPixel, srgb) => new OpenGLTexture2D(uv, pixels, width, height, bytesPerPixel, srgb));
            factory.SetFactoryMethod<Texture2DFactory>((uv, width, height, options) => new OpenGLTexture2D(uv, width, height, options));
            factory.SetFactoryMethod<Texture3DFromRawDataFactory>((uv, layers, width, height, bytesPerPixel, srgb) => new OpenGLTexture3D(uv, layers, width, height, bytesPerPixel, srgb));
            factory.SetFactoryMethod<Texture3DFactory>((uv, width, height, depth, options) => new OpenGLTexture3D(uv, width, height, depth, options));
            factory.SetFactoryMethod<RenderTarget2DFactory>((uv, width, height, usage) => new OpenGLRenderTarget2D(uv, width, height, usage));
            factory.SetFactoryMethod<RenderBuffer2DFactory>((uv, format, width, height, options) => new OpenGLRenderBuffer2D(uv, format, width, height, options));
            factory.SetFactoryMethod<DynamicTexture2DFactory>((uv, width, height, options, state, flushed) => new OpenGLDynamicTexture2D(uv, width, height, options, state, flushed));
            factory.SetFactoryMethod<DynamicTexture3DFactory>((uv, width, height, depth, options, state, flushed) => new OpenGLDynamicTexture3D(uv, width, height, depth, options, state, flushed));
            factory.SetFactoryMethod<SwapChainManagerFactory>((uv) => new OpenGLSwapChainManager(uv));

            // Core effects
            factory.SetFactoryMethod<BasicEffectFactory>((uv) => new OpenGLBasicEffect(uv));
            factory.SetFactoryMethod<SkinnedEffectFactory>((uv) => new OpenGLSkinnedEffect(uv));
            factory.SetFactoryMethod<SpriteBatchEffectFactory>((uv) => new OpenGLSpriteBatchEffect(uv));
            factory.SetFactoryMethod<BlurEffectFactory>((uv) => new OpenGLBlurEffect(uv));

            // BlendState
            var blendStateOpaque = OpenGLBlendState.CreateOpaque(context);
            var blendStateAlphaBlend = OpenGLBlendState.CreateAlphaBlend(context);
            var blendStateAdditive = OpenGLBlendState.CreateAdditive(context);
            var blendStateNonPremultiplied = OpenGLBlendState.CreateNonPremultiplied(context);

            factory.SetFactoryMethod<BlendStateFactory>((uv) => new OpenGLBlendState(uv));
            factory.SetFactoryMethod<BlendStateFactory>("Opaque", (uv) => blendStateOpaque);
            factory.SetFactoryMethod<BlendStateFactory>("AlphaBlend", (uv) => blendStateAlphaBlend);
            factory.SetFactoryMethod<BlendStateFactory>("Additive", (uv) => blendStateAdditive);
            factory.SetFactoryMethod<BlendStateFactory>("NonPremultiplied", (uv) => blendStateNonPremultiplied);

            // DepthStencilState
            var depthStencilStateDefault = OpenGLDepthStencilState.CreateDefault(context);
            var depthStencilStateDepthRead = OpenGLDepthStencilState.CreateDepthRead(context);
            var depthStencilStateNone = OpenGLDepthStencilState.CreateNone(context);

            factory.SetFactoryMethod<DepthStencilStateFactory>((uv) => new OpenGLDepthStencilState(uv));
            factory.SetFactoryMethod<DepthStencilStateFactory>("Default", (uv) => depthStencilStateDefault);
            factory.SetFactoryMethod<DepthStencilStateFactory>("DepthRead", (uv) => depthStencilStateDepthRead);
            factory.SetFactoryMethod<DepthStencilStateFactory>("None", (uv) => depthStencilStateNone);

            // RasterizerState
            var rasterizerStateCullClockwise = OpenGLRasterizerState.CreateCullClockwise(context);
            var rasterizerStateCullCounterClockwise = OpenGLRasterizerState.CreateCullCounterClockwise(context);
            var rasterizerStateCullNone = OpenGLRasterizerState.CreateCullNone(context);

            factory.SetFactoryMethod<RasterizerStateFactory>((uv) => new OpenGLRasterizerState(uv));
            factory.SetFactoryMethod<RasterizerStateFactory>("CullClockwise", (uv) => rasterizerStateCullClockwise);
            factory.SetFactoryMethod<RasterizerStateFactory>("CullCounterClockwise", (uv) => rasterizerStateCullCounterClockwise);
            factory.SetFactoryMethod<RasterizerStateFactory>("CullNone", (uv) => rasterizerStateCullNone);

            // SamplerState
            var samplerStatePointClamp = OpenGLSamplerState.CreatePointClamp(context);
            var samplerStatePointWrap = OpenGLSamplerState.CreatePointWrap(context);
            var samplerStateLinearClamp = OpenGLSamplerState.CreateLinearClamp(context);
            var samplerStateLinearWrap = OpenGLSamplerState.CreateLinearWrap(context);
            var samplerStateAnisotropicClamp = OpenGLSamplerState.CreateAnisotropicClamp(context);
            var samplerStateAnisotropicWrap = OpenGLSamplerState.CreateAnisotropicWrap(context);

            factory.SetFactoryMethod<SamplerStateFactory>((uv) => new OpenGLSamplerState(uv));
            factory.SetFactoryMethod<SamplerStateFactory>("PointClamp", (uv) => samplerStatePointClamp);
            factory.SetFactoryMethod<SamplerStateFactory>("PointWrap", (uv) => samplerStatePointWrap);
            factory.SetFactoryMethod<SamplerStateFactory>("LinearClamp", (uv) => samplerStateLinearClamp);
            factory.SetFactoryMethod<SamplerStateFactory>("LinearWrap", (uv) => samplerStateLinearWrap);
            factory.SetFactoryMethod<SamplerStateFactory>("AnisotropicClamp", (uv) => samplerStateAnisotropicClamp);
            factory.SetFactoryMethod<SamplerStateFactory>("AnisotropicWrap", (uv) => samplerStateAnisotropicWrap);

            factory.SetFactoryMethod<FrameworkGraphicsFactory>((uv, configuration) => new OpenGLGraphicsSubsystem(uv, configuration));

            base.Configure(context, factory);
        }

        /// <inheritdoc/>
        public override void RegisterContentImporters(ContentImporterRegistry importers)
        {
            importers.RegisterImporter<OpenGLFragmentShaderImporter>(".frag");
            importers.RegisterImporter<OpenGLFragmentShaderImporter>(".fragh");

            importers.RegisterImporter<OpenGLVertexShaderImporter>(".vert");
            importers.RegisterImporter<OpenGLVertexShaderImporter>(".verth");

            base.RegisterContentImporters(importers);
        }

        /// <inheritdoc/>
        public override void RegisterContentProcessors(ContentProcessorRegistry processors)
        {
            processors.RegisterProcessor<OpenGLSpriteFontProcessor>();
            processors.RegisterProcessor<OpenGLSpriteFontProcessorFromJObject>();
            processors.RegisterProcessor<OpenGLSpriteFontProcessorFromXDocument>();
            processors.RegisterProcessor<OpenGLSpriteFontTextureProcessor>();
            processors.RegisterProcessor<OpenGLEffectImplementationProcessorFromJObject>();
            processors.RegisterProcessor<OpenGLEffectImplementationProcessorFromShaderSource>();
            processors.RegisterProcessor<OpenGLEffectImplementationProcessorFromXDocument>();
            //processors.RegisterProcessor<OpenGLEffectImplementationProcessorFromXDocumentV1>();
            //processors.RegisterProcessor<OpenGLEffectImplementationProcessorFromXDocumentV2>();
            processors.RegisterProcessor<OpenGLEffectProcessorFromJObject>();
            processors.RegisterProcessor<OpenGLEffectProcessorFromShaderSource>();
            processors.RegisterProcessor<OpenGLEffectProcessorFromXDocument>();
            processors.RegisterProcessor<OpenGLEffectSourceProcessorFromJObject>();
            processors.RegisterProcessor<OpenGLEffectSourceProcessorFromShaderSource>();
            processors.RegisterProcessor<OpenGLEffectSourceProcessorFromXDocument>();
            processors.RegisterProcessor<OpenGLFragmentShaderProcessor>();
            processors.RegisterProcessor<OpenGLTexture2DProcessor>();
            processors.RegisterProcessor<OpenGLTexture3DProcessor>();
            processors.RegisterProcessor<OpenGLVertexShaderProcessor>();
            processors.RegisterProcessor<ShaderSourceProcessor>();

            base.RegisterContentProcessors(processors);
        }

        // Graphics configuration settings.
        private readonly OpenGLGraphicsConfiguration configuration;
    }
}
