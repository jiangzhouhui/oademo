using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinEntity
{
    /// <summary>
    /// 委托的权限对象
    /// </summary>
    public class AgentAuthEntity
    {

        /// <summary>
        /// 委托人
        /// </summary>
        public string delegateName { get; set; }

        /// <summary>
        /// 委托人的权限
        /// </summary>
        public List<string> agentRoles { get; set; }

        /// <summary>
        /// 委托的模块内容
        /// </summary>
        public List<string> moduleNames { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
    }
}
