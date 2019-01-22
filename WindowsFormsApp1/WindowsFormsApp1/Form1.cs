using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Token Token;
        private UserInfo UserInfo;
        private Ranking ranking;
        private SettingForm settingForm;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            settingForm = new SettingForm();
            Token = new Token();
            UserInfo = new UserInfo();
            ranking = new Ranking();
            try
            {
                Authorization(new Uri("https://msapi.itstep.org/api/v1/auth/login"), ref Token);
                ThreadPool.QueueUserWorkItem(loading);
            }
            catch (System.Net.WebException)
            {
                button1.PerformClick();
            }
        }

        private void loading(object obj)
        {
            GetJson<UserInfo>(Token, new Uri("https://msapi.itstep.org/api/v1/settings/user-info"), ref UserInfo);
            using (WebClient web = new WebClient())
            {
                web.DownloadFileAsync(new Uri(UserInfo.photo), "User");
                web.DownloadFileCompleted += Web_DownloadFileCompleted;
            }
            GetJson<Ranking>(Token, new Uri("https://msapi.itstep.org/api/v1/dashboard/progress/leader-group-points"), ref ranking);
            this.Invoke(new Action(() => InitializeLabel()));
        }

        private void InitializeLabel()
        {
            label1.Text = UserInfo.full_name;
            label2.Text = UserInfo.group_name;
            label3.Text = (UserInfo.gaming_points[0].points + UserInfo.gaming_points[1].points).ToString();
            label3.Size = new Size(((label3.Text.Length) * (int)label3.Font.Size) + 16, 22);
            label4.Text = UserInfo.gaming_points[0].points.ToString();
            label4.Size = new Size(((label4.Text.Length) * (int)label4.Font.Size) + 16, 22);
            label5.Text = UserInfo.gaming_points[1].points.ToString();
            label5.Size = new Size(((label5.Text.Length) * (int)label5.Font.Size) + 16, 22);
            label6.Text = UserInfo.achieves_count.ToString();
            if (label6.Text.Length <= 1)
            {
                label6.Size = new Size(((label6.Text.Length) * (int)label6.Font.Size) + 25, 22);
            }
            else
            {
                label6.Size = new Size(((label6.Text.Length) * (int)label6.Font.Size) + 18, 22);
            }

            label7.Text = ranking.studentPosition.ToString();
            label9.Text = ranking.totalCount.ToString();
        }

        private void Web_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            pictureBox1.Image = Image.FromFile("User");
        }

        private void Authorization(Uri uri,ref Token token)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(uri);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";

            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(File.ReadAllBytes("../../data.json"),0, File.ReadAllBytes("../../data.json").Length);
            }

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    Token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(reader.ReadToEnd());
                }
            }
        }

        private void GetJson <T>(Token token, Uri uri, ref T obj)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(uri);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/json";

            webRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token.access_token);

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>((string)reader.ReadToEnd());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms[settingForm.Name] == null)
            {
                settingForm.Show();
                settingForm.Disposed += SettingForm_Disposed;
                settingForm.button1.Click += Button1_Click;
            }
            else
            {
                Application.OpenForms[settingForm.Name].Focus();
                System.Media.SystemSounds.Beep.Play();
            }
        }
        
        private void Button1_Click(object sender, EventArgs e)
        {
            this.OnLoad(new EventArgs());
        }

        private void SettingForm_Disposed(object sender, EventArgs e)
        {
            settingForm.Disposed -= SettingForm_Disposed;
            settingForm = new SettingForm();
            settingForm.button1.Click -= Button1_Click;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Application.OpenForms[settingForm.Name] != null)
            {
                settingForm.Dispose();
            }
        }
    }
}
