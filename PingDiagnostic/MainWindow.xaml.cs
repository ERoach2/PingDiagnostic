using MahApps.Metro.Controls;
using PingDiagnostic.Data;
using PingDiagnostic.Model;
using PingDiagnostic.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PingDiagnostic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private GraphPlot _TracePlot = new GraphPlot();

        private MainWindowViewModel _ViewModel = new MainWindowViewModel();

        private bool _Running = false;

        private int _NumRoutes = 0;

        private string _LogFileName = $"{DateTime.Now.ToString("MMddyyyy_HHmmss")}";
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                _TracePlot.LegendTitle = "Ping Time";
                _TracePlot.DataPoints = new List<Tuple<DateTime, double>>();
                this.Closed += WinClosedHandler;
                this.DataContext = _ViewModel;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Fatal error: {ex.Message}\n{ex.StackTrace}", $"FATAL ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }//END MainWindow()

        private void WinClosedHandler(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Run test task
        /// </summary>
        private void TestTask()
        {
            _NumRoutes = 0;

            _TracePlot.DataPoints = new List<Tuple<DateTime, double>>();

            _ViewModel.ButtonText = "Stop";

            _ViewModel.Traces = new Dictionary<string, TraceRouteViewModel>();
            _ViewModel.TracesDisplay = new List<TraceRouteViewModel>();

            WriteToCsv("Time", "Ping MS");

            double runningAvg = 0;
            int iterations = 0;

            while (_Running == true)
            {
                try
                {
                    bool added = false;

                    //Run a traceroute
                    List<TraceRouteResult> results = TraceRoute.GetTraceRoute(_ViewModel.HostAddress).ToList();
                    iterations++;

                    Dictionary<string, TraceRouteViewModel> tmpCopy = _ViewModel.Traces;
                    double totalTime = 0;

                    foreach (TraceRouteResult route in results)
                    {
                        totalTime += route.TimeMs;
                        if (tmpCopy.ContainsKey(route.Address.ToString()) == true)
                        {
                            tmpCopy[route.Address.ToString()].Add(route);
                        }
                        else
                        {
                            
                            tmpCopy[route.Address.ToString()] = new TraceRouteViewModel(++_NumRoutes, route);
                            WriteToLog($"{tmpCopy[route.Address.ToString()].Name}", tmpCopy[route.Address.ToString()].IPAddress, tmpCopy[route.Address.ToString()].AvgTime.ToString());
                            added = true;
                        }
                    }

                    runningAvg += (totalTime - runningAvg) / (double)iterations;

                    //Reorder
                    List<TraceRouteViewModel> orderedTmp = tmpCopy.Values.ToList().OrderByDescending(route => route.Number).ToList();

                    if(added == true)
                    {
                        
                    }

                    //Update GUI
                    _ViewModel.Traces = tmpCopy;
                    _ViewModel.TracesDisplay = orderedTmp;

                    Tuple<DateTime, double> next = new Tuple<DateTime, double>(DateTime.Now, totalTime);

                    if(totalTime > (runningAvg * 3))
                    {
                        WriteToLog($"====================",$"PING SPIKE!",$"{next.Item1}", $"{next.Item2}",$"====================");
                    }
                    else
                    {
                        WriteToLog($"{next.Item1}", $"{next.Item2}");
                    }

                    //Write to the CSV
                    WriteToCsv($"{next.Item1}", $"{next.Item2}");

                    _TracePlot.DataPoints.Add(next);

                    if (_TracePlot.DataPoints.Count > 1)
                    {
                        this.Dispatcher.Invoke(() => _PingPlot_Graph.Plot("AVG Ping Times", "Time", "MS", new List<GraphPlot>() { _TracePlot }));
                    }
                }
                catch(Exception ex)
                {
                    //Meh
                    WriteToLog("Exception!",$"{ex.Message}",$"{Environment.NewLine}{ex.StackTrace}");
                }

                Thread.Sleep(5000);
            }

            _ViewModel.ButtonText = "Start";
        }//END TestTask()

        //Write to log
        private void WriteToLog(params string[] pMsg)
        {
            string msg = $"[{DateTime.Now.ToString("hh:mm:ss.fff tt")}] {string.Join("|",pMsg)} {Environment.NewLine}";

            File.AppendAllText($"{_LogFileName}.log", msg);
        }//END WriteToLog()

        private void WriteToCsv(params string[] pMsg)
        {
            string msg = $"{string.Join(",", pMsg)} {Environment.NewLine}";

            File.AppendAllText($"{_LogFileName}.csv", msg);
        }//END WriteToLog()

        /// <summary>
        /// Start test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Start_Button_Click(object sender, RoutedEventArgs e)
        {
            if(_Running == false)
            {
                _Running = true;
                Task.Factory.StartNew(() => TestTask());
            }
            else
            {
                _Running = false;

            }

        }//END _Start_Button_Click()
    }//END class MainWindow
}//END Namespace
