using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Model;

namespace WebSocketDB
{

    /// <summary>
    /// 存储接收数据接口
    /// </summary>
    public interface IRec
    {
        int InsertGPSData(LoginModel model);
        int InsertRawData(string contennt,string deviceId);

    }
}
