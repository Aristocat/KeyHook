using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyHook
{
    public partial class Form1 : Form
    {
        KeyboardHook Keyboard;
        List<Keys> Hooked;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Keyboard = new KeyboardHook();
            Keyboard.KeyPressed += new EventHandler<KeyPressedEventArgs>(Keyboard_OnKeyPress);
            Hooked = new List<Keys>();
            foreach (Keys Key in Enum.GetValues(typeof(Keys)))
                try
                {
                    string KeyStr =  Enum.GetName(typeof(Keys), Key).Replace("NumPad", "");
                    if (KeyStr.Length == 1 || KeyStr.StartsWith("D") && KeyStr.Length == 2)
                    {
                        Keyboard.RegisterHotKey(0, Key);
                        Keyboard.RegisterHotKey(KeyHook.ModifierKeys.Shift, Key);
                    }
                }
                catch
                {
                    continue;
                }
        }

        private void Keyboard_OnKeyPress(object sender, KeyPressedEventArgs e)
        {
            string KeyStr = Enum.GetName(typeof(Keys), e.Key).Replace("NumPad", "");
            if (e.Modifier == 0 || e.Modifier == KeyHook.ModifierKeys.Shift)
            {
                if (KeyStr.StartsWith("D") && KeyStr.Length == 2)
                    if (e.Modifier == 0)
                        KeyStr = KeyStr.Replace("D", "");
                    //else
                    //    KeyStr = ShiftKey(KeyStr);
                if (KeyStr.Length == 1)
                {
                    Hooked.Add(e.Key);
                    richTextBox1.Text += KeyStr;
                }
            }
            try
            {
                string KeyName = Enum.GetName(typeof(Keys), e.Key).Replace("NumPad", "");
                if (KeyStr.StartsWith("D") && KeyStr.Length == 2)
                    if (e.Modifier == 0)
                        KeyName = Enum.GetName(typeof(Keys), e.Key).Replace("D", "");
                //else
                //    KeyStr = ShiftKey(KeyStr);
                if (e.Modifier == KeyHook.ModifierKeys.Shift)
                    KeyName = KeyName.ToUpper();
                else
                    KeyName = KeyName.ToLower();
                if (KeyName.Length == 1)
                    SendKeys.Send(KeyName);
                else
                    SendKeys.Send("{" + KeyName + "}");
            }
            catch { }
        }
    }
}
