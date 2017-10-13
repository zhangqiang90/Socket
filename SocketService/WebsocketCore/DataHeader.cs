using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketCore
{
    public class DataHeader
    {
        /// <summary>
        /// 数据是否为有效数据
        /// </summary>
        private bool isValid = true;
        /// <summary>
        /// 数据长度
        /// </summary>
        private int dataLength;
        /// <summary>
        /// 设备ID
        /// </summary>
        private string deviceId;
        /// <summary>
        /// 版本号
        /// </summary>
        private string version;
        /// <summary>
        /// 数据类型
        /// </summary>
        private string type;
        /// <summary>
        /// 数据长度对应位
        /// </summary>
        private byte[] byteLength = new byte[2];

        private byte[] _deviceId = new byte[0];

        private byte[] _content = new byte[0];
        private byte[] _type = new byte[0];
       
        public bool IsValid { get { return this.isValid; } }
        public int DataLength { get { return this.dataLength; } }
        public string DeviceId { get { return this.deviceId; } }

        public string Version { get { return this.version; } }

        public string Type { get { return this.type; } }

        public byte[] Content { get { return this._content; } }

        public DataHeader(byte[] buffer, int RevNumber)
        {
            if (RevNumber< 31)
            {
                isValid = false;
                return;
            }

            //第一个字节
            isValid = (buffer[0] & 0x40) == 0x40;
            //第二个字节
            isValid = (buffer[1] & 0x40) == 0x40;

            //数据长度高低位
            byteLength[0] = buffer[3];
            byteLength[1] = buffer[2];

            dataLength = RevNumber;

            //数据包尾验证
            isValid = (buffer[dataLength - 1] & 0x0A) == 0x0A;
            isValid = (buffer[dataLength - 2] & 0x0D) == 0x0D;


            //设备ID
            _deviceId= new byte[20];
            Buffer.BlockCopy(buffer,5,_deviceId,0,20);
            //数据类型
            _type = new byte[2];
            Buffer.BlockCopy(buffer, 25, _type,0, 2);
            //数据内容
            _content = new byte[dataLength];
            Buffer.BlockCopy(buffer, 0, _content, 0, dataLength);
          
            deviceId = DataHander.HexStringToASCII(_deviceId).ToString().Trim('\0');

            type = DataHander.ByteToString(_type).Replace(" ", "");

        }
        /// <summary>
        /// 获取数据包长度
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int Length(byte[] bytes)
        {
            string s= DataHander.ByteToString(bytes).Replace(" ","");
            return DataHander.GetHexadecimalValue(s);
           
        }
        /// <summary>
        /// 登录回应包
        /// </summary>
        /// <returns></returns>
        public  byte[] GetLoginResponse()
        {

            TimeSpan DT = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string s= ((int)DT.TotalSeconds).ToString("x8");
            byte[] date =DataHander.ByteOrderByDes(DataHander.hexStringToByte(s));
            StringBuilder sb = new StringBuilder();
            sb.Append("4040290004");
            sb.Append(DataHander.byteToHexStr(this._deviceId).Replace(" ", ""));
            sb.Append("9001FFFFFFFF0000");
            sb.Append(DataHander.byteToHexStr(date).Replace(" ", ""));
            sb.Append("A5DD0D0A");
            byte[] login = DataHander.hexStringToByte(sb.ToString());
            return login; 
        }

    }
}
