using System.Net.NetworkInformation;
using System.Windows;


namespace GPing
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Pinger.Start("www.google.com", this);
        }

        public static void Draw(PingReply reply)
        {

        }
    }
}
