using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Graphics.Graphics2D;
using Sedulous.Platform;
using Sedulous.UI;

namespace Sedulous
{
    /// <summary>
    /// Represents a callback that is invoked when the Sedulous Framework logs a debug message.
    /// </summary>
    /// <param name="uv">The Sedulous Context that logged the message.</param>
    /// <param name="level">A <see cref="DebugLevels"/> value representing the debug level of the message.</param>
    /// <param name="message">The debug message text.</param>
    public delegate void DebugCallback(SedulousContext uv, DebugLevels level, String message);

    /// <summary>
    /// Represents a method that is called in response to an Sedulous context event.
    /// </summary>
    /// <param name="uv">The Sedulous context.</param>
    public delegate void SedulousContextEventHandler(SedulousContext uv);

    /// <summary>
    /// Represents the method that is called when an Sedulous context is about to draw the current scene.
    /// </summary>
    /// <param name="uv">The Sedulous context.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Draw(SedulousTime)"/>.</param>
    public delegate void SedulousContextDrawEventHandler(SedulousContext uv, SedulousTime time);

    /// <summary>
    /// Represents the method that is called when an Sedulous context has drawn or is about to draw a particular window.
    /// </summary>
    /// <param name="uv">The Sedulous context.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Draw(SedulousTime)"/>.</param>
    /// <param name="window">The window that was drawn or is about to be drawn.</param>
    public delegate void SedulousContextWindowDrawEventHandler(SedulousContext uv, SedulousTime time, ISedulousWindow window);

    /// <summary>
    /// Represents the method that is called when an Sedulous context updates the application state.
    /// </summary>
    /// <param name="uv">The Sedulous context.</param>
    /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Update(SedulousTime)"/>.</param>
    public delegate void SedulousContextUpdateEventHandler(SedulousContext uv, SedulousTime time);

    /// <summary>
    /// Represents the Sedulous Framework and all of its subsystems.
    /// </summary>
    public abstract class SedulousContext :
        IMessageSubscriber<SedulousMessageID>,
        ICrossThreadSedulousContext,
        IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousContext"/> class.
        /// </summary>
        /// <param name="host">The object that is hosting the Sedulous context.</param>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        public SedulousContext(ISedulousHost host, SedulousConfiguration configuration)
        {
            Contract.Require(host, nameof(host));
            Contract.Require(configuration, nameof(configuration));

            if (configuration.GraphicsConfiguration == null)
                throw new InvalidOperationException(SedulousStrings.MissingGraphicsConfiguration);

            AcquireContext();

            this.IsRunningInServiceMode = configuration.EnableServiceMode;
            this.IsHardwareInputDisabled = configuration.IsHardwareInputDisabled;

            this.Properties = new SedulousContextProperties();
            this.Properties.SupportsHighDensityDisplayModes = configuration.SupportsHighDensityDisplayModes;
            this.Properties.SrgbDefaultForSurface2D = configuration.GraphicsConfiguration.SrgbDefaultForSurface2D;
            this.Properties.SrgbDefaultForSurface3D = configuration.GraphicsConfiguration.SrgbDefaultForSurface3D;
            this.Properties.SrgbDefaultForTexture2D = configuration.GraphicsConfiguration.SrgbDefaultForTexture2D;
            this.Properties.SrgbDefaultForTexture3D = configuration.GraphicsConfiguration.SrgbDefaultForTexture3D;
            this.Properties.SrgbDefaultForRenderBuffer2D = configuration.GraphicsConfiguration.SrgbDefaultForRenderBuffer2D;

            if (SedulousPlatformInfo.CurrentRuntime == SedulousRuntime.CoreCLR)
                this.Properties.SupportsHighDensityDisplayModes = false;

            this.host = host;

            this.thread = Thread.CurrentThread;

            this.messages = new LocalMessageQueue<SedulousMessageID>();
            this.messages.Subscribe(this, SedulousMessages.Quit);

            this.syncContext = new SedulousSynchronizationContext(this);
            ChangeSynchronizationContext(syncContext);

            this.taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.taskFactory = new TaskFactory(taskScheduler);

            InitializeFactory(configuration);
        }

        /// <summary>
        /// Ensures that the specified function produces a valid instance of <see cref="SedulousContext"/>. If it does not,
        /// then the current context is immediately disposed. This method should only be called by Sedulous host implementations.
        /// </summary>
        /// <param name="fn">The function which will create the Sedulous context.</param>
        /// <returns>The Sedulous context that was created.</returns>
        public static SedulousContext EnsureSuccessfulCreation(Func<SedulousContext> fn)
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
        public static SedulousContext RequestCurrent()
        {
            return current;
        }

        /// <summary>
        /// Retrieves the current Sedulous context, throwing an exception if it does not exist.
        /// </summary>
        /// <returns>The current Sedulous context.</returns>
        public static SedulousContext DemandCurrent()
        {
            if (current == null)
                throw new InvalidOperationException(SedulousStrings.ContextMissing);

            return current;
        }

        /// <summary>
        /// Receives a message that has been published to a queue.
        /// </summary>
        /// <param name="type">The type of message that was received.</param>
        /// <param name="data">The data for the message that was received.</param>
        void IMessageSubscriber<SedulousMessageID>.ReceiveMessage(SedulousMessageID type, MessageData data)
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
            Contract.Ensure(thread == Thread.CurrentThread, SedulousStrings.WorkItemsMustBeProcessedOnMainThread);

            var syncContext = SynchronizationContext.Current as SedulousSynchronizationContext;
            if (syncContext != null)
                syncContext.ProcessSingleWorkItem();
        }

        /// <summary>
        /// Processes all queued work items.
        /// </summary>
        public void ProcessWorkItems()
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Ensure(thread == Thread.CurrentThread, SedulousStrings.WorkItemsMustBeProcessedOnMainThread);

            var syncContext = SynchronizationContext.Current as SedulousSynchronizationContext;
            if (syncContext != null)
                syncContext.ProcessWorkItems();
        }

        /// <summary>
        /// Called when a new frame is started.
        /// </summary>
        public void HandleFrameStart()
        {
            SedulousProfiler.BeginSection(SedulousProfilerSections.Frame);
            OnFrameStart();
        }

        /// <summary>
        /// Called when a frame is completed.
        /// </summary>
        public void HandleFrameEnd()
        {
            OnFrameEnd();
            SedulousProfiler.EndSection(SedulousProfilerSections.Frame);
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
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Update(SedulousTime)"/>.</param>
        public virtual void Update(SedulousTime time)
        {

        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Draw(SedulousTime)"/>.</param>
        public virtual void Draw(SedulousTime time)
        {

        }

        /// <summary>
        /// Gets the platform interop subsystem.
        /// </summary>
        /// <returns>The platform interop subsystem.</returns>
        public abstract ISedulousPlatform GetPlatform();

        /// <summary>
        /// Gets the content management subsystem.
        /// </summary>
        /// <returns>The content management subsystem.</returns>
        public abstract ISedulousContent GetContent();

        /// <summary>
        /// Gets the graphics subsystem.
        /// </summary>
        /// <returns>The graphics subsystem.</returns>
        public abstract ISedulousGraphics GetGraphics();

        /// <summary>
        /// Gets the audio subsystem.
        /// </summary>
        /// <returns>The audio subsystem.</returns>
        public abstract ISedulousAudio GetAudio();

        /// <summary>
        /// Gets the input subsystem.
        /// </summary>
        /// <returns>The input subsystem.</returns>
        public abstract ISedulousInput GetInput();

        /// <summary>
        /// Gets the user interface subsystem.
        /// </summary>
        /// <returns>The user interface subsystem.</returns>
        public abstract ISedulousUI GetUI();

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
        /// <remarks>Tasks spawned using this method will not be started until the next call to <see cref="Update(SedulousTime)"/>, and will prevent
        /// the Sedulous context from shutting down until they complete or are canceled.  Do not attempt to <see cref="Task.Wait()"/> on these
        /// tasks from the main Sedulous thread; doing so will introduce a deadlock.</remarks>
        /// <param name="action">The action to perform within the task.</param>
        /// <returns>The <see cref="Task"/> that was spawned.</returns>
        public Task SpawnTask(Action<CancellationToken> action)
        {
            Contract.Require(action, nameof(action));
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.EnsureNot(Disposing, SedulousStrings.CannotSpawnTasks);

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
            Contract.EnsureNot(Disposing, SedulousStrings.CannotQueueWorkItems);

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
            Contract.EnsureNot(Disposing, SedulousStrings.CannotQueueWorkItems);

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
            Contract.EnsureNot(Disposing, SedulousStrings.CannotQueueWorkItems);

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
            Contract.EnsureNot(Disposing, SedulousStrings.CannotQueueWorkItems);

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
        /// <param name="resource">The <see cref="SedulousResource"/> to validate.</param>
        [Conditional("DEBUG")]
        public void ValidateResource(SedulousResource resource)
        {
            if (resource != null && resource.Sedulous != this)
                throw new InvalidOperationException(SedulousStrings.InvalidResource);
        }

        /// <summary>
        /// Gets the context's runtime properties.
        /// </summary>
        public SedulousContextProperties Properties { get; }

        /// <summary>
        /// Gets the version of the runtime on which this context is running.
        /// </summary>
        public Version RuntimeVersion => SedulousPlatformInfo.CurrentRuntimeVersion;

        /// <summary>
        /// Gets the runtime on which this context is running.
        /// </summary>
        public SedulousRuntime Runtime => SedulousPlatformInfo.CurrentRuntime;

        /// <summary>
        /// Gets the platform on which this context is running.
        /// </summary>
        public SedulousPlatform Platform => SedulousPlatformInfo.CurrentPlatform;

        /// <summary>
        /// Gets the object that is hosting the Sedulous context.
        /// </summary>
        public ISedulousHost Host => host;

        /// <summary>
        /// Gets the context's message queue.
        /// </summary>
        public IMessageQueue<SedulousMessageID> Messages => messages;

        /// <summary>
        /// Gets the assembly which provides compatibility services for the current platform.
        /// </summary>
        public Assembly PlatformCompatibilityShimAssembly => platformCompatibilityShimAssembly;

        /// <summary>
        /// Gets the assembly which implements views for the user interface subsystem.
        /// </summary>
        public Assembly ViewProviderAssembly => viewProviderAssembly;

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
        public event SedulousContextEventHandler FrameStart;

        /// <summary>
        /// Occurs when a frame is completed.
        /// </summary>
        public event SedulousContextEventHandler FrameEnd;

        /// <summary>
        /// Occurs when the context is preparing to draw the current scene. This event is called
        /// before the context associates itself to any windows.
        /// </summary>
        public event SedulousContextDrawEventHandler Drawing;

        /// <summary>
        /// Occurs when the context is preparing to draw a particular window.
        /// </summary>
        public event SedulousContextWindowDrawEventHandler WindowDrawing;

        /// <summary>
        /// Occurs after the context has drawn a particular window.
        /// </summary>
        public event SedulousContextWindowDrawEventHandler WindowDrawn;

        /// <summary>
        /// Occurs when the context is about to update the state of its subsystems.
        /// </summary>
        public event SedulousContextUpdateEventHandler UpdatingSubsystems;

        /// <summary>
        /// Occurs when the context is updating the application's state.
        /// </summary>
        public event SedulousContextUpdateEventHandler Updating;

        /// <summary>
        /// Occurs when the context is initialized.
        /// </summary>
        public event SedulousContextEventHandler Initialized;

        /// <summary>
        /// Occurs when the Sedulous context is being shut down.
        /// </summary>
        public event SedulousContextEventHandler Shutdown;
        
        /// <summary>
        /// Acquires an exclusive context claim, preventing other instances from being instantiated.
        /// </summary>
        private void AcquireContext()
        {
            lock (syncObject)
            {
                if (current != null)
                    throw new InvalidOperationException(SedulousStrings.ContextAlreadyExists);

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
        /// Initializes the context and marks it ready for use.
        /// </summary>
        protected void InitializeContext()
        {
            IsInitialized = true;
            
            GetContent().Processors
                .SetFallbackType<SedulousFont>(typeof(SpriteFont));

            OnInitialized();
            OnContextInitialized();
        }

        /// <summary>
        /// Initializes any factory methods that are exposed by the specified assembly.
        /// </summary>
        /// <param name="asm">The assembly for which to initialize factory methods.</param>
        protected void InitializeFactoryMethodsInAssembly(Assembly asm)
        {
            Contract.Require(asm, nameof(asm));

            var initializerTypes = from t in asm.GetTypes()
                                   where t.IsClass && !t.IsAbstract && typeof(ISedulousFactoryInitializer).IsAssignableFrom(t)
                                   select t;

            foreach (var initializerType in initializerTypes)
            {
                var initializerInstance = (ISedulousFactoryInitializer)Activator.CreateInstance(initializerType);
                initializerInstance.Initialize(this, Factory);
            }
        }

        /// <summary>
        /// Initializes the context's view provider.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        protected void InitializeViewProvider(SedulousConfiguration configuration)
        {
            var initializerFactory = TryGetFactoryMethod<UIViewProviderInitializerFactory>();
            if (initializerFactory != null)
            {
                var initializer = initializerFactory();
                initializer.Initialize(this, configuration.ViewProviderConfiguration);
            }
        }

        /// <summary>
        /// Initializes the context's plugins.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        protected void InitializePlugins(SedulousConfiguration configuration)
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
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Update(SedulousTime)"/>.</param>
        protected void UpdateContext(SedulousTime time)
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
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Draw(SedulousTime)"/>.</param>
        protected virtual void OnDrawing(SedulousTime time) =>
            Drawing?.Invoke(this, time);

        /// <summary>
        /// Raises the <see cref="WindowDrawing"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Draw(SedulousTime)"/>.</param>
        /// <param name="window">The window that is about to be drawn.</param>
        protected virtual void OnWindowDrawing(SedulousTime time, ISedulousWindow window) =>
            WindowDrawing?.Invoke(this, time, window);

        /// <summary>
        /// Raises the <see cref="WindowDrawn"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Draw(SedulousTime)"/>.</param>
        /// <param name="window">The window that was just drawn.</param>
        protected virtual void OnWindowDrawn(SedulousTime time, ISedulousWindow window) =>
            WindowDrawn?.Invoke(this, time, window);

        /// <summary>
        /// Raises the <see cref="UpdatingSubsystems"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="Update(SedulousTime)"/>.</param>
        protected virtual void OnUpdatingSubsystems(SedulousTime time) =>
            UpdatingSubsystems?.Invoke(this, time);

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="Update(SedulousTime)"/>.</param>
        protected virtual void OnUpdating(SedulousTime time) =>
            Updating?.Invoke(this, time);

        /// <summary>
        /// Occurs when the context receives a message from its queue.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="data">The message data.</param>
        protected virtual void OnReceivedMessage(SedulousMessageID type, MessageData data)
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
        protected SedulousFactory Factory
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
        /// Initializes the context's object factory.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        private void InitializeFactory(SedulousConfiguration configuration)
        {
            var asmCore = typeof(SedulousContext).Assembly;
            var asmImpl = GetType().Assembly;

            InitializeFactoryMethodsInAssembly(asmCore);
            InitializeFactoryMethodsInAssembly(asmImpl);
            InitializeFactoryMethodsInCompatibilityShim();
            InitializeFactoryMethodsInViewProvider(configuration);
            
            var asmEntry = Assembly.GetEntryAssembly();
            if (asmEntry != null)
                InitializeFactoryMethodsInAssembly(asmEntry);
        }

        /// <summary>
        /// Initializes any factory methods exposed by the current platform compatibility shim.
        /// </summary>
        private void InitializeFactoryMethodsInCompatibilityShim()
        {
            if(host.CompatibilityShim == null)
            {
                throw new InvalidCompatibilityShimException(SedulousStrings.MissingCompatibilityShim.Format($"{Platform}"));
            }

            var shim = host.CompatibilityShim.Assembly;

            if (shim != null)
                InitializeFactoryMethodsInAssembly(shim);

            platformCompatibilityShimAssembly = shim;
        }
        
        /// <summary>
        /// Initializes any factory methods exposed by the registered view provider.
        /// </summary>
        /// <param name="configuration">The Sedulous Framework configuration settings for this context.</param>
        private void InitializeFactoryMethodsInViewProvider(SedulousConfiguration configuration)
        {
            if (String.IsNullOrEmpty(configuration.ViewProviderAssembly))
                return;

            Assembly asm;
            try
            {
                asm = Assembly.Load(configuration.ViewProviderAssembly);
                InitializeFactoryMethodsInAssembly(asm);

                viewProviderAssembly = asm;
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException ||
                    e is FileLoadException ||
                    e is BadImageFormatException)
                {
                    throw new InvalidOperationException(SedulousStrings.InvalidViewProviderAssembly, e);
                }
                throw;
            }
        }

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
        private static SedulousContext current;

        // State values.
        private readonly ISedulousHost host;
        private readonly SedulousSynchronizationContext syncContext;
        private readonly SedulousFactory factory = new SedulousFactory();
        private readonly Thread thread;
        private Assembly platformCompatibilityShimAssembly;
        private Assembly viewProviderAssembly;

        // The context's list of pending tasks.
        private readonly TaskScheduler taskScheduler;
        private readonly TaskFactory taskFactory;
        private readonly List<Task> tasksUpdating = new List<Task>();
        private readonly List<Task> tasksPending = new List<Task>();
        private readonly List<Task> tasksDead = new List<Task>();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        // The context event queue.
        private readonly LocalMessageQueue<SedulousMessageID> messages;
    }
}
