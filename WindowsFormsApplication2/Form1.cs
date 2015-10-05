using System;
using System.IO;
using System.Net;
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

        public async Task<string> DownloadFile(string url)
        {

            try
            {
                HttpWebRequest QSCbox = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse qscBox = (HttpWebResponse)QSCbox.GetResponse();
                //获取文件名
                var name = GetFileInfo(qscBox);

                string filename = GetFilePath(name);
                if (filename == "false")
                {
                    return "false";
                }


                long totalBytes = qscBox.ContentLength;
                progressBar1.Maximum = (int)totalBytes / 1024 +1;
                progressBar1.Minimum = 1;


                if (qscBox.ContentLength != 0)
                {
                    Stream st = qscBox.GetResponseStream();
                    Stream so = new FileStream(filename, FileMode.Create);
                    long totalDownloadedByte = 0;
                    byte[] by = new byte[1024];

                    int osize = await st.ReadAsync(by, 0, @by.Length);

                    while (osize > 0)
                    {
                        if (!dl)
                        {
                            textBox1.Text = "";
                            progressBar1.Value = 1;
                            return "cancle";
                        }
                        totalDownloadedByte = osize + totalDownloadedByte;

                        await so.WriteAsync(by, 0, osize);
                        osize = await st.ReadAsync(by, 0, @by.Length);
                        progressBar1.PerformStep();

                    }
                    MessageBox.Show("下载成功");
                    progressBar1.PerformStep();
                    so.Close();
                    st.Close();

                    return "success";
                }
                MessageBox.Show("文件不存在该文件！");
                return "failed";
            }
            catch (Exception e)
            {
                MessageBox.Show("未知错误");
                return "下载文件失败！" + e;
            }
        }

        public string GetFileInfo(HttpWebResponse dofile)
        {
            var txt = dofile.Headers.ToString();
            var lastModified = dofile.LastModified;
            var contentLength = (dofile.ContentLength / 1024).ToString() + "KB";
            var server = dofile.Server;
            //获取文件名
            var n1 = txt.IndexOf("name=", StringComparison.Ordinal) + 6;
            var n2 = txt.IndexOf("\"; fil", StringComparison.Ordinal);
            var name = txt.Substring(n1, n2 - n1);

            textBox2.Text = "文件名：" + name + "\r\n" +
                            "文件大小：" + contentLength + "\r\n" +
                            "上传时间：" + lastModified + "\r\n" +
                            "服务器版本：" + server + "\r\n";
            return name;
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            var ma = textBox1.Text;
            var url = "http://box.zjuqsc.com/-" + ma;
            button2.Enabled = true;
            dl = true;

            await DownloadFile(url);


        }

        public bool dl = true;
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
                dl = false;
            }

            // MessageBox.Show("it works");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://box.zjuqsc.com/");
        }
    }
}
