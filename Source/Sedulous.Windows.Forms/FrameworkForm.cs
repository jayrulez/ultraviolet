using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sedulous.Core;

namespace Sedulous.Windows.Forms
{
    /// <summary>
    /// Represents the primary Form for a Windows Forms application using the Sedulous framework.
    /// </summary>
    public partial class FrameworkForm : Form, IFrameworkHost
    {
        /// <summary>
        /// Contains native methods used by the host.
        /// </summary>
        private static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Message
            {
                public IntPtr hWnd;
                public uint Msg;
                public IntPtr wParam;
                public IntPtr lParam;
                public uint Time;
                public System.Drawing.Point Point;
            }

            [DllImport("User32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean PeekMessage(out Message message, IntPtr hWnd, uint filterMin, uint filterMax, uint flags);
        }

        /// <summary>
        /// Initializes a new instance of the FrameworkForm class.
        /// </summary>
        public FrameworkForm()
        {
            InitializeComponent();
            InitializeFramework();
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        void IFrameworkHost.Exit()
        {
            Close();
        }

        /// <summary>
        /// Gets the Framework context.
        /// </summary>
        public FrameworkContext FrameworkContext
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return context;
            }
        }

        /// <inheritdoc/>
        public String DeveloperName { get; set; }

        /// <inheritdoc/>
        public String ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the application's primary window is synchronized
        /// to the vertical retrace when rendering (i.e., whether vsync is enabled).
        /// </summary>
        public Boolean SynchronizeWithVerticalRetrace
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                if (!DesignMode)
                {
                    var primary = FrameworkContext.GetPlatform().Windows.GetPrimary();
                    if (primary == null)
                        throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

                    return primary.SynchronizeWithVerticalRetrace;
                }
                return false;
            }
            set
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                if (!DesignMode)
                {
                    var primary = FrameworkContext.GetPlatform().Windows.GetPrimary();
                    if (primary == null)
                        throw new InvalidOperationException(FrameworkStrings.NoPrimaryWindow);

                    primary.SynchronizeWithVerticalRetrace = value;
                }
            }
        }

        /// <inheritdoc/>
        public Boolean IsActive
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return this.WindowState != FormWindowState.Minimized && ContainsFocus;
            }
        }

        /// <inheritdoc/>
        public Boolean IsSuspended
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return false;
            }
        }

        /// <inheritdoc/>
        public Boolean IsFixedTimeStep
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return this.isFixedTimeStep;
            }
            set
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

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
                Contract.EnsureNotDisposed(this, IsDisposed);

                return this.targetElapsedTime;
            }
            set
            {
                Contract.EnsureNotDisposed(this, IsDisposed);
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
                Contract.EnsureNotDisposed(this, IsDisposed);

                return this.inactiveSleepTime;
            }
            set
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                this.inactiveSleepTime = value;
                if (timingLogic != null)
                {
                    timingLogic.InactiveSleepTime = value;
                }
            }
        }

        /// <summary>
        /// Called when the application is creating its Framework context.
        /// </summary>
        /// <returns>The application's Framework context.</returns>
        protected virtual FrameworkContext OnCreatingFrameworkContext()
        {
            return null;
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
        /// Occurs when the Framework context is updating its state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        protected virtual void OnUpdating(FrameworkTime time)
        {

        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        protected virtual void OnShutdown()
        {

        }

        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">A CancelEventArgs that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!DesignMode)
            {
                Application.Idle -= Application_Idle;

                timingLogic.Cleanup();

                if (context != null)
                    context.WaitForPendingTasks(true);
            }
            base.OnClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                SafeDispose.Dispose(components);
                SafeDispose.Dispose(context);

                timingLogic = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the framework.
        /// </summary>
        private void InitializeFramework()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            OnInitializing();

            context = OnCreatingFrameworkContext();
            if (context == null)
                throw new InvalidOperationException(FrameworkStrings.ContextNotCreated);

            context.Initialize();

            this.timingLogic = CreateTimingLogic();
            if (this.timingLogic == null)
                throw new InvalidOperationException(FrameworkStrings.InvalidTimingLogic);

            context.Updating += uv_Updating;
            context.Shutdown += uv_Shutdown;

            Application.Idle += Application_Idle;

            OnInitialized();

            OnLoadingContent();
        }

        /// <summary>
        /// Creates the Framework host timing logic for this host process.
        /// </summary>
        protected virtual IFrameworkTimingLogic CreateTimingLogic()
        {
            var timingLogic = new FrameworkTimingLogic(this);
            timingLogic.IsFixedTimeStep = this.IsFixedTimeStep;
            timingLogic.TargetElapsedTime = this.TargetElapsedTime;
            timingLogic.InactiveSleepTime = this.InactiveSleepTime;
            return timingLogic;
        }

        /// <summary>
        /// Handles the application's Idle event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        private void Application_Idle(Object sender, EventArgs e)
        {
            NativeMethods.Message message;
            while (!NativeMethods.PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
            {
                if (!FrameworkContext.Disposed)
                {
                    Tick();
                }
            }
        }

        /// <summary>
        /// Processes one application tick.
        /// </summary>
        private void Tick()
        {
            timingLogic.RunOneTick();
        }

        /// <summary>
        /// Handles the Framework context's Updating event.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        /// <param name="time">Time elapsed since the last call to Update.</param>
        private void uv_Updating(FrameworkContext context, FrameworkTime time)
        {
            OnUpdating(time);
        }

        /// <summary>
        /// Called when the application is being shut down.
        /// </summary>
        private void uv_Shutdown(FrameworkContext context)
        {
            OnShutdown();
        }

        // The Framework context.
        private FrameworkContext context;
        private IFrameworkTimingLogic timingLogic;

        // The application's tick state.
        private Boolean isFixedTimeStep = FrameworkTimingLogic.DefaultIsFixedTimeStep;
        private TimeSpan targetElapsedTime = FrameworkTimingLogic.DefaultTargetElapsedTime;
        private TimeSpan inactiveSleepTime = FrameworkTimingLogic.DefaultInactiveSleepTime;
    }
}
