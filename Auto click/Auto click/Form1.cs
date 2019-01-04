using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public int Seconds = 0;
        public int Count = 0;

        List<MouseRecord> MouseRecords = new List<MouseRecord>();

        [DllImport("user32.dll")]
        static extern bool SetCursorPos( int X, int Y);

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
            
        }

        private void User_OnMouseActivity(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                MouseRecords.Add(new MouseRecord { MouseButtons = e.Button, Point = e.Location,Seconds = this.Seconds });
                if (!timer1.Enabled)
                {
                    timer1.Start();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MouseRecords.Count == 0)
            {
                label2.Text = $"Info:{Environment.NewLine} No Record Data";
            }
            else
            {
                button2.Text = "'Space'";
                User.KeyDown += User_KeyDown1;
                button1.Enabled = false;
                button2.Enabled = true;
                progressBar2.Maximum = Convert.ToInt32(textBox1.Text);
                progressBar1.Maximum = MouseRecords.ElementAt(MouseRecords.Count - 1).Seconds;
                checkBox1.Enabled = false;
                if (checkBox1.Checked)
                {
                    this.Hide();
                }
                timer2.Start();
            }
        }

        private void User_KeyDown1(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                button2.PerformClick();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MouseRecords.Count > 0)
            {
                timer2.Stop();
                button1.Enabled = true;
                button2.Enabled = false;
            }           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            User.OnMouseActivity += User_OnMouseActivity;
            User.KeyDown += User_KeyDown;
            this.Hide();
        }

        private void User_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
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
            if (Count == MouseRecords.Count)
            {
                timer2.Stop();
                timer2.Interval = 10;
                Count = 0;
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
                        button1.Enabled = true;
                        button2.Text = "Stop";
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
            MessageBox.Show("Coming Soon");
        }
        
    }
}
