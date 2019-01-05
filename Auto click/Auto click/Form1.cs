using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Auto_click.gma.System.Windows;

namespace Auto_click
{
    public partial class Form1 : Form
    {
        public UserActivityHook User = new UserActivityHook();

        public About about;

        public int Seconds = 0;
        public int Count = 0;

        List<MouseRecord> MouseRecords = new List<MouseRecord>();

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos( int X, int Y);

        [DllImport("user32.dll")]
        public static extern short VkKeyScanA(char ch);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);

        public enum Mouse
        {
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MOVE = 0x0001
        }

        public Form1()
        {
            InitializeComponent();       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            about = new About();
            this.Text = "Auto Click";
        }
        
        private void User_OnMouseActivity(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                MouseRecords.Add(new MouseRecord { MouseButtons = e.Button, Point = e.Location,Seconds = this.Seconds});
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MouseRecords.Count == 0)
            {
                label2.Text = $"Info:{Environment.NewLine} No Record Data";
            }
            else if(textBox1.Text == string.Empty)
            {
                MessageBox.Show("Must Input Number Click!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                button1.Text = "Stop";
                button2.Enabled = false;
                button3.Enabled = false;
                User.KeyDown += User_KeyDown1;
                progressBar2.Maximum = Convert.ToInt32(textBox1.Text);
                progressBar1.Maximum = MouseRecords.ElementAt(MouseRecords.Count - 1).Seconds;
                checkBox1.Enabled = false;
                if (checkBox1.Checked)
                {
                    this.Hide();
                }
                timer2.Interval = MouseRecords.ElementAt(0).Seconds;
                timer2.Start();
            }
        }

        private void User_KeyDown1(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                timer2.Stop();
                button1.Text = "Start";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MouseRecords.Clear();          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            User.OnMouseActivity += User_OnMouseActivity;
            User.KeyDown += User_KeyDown;
            timer1.Start();
            this.Hide();
        }

        private void User_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                for (int i = 0; i < Keyboard.Text.Length; i++)
                {
                    keybd_event((byte)VkKeyScanA(Keyboard.Text.ElementAt<char>(i)), 0, 0x0001, (UIntPtr)0);
                }
                MouseRecords.Add(new MouseRecord { MouseButtons = MouseButtons.None, Point = Point.Empty, Seconds = this.Seconds });
            }

            if (e.KeyCode == Keys.Enter)
            {
                User.OnMouseActivity -= User_OnMouseActivity;
                User.KeyDown -= User_KeyDown;
                timer1.Stop();
                Seconds = 0;
                MessageBox.Show("Record Complete");
                this.Show();
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Seconds += 10;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (MouseRecords.ElementAt(Count).MouseButtons == MouseButtons.None)
                {
                    for (int i = 0; i < Keyboard.Text.Length; i++)
                    {
                        keybd_event((byte)VkKeyScanA(Keyboard.Text.ElementAt<char>(i)), 0, 0x0001, (UIntPtr)0);
                    }
                }
                else
                {
                    SetCursorPos(MouseRecords.ElementAt(Count).Point.X, MouseRecords.ElementAt(Count).Point.Y);
                    if (MouseRecords.ElementAt(Count).MouseButtons == MouseButtons.Right)
                    {
                        mouse_event((uint)(Mouse.MOUSEEVENTF_RIGHTDOWN | Mouse.MOUSEEVENTF_RIGHTUP), 0, 0, 0, (System.UIntPtr)0);
                    }
                    else if (MouseRecords.ElementAt(Count).MouseButtons == MouseButtons.Left)
                    {
                        mouse_event((uint)(Mouse.MOUSEEVENTF_LEFTUP | Mouse.MOUSEEVENTF_LEFTDOWN), 0, 0, 0, (System.UIntPtr)0);
                    }
                }
                Count++;
                if (Count == MouseRecords.Count)
                {
                    timer2.Stop();
                    timer2.Interval = 10;
                    Count = 0;
                    if (Convert.ToInt32(textBox1.Text) > 0)
                    {
                        progressBar2.PerformStep();
                        textBox1.Text = $"{Convert.ToInt32(textBox1.Text) - 1}";
                        timer2.Start();
                    }
                    else
                    {
                        checkBox1.Enabled = true;
                        button1.Text = "Start";
                        button2.Enabled = true;
                        button3.Enabled = true;
                        if (checkBox1.Checked)
                        {
                            this.ShowDialog();
                        }
                    }
                }
                else
                {
                    timer2.Interval = MouseRecords.ElementAt(Count).Seconds - MouseRecords.ElementAt(Count - 1).Seconds;
                    progressBar1.Value = MouseRecords.ElementAt(Count).Seconds;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label2.Text = "Info";
            for (int i = 0; i < MouseRecords.Count; i++)
            {
                label2.Text += $"{Environment.NewLine}{MouseRecords.ElementAt(i).MouseButtons.ToString()} " +
                    $" {MouseRecords.ElementAt(i).Point.ToString()}  " +
                    $"{MouseRecords.ElementAt(i).Seconds.ToString()} .{Environment.NewLine}";
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Application.OpenForms[about.Name] == null)
            {
                about.Show();
            }
            else
            {
                SystemSounds.Beep.Play();
                Application.OpenForms[about.Name].Focus();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Keyboard.Text.Length; i++)
            {
                keybd_event((byte)VkKeyScanA(Keyboard.Text.ElementAt<char>(i)), 0, 0x0001, (UIntPtr)0);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
