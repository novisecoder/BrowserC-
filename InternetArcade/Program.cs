using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InternetArcade
{
    static class Program
    {
        public static void Init()
        {
            var settings = new CefSettings
            {
                LogSeverity = LogSeverity.Verbose,
                LogFile = "debug.log",
                RemoteDebuggingPort = 8088,
            };
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InternetArcade());
        }
    }
}
