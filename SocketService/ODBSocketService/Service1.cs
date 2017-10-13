using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WebSocketCore;
using System.IO;

namespace ODBSocketService
{
    public partial class Service1 : ServiceBase
    {
        SocketListen listen;
        public Service1()
        {
            InitializeComponent();
        }
        private string ipAdress = ServiceTools.GetAppSetting("ipAdress");
        private int port = int.Parse(ServiceTools.GetAppSetting("port"));
        protected override void OnStart(string[] args)
        {
            #region 
            string path = @"C:\Logs\";
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            #endregion
            listen = new SocketListen(ipAdress, port);
            listen.Start();
        }

        protected override void OnStop()
        {
            listen.Stop();

        }
    }
}
