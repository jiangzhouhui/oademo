using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinCommon
{
    public class Connection
    {
        /// <summary>
        /// 获取或设置数据库连接串
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置是否记录错误日志
        /// </summary>
        public bool ErrorLog
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置错误日志的文件夹路径
        /// </summary>
        internal string ErrorLogPath
        {
            get;
            set;
        }
    }
}
