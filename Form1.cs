using System;
using System.Text;
using System.Windows.Forms;

using SharpPcap;
using PacketDotNet;

namespace practicaWin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            objAes = new AES();
        }

        AES objAes;

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label1.Enabled = false;
            textBox2.Enabled = false;
        }

        CaptureDeviceList devices = CaptureDeviceList.Instance;
        ICaptureDevice device = null;

        private void start_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            textBox2.Clear();

            button1.Enabled = true;
            label1.Enabled = true;
            textBox2.Enabled = true;

            textBox2.Focus();

            if (devices.Count < 1)
            {
                MessageBox.Show("На этом компьютере не было обнаружено никаких устройств!");
                return;
            }

            for (int i = 0; i < devices.Count; i++)
            {
                richTextBox1.Text += ($"\n{i}.    {devices[i].ToString()}");
            }
        }

        private void stop_Click(object sender, EventArgs e)
        {
            device.StopCapture();
            device.Close();

            button1.Enabled = false;
            label1.Enabled = false;
            textBox2.Enabled = false;
        }

        private void DeviceSelection(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            int num = 0;
            if ((!Int32.TryParse(textBox2.Text, out num)) || (num > devices.Count))
            {
                MessageBox.Show("Неверный номер устройства!");
                textBox2.Clear();
                return;
            }

            device = devices[num];
            device.OnPacketArrival += new PacketArrivalEventHandler(Device_OnPacketArrival);
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            // работа с пакетами
            device.StartCapture();
        }

        private void Program_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            richTextBox1.Invoke((MethodInvoker)delegate
            {
                Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                TcpPacket tcp = (TcpPacket)packet.Extract<TcpPacket>();
                if (tcp != null)
                {
                    byte[] data = tcp.PayloadData;
                    string s = Encoding.UTF8.GetString(data);
                    richTextBox1.Text += ($"{s}\n*---------------------------------------------------------------*\n");
                }
            });
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (device != null)
                stop_Click(sender, e);
        }


        private void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            richTextBox1.Invoke((MethodInvoker)delegate
            {
                Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

                string str = packet.ToString();

                DateTime time = e.Packet.Timeval.Date;
                int len = e.Packet.Data.Length;

                richTextBox1.Text += ($"{time.Hour+3}:{time.Minute}:{time.Second},{time.Millisecond}\n{str}\nLen={len}\n\n");
           });
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DeviceSelection(sender, e);
            }
        }

        private void encrypt_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = objAes.Encrypt(richTextBox1.Text);
        }

        private void decrypt_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = objAes.Decrypt(richTextBox2.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop_Click(sender, e);

            richTextBox1.Clear();

            int num = 0;
            if ((!Int32.TryParse(textBox2.Text, out num)) || (num > devices.Count))
            {
                MessageBox.Show("Неверный номер устройства!");
                textBox2.Clear();
                return;
            }

            device = devices[num];
            device.OnPacketArrival += new PacketArrivalEventHandler(Program_OnPacketArrival);
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            device.StartCapture();
        }

    }
}
