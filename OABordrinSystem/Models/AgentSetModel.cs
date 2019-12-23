using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class AgentSetModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

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
        public string b_StartDate { get; set; }

        /// <summary>
        /// 时分秒开始时间
        /// </summary>
        public string b_StartDateMinute { get; set; }

        /// <summary>
        /// 终止日期
        /// </summary>
        public string b_EndDate { get; set; }

        /// <summary>
        /// 时分秒结束时间
        /// </summary>
        public string b_EndDateMinute { get; set; }

        /// <summary>
        /// 代理模块
        /// </summary>
        public string b_AgentContent { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public string b_IsValid { get; set; }

        /// <summary>
        /// 代理原因
        /// </summary>
        public string b_AgentReason { get; set; }


        /// <summary>
        /// 授权模块列表
        /// </summary>
        public List<string> AgentModuleList { get; set; }
    }
}