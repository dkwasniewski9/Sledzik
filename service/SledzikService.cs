using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using static SledzikLibrary.ServiceLibrary;
using Timer = System.Timers.Timer;

namespace SledzikService
{
    public class SledzikService : ServiceBase
    {
        public string trackedApp;
        EventLog log;
        private static Timer timer;
        private static double threadNumber;
        public SledzikService()
        {
            threadNumber = Convert.ToDouble(Environment.ProcessorCount);
            this.ServiceName = ServiceName;
        }
        protected override void OnStart(string[] args)
        {
            if (args.Length == 0)
            {
                trackedApp = ConfigurationManager.AppSettings["TrackedApp"];
            }
            else
            {
                trackedApp = args[0];
                changeTrackedAppName(trackedApp);
            }
            string sourceName = ConfigurationManager.AppSettings["SourceName"];
            string logsName = ConfigurationManager.AppSettings["LogsName"];
            try
            {
                CreateLog(sourceName, logsName);
            }
            catch (InvalidOperationException ex)
            {
                GetLog(sourceName, logsName);
            }
            timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += OnTimerElapsed;
            timer.Enabled = true;
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (trackedApp == "") return;
            double cpu = 0;
            double ram = 0;
            List<string> instances = new PerformanceCounterCategory("Process")
                                    .GetInstanceNames()
                                    .Where(instance => instance.StartsWith(trackedApp))
                                    .ToList();
            if (instances.Count == 0)
            {
                return;
            }
            List<PerformanceCounter> cpuCounters = CreatePerformanceCounters(instances, "% Processor Time");
            List<PerformanceCounter> ramCounters = CreatePerformanceCounters(instances, "Working Set - Private");
            cpu = SumCounters(cpuCounters, threadNumber);
            ram = SumCounters(ramCounters, 1024 * 1024);
            while (cpu > 100)
            {
                Thread.Sleep(500);
                cpu = SumCounters(ramCounters, 1024 * 1024);
            }
            log.WriteEntry(LogMessage(trackedApp,
                                        instances.Count,
                                        cpu.ToString("F2"),
                                        ram.ToString("F2")),
                                        EventLogEntryType.Information);

        }

        public string changeTrackedAppName(string appName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["TrackedApp"].Value = appName;
            config.Save(ConfigurationSaveMode.Modified);
            return appName;
        }
    }
}