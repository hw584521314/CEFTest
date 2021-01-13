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
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
/*
服务器端程序通信
1.上传结果到服务器
2.一旦服务器接到结果，通知所有客户端更新
3.服务器下发题目到客户端

*/
namespace CEFTest
{
    public partial class Form1 : Form, ILifeSpanHandler
    {
        public ChromiumWebBrowser chromeBrowser;
        public string connectString;
        public Form1()
        {
            InitializeComponent();
            InitBrower();
            EventHandler eh = new EventHandler();


            eh.EventArrived += OnJavascriptEventArrived;
            // Use the default of camelCaseJavascriptNames
            // .Net methods starting with a capitol will be translated to starting with a lower case letter when called from js
            chromeBrowser.JavascriptObjectRepository.Register("boundEvent", eh, isAsync: true, options: BindingOptions.DefaultBinder);

        }



        public void InitBrower()
        {
            CefSettings setting = new CefSettings();
            // 设置语言
            setting.Locale = "zh-CN";

            //cef设置userAgent
            setting.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
            //配置浏览器路径
            //setting.BrowserSubprocessPath = @"x64\CefSharp.BrowserSubprocess.exe";
            // By default CEF uses an in memory cache, to save cached data e.g. to persist cookies you need to specify a cache path
            // NOTE: The executing user must have sufficient privileges to write to this folder.
            setting.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"); ;

            Cef.Initialize(setting, performDependencyCheck: true, browserProcessHandler: null);
            chromeBrowser = new ChromiumWebBrowser(@"http://www.luogu.org");
            chromeBrowser.FrameLoadEnd += OnFrameLoadEnd;
            chromeBrowser.LifeSpanHandler = this;
            // Add it to the form and fill it to the form window.
            Panel p = this.Controls["panel1"] as Panel;
            p.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
        }

        private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            chromeBrowser.ShowDevTools();
            //ListenForEvent(e.Frame, "su", "click");
        }
        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            //Set newBrowser to null unless you're attempting to host the popup in a new instance of ChromiumWebBrowser
            newBrowser = null;
            chromeBrowser.Load(targetUrl);
            return true; //Return true to cancel the popup creation
        }
        public void OnBeforeClose(    IWebBrowser browserControl,    IBrowser browser)
        {

        }
        public void OnAfterCreated(    IWebBrowser browserControl,    IBrowser browser)
        {

        }
        public bool DoClose(    IWebBrowser browserControl,    IBrowser browser)
        {
            return false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SubmitAnswer(chromeBrowser.GetBrowser().GetFrame(null),  "click");
        }

        public void SubmitAnswer(IFrame frame, string eventName)
        {
            if (frame == null)
            {
                throw new ArgumentException("An IFrame instance is required.", "frame");
            }

            // Adds a click event listener to a DOM element with the provided
            // ID. When the element is clicked the ScriptedMethodsBoundObject's
            // RaiseEvent function is invoked. This is one way to get
            // asynchronous events from the web page. Typically though the web
            // page would be aware of window.boundEvent.RaiseEvent and would
            // simply raise it as needed.
            //
            // Scripts should be minified for production builds. The script
            // could also be read from a file...
            var script =
                @"(async function ()
                {
                    await CefSharp.BindObjectAsync('boundEvent');                    
                    console.log('submit');
                    //var elem = document.getElementById('##ID##');
                    var elem=document.querySelector('#app > div.main-container > main > div > section.main > section > div > div > button');
                    if(!elem){
                    console.log('cant get the element');
                    return;
                    }
                    
                    code=document.querySelector('#app > div.main-container > main > div > section.main > section > div > div > div.editor.ace_editor.ace-clouds > div.ace_scroller > div > div.ace_layer.ace_text-layer');
                    if (window.boundEvent){
                        window.boundEvent.raiseEvent('##EVENT##', {content: code.innerText}); 
                    }  
                    elem.click();                 
                    /*elem.addEventListener('##EVENT##', function(e){
                        if (!window.boundEvent){
                            console.log('window.boundEvent does not exist.');
                            return;
                        }                        
                        //NOTE RaiseEvent was converted to raiseEvent in JS (this is configurable when registering the object)
                        
                    });*/
                    
                })();";

            // For simple inline scripts you could use String.Format() but
            // beware of braces in the javascript code. If reading from a file
            // it's probably safer to include tokens that can be replaced via
            // regex.
            //script = Regex.Replace(script, "##ID##", className);
            script = Regex.Replace(script, "##EVENT##", eventName);

            frame.ExecuteJavaScriptAsync(script);
        }

        private void OnJavascriptEventArrived(string eventName, object eventData)
        {
            switch (eventName)
            {
                case "click":
                    {
                        
                        foreach (var item in (IDictionary<string, object>)eventData)
                        {
                            if (item.Key=="content")
                            {
                                MessageBox.Show(item.Value.ToString());
                            }
                        }                        
                        
                        break;
                    }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //获取数据库连接，后面可以通过MQTT来获取配置
            connectString = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            //订阅MQTT的某个topic
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(chromeBrowser.CanGoBack==true)
            {
                chromeBrowser.Back();
            }            
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //
            var script =
                @"(function ()
                {
                    
                    var elem=document.querySelector('#app > div.main-container > main > div > section.side > div > div.info-rows > div:nth-child(2) > span:nth-child(2) > span');
                    if(!elem){
                    alert('还未判分，重新提交');
                    return '-1';
                    }
                    if(elem.innerText=='Accepted')
                    {
                         return '100';
                    }
                    else if(elem.innerText=='Unaccepted')
                    {
                        score=document.querySelector('#app > div.main-container > main > div > section.side > div > div.info-rows > div:nth-child(3) > span:nth-child(2) > span > span');
                        if(!score)
                        {return '0';}
                        return score.innerText;
                    }
                    return '0';
                })();";

            JavascriptResponse response = await chromeBrowser.EvaluateScriptAsync(script);
            if(response.Success==true)
            {//和服务器端程序通信，1
                string result = response.Result as string;
                label1.Text = result;
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            chromeBrowser.Refresh();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);
            if(info.Item!=null)
            {
                var url=string.Format("https://www.luogu.com.cn/problem/{0}", info.Item.Text);
                chromeBrowser.Load(url);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                
            }
        }
    }



    class EventHandler
    {
        public event Action<string, object> EventArrived;

        /// <summary>
        /// This method will be exposed to the Javascript environment. It is
        /// invoked in the Javascript environment when some event of interest
        /// happens.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="eventData">Data provided by the invoker pertaining to the event.</param>
        /// <remarks>
        /// By default RaiseEvent will be translated to raiseEvent as a javascript function.
        /// This is configurable when calling RegisterJsObject by setting camelCaseJavascriptNames;
        /// </remarks>
        public void RaiseEvent(string eventName, object eventData = null)
        {
            if (EventArrived != null)
            {
                EventArrived(eventName, eventData);
            }
        }


    }
}
