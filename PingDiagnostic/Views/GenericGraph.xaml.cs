using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PingDiagnostic.Views
{
    /// <summary>
    /// Interaction logic for GenericGraph.xaml
    /// </summary>
    public partial class GenericGraph : UserControl
    {
        public GenericGraph()
        {
            InitializeComponent();
        }//END GenericGraph()

        /// <summary>
        /// Plot given data
        /// </summary>
        /// <param name="pTitle">Plot Title</param>
        /// <param name="pPlots">Map of plots. Key is line title, values are timestamp and value</param>
        public void Plot(string pTitle, string pXLabel, string pYLabel, List<GraphPlot> pPlots)
        {
            _DataPlot_WpfPlot.plt.Clear();

            //Glu Data
            _DataPlot_WpfPlot.plt.Title(pTitle);
            _DataPlot_WpfPlot.plt.XLabel(pXLabel);
            _DataPlot_WpfPlot.plt.YLabel(pYLabel);

            List<double> dataY = new List<double>();

            List<double> dataX = new List<double>();

            DateTime max = DateTime.MinValue;
            DateTime min = DateTime.MaxValue;

            //Plot each plot
            foreach (GraphPlot plot in pPlots)
            {
                dataY.Clear();
                dataX.Clear();
                plot.DataPoints.ForEach(pt => dataY.Add(pt.Item2));
                plot.DataPoints.ForEach(pt => dataX.Add(pt.Item1.ToOADate()));

                DateTime minFromSet = DateTime.FromOADate(dataX.Min());
                if (minFromSet < min)
                {
                    min = minFromSet;
                }
                DateTime maxFromSet = DateTime.FromOADate(dataX.Max());
                if (maxFromSet > max)
                {
                    max = maxFromSet;
                }

                //Do we have a defined color?
                if (plot.Color.HasValue)
                {
                    _DataPlot_WpfPlot.plt.PlotScatter(dataX.ToArray(), dataY.ToArray(), lineWidth: 1, label: plot.LegendTitle, color: plot.Color);
                }
                else
                {
                    _DataPlot_WpfPlot.plt.PlotScatter(dataX.ToArray(), dataY.ToArray(), lineWidth: 1, label: plot.LegendTitle);
                }
            }



            _DataPlot_WpfPlot.plt.AxisBounds(
           (min.AddDays(-1)).ToOADate(),
           (max.AddDays(1)).ToOADate(),
               0,
               500);



            _DataPlot_WpfPlot.plt.Ticks(dateTimeX: true);
            _DataPlot_WpfPlot.plt.Legend();

            _DataPlot_WpfPlot.Render();

        }//END Plot
    }//END class GenericGraph

    /// <summary>
    /// Plot of data
    /// </summary>
    public class GraphPlot
    {
        /// <summary>
        /// Title of this plot
        /// </summary>
        public string LegendTitle;

        /// <summary>
        /// Data points on graph
        /// </summary>
        public List<Tuple<DateTime, double>> DataPoints;

        /// <summary>
        /// Plot's color
        /// </summary>
        public System.Drawing.Color? Color = null;

        public GraphPlot()
        {
            LegendTitle = string.Empty;
            DataPoints = new List<Tuple<DateTime, double>>();
        }//END GraphPlot()

        public GraphPlot(string pTitle, List<Tuple<DateTime, double>> pDataPoints, System.Drawing.Color? pColor = null)
        {
            LegendTitle = pTitle;
            DataPoints = pDataPoints;
            Color = pColor;
        }//END GraphPlot()
    }//END class GraphPlot

}//END Namespace