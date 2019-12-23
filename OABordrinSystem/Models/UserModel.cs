using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class UserModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string first_Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string b_JobNumber { get; set; }


        /// <summary>
        /// 中文名称
        /// </summary>
        public string b_ChineseName { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string b_EnglishName { get; set; }

        /// <summary>
        /// 中心
        /// </summary>
        public string b_Centre { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string b_Department { get; set; }

        /// <summary>
        /// 中心领导
        /// </summary>
        public string b_CentreLeader { get; set; }


        /// <summary>
        /// 部门领导
        /// </summary>
        public string b_DepartmentLeader { get; set; }


        /// <summary>
        /// 直属领导
        /// </summary>
        public string b_LineLeader { get; set; }


        /// <summary>
        /// 公司代码
        /// </summary>
        public string b_CompanyCode { get; set; }

        /// <summary>
        /// 成本中心代码
        /// </summary>
        public string b_CostCenter { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string b_IdNumber { get; set; }

        /// <summary>
        /// 高级经理
        /// </summary>
        public string b_SeniorManager { get; set; }

        /// <summary>
        /// 总监
        /// </summary>
        public string b_Director { get; set; }

        /// <summary>
        /// VP
        /// </summary>
        public string b_VP { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string b_AffiliatedCompany { get; set; }

    }
}