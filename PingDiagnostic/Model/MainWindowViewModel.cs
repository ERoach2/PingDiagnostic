using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingDiagnostic.Model
{
    /// <summary>
    /// GUI data for main window
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private Dictionary<string, TraceRouteViewModel> _Traces = new Dictionary<string, TraceRouteViewModel>();
        public Dictionary<string,TraceRouteViewModel> Traces
        {
            get
            {
                return _Traces;
            }
            set
            {
                SetProperty(ref _Traces, value);
            }
        }

        private List<TraceRouteViewModel> _TracesDisplay = new List<TraceRouteViewModel>();
        public List<TraceRouteViewModel> TracesDisplay
        {
            get
            {
                return _TracesDisplay;
            }
            set
            {
                SetProperty(ref _TracesDisplay, value);
            }
        }

        private string _HostAddress = "8.8.8.8";
        public string HostAddress
        {
            get
            {
                return _HostAddress;
            }
            set
            {
                SetProperty(ref _HostAddress, value);
            }
        }

        private string _ButtonText = "Start";
        public string ButtonText
        {
            get
            {
                return _ButtonText;
            }
            set
            {
                SetProperty(ref _ButtonText, value);
            }
        }
    }//END class MainWindowViewModel
}//END Namespace
