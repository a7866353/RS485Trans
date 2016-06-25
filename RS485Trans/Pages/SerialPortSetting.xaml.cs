using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS485Trans.Pages
{
    /// <summary>
    /// SerialPortSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortSetting : Page
    {
        private RS485MasterDriver _driver;
        public SerialPortSetting()
        {
            InitializeComponent();
            UpdatePorts();

            ConnectButton.Click += ConnectButton_Click;
            SendButton.Click += SendButton_Click;
            OutputTextBox.TextChanged += OutputTextBox_TextChanged;
        }

        void OutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int textMaxLength = 1000;
            if (OutputTextBox.Text.Length > textMaxLength)
                OutputTextBox.Text = OutputTextBox.Text.Substring(OutputTextBox.Text.Length - textMaxLength);
            OutputTextBox.ScrollToEnd();
        }

        void SendButton_Click(object sender, RoutedEventArgs e)
        {
            
            DataFrame sndFrame = new DataFrame();
            sndFrame.SalveAddress = 0x02;
            sndFrame.Data = ASCIIEncoding.ASCII.GetBytes(InputTextBox.Text);
            sndFrame.Length = (byte)sndFrame.Data.Length;
            DataFrame rcvFrame = _driver.Send(sndFrame);
            OutputTextBox.Text += ASCIIEncoding.ASCII.GetString(sndFrame.Data);
          
        }

        void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string portName = (string)((ComboBoxItem)PortComboBox.SelectedItem).Content;
            _driver = new RS485MasterDriver();
            _driver.Open(portName, 9600);

            TransDriver.Set(_driver);

            // UartTest();


        }

        void SerialPortSetting_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdatePorts();
        }

        private void UpdatePorts()
        {
            PortComboBox.Items.Clear();
            string[] portNames = RS485MasterDriver.GetList();
            foreach(string str in portNames)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = str;
                PortComboBox.Items.Add(item);
            }
            if (PortComboBox.Items.Count > 0)
                PortComboBox.SelectedIndex = 0;
        }



        private void UartTest()
        {
            DataFrame sndFrame = new DataFrame();
            sndFrame.SalveAddress = 0x02;
            sndFrame.Data = new byte[]{1,2,3,4,5,6,7,8,9,10};
            sndFrame.Length = (byte)sndFrame.Data.Length;
            DataFrame rcvFrame = _driver.Send(sndFrame);

        }
    }
}
