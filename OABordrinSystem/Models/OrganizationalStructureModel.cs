using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class OrganizationalStructureModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string b_NodeName { get; set; }


        /// <summary>
        /// 节点代号
        /// </summary>
        public string b_NodeCode { get; set; }

        /// <summary>
        /// 上级节点
        /// </summary>
        public string b_ParentNodeCode { get; set; }

        /// <summary>
        /// 节点负责人
        /// </summary>
        public string b_NodePersonName { get; set; }

        /// <summary>
        ///节点等级
        /// </summary>
        public int? b_NodeLevel { get; set; }

        /// <summary>
        /// 成本中心编码
        /// </summary>
        public string b_CostCenter { get; set; }


        /// <summary>
        /// 公司代码
        /// </summary>
        public string b_CompanyCode { get; set; }


        /// <summary>
        /// 序号
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// 父节点序号
        /// </summary>
        public int ParentNumber { get; set; }





    }
}