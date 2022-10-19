using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.Platform;
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
    public class SDL2SedulousContext : SedulousContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SDL2SedulousContext"/> class.
        /// </summary>
        /// <param name="host">The object that is hosting the Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        public unsafe SDL2SedulousContext(ISedulousHost host, SDL2SedulousConfiguration configuration)
            : base(host, configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            if (!InitSDL(configuration))
                throw new SDL2Exception();

            this.onWindowDrawing = (context, time, window) => 
                ((SDL2SedulousContext)context).OnWindowDrawing(time, window);
            this.onWindowDrawn = (context, time, window) => 
                ((SDL2SedulousContext)context).OnWindowDrawn(time, window);

            eventFilter = new SDL_EventFilter(SDLEventFilter);
            eventFilterPtr = Marshal.GetFunctionPointerForDelegate(eventFilter);
            SDL_SetEventFilter(eventFilterPtr, IntPtr.Zero);

            LoadSubsystemAssemblies(configuration);
            this.swapChainManager = IsRunningInServiceMode ? new DummySwapChainManager(this) : SwapChainManager.Create();
            
            this.platform = InitializePlatformSubsystem(configuration);
            this.graphics = InitializeGraphicsSubsystem(configuration);

            if (!this.platform.IsPrimaryWindowInitialized)
                throw new InvalidOperationException(SedulousStrings.PrimaryWindowMustBeInitialized);

            this.audio = InitializeAudioSubsystem(configuration);
            this.input = InitializeInputSubsystem();
            this.content = InitializeContentSubsystem();
            this.ui = InitializeUISubsystem(configuration);

            PumpEvents();

            InitializeContext();
            InitializeViewProvider(configuration);
            InitializePlugins(configuration);
        }
        
        /// <inheritdoc/>
        public override void UpdateSuspended()
        {
            SDL_PumpEvents();

            base.UpdateSuspended();
        }

        /// <inheritdoc/>
        public override void Update(SedulousTime time)
        {
            Contract.Require(time, nameof(time));
            Contract.EnsureNotDisposed(this, Disposed);

            var sdlinput = GetInput() as SDL2SedulousInput;
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
        public override void Draw(SedulousTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            OnDrawing(time);
            swapChainManager.DrawAndSwap(time, onWindowDrawing, onWindowDrawn);

            base.Draw(time);
        }

        /// <inheritdoc/>
        public override ISedulousPlatform GetPlatform() => platform;

        /// <inheritdoc/>
        public override ISedulousContent GetContent() => content;

        /// <inheritdoc/>
        public override ISedulousGraphics GetGraphics() => graphics;

        /// <inheritdoc/>
        public override ISedulousAudio GetAudio() => audio;

        /// <inheritdoc/>
        public override ISedulousInput GetInput() => input;

        /// <inheritdoc/>
        public override ISedulousUI GetUI() => ui;

        /// <summary>
        /// Gets the assembly that implements the graphics subsystem.
        /// </summary>
        public Assembly GraphicsSubsystemAssembly { get; private set; }

        /// <summary>
        /// Gets the assembly that implements the audio subsystem.
        /// </summary>
        public Assembly AudioSubsystemAssembly { get; private set; }

        /// <inheritdoc/>
        protected override void OnShutdown()
        {
            SDL_SetEventFilter(IntPtr.Zero, IntPtr.Zero);
            SDL_Quit();

            base.OnShutdown();
        }

        /// <summary>
        /// Loads the context's subsystem assemblies.
        /// </summary>
        /// <param name="configuration">The context's configuration settings.</param>
        private void LoadSubsystemAssemblies(SedulousConfiguration configuration)
        {
            if (IsRunningInServiceMode)
                return;

            Assembly LoadSubsystemAssembly(String name)
            {
                try
                {
                    return Assembly.Load(name);
                }
                catch (Exception e)
                {
                    if (e is FileNotFoundException || e is FileLoadException || e is BadImageFormatException)
                    {
                        return null;
                    }
                    throw;
                }
            }

            if (String.IsNullOrEmpty(configuration.GraphicsSubsystemAssembly))
                throw new InvalidOperationException(SDL2Strings.MissingGraphicsAssembly);

            if (String.IsNullOrEmpty(configuration.AudioSubsystemAssembly))
                throw new InvalidOperationException(SDL2Strings.MissingAudioAssembly);

            this.GraphicsSubsystemAssembly = LoadSubsystemAssembly(configuration.GraphicsSubsystemAssembly);
            if (this.GraphicsSubsystemAssembly == null)
                throw new InvalidOperationException(SDL2Strings.InvalidGraphicsAssembly);

            this.AudioSubsystemAssembly = LoadSubsystemAssembly(configuration.AudioSubsystemAssembly);
            if (this.AudioSubsystemAssembly == null)
                throw new InvalidOperationException(SDL2Strings.InvalidAudioAssembly);

            var distinctAssemblies = new[] { this.GraphicsSubsystemAssembly, this.AudioSubsystemAssembly }.Distinct();
            foreach (var distinctAssembly in distinctAssemblies)
                InitializeFactoryMethodsInAssembly(distinctAssembly);
        }

        /// <summary>
        /// Attempts to create a new instance of the specified Sedulous subsystem by dynamically loading it from the specified assembly.
        /// </summary>
        /// <typeparam name="TSubsystem">The subsystem interface type.</typeparam>
        /// <param name="assembly">The assembly from which to load the subsystem implementation.</param>
        /// <param name="configuration">The Sedulous context configuration.</param>
        /// <param name="instance">The subsystem instance that was created.</param>
        /// <returns><see langword="true"/> if the subsystem instance was created; otherwise, <see langword="false"/>.</returns>
        private Boolean TryCreateSubsystemInstance<TSubsystem>(Assembly assembly, SedulousConfiguration configuration, out TSubsystem instance)
        {
            var types = (from t in assembly.GetTypes()
                         where
                          t.IsClass && !t.IsAbstract &&
                          t.GetInterfaces().Contains(typeof(TSubsystem))
                         select t).ToList();

            if (!types.Any() || types.Count > 1)
                throw new InvalidOperationException(SDL2Strings.InvalidAudioAssembly);

            var type = types.Single();

            var ctorWithConfig = type.GetConstructor(new[] { typeof(SedulousContext), typeof(SedulousConfiguration) });
            if (ctorWithConfig != null)
            {
                instance = (TSubsystem)ctorWithConfig.Invoke(new object[] { this, configuration });
                return true;
            }

            var ctorWithoutConfig = type.GetConstructor(new[] { typeof(SedulousContext) });
            if (ctorWithoutConfig != null)
            {
                instance = (TSubsystem)ctorWithoutConfig.Invoke(new object[] { this });
                return true;
            }

            instance = default(TSubsystem);
            return false;
        }

        /// <summary>
        /// Initializes the context's platform subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The platform subsystem.</returns>
        private ISedulousPlatform InitializePlatformSubsystem(SedulousConfiguration configuration)
        {
            if (IsRunningInServiceMode)
            {
                return new DummySedulousPlatform(this);
            }
            else
            {
                var platform = new SDL2SedulousPlatform(this, configuration);
                PumpEvents();
                return platform;
            }
        }

        /// <summary>
        /// Initializes the context's content subsystem.
        /// </summary>
        /// <returns>The content subsystem.</returns>
        private ISedulousContent InitializeContentSubsystem()
        {
            var content = new SedulousContent(this);
            content.RegisterImportersAndProcessors(new[] { GraphicsSubsystemAssembly, AudioSubsystemAssembly }.Distinct());
            content.Importers.RegisterImporter<XmlContentImporter>("prog");
            return content;
        }

        /// <summary>
        /// Initializes the context's graphics subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The graphics subsystem.</returns>
        private ISedulousGraphics InitializeGraphicsSubsystem(SedulousConfiguration configuration)
        {
            if (IsRunningInServiceMode)
                return new DummySedulousGraphics(this);

            if (!TryCreateSubsystemInstance<ISedulousGraphics>(GraphicsSubsystemAssembly, configuration, out var graphics))
                throw new InvalidOperationException(SDL2Strings.InvalidGraphicsAssembly);

            return graphics;
        }

        /// <summary>
        /// Initializes the context's audio subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The audio subsystem.</returns>
        private ISedulousAudio InitializeAudioSubsystem(SedulousConfiguration configuration)
        {
            if (IsRunningInServiceMode)
                return new DummySedulousAudio(this);

            if (!TryCreateSubsystemInstance<ISedulousAudio>(AudioSubsystemAssembly, configuration, out var audio))
                throw new InvalidOperationException(SDL2Strings.InvalidAudioAssembly);

            return audio;
        }

        /// <summary>
        /// Initializes the context's input subsystem.
        /// </summary>
        /// <returns>The input subsystem.</returns>
        private ISedulousInput InitializeInputSubsystem()
        {
            if (IsRunningInServiceMode)
                return new DummySedulousInput(this);

            return new SDL2SedulousInput(this);
        }

        /// <summary>
        /// Initializes the context's UI subsystem.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns>The UI subsystem.</returns>
        private ISedulousUI InitializeUISubsystem(SedulousConfiguration configuration)
        {
            return new SedulousUI(this, configuration);
        }

        /// <summary>
        /// Initializes SDL2.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        /// <returns><see langword="true"/> if SDL2 was successfully initialized; otherwise, <see langword="false"/>.</returns>
        private Boolean InitSDL(SedulousConfiguration configuration)
        {
            var sdlFlags = configuration.EnableServiceMode ?
                SDL_INIT_TIMER | SDL_INIT_EVENTS :
                SDL_INIT_TIMER | SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER | SDL_INIT_EVENTS;

            if (Platform == SedulousPlatform.Windows)
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
                            var glWindowInfo = (SDL2SedulousWindowInfo)GetPlatform().Windows;
                            if (glWindowInfo.DestroyByID((int)@event.window.windowID))
                            {
                                Messages.Publish(SedulousMessages.Quit, null);
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
                        Messages.Publish(SedulousMessages.Quit, null);
                        return true;
                }

                // Publish any SDL events to the message queue.
                var data = Messages.CreateMessageData<SDL2EventMessageData>();
                data.Event = @event;
                Messages.Publish(SDL2SedulousMessages.SDLEvent, data);
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
                    uv.Messages.PublishImmediate(SedulousMessages.ApplicationTerminating, null);
                    return 0;

                case SDL_APP_WILLENTERBACKGROUND:
                    uv.Messages.PublishImmediate(SedulousMessages.ApplicationSuspending, null);
                    return 0;

                case SDL_APP_DIDENTERBACKGROUND:
                    uv.Messages.PublishImmediate(SedulousMessages.ApplicationSuspended, null);
                    return 0;

                case SDL_APP_WILLENTERFOREGROUND:
                    uv.Messages.PublishImmediate(SedulousMessages.ApplicationResuming, null);
                    return 0;

                case SDL_APP_DIDENTERFOREGROUND:
                    uv.Messages.PublishImmediate(SedulousMessages.ApplicationResumed, null);
                    return 0;

                case SDL_APP_LOWMEMORY:
                    uv.Messages.PublishImmediate(SedulousMessages.LowMemory, null);
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                    return 0;
            }

            return 1;
        }
        
        // The SDL event filter.
        private readonly SDL_EventFilter eventFilter;
        private readonly IntPtr eventFilterPtr;

        // Sedulous subsystems.
        private readonly SwapChainManager swapChainManager;
        private readonly ISedulousPlatform platform;
        private readonly ISedulousContent content;
        private readonly ISedulousGraphics graphics;
        private readonly ISedulousAudio audio;
        private readonly ISedulousInput input;
        private readonly ISedulousUI ui;

        // Delegate caches.
        private readonly Action<SedulousContext, SedulousTime, ISedulousWindow> onWindowDrawing;
        private readonly Action<SedulousContext, SedulousTime, ISedulousWindow> onWindowDrawn;
    }
}
