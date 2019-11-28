using CefSharp;
using System;
using System.Threading.Tasks;

namespace InternetArcade
{
    internal class MenuHandler : IContextMenuHandler
    {
        private const int Back = 100;
        private const int Forward = 101;
        private const int Stop = 104;
        private const int Reload = 102;
        private const int Find = 130;
        private const int Print = 131;
        private const int SaveAs = 26500;
        private const int ZoomIn = 26502;
        private const int ZoomOut = 26503;
        private const int ViewSource = 132;
        private const int OpenInNewTab = 26504;
        private const int OpenNewTab = 26505;
        private const double ZoomIncrement = 1.0;

        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //To disable the menu then call clear
            // model.Clear();

            model.Clear();

            //Removing existing menu item
            //bool removed = model.Remove(CefMenuCommand.ViewSource); // Remove "View Source" option

            //Add new custom menu items
            model.AddItem((CefMenuCommand)Back, "Back");
            model.AddItem((CefMenuCommand)Forward, "Forward");
            model.AddItem((CefMenuCommand)Reload, "Reload");
            model.AddItem((CefMenuCommand)Stop, "Stop");
            model.AddSeparator();
            model.AddItem((CefMenuCommand)OpenNewTab, "New Tab");
            model.AddItem((CefMenuCommand)OpenInNewTab, "Open in New Tab");
            model.AddSeparator();
            model.AddItem((CefMenuCommand)Find, "Find");
            model.AddItem((CefMenuCommand)Print, "Print");
            model.AddItem((CefMenuCommand)SaveAs, "Save As");
            model.AddSeparator();
            model.AddItem((CefMenuCommand)ZoomIn, "Zoom In");
            model.AddItem((CefMenuCommand)ZoomOut, "Zoom Out");
            model.AddItem((CefMenuCommand)ViewSource, "View Source");
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            if ((int)commandId == Back)
            {
                browser.GoBack();
            }

            if ((int)commandId == Forward)
            {
                browser.GoForward();
            }

            if ((int)commandId == Stop)
            {
                browser.StopLoad();
            }

            if ((int)commandId == Reload)
            {
                browser.Reload();
            }

            if ((int)commandId == OpenNewTab)
            {

            }

            if ((int)commandId == OpenInNewTab)
            {

            }

            if ((int)commandId == Find)
            {

            }

            if ((int)commandId == Print)
            {
                browser.Print();
            }

            if ((int)commandId == SaveAs)
            {

            }

            if ((int)commandId == ZoomIn)
            {
                var control = browser;
                if (control != null)
                {
                    var task = browser.GetZoomLevelAsync();
                    task.ContinueWith(previous =>
                    {
                        if (previous.Status == TaskStatus.RanToCompletion)
                        {
                            var currentLevel = previous.Result;
                            browser.SetZoomLevel(currentLevel + ZoomIncrement);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }
            }

            if ((int)commandId == ZoomOut)
            {
                var control = browser;
                if (control != null)
                {
                    var task = browser.GetZoomLevelAsync();
                    task.ContinueWith(previous =>
                    {
                        if (previous.Status == TaskStatus.RanToCompletion)
                        {
                            var currentLevel = previous.Result;
                            browser.SetZoomLevel(currentLevel - ZoomIncrement);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }
            }

            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}