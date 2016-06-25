using RS485Trans.Requires;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RS485Trans
{

    class RequireController
    {
        static private RequireController _ctrl;
        static public RequireController Controller
        {
            get
            {
                if (_ctrl == null)
                {
                    _ctrl = new RequireController();
                    _ctrl.Start();
                }
                return _ctrl;
            }
        }

        public enum ReqType
        {
            Seting = 0,
            Normal,
        }

        private List<BasicRequire>[] _reqListArr;
        private bool _workFinish;
        private Thread _workThread;
        private Semaphore _workSem;
        private IMasterDriver _rs485Master;

        private short _deviceAddress = 0x5501;

        private void work()
        {
            BasicRequire currReq = null;
            while(true)
            {
                _workSem.WaitOne();
                currReq = null;

                if (_workFinish == true)
                    break;

                while(true)
                {
                    foreach(List<BasicRequire> reqList in _reqListArr)
                    {
                        foreach(BasicRequire req in reqList)
                        {
                            currReq = req;
                            reqList.Remove(req);
                            break;
                        }
                        if (currReq != null)
                            break;
                    }

                    if (currReq == null)
                        break;

                    // Send Req
                    DataFrame sendFrame = new DataFrame();
                    sendFrame.Data = currReq.GetData();
                    sendFrame.Length = (byte)sendFrame.Data.Length;
                    sendFrame.SalveAddress = _deviceAddress;

                    DataFrame rcvFrame = _rs485Master.Send(sendFrame);
                    currReq.SetResult(rcvFrame.Data);

                    currReq = null;
                }
            }
        }

        public RequireController()
        {
            _reqListArr = new List<BasicRequire>[Enum.GetValues(typeof(ReqType)).Length];
            for (int i = 0; i < _reqListArr.Length; i++)
                _reqListArr[i] = new List<BasicRequire>();
        }
        public void Start()
        {
            for (int i = 0; i < _reqListArr.Length; i++)
                _reqListArr[i] = new List<BasicRequire>();

            _workFinish = false;
            _workSem = new Semaphore(0, 1);
            _workThread = new Thread(new ThreadStart(work));
            _workThread.Start();

            _rs485Master = TransDriver.Get();
            // _rs485Master = new SimMasterDrvier();
        }
        public void Stop()
        {
            _workFinish = true;
            _workSem.Release();
            _workThread.Join();
        }

        public void AddReq(BasicRequire req, ReqType type)
        {
            _reqListArr[(int)type].Add(req);
            try
            {
                _workSem.Release();
            }catch (Exception e)
            {

            }
        }
    }
}
