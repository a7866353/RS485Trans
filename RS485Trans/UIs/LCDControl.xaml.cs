﻿using System;
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
    /// LCDControl.xaml 的交互逻辑
    /// </summary>
    public partial class LCDControl : UserControl
    {
        private LCDReadRequire _readReq;
        private LCDWriteRequire _wirteReq;

        public byte DisplayLine { set; get; }

        public LCDControl()
        {
            InitializeComponent();

            _readReq = new LCDReadRequire();
            _wirteReq = new LCDWriteRequire();

            SendButton.Click += SendButton_Click;

            this.IsVisibleChanged += LCDControl_IsVisibleChanged;
            
        }

        void LCDControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == true)
                UpdateText();
        }

        public void UpdateText()
        {
            _readReq.DisplayLine = DisplayLine;
            string str = _readReq.Read();
            if (_readReq.Result != 0)
                return;
            InputTextBox.Text = str;
        }

        void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string str = InputTextBox.Text;
            if (string.IsNullOrEmpty(str) == true)
                return;

            _wirteReq.DisplayLine = DisplayLine;
            _wirteReq.Send(str);

        }
    }
}
