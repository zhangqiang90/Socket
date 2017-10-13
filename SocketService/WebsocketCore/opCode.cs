using System;
using System.Collections.Generic;
using System.Text;

namespace WebSocketCore
{
    /*
      4位操作码，定义有效负载数据，如果收到了一个未知的操作码，连接也必须断掉，以下是定义的操作码：
      *  %x0 表示连续消息片断
      *  %x1 表示文本消息片断
      *  %x2 表未二进制消息片断
      *  %x3-7 为将来的非控制消息片断保留的操作码
      *  %x8 表示连接关闭
      *  %x9 表示心跳检查的ping
      *  %xA 表示心跳检查的pong
      *  %xB-F 为将来的控制消息片断的保留操作码
    */
    public class OpCode
    {
        public const int Text = 1;
        public const int Binary = 2;
        public const int Close = 8;
        public const int Ping = 9;
        public const int Pong = 10;
    }
}
