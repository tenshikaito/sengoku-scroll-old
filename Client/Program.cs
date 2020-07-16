using Client.Model;
using Library.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            try
            {
                var option = new Option();
                var wording = await FileHelper.loadCharset("zh-tw");
                var players = await FileHelper.loadPlayer<List<PlayerInfo>>();

                AppDomain.CurrentDomain.UnhandledException += onException;

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += onApplicationException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(option, wording, players));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void onApplicationException(object sender, ThreadExceptionEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        private static void onException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.ExceptionObject);
        }
    }
}
