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

namespace RemoteControl
{
    public partial class Form2 : Form
    {
        Form1 form1;
        CoreAudioDevice device = new CoreAudioController().DefaultPlaybackDevice;
        IEnumerable<CoreAudioDevice> devices = new CoreAudioController().GetPlaybackDevices();
        public Form2(Form1 form)
        {
            InitializeComponent();
            form1 = form;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            foreach(CoreAudioDevice d in devices)
            {
                comboBox2.Items.Add(d.FullName);
            }
            if (Convert.ToInt32(device.Volume) < 0 && Convert.ToInt32(device.Volume) > 100)
            {
                MessageBox.Show("No devices found or the default one is disabled");
                Application.Exit();
            }
            else
            {
                trackBar1.Value = Convert.ToInt32(device.Volume);
            }

            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            device.Volume = trackBar1.Value;
        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (CoreAudioDevice d in devices)
            {
                if (d.FullName.Equals(comboBox2.SelectedItem))
                {
                    device = d;
                }
                if (Convert.ToInt32(device.Volume) < 0 && Convert.ToInt32(device.Volume) > 100)
                {
                    MessageBox.Show("The device is disabled");
                    Application.Exit();
                } else
                {
                    trackBar1.Value = Convert.ToInt32(device.Volume);
                }
            }

        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            form1.audioDevice = device;
            e.Cancel = true;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (form1.port != null)
                {
                    form1.port.Close();
                    form1.port.Dispose();
                }
                form1.port = new SerialPort(comboBox1.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);
                form1.port.Open();
                form1.port.DataReceived += new SerialDataReceivedEventHandler(form1.Port_DataReceived);
                if (form1.port.IsOpen)
                {
                    form1.port.Write("Ready");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
