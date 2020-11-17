using System;
using System.Net.NetworkInformation;
using System.Text;

namespace GPing
{
    public class Pinger
    {
        public static void Start(string host, MainWindow MainWindow)
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentException($"Item '{nameof(host)}' can't be empty or null", nameof(host));
            }

            if (MainWindow is null)
            {
                throw new ArgumentNullException(nameof(MainWindow));
            }

            Ping pingSender = new Ping();

            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 10000;

            PingOptions options = new PingOptions(64, true);

            PingReply reply = pingSender.Send(host, timeout, buffer, options);

            if (reply.Status == IPStatus.Success)
            {
                MainWindow.Draw(reply);
            }
        }
    }
}
