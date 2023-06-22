using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.OpenGL;
using Sedulous.Platform;
using Sedulous.SDL2.Graphics;
using Sedulous.SDL2.Messages;
using Sedulous.SDL2.Native;
using Sedulous.SDL2.Platform;
using Sedulous.UI;
using static Sedulous.SDL2.Native.SDL_EventType;
using static Sedulous.SDL2.Native.SDL_Hint;
using static Sedulous.SDL2.Native.SDL_Init;
using static Sedulous.SDL2.Native.SDL_WindowEventID;
using static Sedulous.SDL2.Native.SDLNative;

namespace Sedulous.SDL2
{
    /// <summary>
    /// Represents the base class for Sedulous implementations which use SDL2.
    /// </summary>
    [CLSCompliant(true)]
    public class SDL2FrameworkContext : FrameworkContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SDL2FrameworkContext"/> class.
        /// </summary>
        /// <param name="host">The object that is hosting the Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        public SDL2FrameworkContext(IFrameworkHost host, SDL2FrameworkConfiguration configuration)
            : base(host, configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            if (!InitSDL(configuration))
                throw new SDL2Exception();

            this.configuration = configuration;
        }

        /// <inheritdoc/>
        protected unsafe override void OnInitialize()
        {
            ConfigurePlugins(configuration);
            this.onWindowDrawing = (context, time, window) =>
                ((SDL2FrameworkContext)context).OnWindowDrawing(time, window);
            this.onWindowDrawn = (context, time, window) =>
                ((SDL2FrameworkContext)context).OnWindowDrawn(time, window);

            eventFilter = new SDL_EventFilter(SDLEventFilter);
            eventFilterPtr = Marshal.GetFunctionPointerForDelegate(eventFilter);
            SDL_SetEventFilter(eventFilterPtr, IntPtr.Zero);

            this.swapChainManager = IsRunningInServiceMode ? new DummySwapChainManager(this) : SwapChainManager.Create();

            this.platform = InitializePlatformSubsystem(configuration);
            this.graphics = InitializeGraphicsSubsystem(configuration);

            if (!this.platform.IsPrimaryWindowInitialized)
                throw new InvalidOperationException(FrameworkStrings.PrimaryWindowMustBeInitialized);

            this.audio = InitializeAudioSubsystem(configuration);
            this.input = InitializeInputSubsystem();
            this.content = InitializeContentSubsystem();
            this.ui = InitializeUISubsystem(configuration);

            InitializeViewProvider(configuration);
            InitializePlugins(configuration);

            base.OnInitialize();

            PumpEvents();
        }


        /// <inheritdoc/>
        protected override void ConfigureFactory()
        {
            base.ConfigureFactory();


            // Core classes.
            Factory.SetFactoryMethod<PlatformNativeSurfaceFactory>((source) => new SDL2PlatformNativeSurface(source));
            Factory.SetFactoryMethod<Surface2DFactory>((uv, width, height, options) => new SDL2Surface2D(uv, width, height, options));
            Factory.SetFactoryMethod<Surface2DFromSourceFactory>((uv, source, options) => new SDL2Surface2D(uv, source, options));
            Factory.SetFactoryMethod<Surface2DFromNativeSurfaceFactory>((uv, surface, options) => new SDL2Surface2D(uv, surface, options));
            Factory.SetFactoryMethod<Surface3DFactory>((uv, width, height, depth, bytesPerPixel, options) => new SDL2Surface3D(uv, width, height, depth, bytesPerPixel, options));
            Factory.SetFactoryMethod<CursorFactory>((uv, surface, hx, hv) => new SDL2Cursor(uv, surface, hx, hv));

            // Platform services
            var msgboxService = new SDL2MessageBoxService();
            Factory.SetFactoryMethod<MessageBoxServiceFactory>(() => msgboxService);

            var clipboardService = new SDL2ClipboardService();
            Factory.SetFactoryMethod<ClipboardServiceFactory>(() => clipboardService);

            var powerManagementService = new SDL2PowerManagementService();
            Factory.SetFactoryMethod<PowerManagementServiceFactory>(() => powerManagementService);

            // Graphics API services
            Factory.SetFactoryMethod<OpenGLEnvironmentFactory>((uv) => new SDL2OpenGLEnvironment(uv));
        }

        /// <inheritdoc/>
        public override void UpdateSuspended()
        {
            SDL_PumpEvents();

            base.UpdateSuspended();
        }

        /// <inheritdoc/>
        public override void Update(FrameworkTime time)
        {
            Contract.Require(time, nameof(time));
            Contract.EnsureNotDisposed(this, Disposed);

            var sdlinput = GetInput() as SDL2FrameworkInput;
            if (sdlinput != null)
                sdlinput.ResetDeviceStates();

            if (!PumpEvents())
            {
                return;
            }

            ProcessMessages();

            OnUpdatingSubsystems(time);

            GetPlatform().Update(time);
            GetContent().Update(time);

            if (!IsRunningInServiceMode)
                GetGraphics().Update(time);

            GetAudio().Update(time);
            GetInput().Update(time);
            GetUI().Update(time);

            ProcessMessages();

            OnUpdating(time);

            UpdateContext(time);
        }

        /// <inheritdoc/>
        public override void Draw(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            OnDrawing(time);
            swapChainManager.DrawAndSwap(time, onWindowDrawing, onWindowDrawn);

            base.Draw(time);
        }

        /// <inheritdoc/>
        public override IPlatformSubsystem GetPlatform() => platform;

        /// <inheritdoc/>
        public override IContentSubsystem GetContent() => content;

        /// <inheritdoc/>
        public override IGraphicsSubsystem GetGraphics() => graphics;

        /// <inheritdoc/>
        public override IAudioSubsystem GetAudio() => audio;

        /// <inheritdoc/>
        public override IInputSubsystem GetInput() => input;

        /// <inheritdoc/>
        public override IUISubsystem GetUI() => ui;

        /// <inheritdoc/>
        protected override void OnShutdown()
        {
            SDL_SetEventFilter(IntPtr.Zero, IntPtr.Zero);
            SDL_Quit();

            base.OnShutdown();
        }

        /// <summary>
        /// Initializes the context's platform subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The platform subsystem.</returns>
        private IPlatformSubsystem InitializePlatformSubsystem(FrameworkConfiguration configuration)
        {
            if (IsRunningInServiceMode)
            {
                return new DummyPlatformSubsystem(this);
            }
            else
            {
                var platform = new SDL2PlatformSubsystem(this, configuration);
                PumpEvents();
                return platform;
            }
        }

        /// <summary>
        /// Initializes the context's content subsystem.
        /// </summary>
        /// <returns>The content subsystem.</returns>
        private IContentSubsystem InitializeContentSubsystem()
        {
            var content = new ContentSubsystem(this);
            content.Importers.RegisterImporter<XmlContentImporter>("prog");

            content.Importers.RegisterImporter<SDL2PlatformNativeSurfaceImporter>(".bmp");
            content.Importers.RegisterImporter<SDL2PlatformNativeSurfaceImporter>(".png");
            content.Importers.RegisterImporter<SDL2PlatformNativeSurfaceImporter>(".jpg");
            content.Importers.RegisterImporter<SDL2PlatformNativeSurfaceImporter>(".jpeg");

            content.Processors.RegisterProcessor<SDL2Surface2DProcessor>();
            content.Processors.RegisterProcessor<SDL2Surface3DProcessor>();
            content.Processors.RegisterProcessor<SDL2CursorProcessor>();

            return content;
        }

        /// <summary>
        /// Initializes the context's graphics subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The graphics subsystem.</returns>
        private IGraphicsSubsystem InitializeGraphicsSubsystem(FrameworkConfiguration configuration)
        {
            if (IsRunningInServiceMode)
                return new DummyGraphicsSubsystem(this);

            var graphicsFactory = Factory.TryGetFactoryMethod<FrameworkGraphicsFactory>();
            if (graphicsFactory == null)
            {
                throw new InvalidOperationException(SDL2Strings.MissingGraphicsFactory);
            }

            var graphics = graphicsFactory(this, configuration);

            return graphics;
        }

        /// <summary>
        /// Initializes the context's audio subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The audio subsystem.</returns>
        private IAudioSubsystem InitializeAudioSubsystem(FrameworkConfiguration configuration)
        {
            if (IsRunningInServiceMode)
                return new DummyAudioSubsystem(this);

            var audioFactory = Factory.TryGetFactoryMethod<FrameworkAudioFactory>();
            if (audioFactory == null)
            {
                throw new InvalidOperationException(SDL2Strings.MissingAudioFactory);
            }

            var audio = audioFactory(this, configuration);

            return audio;
        }

        /// <summary>
        /// Initializes the context's input subsystem.
        /// </summary>
        /// <returns>The input subsystem.</returns>
        private IInputSubsystem InitializeInputSubsystem()
        {
            if (IsRunningInServiceMode)
                return new DummyInputSubsystem(this);

            return new SDL2FrameworkInput(this);
        }

        /// <summary>
        /// Initializes the context's UI subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The UI subsystem.</returns>
        private IUISubsystem InitializeUISubsystem(FrameworkConfiguration configuration)
        {
            return new UISubsystem(this, configuration);
        }

        /// <summary>
        /// Initializes SDL2.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns><see langword="true"/> if SDL2 was successfully initialized; otherwise, <see langword="false"/>.</returns>
        private Boolean InitSDL(FrameworkConfiguration configuration)
        {
            var sdlFlags = configuration.EnableServiceMode ?
                SDL_INIT_TIMER | SDL_INIT_EVENTS :
                SDL_INIT_TIMER | SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER | SDL_INIT_EVENTS;

            if (Platform == FrameworkPlatform.Windows)
            {
                if (!SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1"))
                    throw new SDL2Exception();
            }

            return SDL_Init(sdlFlags) == 0;
        }

        /// <summary>
        /// Pumps the SDL2 event queue.
        /// </summary>
        /// <returns><see langword="true"/> if the context should continue processing the frame; otherwise, <see langword="false"/>.</returns>
        private Boolean PumpEvents()
        {
            SDL_Event @event;
            while (SDL_PollEvent(out @event) > 0)
            {
                if (Disposed)
                    return false;

                switch (@event.type)
                {
                    case SDL_WINDOWEVENT:
                        if (@event.window.@event == SDL_WINDOWEVENT_CLOSE)
                        {
                            var glWindowInfo = (SDL2FrameworkWindowInfo)GetPlatform().Windows;
                            if (glWindowInfo.DestroyByID((int)@event.window.windowID))
                            {
                                Messages.Publish(FrameworkMessages.Quit, null);
                                return true;
                            }
                        }
                        break;

                    case SDL_KEYDOWN:
                    case SDL_KEYUP:
                    case SDL_MOUSEBUTTONDOWN:
                    case SDL_MOUSEBUTTONUP:
                    case SDL_MOUSEMOTION:
                    case SDL_MOUSEWHEEL:
                    case SDL_JOYAXISMOTION:
                    case SDL_JOYBALLMOTION:
                    case SDL_JOYBUTTONDOWN:
                    case SDL_JOYBUTTONUP:
                    case SDL_JOYHATMOTION:
                    case SDL_CONTROLLERAXISMOTION:
                    case SDL_CONTROLLERBUTTONDOWN:
                    case SDL_CONTROLLERBUTTONUP:
                        if (IsHardwareInputDisabled)
                        {
                            continue;
                        }
                        break;

                    case SDL_QUIT:
                        Messages.Publish(FrameworkMessages.Quit, null);
                        return true;
                }

                // Publish any SDL events to the message queue.
                var data = Messages.CreateMessageData<SDL2EventMessageData>();
                data.Event = @event;
                Messages.Publish(SDL2FrameworkMessages.SDLEvent, data);
            }
            return !Disposed;
        }
        
        /// <summary>
        /// Filters SDL2 events.
        /// </summary>
        [MonoPInvokeCallback(typeof(SDL_EventFilter))]
        private static unsafe Int32 SDLEventFilter(IntPtr userdata, SDL_Event* @event)
        {
            var uv = RequestCurrent();
            if (uv == null)
                return 1;

            switch (@event->type)
            {
                case SDL_APP_TERMINATING:
                    uv.Messages.PublishImmediate(FrameworkMessages.ApplicationTerminating, null);
                    return 0;

                case SDL_APP_WILLENTERBACKGROUND:
                    uv.Messages.PublishImmediate(FrameworkMessages.ApplicationSuspending, null);
                    return 0;

                case SDL_APP_DIDENTERBACKGROUND:
                    uv.Messages.PublishImmediate(FrameworkMessages.ApplicationSuspended, null);
                    return 0;

                case SDL_APP_WILLENTERFOREGROUND:
                    uv.Messages.PublishImmediate(FrameworkMessages.ApplicationResuming, null);
                    return 0;

                case SDL_APP_DIDENTERFOREGROUND:
                    uv.Messages.PublishImmediate(FrameworkMessages.ApplicationResumed, null);
                    return 0;

                case SDL_APP_LOWMEMORY:
                    uv.Messages.PublishImmediate(FrameworkMessages.LowMemory, null);
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                    return 0;
            }

            return 1;
        }
        
        private readonly FrameworkConfiguration configuration;

        // The SDL event filter.
        private SDL_EventFilter eventFilter;
        private IntPtr eventFilterPtr;

        // Sedulous subsystems.
        private SwapChainManager swapChainManager;
        private IPlatformSubsystem platform;
        private IContentSubsystem content;
        private IGraphicsSubsystem graphics;
        private IAudioSubsystem audio;
        private IInputSubsystem input;
        private IUISubsystem ui;

        // Delegate caches.
        private Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawing;
        private Action<FrameworkContext, FrameworkTime, IFrameworkWindow> onWindowDrawn;
    }
}
