using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Sedulous.BASS;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.FMOD;
using Sedulous.Graphics;
using Sedulous.Input;
using Sedulous.OpenGL;
using Sedulous.SDL2;
using Sedulous.SDL2.Messages;
using Sedulous.SDL2.Native;
using Sedulous.TestFramework;
using Sedulous.TestFramework.Graphics;
using static Sedulous.SDL2.Native.SDL_EventType;
using static Sedulous.SDL2.Native.SDL_Keymod;

namespace Sedulous.TestApplication
{
    /// <summary>
    /// An Sedulous application used for unit testing.
    /// </summary>
    public partial class FrameworkTestApplication : FrameworkApplication, IFrameworkTestApplication
    {
        /// <summary>
        /// Initializes a new instance of the SedulousTestApplication class.
        /// </summary>
        /// <param name="headless">A value indicating whether to create a headless context.</param>
        /// <param name="serviceMode">A value indicating whether to create a service mode context.</param>
        public FrameworkTestApplication(Boolean headless = false, Boolean serviceMode = false)
            : base("Sedulous", "Sedulous Unit Tests")
        {
            PreserveApplicationSettings = false;

            this.headless = headless;
            this.serviceMode = serviceMode;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication WithAudioImplementation(AudioImplementation audioImplementation)
        {
            switch (audioImplementation)
            {
                case AudioImplementation.BASS:
                    return WithPlugin(new BASSAudioPlugin());
                case AudioImplementation.FMOD:
                    return WithPlugin(new FMODAudioPlugin());
                default:
                    throw new ArgumentOutOfRangeException(nameof(audioImplementation));
            }
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication WithConfiguration(Action<FrameworkConfiguration> configurer)
        {
            this.configurer = configurer;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication WithPlugin(FrameworkPlugin plugin)
        {
            if (plugin == null)
                return this;

            if (this.plugins == null)
                this.plugins = new List<FrameworkPlugin>();

            this.plugins.Add(plugin);
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication WithInitialization(Action<FrameworkContext> initializer)
        {
            if (this.initializer != null)
                throw new InvalidOperationException("Initialization has already been configured.");

            this.initializer = initializer;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication WithContent(Action<ContentManager> loader)
        {
            if (this.content != null)
                throw new InvalidOperationException("Content loading has already been configured.");

            this.loader = loader;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication WithDispose(Action disposer)
        {
            if (this.disposer != null)
                throw new InvalidOperationException("Disposal has already been configured.");

            this.disposer = disposer;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication SkipFrames(Int32 frameCount)
        {
            Contract.EnsureRange(frameCount >= 0, nameof(frameCount));

            framesToSkip = frameCount;
            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication OnFrameStart(Int32 frame, Action<IFrameworkTestApplication> action)
        {
            if (frameActions == null)
                frameActions = new List<FrameAction>();

            frameActions.Add(new FrameAction(FrameActionType.FrameStart, frame, action));

            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication OnUpdate(Action<IFrameworkTestApplication, FrameworkTime> action)
        {
            if (frameActions == null)
                frameActions = new List<FrameAction>();

            frameActions.Add(new FrameAction(FrameActionType.Update, -1, action));

            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication OnUpdate(Int32 update, Action<IFrameworkTestApplication, FrameworkTime> action)
        {
            if (frameActions == null)
                frameActions = new List<FrameAction>();

            frameActions.Add(new FrameAction(FrameActionType.Update, update, action));

            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication OnRender(Action<IFrameworkTestApplication, FrameworkTime> action)
        {
            if (frameActions == null)
                frameActions = new List<FrameAction>();

            frameActions.Add(new FrameAction(FrameActionType.Render, -1, action));

            return this;
        }

        /// <inheritdoc/>
        public IFrameworkTestApplication OnRender(Int32 render, Action<IFrameworkTestApplication, FrameworkTime> action)
        {
            if (frameActions == null)
                frameActions = new List<FrameAction>();

            frameActions.Add(new FrameAction(FrameActionType.Render, render, action));

            return this;
        }

        /// <inheritdoc/>
        public StbImageSharp.ImageResult Render(Action<FrameworkContext> renderer)
        {
            if (headless)
                throw new InvalidOperationException("Cannot render a headless window.");

            this.shouldExit = () => true;

            this.renderer = renderer;
            this.Run();

            return image;
        }

        /// <inheritdoc/>
        public void RunUntil(Func<Boolean> predicate)
        {
            this.shouldExit = predicate;
            this.Run();
        }

        /// <inheritdoc/>
        public void RunFor(TimeSpan time)
        {
            RunUntil(() => DateTime.UtcNow >= startTime + time);
        }

        /// <inheritdoc/>
        public void RunForOneFrame()
        {
            RunFor(TimeSpan.Zero);
        }

        /// <inheritdoc/>
        public void RunAllFrameActions()
        {
            RunUntil(() =>
            {
                return
                    !frameActions.Any(x => x.ActionType == FrameActionType.FrameStart && x.ActionIndex >= frameCount) &&
                    !frameActions.Any(x => x.ActionType == FrameActionType.Render && x.ActionIndex >= renderCount) &&
                    !frameActions.Any(x => x.ActionType == FrameActionType.Update && x.ActionIndex >= updateCount);
            });
        }

        /// <inheritdoc/>
        public void SpoofKeyDown(Scancode scancode, Key key, Boolean ctrl, Boolean alt, Boolean shift)
        {
            var data = Sedulous.Messages.CreateMessageData<SDL2EventMessageData>();
            data.Event = new SDL_Event()
            {
                key = new SDL_KeyboardEvent()
                {
                    type = (uint)SDL_KEYDOWN,
                    windowID = (uint)Sedulous.GetPlatform().Windows.GetPrimary().ID,
                    keysym = new SDL_Keysym()
                    {
                        keycode = (SDL_Keycode)key,
                        scancode = (SDL_Scancode)scancode,
                        mod =
                            (ctrl ? KMOD_CTRL : KMOD_NONE) |
                            (alt ? KMOD_ALT : KMOD_NONE) |
                            (shift ? KMOD_SHIFT : KMOD_NONE),
                    },
                }
            };
            Sedulous.Messages.Publish(SDL2FrameworkMessages.SDLEvent, data);
        }

        /// <inheritdoc/>
        public void SpoofKeyUp(Scancode scancode, Key key, Boolean ctrl, Boolean alt, Boolean shift)
        {
            var data = Sedulous.Messages.CreateMessageData<SDL2EventMessageData>();
            data.Event = new SDL_Event()
            {
                key = new SDL_KeyboardEvent()
                {
                    type = (uint)SDL_KEYUP,
                    windowID = (uint)Sedulous.GetPlatform().Windows.GetPrimary().ID,
                    keysym = new SDL_Keysym()
                    {
                        keycode = (SDL_Keycode)key,
                        scancode = (SDL_Scancode)scancode,
                        mod =
                            (ctrl ? KMOD_CTRL : KMOD_NONE) |
                            (alt ? KMOD_ALT : KMOD_NONE) |
                            (shift ? KMOD_SHIFT : KMOD_NONE),
                    },
                }
            };
            Sedulous.Messages.Publish(SDL2FrameworkMessages.SDLEvent, data);
        }

        /// <inheritdoc/>
        public void SpoofKeyPress(Scancode scancode, Key key, Boolean ctrl, Boolean alt, Boolean shift)
        {
            SpoofKeyDown(scancode, key, ctrl, alt, shift);
            SpoofKeyUp(scancode, key, ctrl, alt, shift);
        }

        /// <inheritdoc/>
        protected override FrameworkContext OnCreatingSedulousContext()
        {
            var configuration = new SDL2FrameworkConfiguration();
            configuration.Headless = headless;
            configuration.EnableServiceMode = serviceMode;
            configuration.IsHardwareInputDisabled = true;
            configuration.Debug = true;
            configuration.DebugLevels = DebugLevels.Error | DebugLevels.Warning;
            configuration.DebugCallback = (uv, level, message) =>
            {
                System.Diagnostics.Debug.WriteLine(message);
            };

            var needsGraphicsSubsystem = !plugins?.Any(x => x is OpenGLGraphicsPlugin) ?? true;
            if (needsGraphicsSubsystem)
            {
                plugins = plugins ?? new List<FrameworkPlugin>();
                plugins.Add(new OpenGLGraphicsPlugin());
            }

            var needsAudioSubsystem = !(plugins?.Any(x => x is BASSAudioPlugin || x is FMODAudioPlugin) ?? false);
            if (needsAudioSubsystem)
            {
                plugins = plugins ?? new List<FrameworkPlugin>();
                plugins.Add(new BASSAudioPlugin());
            }

            foreach (var plugin in plugins)
                configuration.Plugins.Add(plugin);

            configurer?.Invoke(configuration);

            return new SDL2FrameworkContext(this, configuration);
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            if (!headless)
                Sedulous.GetPlatform().Windows.GetPrimary().ClientSize = new Size2(480, 360);

            initializer?.Invoke(Sedulous);

            Sedulous.FrameStart += OnFrameStart;

            base.OnInitialized();
        }

        /// <inheritdoc/>
        protected override void OnShutdown()
        {
            if (!Sedulous.Disposed)
            {
                Sedulous.FrameStart -= OnFrameStart;
            }
            base.OnShutdown();
        }

        /// <inheritdoc/>
        protected override void OnLoadingContent()
        {
            var window = Sedulous.GetPlatform().Windows.GetPrimary();

            if (!headless)
            {
                // HACK: AMD drivers produce weird rasterization artifacts when rendering
                // to a NPOT render buffer??? So we have to fix it with this stupid hack???
                var width = MathUtil.FindNextPowerOfTwo(window.DrawableSize.Width);
                var height = MathUtil.FindNextPowerOfTwo(window.DrawableSize.Height);

                rtargetColorBuffer = Texture2D.CreateRenderBuffer(RenderBufferFormat.Color, width, height);
                rtargetDepthStencilBuffer = Texture2D.CreateRenderBuffer(RenderBufferFormat.Depth24Stencil8, width, height);
                rtarget = RenderTarget2D.Create(width, height);
                rtarget.Attach(rtargetColorBuffer);
                rtarget.Attach(rtargetDepthStencilBuffer);
            }

            if (loader != null)
            {
                content = ContentManager.Create(Path.Combine("Resources", "Content"));
                loader(content);
            }

            base.OnLoadingContent();
        }

        /// <inheritdoc/>
        protected override void OnUpdating(FrameworkTime time)
        {
            RunFrameActions(FrameActionType.Update, updateCount, time);

            if (framesToSkip == 0)
            {
                if (shouldExit())
                {
                    Exit();
                }
            }

            updateCount++;

            base.OnUpdating(time);
        }

        /// <inheritdoc/>
        protected override void OnDrawing(FrameworkTime time)
        {
            RunFrameActions(FrameActionType.Render, renderCount, time);

            if (framesToSkip == 0)
            {
                var window = 
                    Sedulous.GetPlatform().Windows.GetPrimary();

                var compositor = window.Compositor as ITestFrameworkCompositor;
                if (compositor != null)
                    compositor.TestFrameworkRenderTarget = rtarget;
                else
                {
                    Sedulous.GetGraphics().SetRenderTarget(rtarget);
                    Sedulous.GetGraphics().SetViewport(new Viewport(0, 0, window.ClientSize.Width, window.ClientSize.Height));
                    Sedulous.GetGraphics().Clear(Color.Black);
                }

                renderer?.Invoke(Sedulous);

                if (compositor != null)
                {
                    window.Compositor.Compose();
                    window.Compositor.Present();
                }

                Sedulous.GetGraphics().SetRenderTargetToBackBuffer();
                Sedulous.GetGraphics().Clear(Color.CornflowerBlue);
                image = ConvertRenderTargetToBitmap(rtarget);
            }
            else
            {
                framesToSkip--;
            }

            renderCount++;

            base.OnDrawing(time);
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                disposer?.Invoke();
                content?.Dispose();
                content = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Occurs at the start of a frame.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        private void OnFrameStart(FrameworkContext uv)
        {
            if (frameCount == 0)
                startTime = DateTime.UtcNow;

            RunFrameActions(FrameActionType.FrameStart, frameCount, null);
            frameCount++;
        }

        /// <summary>
        /// Runs the specified set of frame actions.
        /// </summary>
        private void RunFrameActions(FrameActionType actionType, Int32 actionIndex, FrameworkTime time)
        {
            if (frameActions == null)
                return;

            var actions = frameActions.Where(x => x.ActionType == actionType && (x.ActionIndex < 0 || x.ActionIndex == actionIndex));
            foreach (var action in actions)
            {
                switch (actionType)
                {
                    case FrameActionType.FrameStart:
                        ((Action<IFrameworkTestApplication>)action.Action)(this);
                        break;

                    default:
                        ((Action<IFrameworkTestApplication, FrameworkTime>)action.Action)(this, time);
                        break;
                }
            }
        }

        /// <summary>
        /// Converts the specified render target to a bitmap image.
        /// </summary>
        /// <param name="rt">The render target to convert.</param>
        /// <returns>The converted image.</returns>
        private StbImageSharp.ImageResult ConvertRenderTargetToBitmap(RenderTarget2D rt)
        {
            // HACK: Our buffer has been rounded up to the nearest
            // power of two, so at this point we clip it back down
            // to the size of the window.

            var window = Sedulous.GetPlatform().Windows.GetPrimary();
            var windowWidth = window.DrawableSize.Width;
            var windowHeight = window.DrawableSize.Height;

            var data = new Color[rt.Width * rt.Height];
            rt.GetData(data);

            var img = new StbImageSharp.ImageResult()
            {
                Width = windowWidth,
                Height = windowHeight,
                Comp = StbImageSharp.ColorComponents.RedGreenBlueAlpha,
                Data = new byte[windowWidth * windowHeight * 4]
            };
            var pixel = 0;
            for (int y = 0; y < rt.Height; y++)
            {
                for (int x = 0; x < rt.Width; x++)
                {
                    if (x < windowWidth && y < windowHeight)
                    {
                        var rawColor = data[pixel];

                        img.SetPixel(x, y, rawColor.R, rawColor.G, rawColor.B, 255);
                    }
                    pixel++;
                }
            }

            return img;
        }

        // State values.
        private readonly Boolean headless;
        private readonly Boolean serviceMode;
        private Func<Boolean> shouldExit;
        private ContentManager content;
        private Action<FrameworkConfiguration> configurer;
        private Action<FrameworkContext> initializer;
        private Action<ContentManager> loader;
        private Action<FrameworkContext> renderer;
        private Action disposer;
        private StbImageSharp.ImageResult image;
        private Int32 updateCount;
        private Int32 renderCount;
        private Int32 frameCount;
        private Int32 framesToSkip;
        private DateTime startTime;
        private List<FrameAction> frameActions;
        private List<FrameworkPlugin> plugins;

        // The render target to which the test scene will be rendered.
        private RenderTarget2D rtarget;
        private RenderBuffer2D rtargetColorBuffer;
        private RenderBuffer2D rtargetDepthStencilBuffer;
    }
}