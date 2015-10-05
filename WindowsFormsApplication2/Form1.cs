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

        public async Task<string> DownloadFile(string URL)
        {

            try
            {
                HttpWebRequest QSCbox = (HttpWebRequest)WebRequest.Create(URL);
                HttpWebResponse qscBox = (HttpWebResponse)QSCbox.GetResponse();
                //获取文件名
                var tem = qscBox.Headers.ToString();
                var n1 = tem.IndexOf("name=", StringComparison.Ordinal) + 6;
                var n2 = tem.IndexOf("\"; fil", StringComparison.Ordinal);
                var name = tem.Substring(n1, n2 - n1);
                string filename = GetFilePath(name);



                long totalBytes = qscBox.ContentLength;
                progressBar1.Maximum = (int)totalBytes / 1024;
                progressBar1.Minimum = 1;
                if (qscBox.ContentLength != 0)
                {
                    // MessageBox.Show("文件存在");  
                    Stream st = qscBox.GetResponseStream();
                    Stream so = new FileStream(filename, FileMode.Create);
                    long totalDownloadedByte = 0;
                    byte[] by = new byte[1024];

                    int osize = await st.ReadAsync(by, 0, @by.Length);
                    //  int osize = st.ReadAsync(by, 0, @by.Length);
                    while (osize > 0)
                    {
                        if (!dl)
                        {
                            textBox1.Text = "";
                            progressBar1.Value = 1;
                            return "cancle";
                        }
                        totalDownloadedByte = osize + totalDownloadedByte;
                        // so.Write(by, 0, osize);
                        await so.WriteAsync(by, 0, osize);
                        osize = await st.ReadAsync(by, 0, @by.Length);
                        progressBar1.PerformStep();
                        //   osize = st.Read(by, 0, @by.Length);
                    }
                    MessageBox.Show("下载成功");
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

        
        private async void button1_Click(object sender, EventArgs e)
        {
            var ma = textBox1.Text;
            var url = "http://box.zjuqsc.com/-" + ma;
            button2.Enabled = true;
            //    Thread downLoadThread;
            //    downLoadThread = new Thread(new ThreadStart(TDownloadFile));
            //    downLoadThread.Priority = ThreadPriority.Highest;
            //    downLoadThread.Start();
            ////    downLoadThread.Join();
            //  GetFilePath("");

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
            return "fales";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("你确定要取消吗？", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes )
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
