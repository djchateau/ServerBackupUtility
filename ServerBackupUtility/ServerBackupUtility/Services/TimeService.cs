
using ServerBackupUtility.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;

namespace ServerBackupUtility.Services
{
    public class TimeService
    {
        private static readonly string _server = ConfigurationManager.AppSettings["TimeServer"];

        public static DateTime LocalTime => new TimeService().GetLocalTime();
        public static DateTime UniversalTime => new TimeService().GetUniversalTime();

        private DateTime GetLocalTime()
        {
            DateTime dateTime = GetUniversalTime();
            return dateTime.ToLocalTime();
        }

        private DateTime GetUniversalTime()
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            IList<IPAddress> addresses = Dns.GetHostEntry(_server).AddressList;
            IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                socket.ReceiveTimeout = 3000;
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: TimeService.DateTimeUniversal - " + ex.Message);
            }
            finally
            {
                socket.Close();
            }

            ulong integerPart = ((ulong) ntpData[40] << 24) | ((ulong) ntpData[41] << 16) | ((ulong) ntpData[42] << 8) | ntpData[43];
            ulong fractionPart = ((ulong) ntpData[44] << 24) | ((ulong) ntpData[45] << 16) | ((ulong) ntpData[46] << 8) | ntpData[47];

            ulong milliseconds = integerPart * 1000 + fractionPart * 1000 / 0x100000000L;
            DateTime dateTime = new DateTime(1900, 1, 1).AddMilliseconds((long) milliseconds);

            return dateTime;
        }
    }
}
