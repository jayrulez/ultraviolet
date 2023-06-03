using System;
using Sedulous.Core;
using Sedulous.Input;

namespace Sedulous
{
#pragma warning disable 67
    /// <summary>
    /// Represents a dummy implementation of <see cref="ISedulousInput"/>.
    /// </summary>
    public sealed class DummySedulousInput : SedulousResource, ISedulousInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummySedulousInput"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public DummySedulousInput(SedulousContext uv)
            : base(uv)
        {
        }

        /// <inheritdoc/>
        public void Update(SedulousTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            Updating?.Invoke(this, time);
        }

        /// <inheritdoc/>
        public void ShowSoftwareKeyboard()
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public void ShowSoftwareKeyboard(KeyboardMode mode)
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public void HideSoftwareKeyboard()
        {
            Contract.EnsureNotDisposed(this, Disposed);
        }

        /// <inheritdoc/>
        public Boolean IsKeyboardSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsKeyboardRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public KeyboardDevice GetKeyboard()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public Boolean IsMouseSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsMouseRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public MouseDevice GetMouse()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public Int32 GetGamePadCount()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return 0;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadConnected()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadConnected(Int32 playerIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsGamePadRegistered(Int32 playerIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public GamePadDevice GetGamePadForPlayer(Int32 playerIndex)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public GamePadDevice GetFirstConnectedGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public GamePadDevice GetFirstRegisteredGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public GamePadDevice GetPrimaryGamePad()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public Boolean IsTouchSupported()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceConnected()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceConnected(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceRegistered()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }

        /// <inheritdoc/>
        public Boolean IsTouchDeviceRegistered(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return false;
        }
        
        /// <inheritdoc/>
        public TouchDevice GetTouchDevice(Int32 index)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public TouchDevice GetFirstConnectedTouchDevice()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public TouchDevice GetFirstRegisteredTouchDevice()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public TouchDevice GetPrimaryTouchDevice()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public Boolean EmulateMouseWithTouchInput
        {
            get { return true; }
            set { }
       }

        /// <inheritdoc/>
        public Boolean IsMouseCursorAvailable
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public event SedulousSubsystemUpdateEventHandler Updating;

        /// <inheritdoc/>
        public event KeyboardRegistrationEventHandler KeyboardRegistered;

        /// <inheritdoc/>
        public event MouseRegistrationEventHandler MouseRegistered;
                
        /// <inheritdoc/>
        public event GamePadConnectionEventHandler GamePadConnected;

        /// <inheritdoc/>
        public event GamePadConnectionEventHandler GamePadDisconnected;

        /// <inheritdoc/>
        public event GamePadRegistrationEventHandler GamePadRegistered;

        /// <inheritdoc/>
        public event TouchDeviceRegistrationEventHandler TouchDeviceRegistered;
    }
#pragma warning restore 67
}
