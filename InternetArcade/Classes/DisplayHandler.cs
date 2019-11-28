using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using System.Collections.Generic;
using System.Windows.Forms;

namespace InternetArcade
{
    public class DisplayHandler : IDisplayHandler
    {
        private Control parent;
        private Form fullscreenForm;

        void IDisplayHandler.OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
        {
            var WebBrowser = (ChromiumWebBrowser)browserControl;
            WebBrowser.InvokeOnUiThreadIfRequired(() =>
            {
                if (fullscreen)
                {
                    parent = WebBrowser.Parent;
                    parent.Controls.Remove(WebBrowser);
                    fullscreenForm = new Form();
                    fullscreenForm.FormBorderStyle = FormBorderStyle.None;
                    fullscreenForm.WindowState = FormWindowState.Maximized;
                    fullscreenForm.Controls.Add(WebBrowser);
                    fullscreenForm.ShowDialog(parent.FindForm());
                }
                else
                {
                    fullscreenForm.Controls.Remove(WebBrowser);
                    parent.Controls.Add(WebBrowser);
                    fullscreenForm.Close();
                    fullscreenForm.Dispose();
                    fullscreenForm = null;
                }
            });
        }

        public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
        {
        }

        public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
        {
        }

        public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
        {
        }

        bool IDisplayHandler.OnTooltipChanged(IWebBrowser browserControl, string text)
        {
            return false;
        }

        public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
        {
        }

        bool IDisplayHandler.OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return false;
        }
    }
}
