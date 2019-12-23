using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class TodoModel
    {
        /// <summary>
        /// id编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string RecordNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime create_on { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTimeStr { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public string activityId { get; set; }

        /// <summary>
        /// 活动权限ID
        /// </summary>
        public string activityAssignmentId { get; set; }

    }
}