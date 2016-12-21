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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			int startPort = Convert.ToInt16(portsFrom.Text);
			int endPort = Convert.ToInt16(portsTo.Text);

			scanProgress.Value = 0;
			scanProgress.Maximum = endPort - startPort + 1;

			output.Text = "";
			DoEvents();

			for (int curr = startPort; curr <= endPort; curr++)
			{
				TcpClient scan = new TcpClient();


				try
				{
					scan.Connect(ipAddress.Text, curr);
					output.AppendText("Port " + curr + " open.\n");
					DoEvents();
				}
				catch (Exception)
				{
					output.AppendText("Port " + curr + " closed.\n");
					DoEvents();
				}

				scanProgress.Value = scanProgress.Value + 1;
			}
		}

		public void DoEvents()
		{
			DispatcherFrame frame = new DispatcherFrame(true);
			Dispatcher.CurrentDispatcher.BeginInvoke
			(
			DispatcherPriority.Background,
			(SendOrPostCallback)delegate (object arg)
			{
				var f = arg as DispatcherFrame;
				f.Continue = false;
			},
			frame
			);
			Dispatcher.PushFrame(frame);
		}
	}
}
