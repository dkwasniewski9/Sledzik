using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using static SledzikLibrary.AppLibrary;

namespace dkwasniewskiApp
{

    public partial class MainWindow : Window
    {
        ServiceController sc;
        EventLog log;
        string trackedApp;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                sc = new ServiceController(ServiceName, Environment.MachineName);
                if (sc.Status != ServiceControllerStatus.Running) sc.Start(new string[] { ConfigurationManager.AppSettings["TrackedApp"] });
                log = new EventLog("Sledzik Logs", ".", "SledzikSource");
                if (log.Entries.Count == 0) trackedApp = "Brak wpisów";
                else trackedApp = log.Entries[log.Entries.Count - 1].Message.Split('\n').ElementAt(0);
                TrackedAppName.Content = "Śledzona aplikacja: " + trackedApp;
                TrackedAppInfo.Content = getStatsAboutTrackedApp(trackedApp, log.Entries);
                drawGraph("CPU", graph, log.Entries, trackedApp);
                drawGraph("RAM", graph, log.Entries, trackedApp);
            }
            catch (Exception ex)
            {
                TrackedAppInfo.Content = ex.Message;
            }
        }

        private void TrackedAppFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Executable Files | *.exe";
            fileDialog.InitialDirectory = "C:";
            fileDialog.Title = "Wybierz aplikacje do śledzenia";
            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                var fileName = fileDialog.SafeFileName;
                ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control,
                                                                                    Environment.MachineName,
                                                                                    ServiceName);
                scp.Assert();
                sc.Refresh();

                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                trackedApp = System.IO.Path.GetFileNameWithoutExtension(fileName);
                TrackedAppName.Content = "Śledzona aplikacja: " + trackedApp;
                sc.Start(new string[] { trackedApp });
                graph.Children.Clear();
                drawGraph("CPU", graph, log.Entries, trackedApp);
                drawGraph("RAM", graph, log.Entries, trackedApp);
                TrackedAppInfo.Content = getStatsAboutTrackedApp(trackedApp, log.Entries);
            }

        }



        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            graph.Children.Clear();
            TrackedAppInfo.Content = getStatsAboutTrackedApp(trackedApp, log.Entries);
            TrackedAppName.Content = "Śledzona aplikacja: " + trackedApp;
            drawGraph("CPU", graph, log.Entries, trackedApp);
            drawGraph("RAM", graph, log.Entries, trackedApp);
        }


        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            log.Clear();
            if (log.Entries.Count == 0) TrackedAppName.Content = "Brak wpisów";
            TrackedAppInfo.Content = getStatsAboutTrackedApp(trackedApp, log.Entries);
            graph.Children.Clear();
            drawGraph("CPU", graph, log.Entries, trackedApp);
            drawGraph("RAM", graph, log.Entries, trackedApp);
        }
    }
}
