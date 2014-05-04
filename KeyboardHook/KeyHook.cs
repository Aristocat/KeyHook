using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardHook
{
    public sealed class KeyHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            #region IDisposable Members

            public void Dispose()
            {
                this.DestroyHandle();
            }

            #endregion
        }

        private Dictionary<HotKey, EventHandler<KeyPressedEventArgs>> _keyHandlers;

        private Window _window = new Window();
        private int _currentId;

        public KeyHook()
        {
            _keyHandlers = new Dictionary<HotKey, EventHandler<KeyPressedEventArgs>>();

            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                foreach (HotKey e in _keyHandlers.Keys)
                {
                    if (e.Modifier == args.Modifier && e.Key == args.Key)
                    {
                        _keyHandlers[e](this, args);
                        return;
                    }
                }
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public int RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            _currentId++;

            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn't register the hot key.");

            return _currentId;
        }

        /// <summary>
        /// Unregisters a hot key in the system.
        /// </summary>
        /// <param name="id">The id of the hot key.</param>
        public bool UnregisterHotKey(int id)
        {
            return !UnregisterHotKey(_window.Handle, id);
        }

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Attaches a handler for a hot key.
        /// </summary>
        /// <param name="hotkey">The arguments of the hot key.</param>
        /// <param name="e">The event handler of the hot key.</param>
        public void AttachHandler(HotKey hotkey, EventHandler<KeyPressedEventArgs> e)
        {
            _keyHandlers.Add(hotkey, e);
        }

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }

        #endregion
    }
}
