using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GPing
{
    public partial class MainWindow : Window
    {
        private bool isStarted = false;
        private bool isAppClosing = false;
        private Thread thread;
        private long minValue, maxValue, sum;
        private double avgValue;
        private Label chartPointInfoLabel;

        public string Host { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            chartPointInfoLabel = new Label();
            chartPointInfoLabel.Background = Brushes.White;
            chartPointInfoLabel.Foreground = Brushes.Black;
            StopButton.Visibility = Visibility.Hidden;
            lblAdressIp.Visibility = Visibility.Hidden;
            lblAvgValue.Visibility = Visibility.Hidden;
            lblMaxValue.Visibility = Visibility.Hidden;
            lblMinValue.Visibility = Visibility.Hidden;
            thread = new Thread(new ThreadStart(Pinging));
            thread.Start();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Host))
            {
                tbIpAddress.Text = Host;
            }

            if (tbIpAddress.Text.Equals(""))
            {
                tbIpAddress.Text = "Host name or IP";
                tbIpAddress.BorderBrush = Brushes.Red;
            }
            else
            {
                Host = tbIpAddress.Text;
                try
                {
                    Pinger.Start(Host);
                    lblAdressIp.Content = $"Pinging '{Host}'";
                    tbIpAddress.Visibility = Visibility.Hidden;
                    lblAdressIp.Visibility = Visibility.Visible;
                    StopButton.Visibility = Visibility.Visible;
                    StartButton.Visibility = Visibility.Hidden;
                    lblAvgValue.Visibility = Visibility.Visible;
                    lblMaxValue.Visibility = Visibility.Visible;
                    lblMinValue.Visibility = Visibility.Visible;
                    isStarted = true;

                    DrawGraph();
                }
                catch
                {
                    tbIpAddress.Text = "Host is incorrect";
                    tbIpAddress.BorderBrush = Brushes.Red;
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            isStarted = false;
            StopButton.Visibility = Visibility.Hidden;
            StartButton.Visibility = Visibility.Visible;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isAppClosing = true;
        }

        private void tbIpAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbIpAddress.Text.Equals("Host name or IP") || tbIpAddress.Text.Equals("Host is incorrect"))
            {
                tbIpAddress.Text = "";
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                InvokeStartButton();
            }
        }

        // wyswietlanie informacji w danej pozycji 
        private void chartCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(chartCanvas);
            Color colorAtPoint = GetPixelColor(chartCanvas, p);
            if (colorAtPoint.ToString().Equals("#2FFFADFF"))
            {
                chartPointInfoLabel.Content = $"RTT = {(graphCanvas.ActualHeight - p.Y) / 5} ms";
                chartPointInfoLabel.Margin = new Thickness(p.X, p.Y - 20, 200, 200);
                chartCanvas.Children.Add(chartPointInfoLabel);
            }
            else
            {
                chartCanvas.Children.Remove(chartPointInfoLabel);
            }
        }

        private void Pinging()
        {
            int countOfPing = 1;
            int lastPosition = Convert.ToInt32(chartCanvas.ActualHeight);
            minValue = long.MaxValue;
            maxValue = 0L;
            sum = 0L;
            avgValue = sum / countOfPing;
            int newMargin = 4;
            while (!isAppClosing)
            {
                if (isStarted)
                {
                    PingReply reply = Pinger.Start(Host);
                    int canvasHeight = Convert.ToInt32(chartCanvas.ActualHeight);
                    if (minValue > reply.RoundtripTime)
                    {
                        minValue = reply.RoundtripTime;
                    }
                    if (maxValue < reply.RoundtripTime)
                    {
                        maxValue = reply.RoundtripTime;
                    }
                    sum += reply.RoundtripTime;
                    avgValue = (double)sum / (double)countOfPing;
                    avgValue = Math.Round(avgValue, 3);

                    this.Dispatcher.Invoke(() =>
                    {
                        NewLine(4 * countOfPing, 4 * countOfPing++ + 4, lastPosition, canvasHeight - (Convert.ToInt32(reply.RoundtripTime) * 5), Brushes.GreenYellow, chartCanvas, 2);
                        lblMinValue.Content = $"min {minValue}ms";
                        lblMaxValue.Content = $"max {maxValue}ms";
                        lblAvgValue.Content = $"avg {avgValue}ms";
                        if (4 * countOfPing > chartCanvas.ActualWidth)
                        {
                            chartCanvas.Margin = new Thickness(-newMargin, 23, 73, 10);
                            newMargin += 4;
                        }
                    });

                    lastPosition = canvasHeight - (Convert.ToInt32(reply.RoundtripTime) * 5);

                    Thread.Sleep(250);
                }
            }
        }

        private void NewLine(int x1, int x2, int y1, int y2, Brush brush, Canvas canvas, double thickness = 2)
        {
            Line line = new Line();
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;

            line.StrokeThickness = thickness;
            line.Stroke = brush;

            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            canvas.Children.Add(line);
        }

        private void AddTextToCanvas(double x, double y, string text, Color color)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(color);
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            graphCanvas.Children.Add(textBlock);
        }

        private void DrawGraph()
        {
            NewLine(2, 2, 0, Convert.ToInt32(graphCanvas.ActualHeight), Brushes.White, graphCanvas);

            for (int j = 0; j <= 66; j += 6)
            {
                NewLine(2, 10, Convert.ToInt32(graphCanvas.ActualHeight - (j * 5)), Convert.ToInt32(graphCanvas.ActualHeight - (j * 5)), Brushes.White, graphCanvas);
            }

            for (int j = 0; j <= 66; j += 6)
            {
                AddTextToCanvas(20, graphCanvas.ActualHeight - (j * 5) - 10, $"{j} ms", Colors.White);
            }

            for (int j = 0; j <= 66; j += 6)
            {
                for (int i = 0; i < bgCanvas.ActualWidth / 10; i += 2)
                {
                    NewLine(i * 10, i * 10 + 10, Convert.ToInt32(graphCanvas.ActualHeight - (j * 5)), Convert.ToInt32(graphCanvas.ActualHeight - (j * 5)), Brushes.Gray, bgCanvas, 1);
                }
            }
        }

        public static Color GetPixelColor(Visual visual, Point pt)
        {
            Point ptDpi = getScreenDPI(visual);

            Size srcSize = VisualTreeHelper.GetDescendantBounds(visual).Size;

            //Viewbox uses values between 0 & 1 so normalize the Rect with respect to the visual's Height & Width
            Rect percentSrcRec = new Rect(pt.X / srcSize.Width, pt.Y / srcSize.Height,
                                          1 / srcSize.Width, 1 / srcSize.Height);

            //var bmpOut = new RenderTargetBitmap(1, 1, 96d, 96d, PixelFormats.Pbgra32); //assumes 96 dpi
            var bmpOut = new RenderTargetBitmap((int)(ptDpi.X / 96d),
                                                (int)(ptDpi.Y / 96d),
                                                ptDpi.X, ptDpi.Y, PixelFormats.Default); //generalized for monitors with different dpi

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush { Visual = visual, Viewbox = percentSrcRec },
                                 null, //no Pen
                                 new Rect(0, 0, 1d, 1d));
            }
            bmpOut.Render(dv);

            var bytes = new byte[4];
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel -- 4 * 8 bits per byte = 32)
            bmpOut.CopyPixels(bytes, iStride, 0);

            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static Point getScreenDPI(Visual v)
        {
            //System.Windows.SystemParameters
            PresentationSource source = PresentationSource.FromVisual(v);
            Point ptDpi;
            if (source != null)
            {
                ptDpi = new Point(96.0 * source.CompositionTarget.TransformToDevice.M11,
                                   96.0 * source.CompositionTarget.TransformToDevice.M22);
            }
            else
                ptDpi = new Point(96d, 96d); //default value.

            return ptDpi;
        }

        public void InvokeStartButton()
        {
            StartButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}