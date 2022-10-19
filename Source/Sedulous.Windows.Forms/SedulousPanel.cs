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
    /// Represents a panel designed to be enlisted into an Sedulous context.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class SedulousPanel : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the SedulousPanel class.
        /// </summary>
        public SedulousPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the panel is being drawn.
        /// </summary>
        public event EventHandler Drawing;

        /// <summary>
        /// Occurs when the panel's Sedulous window is about to be created.
        /// </summary>
        public event EventHandler CreatingSedulousWindow;

        /// <summary>
        /// Occurs after the panel's Sedulous window has been crated.
        /// </summary>
        public event EventHandler CreatedSedulousWindow;

        /// <summary>
        /// Occurs when the panel's Sedulous window is about to be destroyed.
        /// </summary>
        public event EventHandler DestroyingSedulousWindow;

        /// <summary>
        /// Occurs after the panel's Sedulous window has been destroyed.
        /// </summary>
        public event EventHandler DestroyedSedulousWindow;

        /// <summary>
        /// Gets the panel's Sedulous context.
        /// </summary>
        public SedulousContext Sedulous
        {
            get
            {
                Contract.EnsureNotDisposed(this, IsDisposed);

                return uv;
            }
        }

        /// <summary>
        /// Gets the panel's Sedulous window.
        /// </summary>
        public ISedulousWindow SedulousWindow
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
        /// Raises the CreatingSedulousWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCreatingSedulousWindow(EventArgs e) =>
            CreatingSedulousWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the CreatedSedulousWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnCreatedSedulousWindow(EventArgs e) =>
            CreatedSedulousWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the DestroyingSedulousWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnDestroyingSedulousWindow(EventArgs e) =>
            DestroyingSedulousWindow?.Invoke(this, e);

        /// <summary>
        /// Raises the DestroyedSedulousWindow event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnDestroyedSedulousWindow(EventArgs e) =>
            DestroyedSedulousWindow?.Invoke(this, e);

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
            if (uv == null && !DesignMode)
            {
                var uvform = TopLevelControl as SedulousForm;
                if (uvform == null)
                    throw new InvalidOperationException(WindowsFormsStrings.SedulousFormRequired);
                
                CreateSedulousWindow(uvform.Sedulous);
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
                if (uv != null && !uv.Disposed)
                {
                    DestroySedulousWindow();
                }
                SafeDispose.Dispose(components);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles the Sedulous window's Drawing event.
        /// </summary>
        /// <param name="window">The window being rendered.</param>
        /// <param name="time">Time elapsed since the last call to Draw.</param>
        private void uvWindow_Drawing(ISedulousWindow window, SedulousTime time)
        {
            OnDrawing(EventArgs.Empty);
        }

        /// <summary>
        /// Enlists the panel in the specified Sedulous context.
        /// </summary>
        /// <param name="uv">The Sedulous context in which to enlist the panel.</param>
        private void CreateSedulousWindow(SedulousContext uv)
        {
            Contract.Require(uv, nameof(uv));

            if (this.uv != null)
                throw new InvalidOperationException(WindowsFormsStrings.PanelAlreadyEnlisted);
            
            OnCreatingSedulousWindow(EventArgs.Empty);

            this.uv = uv;

            this.uvWindow = uv.GetPlatform().Windows.CreateFromNativePointer(this.Handle);
            this.uvWindow.Drawing += uvWindow_Drawing;
            
            OnCreatedSedulousWindow(EventArgs.Empty);
        }

        /// <summary>
        /// Releases the panel from its Sedulous context.
        /// </summary>
        private void DestroySedulousWindow()
        {
            if (this.uv == null)
                throw new InvalidOperationException(WindowsFormsStrings.PanelNotEnlisted);
            
            OnDestroyingSedulousWindow(EventArgs.Empty);

            this.uvWindow.Drawing -= uvWindow_Drawing;
            this.uv.GetPlatform().Windows.Destroy(uvWindow);
            this.uvWindow = null;

            this.uv = null;

            OnDestroyedSedulousWindow(EventArgs.Empty);
        }

        // The panel's ID within the Sedulous context.
        private SedulousContext uv;
        private ISedulousWindow uvWindow;

        // Property values.
        private String designLabel = "uv";
    }
}
