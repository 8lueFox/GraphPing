using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GPing
{
    public partial class MainWindow : Window
    {
        private bool isStarted = false;
        private bool isAppClosing = false;
        private Thread thread;
        
        public MainWindow()
        {
            InitializeComponent();
            thread = new Thread(new ThreadStart(Pinging));
            thread.Start();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = true;
        }

        private void NewLine(int x1, int x2, int y1, int y2, Brush brush)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;

            line.StrokeThickness = 1;
            line.Stroke = brush;

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            chartCanvas.Children.Add(line);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = false;
        }

        private void Pinging()
        {
            int countOfPing = 1;
            int lastPosition = Convert.ToInt32(chartCanvas.ActualHeight) - 10;

            while (!isAppClosing)
            {
                if (isStarted)
                {
                    PingReply reply = Pinger.Start("google.com");
                    int canvasHeight = Convert.ToInt32(chartCanvas.ActualHeight);
                    
                    this.Dispatcher.Invoke(() =>
                    {
                        NewLine(4 * countOfPing, 4 * countOfPing++ + 4, lastPosition, canvasHeight - 10 - (Convert.ToInt32(reply.RoundtripTime) * 5), Brushes.Red);
                    });

                    lastPosition = canvasHeight - 10 - (Convert.ToInt32(reply.RoundtripTime) * 5);

                    Thread.Sleep(100);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isAppClosing = true;
        }
    }
}


/*if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("Address: {0}", reply.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
            }

*/