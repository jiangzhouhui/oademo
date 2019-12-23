using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class ExpenseAuditConfigurationModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string b_RoleName { get; set; }

        /// <summary>
        /// 报销类型
        /// </summary>
        public string b_Type { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string b_CompanyCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 成本中心
        /// </summary>
        public string b_CostCenters { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string b_HandlePersons { get; set; }

        /// <summary>
        /// 是否在用
        /// </summary>
        public string b_IsInUse { get; set; }

    }
}