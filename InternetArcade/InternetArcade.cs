using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using InternetArcade.Classes;

namespace InternetArcade
{
    public partial class InternetArcade : Form
    {
        #region Definitions
        //Define Toolstrip Items
        private readonly ToolStrip adrBar = new ToolStrip();
        private readonly StatusBar errorLbl = new StatusBar();
        private readonly StatusBar loadingLbl = new StatusBar();
        private readonly ToolStripButton backBtn = new ToolStripButton();
        private readonly ToolStripButton fwdBtn = new ToolStripButton();
        private readonly ToolStripButton refreshBtn = new ToolStripButton();
        private readonly ToolStripSpringComboBox adrBarTextBox = new ToolStripSpringComboBox();
        private readonly ToolStripButton menuBtn = new ToolStripButton();
        //Define Browser
        public ChromiumWebBrowser browser;
        private GlobalKeyboardHook gHook;
        private string currentTitle;
        string Url = string.Empty;
        XmlDocument settings = new XmlDocument();
        public static InternetArcade Instance;
        #endregion

        public InternetArcade()
        {
            Instance = this;
            InitializeComponent();
            Text = "Internet Arcade";
            TabDragger DragTabs = new TabDragger(customTabControl, TabForm.TabDragBehavior.TabDragOut);
            if (this.adrBar.Text != "") Url = this.adrBar.Text;

            var settings = new CefSettings();
            Cef.Initialize(settings);
            SetFormTitle(null);
        }

        #region Form Functions
        private void InternetArcade_Load(object sender, EventArgs e)
        {
            //Add Toolstrip Items and Functions
            Controls.Add(adrBar);
            adrBar.Dock = DockStyle.Top;
            adrBar.Items.Add(backBtn);
            backBtn.Text = "Back";
            backBtn.Click += BackBtn_Click;
            adrBar.Items.Add(fwdBtn);
            fwdBtn.Text = "Forward";
            fwdBtn.Click += FwdBtn_Click;
            adrBar.Items.Add(refreshBtn);
            refreshBtn.Text = "Refresh";
            refreshBtn.Click += RefreshBtn_Click;
            adrBar.Items.Add(adrBarTextBox);
            adrBarTextBox.KeyDown += adrBarTextBox_KeyDown;
            //adrBar.Items.Add(new ToolStripButton("Add to Favorites"));
            adrBar.Items.Add(menuBtn);
            menuBtn.Text = "Menu";
            menuBtn.Click += MenuBtn_Click;
            Controls.Add(errorLbl);
            errorLbl.Dock = DockStyle.Bottom;
            Controls.Add(loadingLbl);
            loadingLbl.Dock = DockStyle.Bottom;

            //Load Browser Functions
            gHook = new GlobalKeyboardHook();
            gHook.KeyDown += new KeyEventHandler(InternetArcade_KeyDown);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gHook.HookedKeys.Add(key);
            }
            gHook.hook();
            addNewTab();
        }


        private void InternetArcade_FormClosing(object sender, FormClosingEventArgs e)
        {
            gHook.unhook();
            Cef.Shutdown();
        }

        private void InternetArcade_KeyDown(object sender, KeyEventArgs e)
        {
            if (ContainsFocus)
            {
                if (e.KeyValue == 116)
                {
                    new Thread(() =>
                    {
                        // execute this on the gui thread. (winforms)
                        this.Invoke(new Action(delegate
                        {
                            getCurrentBrowser().Reload();
                        }));
                    }).Start();
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Control".ToUpper()) >= 1
                    && e.KeyValue == 9)
                {
                    if (customTabControl.SelectedIndex < customTabControl.TabPages.Count - 1)
                    {
                        customTabControl.SelectedIndex = customTabControl.SelectedIndex + 1;
                    }
                    else customTabControl.SelectedIndex = 0;
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Control".ToUpper()) >= 1
                        && e.KeyValue == 84)
                {
                    addNewTab();
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Control".ToUpper()) >= 1
                    && e.KeyValue == 115)
                {
                    closeTab();
                }

                if (e.Control && e.Shift && e.KeyCode == Keys.T)
                {
                    restoreTab();
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Control".ToUpper()) >= 1
                    && e.KeyValue == 78)
                {
                    newWindow();
                }

                if (e.KeyValue == 122)
                {
                    fullScreen();
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Control".ToUpper()) >= 1
                    && e.KeyValue == 80)
                {
                    getCurrentBrowser().Print();
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Control".ToUpper()) >= 1
                    && e.KeyValue == 70)
                {
                    getCurrentBrowser().FindForm();
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("ALT".ToUpper()) >= 0
                    && e.KeyValue == 115) //F4 = 115
                {
                    //ALT + F4
                    new Thread(() =>
                    {
                        Invoke(new Action(delegate
                        {
                            Cef.Shutdown();
                            Close();
                        }));
                    }).Start();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }

                if (e.KeyData.ToString().ToUpper().IndexOf("Alt".ToUpper()) >= 0
                   && e.KeyValue == 37)//Left Arrow = 37
                {
                    //ALT + Left Arrow 
                    new Thread(() =>
                    {
                        // execute this on the gui thread. (winforms)
                        this.Invoke(new Action(delegate
                        {
                            previousTab();
                        }));

                    }).Start();
                }
                else if (e.KeyData.ToString().ToUpper().IndexOf("ALT".ToUpper()) >= 0
                  && e.KeyValue == 39)//Forward Arrow = 39
                {
                    //ALT + Right Arrow
                    new Thread(() =>
                    {
                        // execute this on the gui thread. (winforms)
                        this.Invoke(new Action(delegate
                        {
                            nextTab();
                        }));
                    }).Start();
                }
            }
        }


        #endregion

        #region Methods
        private void addNewTab()
        {
            TabPage tpage = new TabPage();
            tpage.BorderStyle = BorderStyle.Fixed3D;
            customTabControl.TabPages.Add(tpage);
            browser = new ChromiumWebBrowser("google.com");
            tpage.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            customTabControl.SelectTab(tpage);
            browser.LoadingStateChanged += OnBrowserLoadingStateChanged;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.AddressChanged += OnBrowserAddressChanged;
            browser.StatusMessage += OnBrowserStatusMessage;
            browser.LoadError += OnBrowserLoadError;
        }

        private void closeTab()
        {
            if (customTabControl.TabCount != 1)
            {
                customTabControl.TabPages.RemoveAt(customTabControl.SelectedIndex);
            }
            else
            {
                Close();
            }
        }

        private void restoreTab()
        {

        }

        private void fullScreen()
        {
            if (!(this.FormBorderStyle == FormBorderStyle.None && this.WindowState == FormWindowState.Maximized))
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                this.TopMost = true;
                //linkBar.Visible = false;
                adrBar.Visible = false;
                //favoritesPanel.Visible = false;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                adrBar.Visible = (settings.DocumentElement.ChildNodes[1].Attributes[0].Value.Equals("True"));
                //linkBar.Visible = (settings.DocumentElement.ChildNodes[2].Attributes[0].Value.Equals("True"));
                //favoritesPanel.Visible = (settings.DocumentElement.ChildNodes[3].Attributes[0].Value.Equals("True"));
            }
        }

        private void newWindow()
        {
            (new InternetArcade()).Show();
        }

        private void previousTab()
        {
            getCurrentBrowser().Back();
        }

        private void nextTab()
        {
            getCurrentBrowser().Forward();
        }

        #endregion

        #region Tab URI
        private void OnBrowserLoadError(object sender, LoadErrorEventArgs e)
        {
            DisplayOutput("Load Error:" + e.ErrorCode + ";" + e.ErrorText);
            DisplayOutput("cef://web/error.html");
        }

        private void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => errorLbl.Text = output);
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs e)
        {
            this.InvokeOnUiThreadIfRequired(() => loadingLbl.Text = e.Value);
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs e)
        {
            InvokeIfNeeded(() =>
            {
                // if current tab
                if (sender == browser)
                {

                    adrBarTextBox.Text = e.Address;

                    SetCanGoBack(browser.CanGoBack);
                    SetCanGoForward(browser.CanGoForward);

                    SetTabTitle((ChromiumWebBrowser)sender, "Loading...");

                }

            });
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs e)
        {
            InvokeIfNeeded(() => 
            {
                SetTabTitle((ChromiumWebBrowser)sender, e.Title);
            });
        }

        private void OnBrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            SetCanGoBack(e.CanGoBack);
            SetCanGoForward(e.CanGoForward);
            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!e.CanReload));
        }
        public void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        private void SetTabTitle(ChromiumWebBrowser sender, string text)
        {

            text = text.Trim();
            if (text == "" || text == "about:blank")
            {
                text = "New Tab";
            }

            browser.Tag = text;

            // get tab of given browser
            CustomTabControl tabStrip = (CustomTabControl)browser.Parent;
            tabStrip.Tag = text;

            // if current tab
            if (sender == browser)
            {
                SetFormTitle(text);
            }
        }

        private void SetFormTitle(string tabName)
        {
            if (tabName.CheckIfValid())
            {
                this.Text = tabName + " - Internet Arcade";
                currentTitle = tabName;
            }
            else
            {
                currentTitle = "New Tab";
            }
        }

        private void SetIsLoading(bool isLoading)
        {
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => fwdBtn.Enabled = canGoForward);
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backBtn.Enabled = canGoBack);
        }


        #endregion

        #region Address Bar
        public ChromiumWebBrowser getCurrentBrowser()
        {
            return (ChromiumWebBrowser)customTabControl.SelectedTab.Controls[0];
        }

        //Show Urls
        private void showAddress()
        {
            XmlDocument myXml = new XmlDocument();
            int i = 0;
            int num = int.Parse(settings.DocumentElement.ChildNodes[6].InnerText.ToString());
            foreach (XmlElement el in myXml.DocumentElement.ChildNodes)
            {
                if (num <= i++) break;
                else adrBar.Items.Add(el.GetAttribute("url").ToString());
            }
        }

        private void adrBarTextBox_DropDown(object sender, EventArgs e)
        {
            adrBar.Items.Clear();
            showAddress();
        }

        private void adrBarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadUrl(adrBarTextBox.Text);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            if ((e.KeyCode == Keys.Back) && e.Control)
            {
                e.SuppressKeyPress = true;
                int selStart = adrBarTextBox.SelectionStart;
                while (selStart > 0 && adrBarTextBox.Text.Substring(selStart - 1, 1) == " ")
                {
                    selStart--;
                }
                int prevSpacePos = -1;
                if (selStart != 0)
                {
                    prevSpacePos = adrBarTextBox.Text.LastIndexOf(' ', selStart - 1);
                }
                adrBarTextBox.Select(prevSpacePos + 1, adrBarTextBox.SelectionStart - prevSpacePos - 1);
                adrBarTextBox.SelectedText = "";
            }
        }

        private void LoadUrl(string url)
        {
            Uri outUri;
            string newUrl = url;
            string urlToLower = url.Trim().ToLower();

            Uri.TryCreate(url, UriKind.Absolute, out outUri);

            if (!(urlToLower.StartsWith("http") /*|| urlToLower.StartsWith(SchemeHandlerFactory.SchemeName) || urlToLower.StartsWith(SchemeHandlerFactory.SchemeNameTest)*/))
            {
                if (outUri == null || outUri.Scheme != Uri.UriSchemeFile) newUrl = "http://" + url;
            }

            if (urlToLower.StartsWith(SchemeHandlerFactory.SchemeName + ":") || urlToLower.StartsWith(SchemeHandlerFactory.SchemeNameTest + ":") ||
                (Uri.TryCreate(newUrl, UriKind.Absolute, out outUri)
            && ((outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps) && newUrl.Contains(".") || outUri.Scheme == Uri.UriSchemeFile)))
            {
                getCurrentBrowser().Load(newUrl);
            }
            else
            {
                string searchURL = "https://www.google.com/webhp?#q=";
                getCurrentBrowser().Load(searchURL + HttpUtility.UrlEncode(url));
            }
        }

        #endregion

        #region Button Events
        private void MenuBtn_Click(object sender, EventArgs e)
        {
            (new MenuList()).Show();
        }
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            browser.Refresh();
        }

        private void FwdBtn_Click(object sender, EventArgs e)
        {
            browser.Forward();
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            browser.Back();
        }
        #endregion
    }
}