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
using RS485Trans.Requires;

namespace RS485Trans.UIs
{
    /// <summary>
    /// RegiestContol.xaml 的交互逻辑
    /// </summary>
    public partial class RegiestContol : UserControl
    {
        private RegiestReadRequire _readReq;
        private RegiestWriteRequire _writeReg;

        public byte RegiestAddress { set; get; }

        public RegiestContol()
        {
            InitializeComponent();

            _readReq = new RegiestReadRequire();
            _writeReg = new RegiestWriteRequire();
            sendButton.Click += sendButton_Click;
            this.IsVisibleChanged += RegiestContol_IsVisibleChanged;
        }

        void RegiestContol_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
                Update();
        }

        void sendButton_Click(object sender, RoutedEventArgs e)
        {
            short res = 0;
            if (short.TryParse(inputTextBox.Text, out res) == false)
                return;
            _writeReg.RegiestAddress = RegiestAddress;
            _writeReg.Value = res;
            _writeReg.DoRequire();

        }

        void Update()
        {
            _readReq.RegiestAddress = RegiestAddress;
            _readReq.DoRequire();
            if (_readReq.Result != 0)
                inputTextBox.Text = "Error!";
            else
                inputTextBox.Text = _readReq.Value.ToString();
        }
    }
}
