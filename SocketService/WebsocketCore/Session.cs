using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.IO;
using WebSocketDB;

namespace WebSocketCore
{
    /// <summary>
    /// 每一个连接的客户端
    /// </summary>
    public class Session
    {
        /// <summary>
        /// 客户端SOCKET
        /// </summary>
        public Socket _workSocket = null;
        private int _id;
        private string _name;
        private const int DefaultBufferSize = 4096;
        private byte[] RecvDataBuffer;

        public event EventHandler<ChatEventArgs> ChatEvent;

        /// <summary>
        /// 客户端ID
        /// </summary>
        public int DevId { get { return _id; } }

        public string DevName { get { return _name; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Session(int id, Socket socket)
        {
            this._workSocket = socket;
            this._id = id;
            RecvDataBuffer = new byte[DefaultBufferSize];
        }

        /// <summary>
        /// 开启独自数据处理
        /// </summary>
        public void Start()
        {

            //开始接受来自该客户端的数据 
            _workSocket.BeginReceive(RecvDataBuffer, 0, RecvDataBuffer.Length, SocketFlags.None,
                new AsyncCallback(AcceptCallBack), this);
        }

        /// <summary>
        /// 往关联的客户端发送数据
        /// </summary>
        public void Response(string message)
        {
            _workSocket.Send(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// 往关联的客户端发送数据
        /// </summary>
        public void Send(string message)
        {
            DataFrame dr = new DataFrame(message);
            var result = _workSocket.Send(dr.GetBytes());
        }

        public void Send(byte[] message)
        {
            _workSocket.Send(message);
        }

        /// <summary>
        /// 断开与客户端的连接
        /// </summary>
        public void Shutdown()
        {
            try
            {
                _workSocket.Shutdown(SocketShutdown.Both);
                _workSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 应客户端要求建立连接后的处理线程
        /// </summary>
        /// <param name="ia"></param>
        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                //判断是否是websocket或socket
                //bool isWebsocket = true;
                //定义一个新的客户端对象
                Session state = (Session)ar.AsyncState;
                Socket client = state._workSocket;

                #region === 消息处理 ===

                //登录成功,则进入消息等待中
                int RevNumber = 0;

                if (client.Connected)
                {
                    #region === 正常或非正常的断线处理 ===

                    try
                    {
                        RevNumber = client.EndReceive(ar);
                        if (RevNumber == 0) { return; }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        using (StreamWriter sw = new StreamWriter(@"C:\Logs\job13" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(ex.Message);
                        }
                        return;
                    }

                    #endregion
                    try
                    {
                        DataHeader dh = new DataHeader(RecvDataBuffer,RevNumber);
                        #region 原始数据插入数据库
                        IRec irec1 = new ReceiveData();
                        irec1.InsertRawData(DataHander.ByteToString(dh.Content).Replace(" ", ""), "");
                        #endregion


                        if (dh.IsValid)
                        {
                            IRec irec = new ReceiveData();
                            switch (dh.Type)
                            {
                                case "1001":
                                    var model = DataHander.LoginHander(dh);
                                    irec.InsertGPSData(model);
                                    break;
                                case "4001":
                                    var model1 = DataHander.GPSHander(dh);
                                    irec.InsertGPSData(model1);
                                    break;
                                default:
                                    break;
                            }


                        }
                    }
                    catch(Exception ex)
                    {
                        using (StreamWriter sw = new StreamWriter(@"C:\Logs\job7" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(ex.Message);
                        }
                    }
                    //继续接收来自来客户端的数据 
                    client.BeginReceive(RecvDataBuffer, 0, RecvDataBuffer.Length, SocketFlags.None,
                        new AsyncCallback(AcceptCallBack), state);

                }
                #endregion

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                using (StreamWriter sw = new StreamWriter(@"C:\Logs\job6" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }
    }
}
