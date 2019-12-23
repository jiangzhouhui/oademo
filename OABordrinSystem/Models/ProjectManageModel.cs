using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class ProjectManageModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string b_ProjectRecordNo { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// b_PmtOrPatLeaderId
        /// </summary>
        public string b_PmtOrPatLeaderId { get; set; }

        /// <summary>
        /// PMT/PAT Leader
        /// </summary>
        public string b_PmtOrPatLeader { get; set; }


        /// <summary>
        /// 项目总监Id
        /// </summary>
        public string b_ProjectDirectorId { get; set; }

        /// <summary>
        /// 项目总监
        /// </summary>
        public string b_ProjectDirector { get; set; }

        /// <summary>
        /// 项目经理Id
        /// </summary>
        public string b_ProjectManagerId { get; set; }


        /// <summary>
        /// 项目经理
        /// </summary>
        public string b_ProjectManager { get; set; }

        /// <summary>
        /// 是否在用
        /// </summary>
        public string b_IsInUse { get; set; }

        /// <summary>
        /// 项目VP
        /// </summary>
        public string b_ProjectVP { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public int b_Sort { get; set; }

        /// <summary>
        /// Pr是否使用
        /// </summary>
        public string b_PrIsUse { get; set; }

        /// <summary>
        /// 适用的公司
        /// </summary>
        public string b_ApplicableCompany { get; set; }

    }
}