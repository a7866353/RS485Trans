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
using System.Timers;

namespace RS485Trans.UIs
{
    /// <summary>
    /// RegiestControl.xaml 的交互逻辑
    /// </summary>
    public partial class RegiestLabel : UserControl
    {
        private RegiestReadRequire _readReq;
        private Timer _timer;
        private delegate void _func();

        public byte RegiestAddress { set; get; }
        public int UpdateInterval { set; get; }

        public RegiestLabel()
        {
            InitializeComponent();
            _readReq = new RegiestReadRequire();
            RegiestAddress = 0;
            UpdateInterval = 500;
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            this.IsVisibleChanged += RegiestControl_IsVisibleChanged;
        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Update();
        }
        void UpdateValue()
        {
            _readReq.RegiestAddress = RegiestAddress;
            _readReq.DoRequire();
            OutputLabel.Content = _readReq.Value;
        }

        void Update()
        {
            this.Dispatcher.Invoke(new _func(UpdateValue));
        }

        void RegiestControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == false)
            {
                _timer.Stop();
            }
            else
            {
                UpdateValue();
                _timer.Interval = UpdateInterval;
                _timer.Start();
            }
        }

    }
}
