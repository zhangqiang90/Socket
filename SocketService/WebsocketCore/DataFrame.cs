using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketCore
{
    /// <summary>
    /// 数据帧
    /// </summary>
    public class DataFrame
    {
        DataFrameHeader _header;
        private byte[] _extend = new byte[0];
        private byte[] _mask = new byte[0];
        private byte[] _content = new byte[0];

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>主要用于解析接收数据</remarks>
        public DataFrame(byte[] buffer)
        {
            //格式化帧头
            _header = new DataFrameHeader(buffer);
            //填充扩展长度字节
            if (_header.Length == 126)
            {
                _extend = new byte[2];
                Buffer.BlockCopy(buffer, 2, _extend, 0, 2);
            }
            else if (_header.Length == 127)
            {
                _extend = new byte[8];
                Buffer.BlockCopy(buffer, 2, _extend, 0, 8);
            }
            //是否有掩码
            if (_header.HasMask)
            {
                _mask = new byte[4];
                Buffer.BlockCopy(buffer, _extend.Length + 2, _mask, 0, 4);
            }            
            //消息体
            if (_extend.Length == 0)
            {
                _content = new byte[_header.Length];
                Buffer.BlockCopy(buffer, _extend.Length + _mask.Length + 2 , _content, 0, _content.Length);
            }
            else if (_extend.Length == 2)
            {

                string sExtendLen = "";
                foreach (byte InByte in _extend)
                {
                    sExtendLen = sExtendLen + String.Format("{0:X2}", InByte);
                }


                _content = new byte[int.Parse(sExtendLen, System.Globalization.NumberStyles.AllowHexSpecifier)];
      
                Buffer.BlockCopy(buffer, _extend.Length + _mask.Length + 2, _content, 0, _content.Length);
            }
            else
            {
                _content = new byte[Convert.ToUInt64(Common.CopyArrayData(buffer, 2, 8))];
                Buffer.BlockCopy(buffer, _extend.Length + _mask.Length + 2, _content, 0, _content.Length);
            }
            //如果有掩码，则需要还原原始数据
            if (_header.HasMask) _content = Mask(_content, _mask);

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>主要用于发送封装数据</remarks>
        public DataFrame(string content)
        {
            _content = Encoding.UTF8.GetBytes(content);
            int length = _content.Length;
            
            if (length < 126)
            {
                _extend = new byte[0];
                _header = new DataFrameHeader(true, false, false, false, OpCode.Text, false, length);
            }
            else if (length < 65536)
            {
                _extend = new byte[2];
                _header = new DataFrameHeader(true, false, false, false, OpCode.Text, false, 126);
                _extend[0] = (byte)(length / 256);
                _extend[1] = (byte)(length % 256);
            }
            else
            {
                _extend = new byte[8];
                _header = new DataFrameHeader(true, false, false, false, OpCode.Text, false, 127);

                int left = length;
                int unit = 256;

                for (int i = 7; i > 1; i--)
                {
                    _extend[i] = (byte)(left % unit);
                    left = left / unit;

                    if (left == 0)
                        break;
                }
            }
        }

        /// <summary>
        /// 获取适合传送的字节数据
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] buffer = new byte[2 + _extend.Length + _mask.Length + _content.Length];
            Buffer.BlockCopy(_header.GetBytes(), 0, buffer, 0, 2);
            Buffer.BlockCopy(_extend, 0, buffer, 2, _extend.Length);
            Buffer.BlockCopy(_mask, 0, buffer, 2 + _extend.Length, _mask.Length);
            Buffer.BlockCopy(_content, 0, buffer, 2 + _extend.Length + _mask.Length, _content.Length);
            return buffer;
        }
        
        /// <summary>
        /// 获取文本
        /// </summary>
        public string Text 
        { 
            get 
            {
                if (_header.OpCode != OpCode.Text)
                    return string.Empty;

                return Encoding.UTF8.GetString(_content); 
            } 
        }

        /// <summary>
        /// 加掩码运算
        /// </summary>
        private byte[] Mask(byte[] data, byte[] mask)
        {
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(data[i] ^ mask[i % 4]);
            }

            return data;
        }

    }
}
