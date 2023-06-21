using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous.Windows.Forms
{
    /// <summary>
    /// Represents a panel designed to be enlisted into an Framework context.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class FrameworkPanel : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the FrameworkPanel class.
        /// </summary>
        public FrameworkPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the panel is being drawn.
        /// </summary>
        public event EventHandler Drawing;

        /// <summary>
        /// Occurs when the panel's Framework window is about to be created.
        /// </summary>
        public event EventHandler CreatingFrameworkWindow;

        /// <summary>
        /// Occurs after the panel's Framework window has been crated.
        /// </summary>
        public event EventHandler CreatedFrameworkWindow;

        /// <summary>
        /// Occurs when the panel's Framework window is about to be destroyed.
        /// </summary>
        public event EventHandler DestroyingFrameworkWindow;

        /// <summary>
        /// Occurs after the panel's Framework window has been destroyed.
        /// </summary>
        public event EventHandler DestroyedFrameworkWindow;

        /// <summary>
        /// Gets the panel's Framework context.
        /// </summary>
        public FrameworkContext FrameworkContext
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return context;
            }
        }

        /// <summary>
        /// Gets the panel's Framework window.
        /// </summary>
        public IFrameworkWindow FrameworkWindow
        {
            get 
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return uvWindow; 
            }
        }

        /// <summary>
        /// Gets or sets the label displayed on this panel at design time.
        /// </summary>
        [DefaultValue("uv")]
        public String DesignLabel
        {
            get 
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return designLabel;
            }
            set
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                designLabel = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the Drawing event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnDrawing(EventArgs e) =>
            Drawing?.Invoke(this, e);

        /// <summary>
        /// Raises the CreatingFrameworkWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCreatingFrameworkWindow(EventArgs e) =>
            CreatingFrameworkWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the CreatedFrameworkWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCreatedFrameworkWindow(EventArgs e) =>
            CreatedFrameworkWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the DestroyingFrameworkWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnDestroyingFrameworkWindow(EventArgs e) =>
            DestroyingFrameworkWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the DestroyedFrameworkWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnDestroyedFrameworkWindow(EventArgs e) =>
            DestroyedFrameworkWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the Paint event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(System.Drawing.Color.Magenta);
                if (!String.IsNullOrEmpty(designLabel))
                {
                    using (var font = new FontFamily("Arial"))
                    {
                        var format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        var path = new GraphicsPath();
                        path.AddString(designLabel, font, (int)FontStyle.Bold, 32f, 
                            new System.Drawing.RectangleF(0, 0, ClientSize.Width, ClientSize.Height), format);

                        e.Graphics.FillPath(Brushes.White, path);
                        e.Graphics.DrawPath(Pens.Black, path);
                    }
                }
            }
            base.OnPaint(e);
        }

        /// <summary>
        /// Raises the PaintBackground event.
        /// </summary>
        /// <param name="e">A PaintEventArgs that contains the event data.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }

        /// <summary>
        /// Raises the Load event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (context == null && !DesignMode)
            {
                var form = TopLevelControl as FrameworkForm;
                if (form == null)
                    throw new InvalidOperationException(WindowsFormsStrings.FrameworkFormRequired);

                CreateFrameworkWindow(form.FrameworkContext);
            }
            base.OnLoad(e);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null && !context.Disposed)
                {
                    DestroyFrameworkWindow();
                }
                SafeDispose.Dispose(components);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles the Framework window's Drawing event.
        /// </summary>
        /// <param name="window">The window being rendered.</param>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        private void uvWindow_Drawing(IFrameworkWindow window, FrameworkTime time)
        {
            OnDrawing(EventArgs.Empty);
        }

        /// <summary>
        /// Enlists the panel in the specified Framework context.
        /// </summary>
        /// <param name="context">The Framework context in which to enlist the panel.</param>
        private void CreateFrameworkWindow(FrameworkContext context)
        {
            Contract.Require(context, nameof(context));

            if (this.context != null)
                throw new InvalidOperationException(WindowsFormsStrings.PanelAlreadyEnlisted);

            OnCreatingFrameworkWindow(EventArgs.Empty);

            this.context = context;

            this.uvWindow = context.GetPlatform().Windows.CreateFromNativePointer(this.Handle);
            this.uvWindow.Drawing += uvWindow_Drawing;

            OnCreatedFrameworkWindow(EventArgs.Empty);
        }

        /// <summary>
        /// Releases the panel from its Framework context.
        /// </summary>
        private void DestroyFrameworkWindow()
        {
            if (this.context == null)
                throw new InvalidOperationException(WindowsFormsStrings.PanelNotEnlisted);

            OnDestroyingFrameworkWindow(EventArgs.Empty);

            this.uvWindow.Drawing -= uvWindow_Drawing;
            this.context.GetPlatform().Windows.Destroy(uvWindow);
            this.uvWindow = null;

            this.context = null;

            OnDestroyedFrameworkWindow(EventArgs.Empty);
        }

        // The panel's ID within the Framework context.
        private FrameworkContext context;
        private IFrameworkWindow uvWindow;

        // Property values.
        private String designLabel = "uv";
    }
}
