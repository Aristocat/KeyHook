using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyboardHook
{
    public class HotKey
    {
        public ModifierKeys Modifier;
        public Keys Key;

        public HotKey(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }
    }
}
