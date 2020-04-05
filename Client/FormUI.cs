using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class FormUI : Form
    {
        public ChromiumWebBrowser chromeBrowser;

        public FormUI()
        {
            InitializeChromium();
        }

        //初始化浏览器并启动
        public void InitializeChromium()
        {
            //需要添加此句代码，否则下面执行会报错
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);
            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("https://www.baidu.com") { Dock = DockStyle.Fill };
            // Add it to the form and fill it to the form window.
            this.Controls.Add(chromeBrowser);

            BrowserSettings browserSettings = new BrowserSettings();
            browserSettings.FileAccessFromFileUrls = CefState.Enabled;
            browserSettings.UniversalAccessFromFileUrls = CefState.Enabled;
            chromeBrowser.BrowserSettings = browserSettings;

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.WcfEnabled = true;
            chromeBrowser.JavascriptObjectRepository.Register("cefCustomObject", this, false);
        }

        //窗体关闭时，记得停止浏览器
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }        
    }
}
