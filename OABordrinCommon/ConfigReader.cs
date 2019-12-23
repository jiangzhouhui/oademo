using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OABordrinCommon
{
    public class ConfigReader
    {
        //暂存配置信息
        private Dictionary<string, XmlDocument> _ConfigList = new Dictionary<string, XmlDocument>();

        //构造函数
        private ConfigReader()
        {
            // 读取配置文件，存放到字典中
            string strAppDomainDirPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string strMainConfigPath = System.IO.Path.Combine(strAppDomainDirPath, @"config\path.config");
            if (!File.Exists(strMainConfigPath))
            {
                return;
            }

            // 读取主配置文件
            XmlDocument docMainConfig = new XmlDocument();
            try
            {
                docMainConfig.Load(strMainConfigPath);
                XmlElement xmlele = docMainConfig.DocumentElement;
                XmlNodeList xmlnodelist = xmlele.SelectNodes("module");
                foreach (XmlElement item in xmlnodelist)
                {
                    if (item.Attributes.Count == 0)
                        continue;
                    if (item.Attributes["name"] != null)
                    {
                        if (item.Attributes["configFileName"] != null)
                        {
                            XmlDocument xmlDoc = GetXmlDoc(Path.Combine(strAppDomainDirPath, @"config\" + item.Attributes["configFileName"].Value));
                            _ConfigList.Add(item.Attributes["name"].Value, xmlDoc);
                        }
                        if (item.Attributes["dbConfigFileName"] != null)
                        {
                            XmlDocument xmlDoc = GetXmlDoc(Path.Combine(strAppDomainDirPath, @"config\" + item.Attributes["dbConfigFileName"].Value));
                            _ConfigList.Add("db" + item.Attributes["name"].Value, xmlDoc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志
                throw ex;
            }
        }

        /// <summary>
        /// 取出XML文档
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private XmlDocument GetXmlDoc(string filepath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            return xmlDoc;
        }

        /// <summary>
        /// 查找节点的值
        /// /// 调用方法：ConfigReader.Create().GetConfig(@"InviteUrl","InviteUrls/Invite")
        /// </summary>
        /// <param name="moduleName">配置的模块名称</param>
        /// <param name="xpath">要查找的节点</param>
        /// <returns>返回节点的值</returns>
        public static string GetConfig(string moduleName, string config)
        {
            ConfigReader cr = ConfigReader.Create();
            if (!cr._ConfigList.ContainsKey(moduleName))
            {
                return string.Empty;
            }

            XmlDocument doc = cr._ConfigList[moduleName];

            XmlElement xmlEle = doc.DocumentElement;
            XmlNode node = xmlEle.SelectSingleNode(config);
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerText;
        }

        public static string GetConfigXML(string moduleName, string config)
        {
            ConfigReader cr = ConfigReader.Create();
            if (!cr._ConfigList.ContainsKey(moduleName))
            {
                return string.Empty;
            }

            XmlDocument doc = cr._ConfigList[moduleName];

            XmlElement xmlEle = doc.DocumentElement;
            XmlNode node = xmlEle.SelectSingleNode(config);
            if (node == null)
            {
                return string.Empty;
            }

            return node.InnerXml;
        }

        /// <summary>
        /// 查找一组节点名称相同 类似 appsetting 属性句称局限以 key / value
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="config">节点路径</param>
        /// <returns>返回值</returns>
        public static Dictionary<string, string> GetDictionary(string moduleName, string config)
        {
            ConfigReader cr = ConfigReader.Create();
            if (!cr._ConfigList.ContainsKey(moduleName))
                return null;
            XmlDocument doc = cr._ConfigList[moduleName];
            XmlElement xmlEle = doc.DocumentElement;
            XmlNodeList xmlNodeList = xmlEle.SelectNodes(config);
            Dictionary<string, string> _dicParams = new Dictionary<string, string>();
            foreach (XmlNode item in xmlNodeList)
            {
                if (item.Attributes["key"] != null && item.Attributes["value"] != null)
                    _dicParams.Add(item.Attributes["key"].Value, item.Attributes["value"].Value);
            }
            return _dicParams;
        }
        /// <summary>
        /// 查找一组节点名称相同,根据节点属名称自动创建列
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="config">节点路径</param>
        /// <returns>返回一个datatable,</returns>
        public static DataTable GetDataTable(string moduleName, string config)
        {
            ConfigReader cr = ConfigReader.Create();
            if (!cr._ConfigList.ContainsKey(moduleName))
                return null;
            XmlDocument doc = cr._ConfigList[moduleName];
            XmlElement xmlEle = doc.DocumentElement;
            XmlNodeList xmlNodeList = xmlEle.SelectNodes(config);
            DataTable dt = new DataTable();
            foreach (XmlNode item in xmlNodeList)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < item.Attributes.Count; i++)
                {
                    if (!dt.Columns.Contains(item.Attributes[i].Name))
                    {
                        DataColumn dataColumn = new DataColumn(item.Attributes[i].Name);
                        dt.Columns.Add(dataColumn);
                    }
                    dr[item.Attributes[i].Name] = item.Attributes[i].Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 返回一个泛型列表
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="config">节点路径</param>
        /// <returns>返回一个泛型</returns>
        public static IList<T> GetConfigList<T>(string moduleName, string config) where T : class, new()
        {
            ConfigReader cr = ConfigReader.Create();
            if (!cr._ConfigList.ContainsKey(moduleName))
                return null;
            XmlDocument doc = cr._ConfigList[moduleName];
            XmlElement xmlEle = doc.DocumentElement;
            XmlNodeList xmlNodeList = xmlEle.SelectNodes(config);
            if (xmlNodeList == null)
                return null;
            IList<T> _list = new List<T>();
            Type type = typeof(T);
            System.Reflection.PropertyInfo[] p = type.GetProperties();
            foreach (XmlNode item in xmlNodeList)
            {
                T obj = new T();
                foreach (PropertyInfo property in p)
                {
                    if (item.Attributes[property.Name] == null)
                        continue;

                    string temp = item.Attributes[property.Name].Value;
                    object propertyValue;
                    if (property.PropertyType == typeof(int))
                    {
                        // 整型
                        int result;
                        int.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(short))
                    {
                        // short型
                        short result;
                        short.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(long))
                    {
                        // 长整型
                        long result;
                        long.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(byte))
                    {
                        // byte型
                        byte result;
                        byte.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        // bool类型
                        bool result;
                        bool.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        // DateTime类型
                        DateTime result;
                        DateTime.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(decimal))
                    {
                        // decimal类型
                        decimal result;
                        decimal.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        // float类型
                        float result;
                        float.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        // double类型
                        double result;
                        double.TryParse(temp, out result);
                        propertyValue = result;
                    }
                    else
                    {
                        // 其它类型，当它是字符串
                        propertyValue = temp;
                    }

                    if (item.Attributes[property.Name] != null)
                        property.SetValue(obj, propertyValue, null);
                }
                _list.Add(obj);
            }
            return _list;
        }
        private static ConfigReader _ConfigReader;
        private static System.IO.FileSystemWatcher watcher;
        private static Object _lock = new object();
        /// <summary>
        /// 返回ConfigReader 对象（单件模式）
        /// </summary>
        /// <returns></returns>
        private static ConfigReader Create()
        {
            if (_ConfigReader != null)
                return _ConfigReader;
            if (watcher == null)
            {
                watcher = new System.IO.FileSystemWatcher();
                watcher.Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
                watcher.Filter = "*.config";
                watcher.Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
                watcher.EnableRaisingEvents = true;
                watcher.IncludeSubdirectories = true;
            }
            if (_ConfigReader == null)
            {
                lock (_lock)
                {
                    _ConfigReader = new ConfigReader();
                }
            }

            return _ConfigReader;
        }

        static void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            _ConfigReader = null;
        }
    }
}
