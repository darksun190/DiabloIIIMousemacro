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
using Timer = System.Timers.Timer;

namespace D3鼠标宏
{
    public partial class Form1 : Form
    {
    
        Timer sTimer;
        Timer lTimer;
        Timer timer3;

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

        //// 快捷键消息处理
        //public void ProcessHotKey(Message m)
        //{
        //    if (m.Msg == 0x312)
        //    {
        //        int id = m.WParam.ToInt32();
        //        HotKeyCallBackHanlder callback;
        //        if (keymap.TryGetValue(id, out callback))
        //            callback();
        //    }
        //}
        /// <summary>
        /// 构造函数
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            sTimer = new Timer(100);
            lTimer = new Timer(10000);
            timer3 = new Timer(100);
            lTimer.AutoReset = false;
            sTimer.AutoReset = true;
            timer3.AutoReset = true;
            lTimer.Elapsed += LTimer_Elapsed;
            sTimer.Elapsed += STimer_Elapsed;
            timer3.Elapsed += Timer3_Elapsed;
            //sTimer.Enabled = true;
            //lTimer.Enabled = true;
        }

        private void Timer3_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            keybd_event(Keys.D3, 0, 0, 0);
            keybd_event(Keys.D3, 0, 2, 0);
        }

        private void STimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("trigger 1");
            keybd_event(Keys.D1, 0, 0, 0);
            keybd_event(Keys.D1, 0, 2, 0);
        }

        private void LTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sTimer.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(Handle, 100, 2, Keys.D3);
            RegisterHotKey(Handle, 101, 2, Keys.D2);
        }


      
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 786:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            startPress3();
                            break;
                        case 101:
                            startPress1();
                            break;
                    }
                    break;

            }
            base.WndProc(ref m);
        }

        private void startPress1()
        {
            keybd_event(Keys.D2, 0, 0, 0);
            keybd_event(Keys.D2, 0, 2, 0);
            sTimer.Start();
            lTimer.Start();
        }

        private void startPress3()
        {
            if(timer3.Enabled)
            {
                timer3.Stop();
            }
            else
            {
                timer3.Start();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, 100);
            UnregisterHotKey(Handle, 101);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int v1 = Int32.Parse(textBox1.Text);
            int v2 = Int32.Parse(textBox2.Text);
            int v3 = Int32.Parse(textBox3.Text);
            sTimer.Interval = v1;
            lTimer.Interval = v2;
            timer3.Interval = v3;
        }
    }
}
