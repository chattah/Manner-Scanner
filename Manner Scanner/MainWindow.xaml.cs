using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Manner_Scanner
{

	
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const int PORT_MINIMUM = 1;
		private const int PORT_MAXIMUM = 65535;
		private int userSetStartPort;
		private int userSetEndPort;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void startScanButton_Click(object sender, RoutedEventArgs e)
		{
			int startPort = Convert.ToInt32(portsFrom.Text);
			int endPort = Convert.ToInt32(portsTo.Text);

			scanProgress.Value = 0;
			scanProgress.Maximum = endPort - startPort + 1;

			var scanList = from i in Enumerable.Range(startPort, endPort - startPort + 1)
						   select ScanPort(i, ipAddressStart.Text)
						   .ContinueWith(t => Report(t.Result),
						   TaskScheduler.FromCurrentSynchronizationContext());

			var tasks = scanList.ToArray();


			output.Text = "";
		}



		private Task<string> ScanPort(int currentPort, string scanAddress)
		{
			return Task.Factory.StartNew(() =>
			{
				using (var tcpportScan = new TcpClient())
				{
					try
					{
						tcpportScan.SendTimeout = 10;
						tcpportScan.Connect(scanAddress, currentPort);
						return "Port " + currentPort + " open.\n";

					}
					catch (Exception)
					{
						return "Port " + currentPort + " closed.\n";
					}

				}
			}, TaskCreationOptions.LongRunning);
		}

		private void Report(object message)
		{
			output.AppendText((string)message);
			scanProgress.Value = scanProgress.Value + 1;
		}

		private void scanAllPorts_Checked(object sender, RoutedEventArgs e)
		{
			userSetStartPort = Convert.ToInt32(portsFrom.Text);
			userSetEndPort = Convert.ToInt32(portsTo.Text);

			portsFrom.Text = PORT_MINIMUM.ToString();
			portsTo.Text = PORT_MAXIMUM.ToString();

			portsFrom.IsEnabled = false;
			portsTo.IsEnabled = false;
		}

		private void scanAllPorts_Unchecked(object sender, RoutedEventArgs e)
		{
			portsFrom.Text = userSetStartPort.ToString();
			portsTo.Text = userSetEndPort.ToString();

			portsFrom.IsEnabled = true;
			portsTo.IsEnabled = true;
		}
	}
}
