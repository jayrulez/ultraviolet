namespace Sedulous.Presentation
{
    partial class PresentationFoundation
    {
        /// <summary>
        /// Draws a diagnostics panel containing various Presentation Foundation performance metrics.
        /// </summary>
        public void DrawDiagnosticsPanel()
        {
            if (diagnosticsPanel == null)
                diagnosticsPanel = new DiagnosticsPanel(FrameworkContext);

            diagnosticsPanel.Draw();
        }

        /// <summary>
        /// Gets the performance statistics which have been collected by the Sedulous Presentation Foundation.
        /// </summary>
        public PresentationFoundationPerformanceStats PerformanceStats { get; } = new PresentationFoundationPerformanceStats();

        // Diagnostics values.
        private DiagnosticsPanel diagnosticsPanel;
    }
}
