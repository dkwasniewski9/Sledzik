using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using System.Reflection;

namespace SledzikLibrary
{
    public class ServiceLibrary
    {
        public static string LogMessage(string trackedApp, int length, string cpu, string ram)
        {
            return $"{trackedApp}\n" +
                            $"{length}\n" +
                            $"{cpu}\n" +
                            $"{ram}\n";
        }

        public static EventLog GetLog(string sourceName, string logName)
        {
            if (!EventLog.SourceExists(sourceName))
            {
                CreateLog(sourceName, logName);
            }
            return new EventLog(logName, ".", sourceName);
        }
        public static void CreateLog(string sourceName, string logsName)
        {
            EventLog.CreateEventSource(sourceName, logsName);
        }
        public static List<PerformanceCounter> CreatePerformanceCounters(List<string> instances, string counterName)
        {
            List<PerformanceCounter> counters = new List<PerformanceCounter>();
            foreach (string instance in instances)
            {
                try
                {
                    counters.Add(CreatePerformanceCounter(instance, counterName));
                }
                catch (InvalidOperationException)
                {
                    instances.Remove(instance);
                }
            }
            return counters;
        }

        public static PerformanceCounter CreatePerformanceCounter(string instance, string counterName)
        {
            PerformanceCounter counter = new PerformanceCounter("Process", counterName, instance);
            counter.NextValue();
            return counter;
        }
        public static double SumCounters(List<PerformanceCounter> counters, double divider)
        {
            double sum = 0;
            foreach (PerformanceCounter counter in counters)
            {
                try
                {
                    sum += NextCalculatedValue(counter, divider);
                }
                catch (InvalidOperationException)
                {
                    counters.Remove(counter);
                    continue;
                }
            }
            return sum;
        }

        public static double NextCalculatedValue(PerformanceCounter counter, double divider)
        {
            return counter.NextValue() / divider;
        }
    }
}
