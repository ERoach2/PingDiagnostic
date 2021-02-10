using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PingDiagnostic.Data
{
    /// <summary>
    /// Run a traceroute
    /// </summary>
    public static class TraceRoute
    {
        private const string Data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        public static IEnumerable<TraceRouteResult> GetTraceRoute(string hostNameOrAddress)
        {
            return GetTraceRoute(hostNameOrAddress, 1);
        }
        private static IEnumerable<TraceRouteResult> GetTraceRoute(string hostNameOrAddress, int ttl)
        {
            Ping pinger = new Ping();
            PingOptions pingerOptions = new PingOptions(ttl, true);
            int timeout = 10000;
            byte[] buffer = Encoding.ASCII.GetBytes(Data);
            PingReply reply = default(PingReply);

            reply = pinger.Send(hostNameOrAddress, timeout, buffer, pingerOptions);

            List<TraceRouteResult> result = new List<TraceRouteResult>();

            if (reply.Status == IPStatus.Success)
            {
                result.Add(new TraceRouteResult() { Address = reply.Address, TimeMs = reply.RoundtripTime });
            }
            else if (reply.Status == IPStatus.TtlExpired || reply.Status == IPStatus.TimedOut)
            {
                //add the currently returned address if an address was found with this TTL
                if (reply.Status == IPStatus.TtlExpired) result.Add(new TraceRouteResult() { Address = reply.Address, TimeMs = reply.RoundtripTime });
                //recurse to get the next address...
                IEnumerable<TraceRouteResult> tempResult = default(IEnumerable<TraceRouteResult>);
                tempResult = GetTraceRoute(hostNameOrAddress, ttl + 1);
                result.AddRange(tempResult);
            }
            else
            {
                //failure 
            }

            return result;
        }


    }//END class TraceRoute

    public class TraceRouteResult
    {
        public IPAddress Address {get;set;}
        public double TimeMs { get; set; }

    }

}//END Namespace
