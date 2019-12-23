using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace OABordrinEntity
{
    public class ApiExpenseReimbursementEntity
    {
        /// <summary>
        /// 公司代码
        /// </summary>
        public string BUKRS { get; set; }

        /// <summary>
        /// 报销单单号
        /// </summary>
        public string XBLNR { get; set; }


        /// <summary>
        /// 凭证日期
        /// </summary>
        public string BLDAT { get; set; }

        /// <summary>
        /// 过账日期
        /// </summary>
        public string BUDAT { get; set; }

        /// <summary>
        /// 凭证抬头文本
        /// </summary>
        public string BKTXT { get; set; }

        /// <summary>
        /// 页数
        /// </summary>
        public int? NUMPG { get; set; }

        /// <summary>
        /// 项目类型
        /// </summary>
        public string PROTYP { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string HKONT { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? DMBTR { get; set; }

        /// <summary>
        /// 成本中心
        /// </summary>
        public string KOSTL { get; set; }

        /// <summary>
        /// 行项目文本
        /// </summary>
        public string SGTXT { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 内部订单编号
        /// </summary>
        public string AUFNR { get; set; }

        /// <summary>
        /// WBS编号
        /// </summary>
        public string POSID { get; set; }

        /// <summary>
        /// 关闭时间
        /// </summary>
        public DateTime? closed_date { get; set; }

        /// <summary>
        /// 税率
        /// </summary>

        public double? b_TaxRate { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal? b_Tax { get; set; }

        /// <summary>
        /// 不含税金额
        /// </summary>
        public decimal? b_TaxFreeAmount { get; set; }

        /// <summary>
        /// 申请人工号
        /// </summary>
        public string b_StaffNo { get; set; }
    }
}
