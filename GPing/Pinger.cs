using System;
using System.Net.NetworkInformation;
using System.Text;

namespace GPing
{
    public class Pinger
    {
        public static PingReply Start(string host)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException($"Item '{nameof(host)}' can't be empty or null", nameof(host));
            }

            Ping pingSender = new Ping();

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 10000;

            PingOptions options = new PingOptions(64, true);

            PingReply reply = pingSender.Send(host, timeout, buffer, options);

            if (reply.Status == IPStatus.Success)
            {
                return reply;
            }

            return null;
        }
    }
}
