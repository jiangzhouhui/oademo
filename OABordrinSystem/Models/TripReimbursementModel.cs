using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class TripReimbursementModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string id { get; set; }

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
        /// 预算编号
        /// </summary>
        public string b_BudgetNumber { get; set; }

        /// <summary>
        /// 报销类型
        /// </summary>
        public string b_Type { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public string b_ApplicationDate { get; set; }

        public DateTime? nb_ApplicationDate { get; set; }

        /// <summary>
        /// 是否国际出差
        /// </summary>
        public bool b_IntalBusiness { get; set; }

        /// <summary>
        /// 是否挂起
        /// </summary>
        public bool b_IsHangUp { get; set; }

        /// <summary>
        /// 代申请人
        /// </summary>
        public string b_Preparer { get; set; }

        /// <summary>
        /// 代申请人工号
        /// </summary>
        public string b_PreparerNo { get; set; }

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
        /// 直属主管
        /// </summary>
        public string b_LineLeader { get; set; }

        /// <summary>
        /// 部门领导
        /// </summary>
        public string b_DeptLeader { get; set; }

        /// <summary>
        /// 中心领导
        /// </summary>
        public string b_DivisionVP { get; set; }

        /// <summary>
        /// 向公司预支现金
        /// </summary>
        public decimal b_AdvancedAmount { get; set; }

        /// <summary>
        /// 总费用
        /// </summary>
        public decimal b_TotalExpense { get; set; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public string b_AmountInTotal { get; set; }

        /// <summary>
        /// indexMum
        /// </summary>
        public int indexNum { get; set; }

        /// <summary>
        /// 附件张数
        /// </summary>
        public int b_AttachmentsQuantity { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string b_Remark { get; set; }

        /// <summary>
        /// 出差单号
        /// </summary>
        public string b_BTRecordNo { get; set; }

        /// <summary>
        /// 出差预算
        /// </summary>
        public decimal b_TravelBudget { get; set; }

        /// <summary>
        /// 是否超出预算
        /// </summary>
        public string b_IsBeyondBudget { get; set; }

        /// <summary>
        /// 超出理由
        /// </summary>
        public string b_BeyondReason { get; set; }

        /// <summary>
        /// 历史备注
        /// </summary>
        public string OldRemark { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 当前步骤流程ID
        /// </summary>
        public string activityId { get; set; }

        /// <summary>
        /// 活动权限ID
        /// </summary>
        public string activityAssignmentId { get; set; }

        /// <summary>
        /// 挂起所在的流程节点
        /// </summary>
        public string b_HangUpActivityName { get; set; }

        //操作
        public string operation { get; set; }

        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 文件Id
        /// </summary>
        public List<string> fileIds { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditorStr { get; set; }

        /// <summary>
        /// 大写ing
        /// </summary>
        public string b_AmountInWords { get; set; }

        /// <summary>
        /// 小写ing
        /// </summary>
        public decimal b_TotalAmount { get; set; }

        /// <summary>
        /// 住宿合计大写
        /// </summary>
        public string b_HotelInWords { get; set; }

        /// <summary>
        /// 住宿合计小写
        /// </summary>
        public decimal b_HotelAmount { get; set; }

        /// <summary>
        /// 交通合计大写
        /// </summary>
        public string b_TrafInWords { get; set; }

        /// <summary>
        /// 交通合计小写
        /// </summary>
        public decimal b_TrafAmount { get; set; }

        /// <summary>
        /// 补贴合计大写
        /// </summary>
        public string b_MealInWords { get; set; }

        /// <summary>
        /// 补贴合计小写
        /// </summary>
        public decimal b_MealAmount { get; set; }

        /// <summary>
        /// 其他合计大写
        /// </summary>
        public string b_OthInWords { get; set; }

        /// <summary>
        /// 其他合计小写
        /// </summary>
        public decimal b_OthAmount { get; set; }

        /// <summary>
        /// 住宿费
        /// </summary>
        public List<HotelExpense> HotelExpenseItems { get; set; }

        /// <summary>
        /// 交通费
        /// </summary>
        public List<TrafficExpense> TrafficExpenseItems { get; set; }

        /// <summary>
        /// 餐费及固定补贴
        /// </summary>
        public List<Mealsandfixedsubsidies> MealsSubsidiesItems { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public List<Others> OthersItems { get; set; }

        /// <summary>
        /// 借款明细
        /// </summary>
        public List<LoanItems> LoanItems { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public List<HistoryModel> HistoryList { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileModel> Files { get; set; }
    }

    public class HotelExpense
    {

        /// <summary>
        ///Id 
        /// </summary>
        public string id { get; set; }
        
        /// <summary>
        /// 开始日期
        /// </summary>
        public string b_StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string b_EndDate { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string b_City { get; set; }

        /// <summary>
        /// 酒店
        /// </summary>
        public string b_Hotel { get; set; }

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
        /// 天数
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
        public string b_HotelInWords { get; set; }
        

        /// <summary>
        /// 小写
        /// </summary>
        public decimal b_HotelAmount { get; set; }

    }

    public class TrafficExpense
    {

        /// <summary>
        ///Id 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string b_StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string b_EndDate { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string b_Type { get; set; }

        /// <summary>
        /// 线路起
        /// </summary>
        public string b_StartPoint { get; set; }

        /// <summary>
        /// 线路止
        /// </summary>
        public string b_EndPoint { get; set; }

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
        /// 数量/公里
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
        public string b_TrafInWords { get; set; }

        /// <summary>
        /// 小写
        /// </summary>
        public decimal b_TrafAmount { get; set; }

    }

    public class Mealsandfixedsubsidies
    {

        /// <summary>
        ///Id 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string b_StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string b_EndDate { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string b_Place { get; set; }

        /// <summary>
        /// 同行者人数
        /// </summary>
        public string b_CompanionAmount { get; set; }

        /// <summary>
        /// 同行者姓名
        /// </summary>
        public string b_CompanionName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string b_Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public float b_Rate { get; set; }

        /// <summary>
        /// 固定补贴（人民币）
        /// </summary>
        public decimal b_FixedSubsidy { get; set; }

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
        public string b_MealInWords { get; set; }

        /// <summary>
        /// 小写
        /// </summary>
        public decimal b_MealAmount { get; set; }

    }

    public class Others
    {

        /// <summary>
        ///Id 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string b_StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string b_EndDate { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string b_Place { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string b_Type { get; set; }

        /// <summary>
        /// 事由（申请其他费用）
        /// </summary>
        public string b_Reason { get; set; }

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
        /// 数量
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
        public string b_OthInWords { get; set; }

        /// <summary>
        /// 小写
        /// </summary>
        public decimal b_OthAmount { get; set; }

    }

    public class LoanItems
    {
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