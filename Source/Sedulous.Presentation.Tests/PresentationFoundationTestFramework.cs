using System;
using System.IO;
using Sedulous.Content;
using Sedulous.Core.TestFramework;
using Sedulous.Presentation.Styles;
using Sedulous.TestApplication;
using Sedulous.TestFramework;
using Sedulous.UI;

namespace Sedulous.Presentation.Tests
{
    /// <summary>
    /// Represents the base class for tests which require the Presentation Foundation.
    /// </summary>
    public class PresentationFoundationTestFramework : SedulousApplicationTestFramework
    {
        /// <summary>
        /// Gets the element which currently has focus.
        /// </summary>
        /// <typeparam name="T">The type of element which is expected to have focus.</typeparam>
        /// <param name="app">The Sedulous test application.</param>
        /// <returns>The element which currently has focus.</returns>
        protected T GetElementWithFocus<T>(ISedulousTestApplication app) where T : UIElement
        {
            var screen = app.Sedulous.GetUI().GetScreens().Peek();
            if (screen == null)
                return null;

            var view = screen.View as PresentationFoundationView;
            if (view == null)
                return null;

            return view.ElementWithFocus as T;
        }

        /// <summary>
        /// Wraps the element with keyboard focus for evaluation.
        /// </summary>
        /// <param name="app">The test application.</param>
        /// <returns>The wrapped element.</returns>
        protected ObjectResult<UIElement> TheElementWithFocus(ISedulousTestApplication app)
        {
            return TheResultingObject(GetElementWithFocus<UIElement>(app));
        }

        /// <summary>
        /// Wraps the element with keyboard focus for evaluation.
        /// </summary>
        /// <param name="app">The test application.</param>
        /// <returns>The wrapped element.</returns>
        protected ObjectResult<T> TheElementWithFocus<T>(ISedulousTestApplication app) where T : UIElement
        {
            return TheResultingObject(GetElementWithFocus<T>(app));
        }

        /// <summary>
        /// Initializes a test application which displays the specified Presentation Foundation view.
        /// </summary>
        protected ISedulousTestApplication GivenAPresentationFoundationTestFor<T>(Func<ContentManager, T> ctor) where T : UIScreen
        {
            var globalStyleSheet = default(GlobalStyleSheet);
            var screen = default(UIScreen);

            return GivenAnSedulousApplication()
                .WithPresentationFoundationConfigured()
                .WithInitialization(uv =>
                {
                    var upf = uv.GetUI().GetPresentationFoundation();
                    upf.CompileExpressions(Path.Combine("Resources", "Content"), CompileExpressionsFlags.GenerateInMemory | CompileExpressionsFlags.WorkInTemporaryDirectory);
                    upf.LoadCompiledExpressions();
                })
                .WithContent(content =>
                {
                    content.Sedulous.GetContent().Manifests.Load(Path.Combine("Resources", "Content", "Manifests", "Global.manifest"));

                    globalStyleSheet = GlobalStyleSheet.Create();
                    globalStyleSheet.Append(content, "UI/DefaultUIStyles");

                    content.Sedulous.GetUI().GetPresentationFoundation().SetGlobalStyleSheet(globalStyleSheet);

                    screen = ctor(content);
                    content.Sedulous.GetUI().GetScreens().Open(screen);
                })
                .WithDispose(() =>
                {
                    screen?.Dispose();
                    globalStyleSheet?.Dispose();
                });
        }
    }
}
