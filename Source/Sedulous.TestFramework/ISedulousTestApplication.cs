using System;
using System.Drawing;
using Sedulous.Content;
using Sedulous.Input;

namespace Sedulous.TestFramework
{
    /// <summary>
    /// Represents an Sedulous application used in unit tests.
    /// </summary>
    public interface ISedulousTestApplication : IDisposable
    {
        /// <summary>
        /// Specifies the application's Audio subsystem implementation.
        /// </summary>
        /// <param name="audioImplementation">A <see cref="AudioImplementation"/> value corresponding to one of 
        /// the Sedulous Framework's Audio subsystem implementations.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication WithAudioImplementation(AudioImplementation audioImplementation);

        /// <summary>
        /// Specifies that a modification should be made to the application's configuration.
        /// </summary>
        /// <param name="configurer">An action which performs the configuration change.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication WithConfiguration(Action<SedulousConfiguration> configurer);

        /// <summary>
        /// Specifies that a plugin should be added to the list which is loaded by Sedulous.
        /// </summary>
        /// <param name="plugin">The plugin to load.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication WithPlugin(SedulousPlugin plugin);

        /// <summary>
        /// Specifies the application's initialization code.
        /// </summary>
        /// <param name="initializer">An action which will initialize the application.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication WithInitialization(Action<SedulousContext> initializer);

        /// <summary>
        /// Specifies the application's content loading code.
        /// </summary>
        /// <param name="loader">An action which will load the unit test's required content.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication WithContent(Action<ContentManager> loader);

        /// <summary>
        /// Specifies the application's disposal code.
        /// </summary>
        /// <param name="disposer">An action which will dispose the unit test's resources.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication WithDispose(Action disposer);

        /// <summary>
        /// Registers an action to be performed at the start of the specified frame.
        /// </summary>
        /// <param name="frame">The index of the frame in which to perform the action.</param>
        /// <param name="action">The action to perform on the specified frame.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication OnFrameStart(Int32 frame, Action<ISedulousTestApplication> action);

        /// <summary>
        /// Registers an action to be performed at the start of every update.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication OnUpdate(Action<ISedulousTestApplication, SedulousTime> action);

        /// <summary>
        /// Registers an action to be performed at the start of the specified update.
        /// </summary>
        /// <param name="update">The index of the update in which to perform the action.</param>
        /// <param name="action">The action to perform on the specified update.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication OnUpdate(Int32 update, Action<ISedulousTestApplication, SedulousTime> action);

        /// <summary>
        /// Registers an action to be performed at the start of every render.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication OnRender(Action<ISedulousTestApplication, SedulousTime> action);

        /// <summary>
        /// Registers an action to be performed at the start of the specified render.
        /// </summary>
        /// <param name="render">The index of the render in which to perform the action.</param>
        /// <param name="action">The action to perform on the specified render.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication OnRender(Int32 render, Action<ISedulousTestApplication, SedulousTime> action);

        /// <summary>
        /// Skips the specified number of frames prior to rendering the tested scene.
        /// </summary>
        /// <param name="frameCount">The number of frames to skip.</param>
        /// <returns>The Sedulous test application.</returns>
        ISedulousTestApplication SkipFrames(Int32 frameCount);

        /// <summary>
        /// Renders a scene and outputs the resulting image.
        /// </summary>
        /// <param name="renderer">An action which will render the desired scene.</param>
        /// <returns>A bitmap containing the result of rendering the specified scene.</returns>
        StbImageSharp.ImageResult Render(Action<SedulousContext> renderer);

        /// <summary>
        /// Runs the application until the specified predicate is true.
        /// </summary>
        /// <param name="predicate">The predicate that evaluates when the application should exit.</param>
        void RunUntil(Func<Boolean> predicate);

        /// <summary>
        /// Runs the application until the specified period of time has elapsed.
        /// </summary>
        /// <param name="time">The amount of time for which to run the application.</param>
        void RunFor(TimeSpan time);

        /// <summary>
        /// Runs a single frame of the application.
        /// </summary>
        void RunForOneFrame();

        /// <summary>
        /// Runs the application until there are no more frame actions.
        /// </summary>
        void RunAllFrameActions();

        /// <summary>
        /// Immediately pushes a message onto the Sedulous message queue which spoofs a key down event.
        /// </summary>
        /// <param name="scancode">The <see cref="Scancode"/> value of the key to press.</param>
        /// <param name="key">The <see cref="Key"/> value of the key to press.</param>
        /// <param name="ctrl">A value indicating whether the Ctrl modifier is active.</param>
        /// <param name="alt">A value indicating whether the Alt modifier is active.</param>
        /// <param name="shift">A value indicating whether the Shift modifier is active.</param>
        void SpoofKeyDown(Scancode scancode, Key key, Boolean ctrl, Boolean alt, Boolean shift);

        /// <summary>
        /// Immediately pushes a message onto the Sedulous message queue which spoofs a key up event.
        /// </summary>
        /// <param name="scancode">The <see cref="Scancode"/> value of the key to release.</param>
        /// <param name="key">The <see cref="Key"/> value of the key to release.</param>
        /// <param name="ctrl">A value indicating whether the Ctrl modifier is active.</param>
        /// <param name="alt">A value indicating whether the Alt modifier is active.</param>
        /// <param name="shift">A value indicating whether the Shift modifier is active.</param>
        void SpoofKeyUp(Scancode scancode, Key key, Boolean ctrl, Boolean alt, Boolean shift);

        /// <summary>
        /// Immediately pushes a message onto the Sedulous message queue which spoofs a key down event,
        /// followed by a message which spoofs a key up event.
        /// </summary>
        /// <param name="scancode">The <see cref="Scancode"/> value of the key to press.</param>
        /// <param name="key">The <see cref="Key"/> value of the key to press.</param>
        /// <param name="ctrl">A value indicating whether the Ctrl modifier is active.</param>
        /// <param name="alt">A value indicating whether the Alt modifier is active.</param>
        /// <param name="shift">A value indicating whether the Shift modifier is active.</param>
        void SpoofKeyPress(Scancode scancode, Key key, Boolean ctrl, Boolean alt, Boolean shift);

        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        SedulousContext Sedulous { get; }
    }
}
