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
                    _ctrl = new RequireController();
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
        private RS485MasterDriver _rs485Master;

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

                    DataFrame rcvFrame = _rs485Master.Send(sendFrame);
                    currReq.SetResult(rcvFrame.Data);
                }
            }
        }

        public RequireController()
        {
            _reqListArr = new List<BasicRequire>[Enum.GetValues(typeof(ReqType)).Length];
        }
        public void Start()
        {
            for (int i = 0; i < _reqListArr.Length; i++)
                _reqListArr[i] = new List<BasicRequire>();

            _workFinish = false;
            _workSem = new Semaphore(0, 1);
            _workThread = new Thread(new ThreadStart(work));
            _workThread.Start();

            _rs485Master = new RS485MasterDriver();
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
            _workSem.Release();
        }
    }
}
