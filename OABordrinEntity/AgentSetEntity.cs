using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinEntity
{
    public class AgentSetEntity
    {
        /// <summary>
        /// 委托人
        /// </summary>
        public string b_DelegateName { get; set; }

        /// <summary>
        /// 代理人
        /// </summary>
        public string b_AgentName { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        public DateTime b_StartDate { get; set; }

        /// <summary>
        /// 终止日期
        /// </summary>
        public DateTime b_EndDate { get; set; }

        /// <summary>
        /// 代理模块
        /// </summary>
        public string b_AgentContent { get; set; }

        /// <summary>
        /// 代理原因
        /// </summary>
        public string b_AgentReason { get; set; }

    }
}
