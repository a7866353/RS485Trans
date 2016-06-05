using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace RS485Trans
{
    class DataFrame
    {
        public short SalveAddress;
        public byte Length;
        public byte[] Data;

        public byte[] Header
        {
            get
            {
                byte[] buf = new byte[3];
                BitConverter.GetBytes(SalveAddress).CopyTo(buf, 0);
                BitConverter.GetBytes(Length).CopyTo(buf, 2);

                return buf;
            }
        }

    }

    class DataFrameAnalyzer
    {
        private short _targetAddress;
        private enum State
        {
            RcvAddress,
            RcvLen,
            RcvData,
        }
        private State _state;
        private byte[] _addrBuf;
        private byte[] _dataBuf;
        private int _pos;
        public DataFrameAnalyzer()
        {
            _addrBuf = new byte[2];
        }

        public void SetTargetAddress(short addr)
        {
            _targetAddress = addr;
            _state = State.RcvAddress;
            _addrBuf.Initialize();
        }
        public DataFrame Set(byte data)
        {
            switch(_state)
            {
                case State.RcvAddress:
                    _addrBuf[0] = data;
                    if(BitConverter.ToUInt16(_addrBuf,0) != _targetAddress)
                    {
                        _addrBuf[1] = _addrBuf[0];
                        return null;
                    }
                
                    // target address right
                    _state = State.RcvLen;
                    break;

                case State.RcvLen:
                    _pos = 0;
                    _dataBuf = new byte[data];
                    _state = State.RcvData;
                    break;

                case State.RcvData:
                    _dataBuf[_pos] = data;
                    _pos++;
                    if( _pos<_dataBuf.Length)
                        return null;

                    DataFrame frame = new DataFrame();
                    frame.SalveAddress = _targetAddress;
                    frame.Length = (byte)_dataBuf.Length;
                    frame.Data = _dataBuf;

                    _state = State.RcvAddress;
                    _addrBuf.Initialize();

                    return frame;
                    break;

                default:
                    return null;
                    break;
            }
            return null;
        }
    }

    class RS485MasterDriver
    {
        static public string[] GetList()
        {
            return SerialPort.GetPortNames();
        }


        private SerialPort _sp;
        private DataFrameAnalyzer _frameAnalyzer;
        private bool _isRcvState;
        private DataFrame _rcvFrame;
        private Semaphore _rcvSem;

        private void _sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = _sp.BytesToRead;
            if( len <= 0 )
                return;
            byte[] tmpBuf = new byte[len];
            _sp.Read(tmpBuf, 0, len);

            if (_isRcvState == false)
                return;

            foreach(byte d in tmpBuf)
            {
                DataFrame frame = _frameAnalyzer.Set(d);
                if(frame != null)
                {
                    _rcvFrame = frame;
                    _rcvSem.Release();
                    return;
                }
            }
            
        } 


        public int RecevieTimeout = 1000;
        public bool IsOpen
        {
            get { return _sp.IsOpen; }
        }

        public enum ErrorCode
        {
            OK,
            PortIsAlreadyOpend,
            Error,
        }

        public RS485MasterDriver()
        {
            _sp = new SerialPort();
            _sp.DataReceived += _sp_DataReceived;
            _frameAnalyzer = new DataFrameAnalyzer();
        }


        public ErrorCode Open(string portName, int baudRate)
        {
            if(_sp.IsOpen == true)
                return ErrorCode.PortIsAlreadyOpend;

            _sp.PortName = portName;
            _sp.BaudRate = baudRate;
            _sp.Parity = Parity.None;
            _sp.StopBits = StopBits.One;

            try
            {
                _sp.Open();
            }
            catch(Exception e)
            {
                return ErrorCode.Error;
            }

            _rcvFrame = null;
            _rcvSem = new Semaphore(0, 1);
            _isRcvState = false;

            return ErrorCode.OK;
        }
        public void Close()
        {
            _sp.Close();
        }

        public DataFrame Send(DataFrame frame)
        {
            // Clear rcv control
            _frameAnalyzer.SetTargetAddress(frame.SalveAddress);
            _rcvSem.WaitOne(0);
            _isRcvState = true;

            // Send data
            _sp.Write(frame.Header, 0, frame.Header.Length);
            _sp.Write(frame.Data, 0, frame.Length);

            // Recevie data
            if (_rcvSem.WaitOne(RecevieTimeout) == false)
                return null;

            if (_rcvFrame == null)
                throw (new Exception("Error!"));
            DataFrame res = _rcvFrame;
            _rcvFrame = null;
            _isRcvState = false;

            return res;
        }
    }
}
