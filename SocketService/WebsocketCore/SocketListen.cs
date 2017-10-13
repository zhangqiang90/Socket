using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System;
using System.Text;
using System.IO;
using WebSocketDB;

namespace WebSocketCore
{
    public class SocketListen
    {
        #region === 私有变量 ===

        private IPAddress m_serverip;               //绑定的IP地址
        private int m_serverport;                   //使用的端口
        private Socket m_server = null;             //用于侦听的Socket套接字
        private Hashtable m_clients;                //客户端列表
        private bool Started = false;               //运行中
        private int m_id = 1;                       //新分配给客户端的Id
        private int num = 0;

        private ManualResetEvent LoginValidataDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);

        #endregion

        #region === 公开属性 ===

        /// <summary>
        /// 使用的服务器IP
        /// </summary>
        public IPAddress SVRIP { get { return m_serverip; } }

        /// <summary>
        /// 使用的服务器端口
        /// </summary>
        public int SVRPort { get { return m_serverport; } }

        /// <summary>
        /// 返回在线客户端数
        /// </summary>
        public int OnlineNumber { get { return m_clients.Count; } }

        /// <summary>
        /// 服务器运行状态
        /// </summary>
        public bool RunState { get { return Started; } }

        #endregion

        #region === 公开方法 ===

        /// <summary>
        /// 构造函数
        /// </summary>
        public SocketListen()
        {
            m_serverip = IPAddress.Parse("127.0.0.1");
            m_serverport = 1005;
            m_clients = new Hashtable();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ServerIP">服务器IP地址</param>
        /// <param name="ServerPort">服务器端口</param>
        public SocketListen(string ServerIP, int ServerPort)
        {
            m_serverip = IPAddress.Parse(ServerIP);
            m_serverport = ServerPort;
            m_clients = new Hashtable();
        }

        /// <summary>
        /// 开始侦听
        /// </summary>
        public void Start()
        {
            try
            {
                //绑定端口,启动侦听
                IPEndPoint ipe = new IPEndPoint(m_serverip, m_serverport);
                m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_server.Bind(ipe);
                m_server.Listen(10);
                //设置异步方法接受客户端连接 
                m_server.BeginAccept(new AsyncCallback(AcceptConn), m_server);

                Started = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Started = false;
                using (StreamWriter sw = new StreamWriter(@"C:\Logs\job4" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            try
            {
                Started = false;

                foreach (Session co in m_clients.Values)
                {
                    co.Shutdown();
                }
                m_clients.Clear(); m_clients = null;

                m_server.Close();
                //强制垃圾回收处理
                GC.Collect();
                GC.SuppressFinalize(this);
                Thread.Sleep(1000);
            }
            catch
            {
            }
            finally
            {
                try
                {
                    m_server.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// 所有在线的客户端列表
        /// </summary>
        /// <returns></returns>
        public string OnlineClients
        {
            get
            {
                string strList = string.Empty;
                foreach (DictionaryEntry de in m_clients)
                {
                    Session session = (Session)de.Value;
                    strList += session.DevId.ToString() + "|" + session.DevName + ";";
                }
                return strList.Trim(';');
            }
        }

        #endregion

        #region === 私有方法 ===

        private void AcceptConn(IAsyncResult ar)
        {
            try
            {
                //如果服务器停止了服务,就不能再接收新的客户端 
                if (!Started)
                    return;

                //缓冲区的大小
                byte[] buffer = new byte[512];

                //获取用户定义的对象，它限定或包含关于异步操作的信息。
                Socket listener = (Socket)ar.AsyncState;
                //异步接受传入的连接尝试，并创建新的 Socket 来处理远程主机通信。
                Socket client = listener.EndAccept(ar);

                Session state = new Session(m_id++, client);
                state.ChatEvent += new EventHandler<ChatEventArgs>(state_ChatEvent);

                //登录认证,只有等ReceiveLoginValidateCallback执行完后才会继续
                LoginValidataDone.Reset();
                client.BeginReceive(buffer, 0, 512, SocketFlags.None, new AsyncCallback(ReceiveLoginValidateCallback), client);
                LoginValidataDone.WaitOne();

               
                DataHeader dh = new DataHeader(buffer, num);

                IRec irec = new ReceiveData();
                irec.InsertRawData(DataHander.ByteToString(dh.Content).Replace(" ", ""), "");

                Handshake hh = new Handshake(Encoding.UTF8.GetString(buffer, 0, 512));

                //进行一个初步的协议判定，如果不是websocket协议，直接断开,忽略掉
                if (hh.GetValue("Upgrade").ToLower() != "websocket"&& hh.GetValue("Upgrade").ToLower() != "")
                {
                    try
                    {

                        client.Shutdown(SocketShutdown.Both); client.Close();

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex.Message);
                        using (StreamWriter sw = new StreamWriter(@"C:\Logs\job1" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(ex.Message);
                        }
                    }
                    //继续接受客户端 
                    m_server.BeginAccept(new AsyncCallback(AcceptConn), m_server);
                    return;
                }
                if (dh.Type == "1001")
                {

                    state.Send(dh.GetLoginResponse());
                    try
                    {
                        var model = DataHander.LoginHander(dh);
                        irec.InsertGPSData(model);
                    }
                    catch(Exception)
                    {

                    }
                }
                else
                {
                    //服务器作出握手回应
                    state.Response(hh.Response);
                }


                state.Start();

                //将客户端信息置于列表中
                if (hh.GetValue("Upgrade").ToLower() == "websocket")
                {
                    m_clients.Add(state.DevId, state);
                }


                //继续接受客户端 
                m_server.BeginAccept(new AsyncCallback(AcceptConn), m_server);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //设置异步方法接受客户端连接 
                m_server.BeginAccept(new AsyncCallback(AcceptConn), m_server);
                using (StreamWriter sw = new StreamWriter(@"C:\Logs\job2" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        void state_ChatEvent(object sender, ChatEventArgs e)

        {
            if (e.CmdId == "MSG")
            {
                if (e.ToString().Length > 0)
                {

                    string sMsg = e.ToString().Split('|')[1];

                    SendToAllDev(e.From, sMsg);

                    insertSendData(sMsg);

                }

            }
            if (e.CmdId == "Rec")
            {

                if (e.ToString().Length > 0)
                {
                    insertRecData(e.ToString().Split('|')[1]);
                }
            }

        }

        /// <summary>
        /// 将页面发送的数据写入数据库
        /// </summary>
        /// <param name="msg"></param>
        public void insertSendData(string msg)
        {

            //ISend isend = new SendData();

            //SendDataModel model = new SendDataModel();

            //model.DtuData = msg;
            //model.IsSuccess = 1;
            //model.SendTime = DateTime.Now;

            //isend.insertSendData(model);
        }
        /// <summary>
        /// 将DTU上传的数据写入数据库
        /// </summary>
        /// <param name="msg"></param>
        public void insertRecData(string sMsg)
        {
            if (sMsg.Length > 0)
            {
                //IRec irec = new ReceiveData();

                //ReceiveDataModel model = new ReceiveDataModel();

                //model.DtuData = sMsg;
                //model.RecTime = DateTime.Now;

                //irec.insertReceiveData(model);
            }
        }
        /// <summary>
        /// 通知所有设备
        /// </summary>
        /// <param name="userId">不需要通知的客户端</param>
        /// <param name="buffer">要发送的字节数组</param>
        private void SendToAllDev(int fromDevId, string sMsg)
        {
            try
            {
                //发送数据到全部设备
                foreach (Session co in m_clients.Values)
                {
                    try
                    {
                        if (co.DevName == fromDevId.ToString())
                        {
                            co.Send(sMsg);
                            using (StreamWriter sw = new StreamWriter(@"C:\Logs\job11" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                            {
                                sw.WriteLine(sMsg);
                                sw.Write(co._workSocket.AddressFamily);
                            }
                        }
                        Thread.Sleep(20);
                    }
                    catch(Exception e)
                    {
                        using (StreamWriter sw = new StreamWriter(@"C:\Logs\job12" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                        {
                            sw.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(@"C:\Logs\job3" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveLoginValidateCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                num= client.EndReceive(ar);
                LoginValidataDone.Set();
            }
            catch (Exception ex)
            {
                LoginValidataDone.Set();
                Console.WriteLine(ex.Message);
                using (StreamWriter sw = new StreamWriter(@"C:\Logs\job5" + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }

        #endregion
    }
}
