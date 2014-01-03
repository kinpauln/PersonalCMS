using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace PersonalCMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            int paiMing = 0;

            string urlWord = HttpUtility.UrlEncode(TextBox1.Text, Encoding.GetEncoding("utf-8"));

            for (int i = 0; i < 500; i += 10)
            {
                string queryUrl = "http://www.baidu.com/s?lm=0&si=&rn=10&ie=gb2312&ct=0&wd=" + urlWord + "&pn=" + i.ToString() + "&ver=0&cl=3";
                WebRequest request = WebRequest.Create(queryUrl);
                WebResponse response = request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, Encoding.Default);
                string htmlCode = sr.ReadToEnd();
                resStream.Close();
                sr.Close();
                int startPos = htmlCode.IndexOf("<DIV id=ScriptDiv></DIV>");
                int endPos = htmlCode.IndexOf("<br clear=all>");
                string result = htmlCode.Substring(startPos, endPos - startPos);
                string[] info = SplitString(result, "百度快照");
                int ret = getBaiduPaiMing(info, TextBox2.Text.Trim());
                if (ret != 0)
                {
                    paiMing += ret;
                    break;
                }
                else
                    paiMing += 10;
            }
            if (paiMing == 0 || paiMing > 500)
                lbBaiduResult.Text = "没有排名";
            else
                lbBaiduResult.Text = "排名：" + paiMing.ToString();
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            int paiMing = 0;
            string urlWord = HttpUtility.UrlEncode(TextBox3.Text, Encoding.GetEncoding("gb2312"));
            for (int i = 0; i < 500; i += 10)
            {
                string queryUrl = "http://www.google.cn/search?hl=zh-CN&newwindow=1&q=" + urlWord + "&start=" + i.ToString() + "&sa=N";
                WebRequest request = WebRequest.Create(queryUrl);
                WebResponse response = request.GetResponse();
                Stream resStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(resStream, Encoding.Default);
                string htmlCode = sr.ReadToEnd();
                resStream.Close();
                sr.Close();
                int startPos = htmlCode.IndexOf("class=hd>搜索结果</h2>");
                int endPos = htmlCode.IndexOf(">相关服务：<td");
                string result = htmlCode.Substring(startPos, endPos - startPos);
                string[] info = SplitString(result, ">网页快照</a>");
                int ret = getGooglePaiMing(info, TextBox4.Text.Trim());
                if (ret != 0)
                {
                    paiMing += ret;
                    break;
                }
                else
                    paiMing += 10;
            }
            if (paiMing == 0 || paiMing > 500)
                lbGoogleResult.Text = "没有排名";
            else
                lbGoogleResult.Text = "排名：" + paiMing.ToString();
        }

        static public int getBaiduPaiMing(string[] info, string urlStr)
        {
            for (int i = 0; i < info.Length; i++)
            {
                int m = info[i].IndexOf("href=\"http://") + 6;
                int n = info[i].IndexOf("target=\"_blank\"><font") - 2;
                string ret = info[i].Substring(m, n - m);
                if (ret == urlStr)
                    return i + 1;
            }
            return 0;
        }
        static public int getGooglePaiMing(string[] info, string urlStr)
        {
            for (int i = 0; i < info.Length; i++)
            {
                int m = info[i].IndexOf("<h3 class=r><a href=\"http://") + 21;
                int n = info[i].IndexOf("target=_blank class=l", m);   // info[i].IndexOf("target=_blank class=l") - 2;  
                string ret = info[i].Substring(m, n - m - 2);

                if (ret == urlStr)
                    return i + 1;
            }
            return 0;
        }
        static public string[] SplitString(string str, string separator)
        {
            string tmp = str;
            Hashtable ht = new Hashtable();
            int i = 0;
            int pos = tmp.IndexOf(separator);
            while (pos != -1)
            {
                ht.Add(i, tmp.Substring(0, pos));
                tmp = tmp.Substring(pos + separator.Length);
                pos = tmp.IndexOf(separator);
                i++;
            }
            ht.Add(i, tmp);
            string[] array = new string[10];
            for (int j = 0; j < 10; j++)
                array[j] = ht[j].ToString();

            return array;
        } 
    }
}
