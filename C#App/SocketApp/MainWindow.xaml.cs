using System;
using System.Windows;
using System.Windows.Controls;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers;


namespace Painting
{
    public partial class MainWindow : Window
    {
        ABBRobot Robot;
        NetworkScanner Netscaner { get; set; }
        ControllerInfoCollection Controllers { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            
            Robot = new ABBRobot();
            NetScan();

            Robot.SendMessageEvent += (msg) => rtbLog.AppendText(String.Format("\rC# : {0}", msg)); 
            Robot.GetMessageEvent += (msg) => rtbLog.AppendText(String.Format("\rABB: {0}", msg));
        }

        private void NetScan()
        {
            Netscaner = new NetworkScanner();
            Netscaner.Scan();
            Controllers = Netscaner.Controllers;

            foreach (ControllerInfo c in Controllers)
            {
                cbox_Controllers.Items.Add(c);
            }
        }

        private void cbox_Controllers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var comboBoxControllers = sender as ComboBox;
                Robot.Connect((ControllerInfo)comboBoxControllers?.SelectedItem);

                btnSend.IsEnabled = true;

                Robot.OpenSocket();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnSend_Click(object sender, RoutedEventArgs e) => Robot.SendMessage(tbSend.Text);
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Robot.CloseSocket();

    }
}
