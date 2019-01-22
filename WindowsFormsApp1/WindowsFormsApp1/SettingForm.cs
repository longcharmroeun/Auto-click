using System;
using System.   Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class SettingForm : Form
    {
        private Authentication authentication;
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            authentication = new Authentication();
            authentication = Newtonsoft.Json.JsonConvert.DeserializeObject<Authentication>(System.Text.Encoding.UTF8.GetString(Properties.Resources.data).TrimEnd('\0'));
            textBox1.Text = authentication.username;
            textBox2.Text = authentication.password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            authentication.password = textBox2.Text;
            authentication.username = textBox1.Text;
            File.WriteAllText("../../data.json", Newtonsoft.Json.JsonConvert.SerializeObject(authentication));       
            this.Close();
        }
    }
}
