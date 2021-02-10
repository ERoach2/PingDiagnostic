using PingDiagnostic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PingDiagnostic.Model
{
    /// <summary>
    /// Data from traceroute
    /// </summary>
    public class TraceRouteViewModel : ViewModelBase
    {
        private int _Number = 0;
        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                SetProperty(ref _Number, value);
            }
        }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetProperty(ref _Name, value);
            }
        }

        private string _IPAddress;
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                SetProperty(ref _IPAddress, value);
            }
        }

        private double _MinTime;
        public double MinTime
        {
            get
            {
                return _MinTime;
            }
            set
            {
                SetProperty(ref _MinTime, value);
            }
        }

        private double _MaxTime;
        public double MaxTime
        {
            get
            {
                return _MaxTime;
            }
            set
            {
                SetProperty(ref _MaxTime, value);
            }
        }

        private double _AvgTime;
        public double AvgTime
        {
            get
            {
                return _AvgTime;
            }
            set
            {
                SetProperty(ref _AvgTime, value);
            }
        }

        private int _NumReadings = 0;

        public TraceRouteViewModel(int pNumber, TraceRouteResult pResult)
        {
            Number = pNumber;
            IPAddress = pResult.Address.ToString();
            Name = Dns.GetHostEntry(pResult.Address.ToString()).HostName;
            AvgTime = pResult.TimeMs;
            MinTime = pResult.TimeMs;
            MaxTime = pResult.TimeMs;

            _NumReadings++;
        }
        public void Add(TraceRouteResult pResult)
        {
            _NumReadings++;

            AvgTime += (pResult.TimeMs - AvgTime) / _NumReadings;

            if(pResult.TimeMs < MinTime)
            {
                MinTime = pResult.TimeMs;
            }

            if (pResult.TimeMs > MaxTime)
            {
                MaxTime = pResult.TimeMs;
            }
        }
    }//END class TraceRouteViewModel
}//END Namespace
