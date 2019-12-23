using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class ExpenseReimbursementModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string b_RecordNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string b_CompanyCode { get; set; }

        /// <summary>
        /// 报销所在地
        /// </summary>
        public string b_ReimbursementPlace { get; set; }


        /// <summary>
        /// 是否预算内
        /// </summary>
        public bool b_IsBudgetary { get; set; }

        /// <summary>
        /// 代申请人
        /// </summary>
        public string b_Preparer { get; set; }


        /// <summary>
        /// 代申请人工号
        /// </summary>
        public string b_PreparerNo { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public string b_ApplicationDate { get; set; }

        public DateTime? nb_ApplicationDate { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string b_Employee { get; set; }

        /// <summary>
        /// 申请人工号
        /// </summary>
        public string b_StaffNo { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string b_Position { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string b_Dept { get; set; }

        /// <summary>
        /// 成本中心代码
        /// </summary>
        public string b_CostCenter { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string b_Tel { get; set; }

        /// <summary>
        /// 向公司预支现金
        /// </summary>
        public decimal b_AdvancedAmount { get; set; }


        /// <summary>
        /// 总费用
        /// </summary>
        public decimal b_TotalExpense { get; set; }

        /// <summary>
        /// 应退还公司
        /// </summary>
        public string b_DueCompany { get; set; }

        /// <summary>
        /// 是否挂起
        /// </summary>
        public bool b_IsHangUp { get; set; }


        /// <summary>
        /// 合计(大写)
        /// </summary>
        public string b_AmountInWords { get; set; }

        /// <summary>
        /// 合计(小写)
        /// </summary>
        public decimal b_TotalAmount { get; set; }

        /// <summary>
        /// 报销类型
        /// </summary>
        public string b_Type { get; set; }


        /// <summary>
        /// 直属领导
        /// </summary>
        public string b_LineLeader { get; set; }

        /// <summary>
        /// 部门领导
        /// </summary>
        public string b_DepartmentLeader { get; set; }

        /// <summary>
        /// 中心领导
        /// </summary>
        public string b_DivisionVP { get; set; }

        /// <summary>
        /// 附件张数
        /// </summary>
        public int b_AttachmentsQuantity { get; set; }

        /// <summary>
        /// 挂起活动的名称
        /// </summary>
        public string b_HangUpActivityName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string b_Remark { get; set; }

        /// <summary>
        /// 历史备注
        /// </summary>
        public string OldRemark { get; set; }

        /// <summary>
        /// 当前处理人
        /// </summary>
        public string AuditorStr { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>

        public string status { get; set; }

        /// <summary>
        /// 活动Id
        /// </summary>
        public string activityId { get; set; }

        /// <summary>
        /// 活动授权Id
        /// </summary>
        public string activityAssignmentId { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        public string operation { get; set; }


        /// <summary>
        /// 报销明细
        /// </summary>
        public List<ReimbursementItem> ReimbursementItems { get; set; }

        /// <summary>
        /// 借款明细
        /// </summary>
        public List<LoanItem> LoanItems { get; set; }


        /// <summary>
        /// 日志
        /// </summary>
        public List<HistoryModel> HistoryList { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileModel> Files { get; set; }


        /// <summary>
        /// 文件Id
        /// </summary>
        public List<string> fileIds { get; set; }

    }

    /// <summary>
    /// 报销明细
    /// </summary>
    public class ReimbursementItem
    {
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string b_Date { get; set; }


        public DateTime nb_Date { get; set; }


        /// <summary>
        /// 费用类别名称
        /// </summary>
        public string b_CategoryNumber { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 预算编号
        /// </summary>
        public string b_BudgetNumber { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string b_Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public float b_Rate { get; set; }

        /// <summary>
        /// 原币单价
        /// </summary>
        public decimal b_OriginalCurrency { get; set; }

        /// <summary>
        /// 数量/人数
        /// </summary>
        public int b_Count { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public float b_TaxRate { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal b_Tax { get; set; }

        /// <summary>
        /// 不含税金额
        /// </summary>
        public decimal b_TaxFreeAmount { get; set; }

        /// <summary>
        /// 小计（人民币）
        /// </summary>
        public decimal b_CNYSubtotal { get; set; }

        /// <summary>
        /// 大写
        /// </summary>
        public string b_AmountInWords { get; set; }

        /// <summary>
        /// 小写
        /// </summary>
        public decimal b_Amount { get; set; }

    }


    /// <summary>
    /// 借款明细
    /// </summary>
    public class LoanItem
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }


        /// <summary>
        /// 借款单单号
        /// </summary>
        public string b_LoanOrderNo { get; set; }

        /// <summary>
        /// 费用日期
        /// </summary>
        public string b_Date { get; set; }


        public DateTime nb_Date { get; set; }

        /// <summary>
        /// 借款人
        /// </summary>
        public string b_Borrower { get; set; }

        /// <summary>
        /// 借款金额
        /// </summary>
        public decimal b_LoanAmount { get; set; }

        /// <summary>
        /// 借款事由
        /// </summary>
        public string b_LoanReason { get; set; }

    }


}