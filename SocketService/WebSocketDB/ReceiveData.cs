using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket.Model;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace WebSocketDB
{
    public class ReceiveData : IRec
    {
        //数据库连接字符串
        public static readonly string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        /// <summary>
        /// GPS数据插入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertGPSData(LoginModel model)
        {
            string sql = "insert into obd_position(ctime,deleted,mtime,obdid,lng,lat) values(@ctime,@deleted,@mtime,@obdid,@lng,@lat)";

            MySqlParameter[] pram = new MySqlParameter[]
              {
                  new MySqlParameter("@ctime",model.Date.AddHours(8)),
                  new MySqlParameter("@deleted","0"),
                  new MySqlParameter("@mtime",DateTime.Now),
                  new MySqlParameter("@obdid",model.DeviceId),
                  new MySqlParameter("@lng",model.lng),
                  new MySqlParameter("@lat",model.lat)
              };

            int result = MySqlHelper.ExecuteNonQuery(connStr, System.Data.CommandType.Text, sql, pram);

            if (result > 0)
            {
                return result;
            }
            else
            {
                return 0;
            }

        }
        /// <summary>
        /// 原始数据插入
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public int InsertRawData(string content,string deviceId)
        {
            string sql = "insert into obd_data(ctime,deleted,mtime,data,obdid) values(@ctime,@deleted,@mtime,@data,@obdid)";

            MySqlParameter[] pram = new MySqlParameter[]
              {
                  new MySqlParameter("@ctime",DateTime.Now),
                  new MySqlParameter("@deleted","0"),
                  new MySqlParameter("@mtime",null),
                  new MySqlParameter("@data",content),
                  new MySqlParameter("@obdid",deviceId),
              };

            int result = MySqlHelper.ExecuteNonQuery(connStr, System.Data.CommandType.Text, sql, pram);

            if (result > 0)
            {
                return result;
            }
            else
            {
                return 0;
            }
        }
    }
}
