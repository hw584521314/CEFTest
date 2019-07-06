using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace CEFTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitBrower();           
            
            
        }
     
        public ChromiumWebBrowser chromeBrowser;
        public void InitBrower()
        {
            CefSettings setting = new CefSettings();
            // 设置语言
            setting.Locale = "zh-CN";
            //cef设置userAgent
            setting.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
            //配置浏览器路径
            //setting.BrowserSubprocessPath = @"x64\CefSharp.BrowserSubprocess.exe";
            Cef.Initialize(setting, performDependencyCheck: true, browserProcessHandler: null);
            chromeBrowser = new ChromiumWebBrowser(@"C:\Users\huangwei\Documents\Visual Studio 2015\Projects\CEFTest\CEFTest\index.html");
            // Add it to the form and fill it to the form window.
            Panel p = this.Controls["panel1"] as Panel;
            p.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string content = "<h1>this is another content</p>";
            chromeBrowser.ExecuteScriptAsync(string.Format("document.getElementById('container').innerHTML='{0}'", textBox1.Text));
        }
    }
}
