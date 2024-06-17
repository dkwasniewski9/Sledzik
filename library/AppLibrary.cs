using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SledzikLibrary
{
    public class AppLibrary
    {
        public static string ServiceName = "Śledzik";
        public static void drawGraph(string graphName, Canvas canvas, EventLogEntryCollection entries, string trackedApp)
        {
            List<double> values = new List<double>(); ;
            double height = 0;
            double width;
            double lastX = 0;
            double lastY = canvas.Height;
            SolidColorBrush brush = Brushes.White;
            if (graphName == "CPU")
            {
                foreach (EventLogEntry entry in entries)
                {
                    if (entry.Message.StartsWith(trackedApp))
                    {
                        var message = entry.Message.Split('\n');
                        var value = Convert.ToDouble(message[2]);
                        if (value == 0) continue;
                        values.Add(value);
                    }
                }
                height = canvas.Height / 100;
                brush = Brushes.Red;
            }
            else if (graphName == "RAM")
            {
                foreach (EventLogEntry entry in entries)
                {
                    if (entry.Message.StartsWith(trackedApp))
                    {
                        var message = entry.Message.Split('\n');
                        var value = Convert.ToDouble(message[3]);
                        if (value == 0) continue;
                        values.Add(value);
                    }
                }
                height = canvas.Height / (new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024));
                brush = Brushes.Blue;

            }
            if (values.Count == 0) return;
            width = canvas.Width / values.Count;
            Line lineY = new Line();
            lineY.X1 = 0;
            lineY.Y1 = 0;
            lineY.X2 = 0;
            lineY.Y2 = canvas.Height;
            lineY.Stroke = Brushes.Black;
            lineY.HorizontalAlignment = HorizontalAlignment.Left;
            lineY.VerticalAlignment = VerticalAlignment.Center;
            lineY.StrokeThickness = 3;
            Line lineX = new Line();
            lineX.X1 = 0;
            lineX.Y1 = canvas.Height;
            lineX.X2 = canvas.Width;
            lineX.Y2 = canvas.Height;
            lineX.Stroke = Brushes.Black;
            lineX.HorizontalAlignment = HorizontalAlignment.Left;
            lineX.VerticalAlignment = VerticalAlignment.Center;
            lineX.StrokeThickness = 3;
            canvas.Children.Add(lineX);
            canvas.Children.Add(lineY);
            foreach (var value in values)
            {
                Line line = new Line();
                line.Stroke = brush;
                line.X1 = lastX;
                line.Y1 = lastY;
                line.X2 = lastX + width;
                line.Y2 = canvas.Height - height * value;
                lastX = line.X2;
                lastY = line.Y2;
                line.HorizontalAlignment = HorizontalAlignment.Left;
                line.VerticalAlignment = VerticalAlignment.Center;
                line.StrokeThickness = 3;
                canvas.Children.Add(line);
            }
        }
        public static string getStatsAboutTrackedApp(string appName, EventLogEntryCollection entries)
        {
            int entriesNumber = 0;
            double averageCPU = 0;
            double averageMemory = 0;
            double highestCPU = 0;
            double highestMemory = 0;
            int averageProcessCount = 0;
            int highestProcessCount = 0;
            double totalPhysicalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024 * 1024);
            foreach (EventLogEntry entry in entries)
            {
                if (entry.Message.StartsWith(appName))
                {
                    var message = entry.Message.Split('\n');
                    if (message[1] == "0") continue;
                    ++entriesNumber;
                    averageCPU += Convert.ToDouble(message[2]);
                    averageMemory += Convert.ToDouble(message[3]);
                    averageProcessCount += Convert.ToInt32(message[1]);
                    if (Convert.ToDouble(message[2]) > highestCPU) highestCPU = Convert.ToDouble(message[2]);
                    if (Convert.ToDouble(message[3]) > highestMemory)
                    {
                        highestMemory = Convert.ToDouble(message[3]);
                    }
                    if (Convert.ToInt32(message[1]) > highestProcessCount) highestProcessCount = Convert.ToInt32(message[1]);
                }
            }
            if (entriesNumber == 0) return "Brak danych";
            averageCPU /= entriesNumber;
            averageMemory /= entriesNumber;
            averageProcessCount /= entriesNumber;

            return $"Zapisane wpisy: {entriesNumber}\n" +
                   $"Średnie zużycie CPU: {averageCPU:F2}%\n" +
                   $"Średnie zużycie RAM: {averageMemory:F2} MB\n" +
                   $"Średnia ilosc procesow: {averageProcessCount} \n" +
                   $"Najwyższe zużycie CPU: {highestCPU:F2} %\n" +
                   $"Najwyższe zużycie RAM: {highestMemory:F2} MB \n" +
                   $"Najwyższa ilosc procesow: {highestProcessCount} \n";
        }
    }
}
