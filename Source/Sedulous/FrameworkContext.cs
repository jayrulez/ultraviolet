using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Graphics;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Graphics.Graphics3D;
using Sedulous.Platform;
using Sedulous.UI;

namespace Sedulous
{
    /// <summary>
    /// Represents a callback that is invoked when the Sedulous Framework logs a debug message.
    /// </summary>
    /// <param name="context">The Sedulous Context that logged the message.</param>
    /// <param name="level">A <see cref="DebugLevels"/> value representing the debug level of the message.</param>
    /// <param name="message">The debug message text.</param>
    public delegate void DebugCallback(FrameworkContext context, DebugLevels level, String message);

    /// <summary>
    /// Represents a method that is called in response to an Sedulous context event.
    /// </summary>
    /// <param name="context">The Sedulous context.</param>
    public delegate void FrameworkContextEventHandler(FrameworkContext context);

    /// <summary>
    /// Represents the method that is called when an Sedulous context is about to draw the current scene.
    /// </summary>
    /// <param name="context">The Sedulous context.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
    public delegate void FrameworkContextDrawEventHandler(FrameworkContext context, FrameworkTime time);

    /// <summary>
    /// Represents the method that is called when an Sedulous context has drawn or is about to draw a particular window.
    /// </summary>
    /// <param name="context">The Sedulous context.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
    /// <param name="window">The window that was drawn or is about to be drawn.</param>
    public delegate void FrameworkContextWindowDrawEventHandler(FrameworkContext context, FrameworkTime time, IFrameworkWindow window);

    /// <summary>
    /// Represents the method that is called when an Sedulous context updates the application state.
    /// </summary>
    /// <param name="context">The Sedulous context.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
    public delegate void FrameworkContextUpdateEventHandler(FrameworkContext context, FrameworkTime time);

    /// <summary>
    /// Represents the Sedulous Framework and all of its subsystems.
    /// </summary>
    public abstract class FrameworkContext
        :
        IMessageSubscriber<FrameworkMessageID>,
        ICrossThreadFrameworkContext,
        IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkContext"/> class.
        /// </summary>
        /// <param name="host">The object that is hosting the Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        public FrameworkContext(IFrameworkHost host, FrameworkConfiguration configuration)
        {
            Contract.Require(host, nameof(host));
            Contract.Require(configuration, nameof(configuration));

            if (configuration.GraphicsConfiguration == null)
                throw new InvalidOperationException(FrameworkStrings.MissingGraphicsConfiguration);

            AcquireContext();

            this.IsRunningInServiceMode = configuration.EnableServiceMode;
            this.IsHardwareInputDisabled = configuration.IsHardwareInputDisabled;

            this.Properties = new FrameworkContextProperties();
            this.Properties.SupportsHighDensityDisplayModes = configuration.SupportsHighDensityDisplayModes;
            this.Properties.SrgbDefaultForSurface2D = configuration.GraphicsConfiguration.SrgbDefaultForSurface2D;
            this.Properties.SrgbDefaultForSurface3D = configuration.GraphicsConfiguration.SrgbDefaultForSurface3D;
            this.Properties.SrgbDefaultForTexture2D = configuration.GraphicsConfiguration.SrgbDefaultForTexture2D;
            this.Properties.SrgbDefaultForTexture3D = configuration.GraphicsConfiguration.SrgbDefaultForTexture3D;
            this.Properties.SrgbDefaultForRenderBuffer2D = configuration.GraphicsConfiguration.SrgbDefaultForRenderBuffer2D;

            if (FrameworkPlatformInfo.CurrentRuntime == FrameworkRuntime.CoreCLR)
                this.Properties.SupportsHighDensityDisplayModes = false;

            this.host = host;

            this.thread = Thread.CurrentThread;

            this.messages = new LocalMessageQueue<FrameworkMessageID>();
            this.messages.Subscribe(this, FrameworkMessages.Quit);

            this.syncContext = new FrameworkSynchronizationContext(this);
            ChangeSynchronizationContext(syncContext);

            this.taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.taskFactory = new TaskFactory(taskScheduler);
        }

        /// <summary>
        /// Ensures that the specified function produces a valid instance of <see cref="FrameworkContext"/>. If it does not,
        /// then the current context is immediately disposed. This method should only be called by Sedulous host implementations.
        /// </summary>
        /// <param name="fn">The function which will create the Sedulous context.</param>
        /// <returns>The Sedulous context that was created.</returns>
        public static FrameworkContext EnsureSuccessfulCreation(Func<FrameworkContext> fn)
        {
            Contract.Require(fn, nameof(fn));

            try
            {
                var context = fn();
                if (context == null)
                {
                    throw new InvalidOperationException();
                }
                return context;
            }
            catch (Exception e1)
            {
                try
                {
                    var current = RequestCurrent();
                    if (current != null)
                    {
                        current.Dispose();
                    }
                }
                catch (Exception e2)
                {
                    var error = new StringBuilder();
                    error.AppendLine(Assembly.GetEntryAssembly().FullName);
                    error.AppendLine();
                    error.AppendLine($"An exception occurred while creating the Sedulous context, and Sedulous failed to cleanly shut down.");
                    error.AppendLine();
                    error.AppendLine($"Exception which occurred during context creation:");
                    error.AppendLine();
                    error.AppendLine(e1.ToString());
                    error.AppendLine();
                    error.AppendLine($"Exception which occurred during shutdown:");
                    error.AppendLine();
                    error.AppendLine(e2.ToString());

                    try
                    {
                        var errorDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        var errorPath = $"uv-error-{DateTime.Now:yyyy-MM-dd-HH-mm-ss-fff}.txt";
                        File.WriteAllText(Path.Combine(errorDir, errorPath), error.ToString());
                    }
                    catch (IOException) { }
                }
                throw;
            }
        }

        /// <summary>
        /// Retrieves the current Sedulous context, throwing an exception if it does not exist.
        /// </summary>
        /// <returns>The current Sedulous context, or <see langword="null"/> if no contex exists.</returns>
        public static FrameworkContext RequestCurrent()
        {
            return current;
        }

        /// <summary>
        /// Retrieves the current Sedulous context, throwing an exception if it does not exist.
        /// </summary>
        /// <returns>The current Sedulous context.</returns>
        public static FrameworkContext DemandCurrent()
        {
            if (current == null)
                throw new InvalidOperationException(FrameworkStrings.ContextMissing);

            return current;
        }

        /// <summary>
        /// Receives a message that has been published to a queue.
        /// </summary>
        /// <param name="type">The type of message that was received.</param>
        /// <param name="data">The data for the message that was received.</param>
        void IMessageSubscriber<FrameworkMessageID>.ReceiveMessage(FrameworkMessageID type, MessageData data)
        {
            Contract.EnsureNotDisposed(this, Disposed);

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
        /// Processes the context's message queue.
        /// </summary>
        public void ProcessMessages()
        {
            messages.Process();
        }

        /// <summary>
        /// Processes a single queued work item, if any work items have been queued.
        /// </summary>
        public void ProcessSingleWorkItem()
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Ensure(thread == Thread.CurrentThread, FrameworkStrings.WorkItemsMustBeProcessedOnMainThread);

            var syncContext = SynchronizationContext.Current as FrameworkSynchronizationContext;
            if (syncContext != null)
                syncContext.ProcessSingleWorkItem();
        }

        /// <summary>
        /// Processes all queued work items.
        /// </summary>
        public void ProcessWorkItems()
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Ensure(thread == Thread.CurrentThread, FrameworkStrings.WorkItemsMustBeProcessedOnMainThread);

            var syncContext = SynchronizationContext.Current as FrameworkSynchronizationContext;
            if (syncContext != null)
                syncContext.ProcessWorkItems();
        }

        /// <summary>
        /// Called when a new frame is started.
        /// </summary>
        public void HandleFrameStart()
        {
            FrameworkProfiler.BeginSection(FrameworkProfilerSections.Frame);
            OnFrameStart();
        }

        /// <summary>
        /// Called when a frame is completed.
        /// </summary>
        public void HandleFrameEnd()
        {
            OnFrameEnd();
            FrameworkProfiler.EndSection(FrameworkProfilerSections.Frame);
        }

        /// <summary>
        /// Updates the game state while the application is suspended.
        /// </summary>
        public virtual void UpdateSuspended()
        {

        }

        /// <summary>
        /// Updates the game state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public virtual void Update(FrameworkTime time)
        {

        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        public virtual void Draw(FrameworkTime time)
        {

        }

        /// <summary>
        /// Gets the platform interop subsystem.
        /// </summary>
        /// <returns>The platform interop subsystem.</returns>
        public abstract IPlatformSubsystem GetPlatform();

        /// <summary>
        /// Gets the content management subsystem.
        /// </summary>
        /// <returns>The content management subsystem.</returns>
        public abstract IContentSubsystem GetContent();

        /// <summary>
        /// Gets the graphics subsystem.
        /// </summary>
        /// <returns>The graphics subsystem.</returns>
        public abstract IGraphicsSubsystem GetGraphics();

        /// <summary>
        /// Gets the audio subsystem.
        /// </summary>
        /// <returns>The audio subsystem.</returns>
        public abstract IAudioSubsystem GetAudio();

        /// <summary>
        /// Gets the input subsystem.
        /// </summary>
        /// <returns>The input subsystem.</returns>
        public abstract IInputSubsystem GetInput();

        /// <summary>
        /// Gets the user interface subsystem.
        /// </summary>
        /// <returns>The user interface subsystem.</returns>
        public abstract IUISubsystem GetUI();

        /// <summary>
        /// Gets the factory method of the specified delegate type.
        /// </summary>
        /// <typeparam name="T">The delegate type of the factory method to retrieve.</typeparam>
        /// <returns>The default factory method of the specified delegate type, or <see langword="null"/> if no such factory method is registered.</returns>
        public T TryGetFactoryMethod<T>() where T : class
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return factory.TryGetFactoryMethod<T>();
        }

        /// <summary>
        /// Attempts to retrieve a named factory method of the specified delegate type.
        /// </summary>
        /// <typeparam name="T">The delegate type of the factory method to retrieve.</typeparam>
        /// <param name="name">The name of the factory method to retrieve.</param>
        /// <returns>The specified named factory method, or <see langword="null"/> if no such factory method is registered.</returns>
        public T TryGetFactoryMethod<T>(String name) where T : class
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return factory.TryGetFactoryMethod<T>(name);
        }

        /// <summary>
        /// Gets the factory method of the specified delegate type.
        /// </summary>
        /// <typeparam name="T">The delegate type of the factory method to retrieve.</typeparam>
        /// <returns>The default factory method of the specified delegate type.</returns>
        public T GetFactoryMethod<T>() where T : class
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return factory.GetFactoryMethod<T>();
        }

        /// <summary>
        /// Gets a named factory method of the specified delegate type.
        /// </summary>
        /// <typeparam name="T">The delegate type of the factory method to retrieve.</typeparam>
        /// <param name="name">The name of the factory method to retrieve.</param>
        /// <returns>The specified named factory method.</returns>
        public T GetFactoryMethod<T>(String name) where T : class
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.RequireNotEmpty(name, nameof(name));

            return factory.GetFactoryMethod<T>(name);
        }

        /// <summary>
        /// Waits for any pending tasks to complete.
        /// </summary>
        /// <param name="cancel">A value indicating whether to cancel pending tasks.</param>
        public void WaitForPendingTasks(Boolean cancel = false)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (cancel)
                cancellationTokenSource.Cancel();

            while (true)
            {
                var done = true;

                lock (tasksPending)
                {
                    foreach (var task in tasksPending)
                    {
                        if (task.Status != TaskStatus.Created && !task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                        {
                            done = false;
                        }
                    }
                }

                if (done)
                    break;

                ProcessWorkItems();
                Thread.Yield();
            }
        }

        /// <summary>
        /// Spawns a new task.
        /// </summary>
        /// <remarks>Tasks spawned using this method will not be started until the next call to <see cref="Update(FrameworkTime)"/>, and will prevent
        /// the Sedulous context from shutting down until they complete or are canceled.  Do not attempt to <see cref="Task.Wait()"/> on these
        /// tasks from the main Sedulous thread; doing so will introduce a deadlock.</remarks>
        /// <param name="action">The action to perform within the task.</param>
        /// <returns>The <see cref="Task"/> that was spawned.</returns>
        public Task SpawnTask(Action<CancellationToken> action)
        {
            Contract.Require(action, nameof(action));
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureNot(Disposing, FrameworkStrings.CannotSpawnTasks);

            var token = cancellationTokenSource.Token;
            var task = taskFactory.StartNew(() => action(token), token, TaskCreationOptions.None, TaskScheduler.Default);

            lock (tasksPending)
                tasksPending.Add(task);

            return task;
        }

        /// <summary>
        /// Queues a work item for execution on Sedulous's main thread.
        /// </summary>
        /// <param name="workItem">The work item to execute on Sedulous's main thread.</param>
        /// <param name="state">An object containing state to pass to the work item.</param>
        /// <param name="options">A set of <see cref="WorkItemOptions"/> flags which indicate how the task should be executed.</param>
        /// <returns>A <see cref="Task"/> that encapsulates the work item.</returns>
        public Task QueueWorkItem(Action<Object> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None)
        {
            Contract.Require(workItem, nameof(workItem));
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureNot(Disposing, FrameworkStrings.CannotQueueWorkItems);

            if (IsExecutingOnCurrentThread && (options & WorkItemOptions.ForceAsynchronousExecution) != WorkItemOptions.ForceAsynchronousExecution)
            {
                workItem(state);
                return (options & WorkItemOptions.ReturnNullOnSynchronousExecution) == WorkItemOptions.ReturnNullOnSynchronousExecution ?
                    null : Task.FromResult(true);
            }
            return taskFactory.StartNew(workItem, state);
        }

        /// <summary>
        /// Queues a work item for execution on Sedulous's main thread.
        /// </summary>
        /// <param name="workItem">The work item to execute on Sedulous's main thread.</param>
        /// <param name="state">An object containing state to pass to the work item.</param>
        /// <param name="options">A set of <see cref="WorkItemOptions"/> flags which indicate how the task should be executed.</param>
        /// <returns>A <see cref="Task"/> that encapsulates the work item.</returns>
        public Task QueueWorkItem(Func<Object, Task> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None)
        {
            Contract.Require(workItem, nameof(workItem));
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureNot(Disposing, FrameworkStrings.CannotQueueWorkItems);

            if (IsExecutingOnCurrentThread && (options & WorkItemOptions.ForceAsynchronousExecution) != WorkItemOptions.ForceAsynchronousExecution)
            {
                var result = workItem(state);
                return (options & WorkItemOptions.ReturnNullOnSynchronousExecution) == WorkItemOptions.ReturnNullOnSynchronousExecution ?
                    null : Task.FromResult(result).Unwrap();
            }
            return taskFactory.StartNew(workItem, state).Unwrap();
        }

        /// <summary>
        /// Queues a work item for execution on Sedulous's main thread.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the work item.</typeparam>
        /// <param name="workItem">The work item to execute on Sedulous's main thread.</param>
        /// <param name="state">An object containing state to pass to the work item.</param>
        /// <param name="options">A set of <see cref="WorkItemOptions"/> flags which indicate how the task should be executed.</param>
        /// <returns>A <see cref="Task"/> that encapsulates the work item.</returns>
        public Task<T> QueueWorkItem<T>(Func<Object, T> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None)
        {
            Contract.Require(workItem, nameof(workItem));
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureNot(Disposing, FrameworkStrings.CannotQueueWorkItems);

            if (IsExecutingOnCurrentThread && (options & WorkItemOptions.ForceAsynchronousExecution) != WorkItemOptions.ForceAsynchronousExecution)
            {
                var result = workItem(state);
                return (options & WorkItemOptions.ReturnNullOnSynchronousExecution) == WorkItemOptions.ReturnNullOnSynchronousExecution ?
                    null : Task.FromResult(result);
            }
            return taskFactory.StartNew(workItem, state);
        }

        /// <summary>
        /// Queues a work item for execution on Sedulous's main thread.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the work item.</typeparam>
        /// <param name="workItem">The work item to execute on Sedulous's main thread.</param>
        /// <param name="state">An object containing state to pass to the work item.</param>
        /// <param name="options">A set of <see cref="WorkItemOptions"/> flags which indicate how the task should be executed.</param>
        /// <returns>A <see cref="Task"/> that encapsulates the work item.</returns>
        public Task<T> QueueWorkItem<T>(Func<Object, Task<T>> workItem, Object state = null, WorkItemOptions options = WorkItemOptions.None)
        {
            Contract.Require(workItem, nameof(workItem));
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureNot(Disposing, FrameworkStrings.CannotQueueWorkItems);

            if (IsExecutingOnCurrentThread && (options & WorkItemOptions.ForceAsynchronousExecution) != WorkItemOptions.ForceAsynchronousExecution)
            {
                var result = workItem(state);
                return (options & WorkItemOptions.ReturnNullOnSynchronousExecution) == WorkItemOptions.ReturnNullOnSynchronousExecution ?
                    null : Task.FromResult(result).Unwrap();
            }
            return taskFactory.StartNew(workItem, state).Unwrap();
        }

        /// <summary>
        /// Ensures that the specified resource was created by this context.
        /// This method is compiled out if the <c>DEBUG</c> compilation symbol is not specified.
        /// </summary>
        /// <param name="resource">The <see cref="FrameworkResource"/> to validate.</param>
        [Conditional("DEBUG")]
        public void ValidateResource(FrameworkResource resource)
        {
            if (resource != null && resource.FrameworkContext != this)
                throw new InvalidOperationException(FrameworkStrings.InvalidResource);
        }

        /// <summary>
        /// Gets the context's runtime properties.
        /// </summary>
        public FrameworkContextProperties Properties { get; }

        /// <summary>
        /// Gets the version of the runtime on which this context is running.
        /// </summary>
        public Version RuntimeVersion => FrameworkPlatformInfo.CurrentRuntimeVersion;

        /// <summary>
        /// Gets the runtime on which this context is running.
        /// </summary>
        public FrameworkRuntime Runtime => FrameworkPlatformInfo.CurrentRuntime;

        /// <summary>
        /// Gets the platform on which this context is running.
        /// </summary>
        public FrameworkPlatform Platform => FrameworkPlatformInfo.CurrentPlatform;

        /// <summary>
        /// Gets the object that is hosting the Sedulous context.
        /// </summary>
        public IFrameworkHost Host => host;

        /// <summary>
        /// Gets the context's message queue.
        /// </summary>
        public IMessageQueue<FrameworkMessageID> Messages => messages;

        /// <summary>
        /// Gets or sets a value indicating whether the context is currently processing messages
        /// from the physical input devices.
        /// </summary>
        public Boolean IsHardwareInputDisabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the context is running in service mode.
        /// </summary>
        public Boolean IsRunningInServiceMode { get; }

        /// <summary>
        /// Gets a value indicating whether the current thread is the thread which
        /// created the Sedulous context.
        /// </summary>
        /// <remarks>Many tasks, such as content loading, must take place on the Sedulous
        /// context's main thread.  Such tasks can be queued using the <see cref="QueueWorkItem(Action{Object}, Object, WorkItemOptions)"/> method
        /// or one of its overloads, which will run the task at the start of the next update.</remarks>
        public Boolean IsExecutingOnCurrentThread => Thread.CurrentThread == thread;

        /// <summary>
        /// Gets a value indicating whether the context has been initialized.
        /// </summary>
        public Boolean IsInitialized { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the object is in the process of being disposed.
        /// </summary>
        public Boolean Disposing { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        public Boolean Disposed { get; private set; }

        /// <summary>
        /// Occurs when the current context is invalidated.
        /// </summary>
        public static event EventHandler ContextInvalidated;

        /// <summary>
        /// Occurs when the current context is initialized.
        /// </summary>
        public static event EventHandler ContextInitialized;

        /// <summary>
        /// Occurs when a new frame is started.
        /// </summary>
        public event FrameworkContextEventHandler FrameStart;

        /// <summary>
        /// Occurs when a frame is completed.
        /// </summary>
        public event FrameworkContextEventHandler FrameEnd;

        /// <summary>
        /// Occurs when the context is preparing to draw the current scene. This event is called
        /// before the context associates itself to any windows.
        /// </summary>
        public event FrameworkContextDrawEventHandler Drawing;

        /// <summary>
        /// Occurs when the context is preparing to draw a particular window.
        /// </summary>
        public event FrameworkContextWindowDrawEventHandler WindowDrawing;

        /// <summary>
        /// Occurs after the context has drawn a particular window.
        /// </summary>
        public event FrameworkContextWindowDrawEventHandler WindowDrawn;

        /// <summary>
        /// Occurs when the context is about to update the state of its subsystems.
        /// </summary>
        public event FrameworkContextUpdateEventHandler UpdatingSubsystems;

        /// <summary>
        /// Occurs when the context is updating the application's state.
        /// </summary>
        public event FrameworkContextUpdateEventHandler Updating;

        /// <summary>
        /// Occurs when the context is initialized.
        /// </summary>
        public event FrameworkContextEventHandler Initialized;

        /// <summary>
        /// Occurs when the Sedulous context is being shut down.
        /// </summary>
        public event FrameworkContextEventHandler Shutdown;
        
        /// <summary>
        /// Acquires an exclusive context claim, preventing other instances from being instantiated.
        /// </summary>
        private void AcquireContext()
        {
            lock (syncObject)
            {
                if (current != null)
                    throw new InvalidOperationException(FrameworkStrings.ContextAlreadyExists);

                current = this;
            }
        }

        /// <summary>
        /// Releases the current exclusive context claim.
        /// </summary>
        private void ReleaseContext()
        {
            lock (syncObject)
            {
                if (current == this)
                {
                    current = null;
                    OnContextInvalidated();
                }
            }
        }

        /// <summary>
        /// Configures the context's factory.
        /// </summary>
        public void ConfigureFactory(Action<FrameworkFactory> configureAction)
        {
            if (IsInitialized)
                throw new Exception("Cannot configure factory on already initialized context.");

            if (configureAction != null)
            {
                configureAction(factory);
            }
        }

        /// <summary>
        /// Initializes the context
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
                throw new Exception("Context was already initialized.");

            ConfigureFactory();
            OnInitialize();
            {
                GetContent().Processors.SetFallbackType<FrameworkFont>(typeof(SpriteFont));

                // Content
                GetContent().Importers.RegisterImporter<JsonContentImporter>(".json");
                GetContent().Importers.RegisterImporter<TextContentImporter>(".txt");
                GetContent().Importers.RegisterImporter<XmlContentImporter>(".xml");

                GetContent().Processors.RegisterProcessor<JsonContentProcessor>();
                GetContent().Processors.RegisterProcessor<PassthroughContentProcessor>();
                GetContent().Processors.RegisterProcessor<XmlContentProcessor>();

                GetContent().Processors.RegisterProcessor<CursorCollectionProcessorFromJObject>();
                GetContent().Processors.RegisterProcessor<CursorCollectionProcessorFromXDocument>();

                // Graphics 2D
                GetContent().Importers.RegisterImporter<SpriteImporterToJObject>(".jssprite");
                GetContent().Importers.RegisterImporter<SpriteImporterToXDocument>(".sprite");

                GetContent().Processors.RegisterProcessor<SpriteProcessorFromJObject>();
                GetContent().Processors.RegisterProcessor<SpriteProcessorFromXDocument>();

                // Graphics 3D
                GetContent().Importers.RegisterImporter<GlbModelImporter>(".glb");
                GetContent().Importers.RegisterImporter<GltfModelImporter>(".gltf");
                GetContent().Importers.RegisterImporter<StlModelImporter>(".stl");

                GetContent().Processors.RegisterProcessor<GltfModelProcessor>();
                GetContent().Processors.RegisterProcessor<GltfSkinnedModelProcessor>();
                GetContent().Processors.RegisterProcessor<StlModelProcessor>();

                // Graphics
                GetContent().Processors.RegisterProcessor<TextureAtlasProcessorFromJObject>();
                GetContent().Processors.RegisterProcessor<TextureAtlasProcessorFromXDocument>();

                // Curve
                GetContent().Processors.RegisterProcessor<CurveProcessor>();

                // UI
                GetContent().Processors.RegisterProcessor<UIPanelDefinitionProcessor>();
            }

            IsInitialized = true;

            OnInitialized();
            OnContextInitialized();
        }

        /// <summary>
        /// Initializes the context and marks it ready for use.
        /// Responsible for configuring plugins, initializing plugins, and creating subsystems
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Initializes the context's view provider.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        protected void InitializeViewProvider(FrameworkConfiguration configuration)
        {
            var initializerFactory = TryGetFactoryMethod<UIViewProviderInitializerFactory>();
            if (initializerFactory != null)
            {
                var initializer = initializerFactory();
                initializer.Initialize(this, configuration.ViewProviderConfiguration);
            }
        }

        /// <summary>
        /// Configures the context's factories.
        /// </summary>
        protected virtual void ConfigureFactory()
        {
            factory.SetFactoryMethod(this.IsRunningInServiceMode ?
                new SpriteBatchFactory((uv) => null) :
                new SpriteBatchFactory((uv) => new SpriteBatch(uv)));
        }

        /// <summary>
        /// Configures the context's plugins.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        protected void ConfigurePlugins(FrameworkConfiguration configuration)
        {
            foreach (var plugin in configuration.Plugins)
            {
                plugin.Configure(this, Factory);
                plugin.Configured = true;
            }
        }

        /// <summary>
        /// Initializes the context's plugins.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        protected void InitializePlugins(FrameworkConfiguration configuration)
        {
            foreach (var plugin in configuration.Plugins)
            {
                plugin.Initialize(this, Factory);
                plugin.Initialized = true;
            }
        }

        /// <summary>
        /// Updates the context's state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        protected void UpdateContext(FrameworkTime time)
        {
            ProcessWorkItems();
            UpdateTasks();
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; 
        /// <see langword="false"/> if the object is being finalized.</param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            try
            {
                if (disposing)
                {
                    SafeDispose.Dispose(GetUI());
                    SafeDispose.Dispose(GetInput());
                    SafeDispose.Dispose(GetContent());
                    SafeDispose.Dispose(GetPlatform());
                    SafeDispose.Dispose(GetGraphics());
                    SafeDispose.Dispose(GetAudio());
                }

                WaitForPendingTasks(true);

                this.Disposing = true;

                ProcessWorkItems();
                OnShutdown();
            }
            finally
            {
                ChangeSynchronizationContext(null);

                this.Disposed = true;
                this.Disposing = false;

                ReleaseContext();
            }
        }

        /// <summary>
        /// Raises the <see cref="FrameStart"/> event.
        /// </summary>
        protected virtual void OnFrameStart() =>
            FrameStart?.Invoke(this);

        /// <summary>
        /// Raises the <see cref="FrameEnd"/> event.
        /// </summary>
        protected virtual void OnFrameEnd() =>
            FrameEnd?.Invoke(this);

        /// <summary>
        /// Raises the <see cref="Drawing"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        protected virtual void OnDrawing(FrameworkTime time) =>
            Drawing?.Invoke(this, time);

        /// <summary>
        /// Raises the <see cref="WindowDrawing"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        /// <param name="window">The window that is about to be drawn.</param>
        protected virtual void OnWindowDrawing(FrameworkTime time, IFrameworkWindow window) =>
            WindowDrawing?.Invoke(this, time, window);

        /// <summary>
        /// Raises the <see cref="WindowDrawn"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Draw(FrameworkTime)"/>.</param>
        /// <param name="window">The window that was just drawn.</param>
        protected virtual void OnWindowDrawn(FrameworkTime time, IFrameworkWindow window) =>
            WindowDrawn?.Invoke(this, time, window);

        /// <summary>
        /// Raises the <see cref="UpdatingSubsystems"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="Update(FrameworkTime)"/>.</param>
        protected virtual void OnUpdatingSubsystems(FrameworkTime time) =>
            UpdatingSubsystems?.Invoke(this, time);

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="Update(FrameworkTime)"/>.</param>
        protected virtual void OnUpdating(FrameworkTime time) =>
            Updating?.Invoke(this, time);

        /// <summary>
        /// Occurs when the context receives a message from its queue.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="data">The message data.</param>
        protected virtual void OnReceivedMessage(FrameworkMessageID type, MessageData data)
        {

        }

        /// <summary>
        /// Raises the <see cref="Initialized"/> event.
        /// </summary>
        protected virtual void OnInitialized() =>
            Initialized?.Invoke(this);

        /// <summary>
        /// Raises the <see cref="Shutdown"/> event.
        /// </summary>
        protected virtual void OnShutdown() =>
            Shutdown?.Invoke(this);

        /// <summary>
        /// Gets the context's object factory.
        /// </summary>
        protected FrameworkFactory Factory
        {
            get { return factory; }
        }

        /// <summary>
        /// Raises the <see cref="ContextInitialized"/> event.
        /// </summary>
        private static void OnContextInitialized() =>
            ContextInitialized?.Invoke(null, EventArgs.Empty);

        /// <summary>
        /// Raises the <see cref="ContextInvalidated"/> event.
        /// </summary>
        private static void OnContextInvalidated() =>
            ContextInvalidated?.Invoke(null, EventArgs.Empty);

        /// <summary>
        /// Updates the context's list of tasks.
        /// </summary>
        private void UpdateTasks()
        {
            try
            {
                lock (tasksPending)
                {
                    if (tasksPending.Count == 0)
                        return;

                    foreach (var task in tasksPending)
                        tasksUpdating.Add(task);
                }

                foreach (var task in tasksUpdating)
                {
                    if (task.Status == TaskStatus.Created)
                        task.Start();

                    if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
                        tasksDead.Add(task);
                }

                lock (tasksPending)
                {
                    foreach (var task in tasksDead)
                        tasksPending.Remove(task);
                }
            }
            finally
            {
                tasksDead.Clear();
                tasksUpdating.Clear();
            }
        }
        
        /// <summary>
        /// Changes the current thread's synchronization context.
        /// </summary>
        private void ChangeSynchronizationContext(SynchronizationContext syncContext)
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        }

        // The singleton instance of the Sedulous context.
        private static readonly Object syncObject = new Object();
        private static FrameworkContext current;

        // State values.
        private readonly IFrameworkHost host;
        private readonly FrameworkSynchronizationContext syncContext;
        private readonly FrameworkFactory factory = new FrameworkFactory();
        private readonly Thread thread;

        // The context's list of pending tasks.
        private readonly TaskScheduler taskScheduler;
        private readonly TaskFactory taskFactory;
        private readonly List<Task> tasksUpdating = new List<Task>();
        private readonly List<Task> tasksPending = new List<Task>();
        private readonly List<Task> tasksDead = new List<Task>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // The context event queue.
        private readonly LocalMessageQueue<FrameworkMessageID> messages;
    }
}
