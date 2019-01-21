using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
            var http = (HttpWebRequest)WebRequest.Create(new Uri("https://msapi.itstep.org/api/v1/auth/login"));
            http.UseDefaultCredentials = true;
            http.PreAuthenticate = true;
            http.Credentials = CredentialCache.DefaultCredentials;
            http.KeepAlive = true;
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            Stream newStream = http.GetRequestStream();
            newStream.Write(Properties.Resources.data, 0, Properties.Resources.data.Length);
            newStream.Close();

            if (http.KeepAlive)
            {
                WebResponse web = http.GetResponse();
                for (int i = 0; i < web.Headers.Count; i++)
                {
                    richTextBox1.Text += web.Headers[i].ToString();
                    richTextBox1.Text += Environment.NewLine;
                }
                
            }*/

            WebClient webClient = new WebClient();
            byte[] vs = webClient.DownloadData(new Uri("https://jsonformatter.curiousconcept.com/"));
            richTextBox1.Text = System.Text.Encoding.UTF8.GetString(vs).TrimEnd('\0');
        }
    }
}
