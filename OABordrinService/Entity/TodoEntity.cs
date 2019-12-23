using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinService.Entity
{
    public class TodoEntity
    {
        /// <summary>
        /// 单据名称
        /// </summary>
        public string RecordName { get; set; }
        
        /// <summary>
        /// 申请人
        /// </summary>
        public string Applicant { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string FlowStatus { get; set; }
        
        /// <summary>
        /// 申请日期
        /// </summary>
        public string ApplicationDate { get; set; }

        /// <summary>
        /// 当前处理人
        /// </summary>
        public string AuditorStr { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string LinkStr { get; set; }

    }
}