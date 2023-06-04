using System;
using System.IO;
using System.Linq;
using System.Threading;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Graphics;
using Sedulous.Input;
using Sedulous.Platform;
using Sedulous.Shims.NETCore;
using Sedulous.Shims.NETCore.Graphics;
using Sedulous.Shims.NETCore.Input;
using Sedulous.Shims.NETCore.Platform;

namespace Sedulous
{
    /// <summary>
    /// Represents an application running on top of the Sedulous Framework.
    /// </summary>
    public abstract partial class FrameworkApplication :
        IMessageSubscriber<FrameworkMessageID>,
        IFrameworkComponent,
        IFrameworkHost,
        IDisposable
    {
        /// <summary>
        /// Initializes the <see cref="FrameworkApplication"/> type.
        /// </summary>
        static FrameworkApplication()
        {
            var baseDir = AppContext.BaseDirectory;
            if (baseDir != null)
                Directory.SetCurrentDirectory(baseDir);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkApplication"/> class.
        /// </summary>
        /// <param name="developerName">The name of the company or developer that built this application.</param>
        /// <param name="applicationName">The name of the application </param>
        protected FrameworkApplication(String developerName, String applicationName)
        {
            Contract.RequireNotEmpty(developerName, nameof(developerName));
            Contract.RequireNotEmpty(applicationName, nameof(applicationName));

            PreserveApplicationSettings = true;

            this.DeveloperName = developerName;
            this.ApplicationName = applicationName;
            this.CompatibilityShim = new NETCoreFrameworkPlatformCompatibilityShim();

            InitializeApplication();
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageID>.ReceiveMessage(FrameworkMessageID type, MessageData data)
        {
            OnReceivedMessage(type, data);
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Runs the Sedulous application.
        /// </summary>
        public void Run()
        {
            Contract.EnsureNotDisposed(this, disposed);

            OnInitializing();

            CreateFrameworkContext();

            InitializeFrameworkContext();

            OnInitialized();

            OnLoadingContent();

            running = true;
            while (running)
            {
                if (IsSuspended)
                {
                    timingLogic.RunOneTickSuspended();
                }
                else
                {
                    timingLogic.RunOneTick();
                }
                Thread.Yield();
            }

            timingLogic.Cleanup();

            context.WaitForPendingTasks(true);
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        public void Exit()
        {
            Contract.EnsureNotDisposed(this, disposed);

            if (FrameworkPlatformInfo.CurrentPlatform == FrameworkPlatform.iOS)
            {
                System.Diagnostics.Debug.WriteLine(FrameworkStrings.CannotQuitOniOS);
            }
            else
            {
                running = false;
            }
        }

        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        public FrameworkContext FrameworkContext
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);
                Contract.Ensure(created, FrameworkStrings.ContextMissing);

                return context;
            }
        }

        /// <inheritdoc/>
        public String DeveloperName { get; }

        /// <inheritdoc/>
        public String ApplicationName { get; }

        /// <inheritdoc/>
        public IFrameworkPlatformCompatibilityShim CompatibilityShim { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the application's primary window is synchronized
        /// to the vertical retrace when rendering (i.e., whether vsync is enabled).
        /// </summary>
        public Boolean SynchronizeWithVerticalRetrace
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                if (primary == null)
                    throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

                return primary.SynchronizeWithVerticalRetrace;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);

                if (primary == null)
                    throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

                primary.SynchronizeWithVerticalRetrace = value;
            }
        }

        /// <inheritdoc/>
        public Boolean IsActive
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                if (primary == null)
                    return false;

                lock (stateSyncObject)
                    return primary.Active && !suspended;
            }
        }

        /// <inheritdoc/>
        public Boolean IsSuspended
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                lock (stateSyncObject)
                    return suspended;
            }
        }

        /// <inheritdoc/>
        public Boolean IsFixedTimeStep
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                return this.isFixedTimeStep;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);

                this.isFixedTimeStep = value;
                if (timingLogic != null)
                {
                    timingLogic.IsFixedTimeStep = value;
                }
            }
        }

        /// <inheritdoc/>
        public TimeSpan TargetElapsedTime
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                return this.targetElapsedTime;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);
                Contract.EnsureRange(value.TotalMilliseconds >= 0, nameof(value));

                this.targetElapsedTime = value;
                if (timingLogic != null)
                {
                    timingLogic.TargetElapsedTime = value;
                }
            }
        }

        /// <inheritdoc/>
        public TimeSpan InactiveSleepTime
        {
            get
            {
                Contract.EnsureNotDisposed(this, disposed);

                return this.inactiveSleepTime;
            }
            set
            {
                Contract.EnsureNotDisposed(this, disposed);

                this.inactiveSleepTime = value;
                if (timingLogic != null)
                {
                    timingLogic.InactiveSleepTime = value;
                }
            }
        }

        /// <summary>
        /// Called when the application is creating its Sedulous context.
        /// </summary>
        /// <returns>The Sedulous context.</returns>
        protected abstract FrameworkContext OnCreatingFrameworkContext();

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        protected virtual void Dispose(Boolean disposing)
        {
            lock (stateSyncObject)
            {
                if (!disposed)
                {
                    if (disposing && context != null)
                    {
                        context.Messages.Unsubscribe(this);

                        DisposePlatformResources();

                        if (primary != null)
                        {
                            primary.Drawing -= uv_Drawing;
                            primary = null;
                        }

                        context.Dispose();

                        context.Updating -= uv_Updating;
                        context.Shutdown -= uv_Shutdown;
                        context.WindowDrawing -= uv_WindowDrawing;
                        context.WindowDrawn -= uv_WindowDrawn;

                        timingLogic = null;
                    }
                    disposed = true;
                }
            }
        }

        /// <summary>
        /// Called when the application is initializing.
        /// </summary>
        protected virtual void OnInitializing()
        {

        }

        /// <summary>
        /// Called after the application has been initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {

        }

        /// <summary>
        /// Called when the application is loading content.
        /// </summary>
        protected virtual void OnLoadingContent()
        {

        }

        /// <summary>
        /// Called when the application state is being updated.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        protected virtual void OnUpdating(FrameworkTime time)
        {

        }

        /// <summary>
        /// Called when the scene is being drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        protected virtual void OnDrawing(FrameworkTime time)
        {

        }

        /// <summary>
        /// Called when one of the application's windows is about to be drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        /// <param name="window">The window that is about to be drawn.</param>
        protected virtual void OnWindowDrawing(FrameworkTime time, IFrameworkWindow window)
        {

        }

        /// <summary>
        /// Called after one of the application's windows has been drawn.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        /// <param name="window">The window that was just drawn.</param>
        protected virtual void OnWindowDrawn(FrameworkTime time, IFrameworkWindow window)
        {

        }

        /// <summary>
        /// Called when the application is about to be suspended.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnSuspending()
        {
        }

        /// <summary>
        /// Called when the application has been suspended.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnSuspended()
        {
            SaveSettings();
        }

        /// <summary>
        /// Called when the application is about to be resumed.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnResuming()
        {

        }

        /// <summary>
        /// Called when the application has been resumed.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnResumed()
        {

        }

        /// <summary>
        /// Called when the operating system is attempting to reclaim memory.
        /// </summary>
        /// <remarks>When implementing this method, be aware that it can potentially be called
        /// from a thread other than the main Sedulous thread.</remarks>
        protected internal virtual void OnReclaimingMemory()
        {

        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        protected virtual void OnShutdown()
        {

        }

        /// <summary>
        /// Occurs when the context receives a message from its queue.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="data">The message data.</param>
        protected virtual void OnReceivedMessage(FrameworkMessageID type, MessageData data)
        {
            if (type == FrameworkMessages.ApplicationTerminating)
            {
                running = false;
            }
            else if (type == FrameworkMessages.ApplicationSuspending)
            {
                OnSuspending();

                lock (stateSyncObject)
                    suspended = true;
            }
            else if (type == FrameworkMessages.ApplicationSuspended)
            {
                OnSuspended();
            }
            else if (type == FrameworkMessages.ApplicationResuming)
            {
                OnResuming();
            }
            else if (type == FrameworkMessages.ApplicationResumed)
            {
                timingLogic?.ResetElapsed();

                lock (stateSyncObject)
                    suspended = false;

                OnResumed();
            }
            else if (type == FrameworkMessages.LowMemory)
            {
                OnReclaimingMemory();
            }
            else if (type == FrameworkMessages.Quit)
            {
                if (FrameworkPlatformInfo.CurrentPlatform == FrameworkPlatform.iOS)
                {
                    System.Diagnostics.Debug.WriteLine(FrameworkStrings.CannotQuitOniOS);
                }
                else
                {
                    running = false;
                }
            }
        }

        /// <summary>
        /// Creates the timing logic for this host process.
        /// </summary>
        protected virtual IFrameworkHostTimingLogic CreateTimingLogic()
        {
            var timingLogic = new FrameworkHostTimingLogic(this);
            timingLogic.IsFixedTimeStep = this.IsFixedTimeStep;
            timingLogic.TargetElapsedTime = this.TargetElapsedTime;
            timingLogic.InactiveSleepTime = this.InactiveSleepTime;
            return timingLogic;
        }

        /// <summary>
        /// Ensures that the assembly which contains the specified type is linked on platforms
        /// which require ahead-of-time compilation.
        /// </summary>
        /// <typeparam name="T">One of the types defined by the assembly to link.</typeparam>
        protected void EnsureAssemblyIsLinked<T>()
        {
            Console.WriteLine("Touching '" + typeof(T).Assembly.FullName + "' to ensure linkage...");
        }

        /// <summary>
        /// Uses a file source which is appropriate to the current platform.
        /// </summary>
        /// <returns><see langword="true"/> if a platform-specific file source was used; otherwise, <see langword="false"/>.</returns>
        protected Boolean UsePlatformSpecificFileSource()
        {
            return false;
        }

        /// <summary>
        /// Sets the file system source to an archive file loaded from a manifest resource stream,
        /// if the specified manifest resource exists.
        /// </summary>
        /// <param name="name">The name of the manifest resource being loaded as the file system source.</param>
        /// <returns><see langword="true"/> if the file source was set; otherwise, <see langword="false"/>.</returns>
        protected Boolean SetFileSourceFromManifestIfExists(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            var asm = GetType().Assembly;
            if (asm.GetManifestResourceNames().Contains(name))
            {
                FileSystemService.Source = ContentArchive.FromArchiveFile(() =>
                {
                    return asm.GetManifestResourceStream(name);
                });
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the file system source to an archive file loaded from a manifest resource stream.
        /// </summary>
        /// <param name="name">The name of the manifest resource being loaded as the file system source.</param>
        protected void SetFileSourceFromManifest(String name)
        {
            Contract.RequireNotEmpty(name, nameof(name));

            var asm = GetType().Assembly;
            if (!asm.GetManifestResourceNames().Contains(name))
                throw new FileNotFoundException(name);

            FileSystemService.Source = ContentArchive.FromArchiveFile(() =>
            {
                return asm.GetManifestResourceStream(name);
            });
        }

        /// <summary>
        /// Populates the specified Sedulous configuration with the application's initial values.
        /// </summary>
        /// <param name="configuration">The <see cref="FrameworkConfiguration"/> to populate.</param>
        protected void PopulateConfiguration(FrameworkConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            PopulateConfigurationFromSettings(configuration);
        }

        /// <summary>
        /// Gets the directory that contains the application's local configuration files.
        /// If the directory does not already exist, it will be created.
        /// </summary>
        /// <returns>The directory that contains the application's local configuration files.</returns>
        protected String GetLocalApplicationSettingsDirectory()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DeveloperName, ApplicationName);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets the directory that contains the application's roaming configuration files.
        /// If the directory does not already exist, it will be created.
        /// </summary>
        /// <returns>The directory that contains the application's roaming configuration files.</returns>
        protected String GetRoamingApplicationSettingsDirectory()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DeveloperName, ApplicationName);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application's internal framework settings
        /// should be preserved between instances.
        /// </summary>
        protected Boolean PreserveApplicationSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the application's state.
        /// </summary>
        partial void InitializeApplication();

        /// <summary>
        /// Disposes any platform-specific resources.
        /// </summary>
        partial void DisposePlatformResources();

        /// <summary>
        /// Creates the application's Sedulous context.
        /// </summary>
        private void CreateFrameworkContext()
        {
            LoadSettings();

            context = FrameworkContext.EnsureSuccessfulCreation(OnCreatingFrameworkContext);
            if (context == null)
                throw new InvalidOperationException(FrameworkStrings.ContextNotCreated);

            this.created = true;
        }

        private void InitializeFrameworkContext()
        {
            context.ConfigureFactory((factory) =>
            {
                factory.SetFactoryMethod<SurfaceSourceFactory>((stream) => new NETCoreSurfaceSource(stream));
                factory.SetFactoryMethod<SurfaceSaverFactory>(() => new NETCoreSurfaceSaver());
                factory.SetFactoryMethod<IconLoaderFactory>(() => new NETCoreIconLoader());
                factory.SetFactoryMethod<FileSystemServiceFactory>(() => new FileSystemService());
                factory.SetFactoryMethod<ScreenRotationServiceFactory>((display) => new NETCoreScreenOrientationService(display));

                switch (FrameworkPlatformInfo.CurrentPlatform)
                {
                    case FrameworkPlatform.Windows:
                        factory.SetFactoryMethod<ScreenDensityServiceFactory>((display) => new NETCoreScreenDensityService_Windows(context, display));
                        break;

                    default:
                        factory.SetFactoryMethod<ScreenDensityServiceFactory>((display) => new NETCoreScreenDensityService(context, display));
                        break;
                }

                var softwareKeyboardService = new NETCoreSoftwareKeyboardService();
                factory.SetFactoryMethod<SoftwareKeyboardServiceFactory>(() => softwareKeyboardService);
            });

            context.Initialize();

            ApplySettings();

            this.timingLogic = CreateTimingLogic();
            if (this.timingLogic == null)
                throw new InvalidOperationException(FrameworkStrings.InvalidTimingLogic);

            this.context.Messages.Subscribe(this,
                FrameworkMessages.ApplicationTerminating,
                FrameworkMessages.ApplicationSuspending,
                FrameworkMessages.ApplicationSuspended,
                FrameworkMessages.ApplicationResuming,
                FrameworkMessages.ApplicationResumed,
                FrameworkMessages.LowMemory,
                FrameworkMessages.Quit);
            this.context.Updating += uv_Updating;
            this.context.Shutdown += uv_Shutdown;
            this.context.WindowDrawing += uv_WindowDrawing;
            this.context.WindowDrawn += uv_WindowDrawn;

            this.context.GetPlatform().Windows.PrimaryWindowChanging += uv_PrimaryWindowChanging;
            this.context.GetPlatform().Windows.PrimaryWindowChanged += uv_PrimaryWindowChanged;
            HookPrimaryWindowEvents();
        }

        /// <summary>
        /// Hooks into the primary window's events.
        /// </summary>
        private void HookPrimaryWindowEvents()
        {
            if (primary != null)
            {
                primary.Drawing -= uv_Drawing;
            }

            primary = context.GetPlatform().Windows.GetPrimary();

            if (primary != null)
            {
                primary.Drawing += uv_Drawing;
            }
        }

        /// <summary>
        /// Loads the application's settings.
        /// </summary>
        partial void LoadSettings();

        /// <summary>
        /// Saves the application's settings.
        /// </summary>
        partial void SaveSettings();

        /// <summary>
        /// Applies the application's settings.
        /// </summary>
        partial void ApplySettings();

        /// <summary>
        /// Populates the Sedulous configuration from the application settings.
        /// </summary>
        partial void PopulateConfigurationFromSettings(FrameworkConfiguration configuration);

        /// <summary>
        /// Handles the Sedulous window manager's PrimaryWindowChanging event.
        /// </summary>
        /// <param name="window">The primary window.</param>
        private void uv_PrimaryWindowChanging(IFrameworkWindow window)
        {
            SaveSettings();
        }

        /// <summary>
        /// Handles the Sedulous window manager's PrimaryWindowChanged event.
        /// </summary>
        /// <param name="window">The primary window.</param>
        private void uv_PrimaryWindowChanged(IFrameworkWindow window)
        {
            HookPrimaryWindowEvents();
        }

        /// <summary>
        /// Handles the Sedulous window's Drawing event.
        /// </summary>
        /// <param name="window">The window being drawn.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        private void uv_Drawing(IFrameworkWindow window, FrameworkTime time)
        {
            OnDrawing(time);
        }

        /// <summary>
        /// Handles the Sedulous context's Updating event.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        private void uv_Updating(FrameworkContext context, FrameworkTime time)
        {
            OnUpdating(time);
        }

        /// <summary>
        /// Handles the Sedulous context's Shutdown event.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        private void uv_Shutdown(FrameworkContext context)
        {
            OnShutdown();
        }

        /// <summary>
        /// Handles the Sedulous context's <see cref="FrameworkContext.WindowDrawing"/> event.
        /// </summary>
        private void uv_WindowDrawing(FrameworkContext context, FrameworkTime time, IFrameworkWindow window)
        {
            OnWindowDrawing(time, window);
        }

        /// <summary>
        /// Handles the Sedulous context's <see cref="FrameworkContext.WindowDrawn"/> event.
        /// </summary>
        private void uv_WindowDrawn(FrameworkContext context, FrameworkTime time, IFrameworkWindow window)
        {
            OnWindowDrawn(time, window);
        }

        // Property values.
        private FrameworkContext context;

        // State values.
        private readonly Object stateSyncObject = new Object();
        private IFrameworkHostTimingLogic timingLogic;
        private Boolean created;
        private Boolean running;
        private Boolean suspended;
        private Boolean disposed;
        private IFrameworkWindow primary;

        // The application's tick state.
        private Boolean isFixedTimeStep = FrameworkHostTimingLogic.DefaultIsFixedTimeStep;
        private TimeSpan targetElapsedTime = FrameworkHostTimingLogic.DefaultTargetElapsedTime;
        private TimeSpan inactiveSleepTime = FrameworkHostTimingLogic.DefaultInactiveSleepTime;
    }
}