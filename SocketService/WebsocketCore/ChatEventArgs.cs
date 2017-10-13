using System;

namespace WebSocketCore
{
    /// <summary>
    /// 事件参数
    /// </summary>
    public class ChatEventArgs : EventArgs
    {        
        /// <summary>
        /// 命令码
        /// </summary>
        public string CmdId { get; set; }

        /// <summary>
        /// 消息来源
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// 消息去向
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChatEventArgs()
        {
            this.CmdId = string.Empty;
            this.From = 0;
            this.To = 0;
            this.Content = string.Empty;
        }

        public override string ToString()
        {
            return String.Format("{0},{1},{2},{3}", this.CmdId, this.From, this.To, this.Content);
        }
    }
}
