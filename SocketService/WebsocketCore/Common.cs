using System;
using System.Net;
using System.Net.Sockets;

namespace WebSocketCore
{
    /// <summary>
    /// 公共方法封装
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 通讯ID长度
        /// </summary>
        public const byte IdLength = 36;
        
        /// <summary>
        /// 将一个数组中的连续部分复制到另一数组
        /// </summary>
        /// <param name="source">源</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">长度</param>
        public static byte[] CopyArrayData(byte[] source,int startIndex, int length)
        {
            byte[] result = new byte[length];

            for (var i = 0; i < length; i++)
            {
                result[i] = source[startIndex + i];
            }

            return result;
        }
    }
}
