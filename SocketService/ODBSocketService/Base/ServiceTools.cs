using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ODBSocketService
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class ServiceTools : System.Configuration.IConfigurationSectionHandler
    {
        /// <summary>
        /// 获取AppSettings节点值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }

        /// <summary>
        /// 获取configSections节点
        /// </summary>
        /// <returns></returns>
        public static XmlNode GetConfigSections()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
            return doc.DocumentElement.FirstChild;
        }

        /// <summary>
        /// 获取section节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static NameValueCollection GetSection(string nodeName)
        {
            return (NameValueCollection)ConfigurationManager.GetSection(nodeName);
        }

        /// <summary>
        /// 停止Windows服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        public static void WindowsServiceStop(string serviceName)
        {
            System.ServiceProcess.ServiceController control = new System.ServiceProcess.ServiceController(serviceName);
            control.Stop();
            control.Dispose();
        }



        /// <summary>
        /// 实现接口以读写app.config
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="configContext"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            System.Configuration.NameValueSectionHandler handler = new System.Configuration.NameValueSectionHandler();
            return handler.Create(parent, configContext, section);
        }

    }
}
