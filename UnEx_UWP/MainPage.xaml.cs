using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UnEx_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string logPath = @"C:\Logs\tracker.log";
        List<Exception> tracker = new List<Exception>();

        public MainPage()
        {
            AppCenter.LogLevel = LogLevel.Verbose;

            AppCenter.Start("8a339cec-4fdb-4718-b866-29870cde5f91",
                  typeof(Analytics), typeof(Crashes));

            this.InitializeComponent();
            
            AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Crashes.TrackError(e.Exception);
            //ThrowMeSomeException(e.Exception);
            //tracker.Add(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            tracker.Add((Exception)e.ExceptionObject);

            foreach (Exception ex in tracker)
            {
                WriteExceptionTracker(ex);
            }
        }

        private void WriteExceptionTracker(Exception exception)
        {
            if (exception.InnerException != null)
            {
                WriteExceptionTracker(exception.InnerException);
            }
            else
            {
                Crashes.TrackError(exception);
            }
        }

        private void btnHandledException_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = new Exception("btnHandledException_Click");

            try
            {
                //throw exception;
                WriteExceptionTracker(exception);
            }
            catch
            {
                //Alow the default handler the take this CurrentDomain_FirstChanceException
            }
        }

        private void btnUnhandledException_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = new Exception("btnUnhandledException_Click");
            throw exception;
        }

        private void btnUnhandledExceptionPropagate_Click(object sender, RoutedEventArgs e)
        {
            Exception exception = new Exception("btnUnhandledExceptionPropagate_Click");
            try
            {
                throw exception;
            }
            catch
            {
                //Alow the default handler the take this CurrentDomain_FirstChanceException
            }
        }



        private void btnTaskException_Click(object sender, RoutedEventArgs e)
        {
            Task task = Task.Run(() => { level(false); }).ContinueWith((x) =>
            {
                //ThrowMeSomeException(x.Exception);                
            });
        }

        private void btnReadFirstChanceExceptions_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "";
            int i = 1;
            foreach (Exception ex in tracker)
            {
                textBlock.Text += i.ToString();
                textBlock.Text += System.Environment.NewLine;
                textBlock.Text += ex.Message;
                textBlock.Text += System.Environment.NewLine;
                textBlock.Text += ex.StackTrace;
                textBlock.Text += System.Environment.NewLine;
                i++;
            }
        }

        private void level(bool handled)
        {
            System.Threading.Thread.Sleep(5000);
            level1(handled);
        }

        private void level0(bool handled)
        {
            level1(handled);
        }

        private void level1(bool handled)
        {
            level3(handled);
        }

        private void level3(bool handled)
        {
            Exception exception = new Exception("Level3Exception");
            throw exception;

        }
    }
}
