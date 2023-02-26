using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaaSArcGISIOS.ShellView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SSRViewPage : ContentPage
    {
        private Data.SingletonData mSharedData;
        private InternalEngine mInternalEngine;
        private System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem> mClockCorrectionList;
        private System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem> mIonosphereDelayList;
        private System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem> mOrbitCorrectionList;
        private System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem> mSignalBiasList;
        private System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem> mTroposphereDelayList;

        public SSRViewPage()
        {
            InitializeComponent();
            mSharedData = Data.SingletonData.mInstance;
            mClockCorrectionList = new System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem>();
            mIonosphereDelayList = new System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem>();
            mOrbitCorrectionList = new System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem>();
            mSignalBiasList = new System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem>();
            mTroposphereDelayList = new System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem>();
            mInternalEngine = new InternalEngine(this, SSRReceiveSSRCallback);
            xClockCorrectionView.ItemsSource = mClockCorrectionList;
            xIonosphereDelayView.ItemsSource = mIonosphereDelayList;
            xOrbitCorrectionView.ItemsSource = mOrbitCorrectionList;
            xSignalBiasView.ItemsSource = mSignalBiasList;
            xTroposphereDelayView.ItemsSource = mTroposphereDelayList;

            mInternalEngine.Start();
        }

        protected override void OnAppearing()
        {
            //mInternalEngine.Start();
        }

        protected override void OnDisappearing()
        {
            mClockCorrectionList.Clear();
            mIonosphereDelayList.Clear();
            mOrbitCorrectionList.Clear();
            mSignalBiasList.Clear();
            mTroposphereDelayList.Clear();

            //mInternalEngine.Stop();
        }

        private void SSRReceiveSSRCallback(byte[] pBuffer)
        {
            Model.SSRDecoder.SSRBufferDecoder(pBuffer, SSRDecoderObjectCallBack);
        }

        private void SSRDecoderObjectCallBack(Model.SSRDecoderObject pSSRDecoderObject)
        {
            Model.SSRParser.GetSSRObjectParserData(pSSRDecoderObject,
                ClockCallback, IonospherCallback, pOrbitCallback, SignalCallback, TroposphereCallback);
        }
        private void SignalCallback(Model.SignalBiasData pResult)
        {
            try
            {
                mSharedData.mSSRDictionary.Push(new Model.HTTPSSR()
                {
                    recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    sbData = pResult,
                    tddData = null,
                    obcData = null,
                    lddData = null,
                    ccdData = null,
                });
            }
            catch
            {

            }

            InsertSSRData(mSignalBiasList, "SignalBias -> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pResult.ToString());
        }

        private void TroposphereCallback(Model.TroposphereDelayData pResult)
        {
            try
            {
                mSharedData.mSSRDictionary.Push(new Model.HTTPSSR()
                {
                    recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    sbData = null,
                    tddData = pResult,
                    obcData = null,
                    lddData = null,
                    ccdData = null,
                });
            }
            catch
            {

            }

            InsertSSRData(mTroposphereDelayList, "TroposphereDelay -> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pResult.ToString());
        }

        private void pOrbitCallback(Model.OrbitCorrectionData pResult)
        {
            try
            {
                mSharedData.mSSRDictionary.Push(new Model.HTTPSSR()
                {
                    recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    sbData = null,
                    tddData = null,
                    obcData = pResult,
                    lddData = null,
                    ccdData = null,
                });
            }
            catch
            {

            }

            InsertSSRData(mOrbitCorrectionList, "OrbitCorrection -> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pResult.ToString());
        }

        private void IonospherCallback(Model.IonosphereDelayData pResult)
        {
            try
            {
                mSharedData.mSSRDictionary.Push(new Model.HTTPSSR()
                {
                    recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    sbData = null,
                    tddData = null,
                    obcData = null,
                    lddData = pResult,
                    ccdData = null,
                });
            }
            catch
            {

            }

            InsertSSRData(mIonosphereDelayList, "IonosphereDelay -> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pResult.ToString());
        }

        private void ClockCallback(Model.ClockCorrectionData pResult)
        {
            try
            {
                mSharedData.mSSRDictionary.Push(new Model.HTTPSSR()
                {
                    recordDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    sbData = null,
                    tddData = null,
                    obcData = null,
                    lddData = null,
                    ccdData = pResult,
                });
            }
            catch
            {

            }

            InsertSSRData(mClockCorrectionList, "ClockCorrection -> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pResult.ToString());
        }

        private void InsertSSRData(System.Collections.ObjectModel.ObservableCollection<InternalDisplayListViewItem> pList, string pTitle, string pMsg)
        {
            this.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                if (pList.Count >= 15)
                {
                    pList.Remove(pList.Last());
                }
                pList.Insert(0, new InternalDisplayListViewItem() { sTitle = pTitle, sMessage = pMsg });
            });
        }

        private class InternalDisplayListViewItem
        {
            public string sTitle { set; get; }
            public string sMessage { set; get; }
        }

        private class InternalEngine : Common.EngineThread
        {
            private SSRViewPage mParent;
            private System.Net.Sockets.Socket mSocket;
            private string mAddress;
            private int mPort;
            private Queue<byte[]> mReceiveQueue;
            private Action<byte[]> mReceiveAction;

            public InternalEngine(SSRViewPage pParent, Action<byte[]> pAction)
            {
                mParent = pParent;
                mAddress = "RTS2.ngii.go.kr";
                mPort = 2101;
                mSocket = new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                mReceiveQueue = new Queue<byte[]>();
                mReceiveAction = pAction;
            }

            public void SessionWrite(System.Net.Sockets.Socket pSocket)
            {
                try
                {
                    string msg = "GET " + "/SSR-SSRG " + "HTTP/1.1\r\nUser-Agent:" + "NTRIP node-ssrg Client/0.1" + "\r\nAuthorization: " + "Basic ";
                    msg = msg + System.Convert.ToBase64String(Encoding.UTF8.GetBytes("hjk1210p:ngii")) + "\n\n";
                    var writebuffer = Encoding.UTF8.GetBytes(msg);

                    pSocket.BeginSend(writebuffer, 0, writebuffer.Length, System.Net.Sockets.SocketFlags.None, pResult =>
                    {
                        if (pResult.AsyncState != null)
                        {
                            System.Net.Sockets.Socket target = pResult.AsyncState as System.Net.Sockets.Socket;
                            if (target != null)
                            {
                                int writesize = target.EndSend(pResult);

                                Console.WriteLine("Socket write to {0} , length : {1}", target.RemoteEndPoint.ToString(), writesize);

                                SessionReceive(target);
                            }
                        }
                    }, pSocket);
                }
                catch
                {

                }
            }

            private void SessionReceive(System.Net.Sockets.Socket pSocket)
            {
                try
                {
                    SocketObject target = new SocketObject();
                    target.WorkSocket = pSocket;

                    pSocket.BeginReceive(target.DataBuffer, 0, target.DataBuffer.Length, System.Net.Sockets.SocketFlags.None, new AsyncCallback(ReceiveCallBack), target);
                }
                catch
                {

                }
            }

            private void ReceiveCallBack(IAsyncResult ar)
            {
                try
                {
                    if (ar.AsyncState != null)
                    {
                        SocketObject target = ar.AsyncState as SocketObject;

                        int readsize = target.WorkSocket.EndReceive(ar);
                        Console.WriteLine("Socket read to {0} , length : {1}", target.WorkSocket.RemoteEndPoint.ToString(), readsize);

                        if (readsize > 0)
                        {
                            byte[] result = new byte[readsize];
                            Buffer.BlockCopy(target.DataBuffer, 0, result, 0, readsize);
                            mReceiveQueue.Enqueue(result);
                            target.WorkSocket.BeginReceive(target.DataBuffer, 0, target.DataBuffer.Length, System.Net.Sockets.SocketFlags.None, new AsyncCallback(ReceiveCallBack), target);
                        }
                        else
                        {

                        }
                    }
                }
                catch (Exception ets)
                {
                    Console.WriteLine(ets.Message);
                }
            }

            protected override void begin()
            {
                try
                {
                    mSocket.BeginConnect(mAddress, mPort, pResult =>
                    {
                        if (pResult.AsyncState != null)
                        {
                            System.Net.Sockets.Socket target = pResult.AsyncState as System.Net.Sockets.Socket;
                            if (target != null)
                            {
                                target.EndConnect(pResult);
                                Console.WriteLine("Socket connected to {0}", target.RemoteEndPoint.ToString());

                                SessionWrite(target);
                            }
                        }
                    }, this.mSocket);
                }
                catch
                {

                }
            }

            protected override void end()
            {
                try
                {
                    mReceiveQueue.Clear();
                    mSocket.BeginDisconnect(true, pResult =>
                    {
                        if (pResult.AsyncState != null)
                        {
                            System.Net.Sockets.Socket target = pResult.AsyncState as System.Net.Sockets.Socket;
                            if (target != null)
                            {
                                target.EndDisconnect(pResult);
                            }
                        }
                    }, this.mSocket);
                }
                catch
                {

                }
            }

            protected override void loop()
            {
                try
                {
                    if (mReceiveQueue.Count > 0)
                    {
                        mReceiveAction(mReceiveQueue.Dequeue());
                    }
                }
                catch
                {

                }
            }

            private class SocketObject
            {
                public System.Net.Sockets.Socket WorkSocket = null;
                public const int BufferSize = 65536;
                public byte[] DataBuffer = new byte[BufferSize];
                public StringBuilder StrBuilder = new StringBuilder();
            }
        }
    }
}