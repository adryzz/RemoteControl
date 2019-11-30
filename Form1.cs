using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioSwitcher.AudioApi.CoreAudio;
using System.IO.Ports;
using System.Runtime.InteropServices;

namespace RemoteControl
{
    public partial class Form1 : Form
    {

        Form2 form2;
        public CoreAudioDevice audioDevice;
        public SerialPort port;
        public Form1()
        {
            InitializeComponent();
            form2 = new Form2(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            form2.Show();
            timer1.Start();
        }

        private void initNot()
        {
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = "Remote control has been started";
            notifyIcon1.BalloonTipTitle = "Remote Control";
            notifyIcon1.ShowBalloonTip(1000);
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!form2.Visible)
            {
                form2.Show();
                timer1.Start();
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("RemoteControl By Adryzz\nNot for commercial use\n<github.com/adryyyy>", "About");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!form2.Visible)
            {
                initNot();
                timer1.Stop();
            }
        }

        public void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = port.ReadExisting();
            if (data.Contains("VOLUME"))
            {
                AdjustVolume(data);
            }
            else if (data.Equals("PLAYPAUSE"))
            {
                PlayPause();
            }
            else if (data.Equals("NEXT"))
            {
                Next();
            }
            else if (data.Equals("PREV"))
            {
                Previous();
            }
            else if (data.Equals("MUTE"))
            {
                Mute();
            }
            else if (data.Equals("UNMUTE"))
            {
                UnMute();
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
        public const int VK_MEDIA_PREV_TRACK = 0xB1;
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

        private void PlayPause()
        {
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        private void Next()
        {
            keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            keybd_event(VK_MEDIA_NEXT_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        private void Previous()
        {
            keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            keybd_event(VK_MEDIA_PREV_TRACK, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        private void AdjustVolume(string NewData)
        {
            var replacements = new[] { new { Find = "VOLUME ", Replace = "" } };

            foreach (var set in replacements)
            {
                NewData = NewData.Replace(set.Find, set.Replace);
            }
            audioDevice.Volume = int.Parse(NewData);
        }

        private void Mute()
        {
            audioDevice.Mute(true);
        }

        private void UnMute()
        {
            audioDevice.Mute(false);
        }

        private void AudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("control", "Mmsys.cpl");
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}