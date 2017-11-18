using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
namespace D3鼠标宏
{
    public partial class Form1 : Form
    {
        private Thread pressing;
        bool ok;
        static AutoResetEvent are = new AutoResetEvent(false);
        [DllImport("user32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int modifiers, Keys vk);
        [DllImport("user32.dll")]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        int keyid = 10;     //区分不同的快捷键
        Dictionary<int, HotKeyCallBackHanlder> keymap = new Dictionary<int, HotKeyCallBackHanlder>();   //每一个key对于一个处理函数
        public delegate void HotKeyCallBackHanlder();
        //组合控制键
        public enum HotkeyModifiers
        {
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        //注册快捷键
        public void Regist(IntPtr hWnd, int modifiers, Keys vk, HotKeyCallBackHanlder callBack)
        {
            int id = keyid++;
            if (!RegisterHotKey(hWnd, id, modifiers, vk))
                throw new Exception("注册失败！");
            keymap[id] = callBack;
        }

        // 注销快捷键
        public void UnRegist(IntPtr hWnd, HotKeyCallBackHanlder callBack)
        {
            foreach (KeyValuePair<int, HotKeyCallBackHanlder> var in keymap)
            {
                if (var.Value == callBack)
                {
                    UnregisterHotKey(hWnd, var.Key);
                    return;
                }
            }
        }

        // 快捷键消息处理
        public void ProcessHotKey(Message m)
        {
            if (m.Msg == 0x312)
            {
                int id = m.WParam.ToInt32();
                HotKeyCallBackHanlder callback;
                if (keymap.TryGetValue(id, out callback))
                    callback();
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(Handle, 100, 2, Keys.B);
            pressing = new Thread(new ThreadStart(looppress3));
            pressing.Start();
           
        }

        private void looppress3()
        {
            while(true)
            {
                if (ok)
                {
                    Thread.Sleep(400);
                    keybd_event(Keys.D3, 0, 0, 0);
                    keybd_event(Keys.D3, 0, 2, 0);
                }
                else
                {
                    are.WaitOne();
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 786:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            startPress();
                            break;
                    }
                    break;

            }
            base.WndProc(ref m);
        }

        private void startPress()
        {
            if(ok)
            {
                ok = false;
            }
            else 
            {
                ok = true;
                are.Set();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            are.Close();
            pressing.Abort();
            UnregisterHotKey(Handle, 100);
        }
    }
}
