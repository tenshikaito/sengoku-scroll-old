using Library.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var option = new Option();
                var wording = await FileHelper.loadCharset("zh-tw");

                AppDomain.CurrentDomain.UnhandledException += onException;

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += onApplicationException;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(option, wording));
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
