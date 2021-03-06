﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
        }

        public long TotalLenth = 0;
        public async Task<string> DownloadFile(string url)
        {

            try
            {
                HttpWebRequest qsCbox = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse qscBox = (HttpWebResponse)qsCbox.GetResponse();
                //获取文件名
                var name = GetFileInfo(qscBox);

                string filename = GetFilePath(name);
                if (filename == "false")
                {
                    DownLoadComplete(2);
                    return "false";
                }


                long totalBytes = qscBox.ContentLength;

                progressBar1.Maximum = (int) totalBytes / 6147;
                progressBar1.Minimum = 0;
                progressBar1.Step = 1;
                label3.Visible = true;

                if (totalBytes != 0)
                {
                    timer1.Start();
                    Stream st = qscBox.GetResponseStream();
                    Stream so = new FileStream(filename, FileMode.Create);

                    byte[] by = new byte[8196];

                    int osize = await st.ReadAsync(by, 0, @by.Length);
                    var n = 0;
                    while (osize > 0)
                    {

                        if (!Dl)
                        {
                            DownLoadComplete(3);
                            return "cancle";
                        }
                        await so.WriteAsync(by, 0, osize);
                        osize = await st.ReadAsync(by, 0, @by.Length);
                        progressBar1.PerformStep();
                        TotalLenth += 8196;
                       // progressBar1.Value = (int)(TotalLenth / totalBytes * 100);
                     //   progressBar1.Increment((int) (8196/totalBytes));
                        n++;
                    }
                    DownLoadComplete(1, filename, totalBytes);

                    so.Close();
                    st.Close();

                    return "success";
                }
                MessageBox.Show("文件不存在该文件！");
                return "failed";
            }
            catch (Exception e)
            {
                MessageBox.Show("错误原因：" + e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DownLoadComplete(2);
                return "下载文件失败！" + e;
            }
        }

        public void GetSpeed(long length)
        {
            label3.Text = "\r\n" + "下载速度：" + length * 5 / 1024.0 + "KB/s";
        }


        public void DownLoadComplete(int num, string filename, long contentLength)
        {
            timer1.Stop();
            textBox2.Text += "--------------------" + "\r\n" + "总共用时：" + _time / 10.0 + "s" + "\r\n" +
                "平均速度：" + Math.Round(contentLength / 1024 / (_time / 10.0), 2) + "KB/s" + "\r\n";
            var result = MessageBox.Show("下载成功，是否打开文件", "下载成功", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("Explorer.exe", filename);
            }
            button2.Enabled = false;
            // DownLoadComplete(2);

        }

        public void DownLoadComplete(int num)
        {
            if (num == 2)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                progressBar1.Value = 0;
                button2.Enabled = false;

            }
            else if (num == 3)
            {
                MessageBox.Show("取消成功");
                DownLoadComplete(2);

            }
        }


        public string GetFileInfo(HttpWebResponse dofile)
        {
            var txt = dofile.Headers.ToString();
            var lastModified = dofile.LastModified;
            var contentLength = dofile.ContentLength; // 1024).ToString() + "KB";
            var content = string.Empty;
            if (contentLength / 1024 > 1024)
            {
                content = Math.Round(contentLength / 1024.0 / 1024.0, 2) + "MB";
            }
            else
            {
                content = (contentLength / 1024) + "KB";
            }

            var server = dofile.Server;
            //获取文件名
            var n1 = txt.IndexOf("name=", StringComparison.Ordinal) + 6;
            var n2 = txt.IndexOf("\"; fil", StringComparison.Ordinal);
            var name = txt.Substring(n1, n2 - n1);

            textBox2.Text = "文件名：" + name + "\r\n" +
                            "文件大小：" + content + "\r\n" +
                            "上传时间：" + lastModified + "\r\n" +
                            "服务器版本：" + server + "\r\n";
            return name;
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            var ma = textBox1.Text.Trim();
            _time = 0;
            var url = "http://box.zjuqsc.com/-" + ma;
            button2.Enabled = true;
            Dl = true;
            await DownloadFile(url);

        }

        public bool Dl = true;


        /// <summary>
        /// 得到用户储存的地址，如果正确的话
        /// </summary>
        /// <param name="filename">要下载的文件名</param>
        /// <returns>返回最终的文件地址</returns>
        public string GetFilePath(string filename)
        {
            FileDialog filepath = new SaveFileDialog();
            filepath.FileName = filename;
            if (filepath.ShowDialog() == DialogResult.OK)
            {
                return filepath.FileName.ToString();
            }
            return "false";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("你确定要取消吗？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Dl = false;
            }

            // MessageBox.Show("it works");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://box.zjuqsc.com/");
        }


    
        private long _time = 0;
        public long LastLength = 0;
        private long len1 = 0;
        private long len2 = 0;

        /// <summary>
        /// 总时钟，同时计算下载速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            _time++;
            //奇偶分开，目的是降低刷新频率，同时提高数据的稳定性。
            if (_time%2 == 0)
                len1 = TotalLenth - LastLength;
                if (len1  !=0)
                    GetSpeed((len1+len2)/2 );
            else 
                len2 =TotalLenth -LastLength;
            LastLength = TotalLenth;
        }
    }
}
