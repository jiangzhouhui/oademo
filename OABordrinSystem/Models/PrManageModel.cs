using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class PrManageModel
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// PR单号
        /// </summary>
        public string b_PrRecordNo { get; set; }

        /// <summary>
        /// 采购类型
        /// </summary>
        public string b_PrType { get; set; }

        /// <summary>
        /// 需求部门
        /// </summary>
        public string b_BusinessDepartment { get; set; }

        /// <summary>
        /// 预算
        /// </summary>
        public decimal b_Budget { get; set; }

        /// <summary>
        /// 申请人Id
        /// </summary>
        public string b_ApplicantId { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string b_Applicant { get; set; }

        /// <summary>
        /// 提出日期
        /// </summary>
        public string b_RaisedDate { get; set; }

        /// <summary>
        /// 提出日期
        /// </summary>
        public DateTime nb_RaisedDate { get; set; }


        /// <summary>
        /// 邮箱
        /// </summary>
        public string b_EmailAddress { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string b_PhoneNo { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 预算号
        /// </summary>
        [StringLength(24, ErrorMessage = "{0} is too long.")]
        public string b_BudgetCode { get; set; }

        /// <summary>
        ///项目领导Id
        /// </summary>
        public string b_ProjectLeaderId { get; set; }

        /// <summary>
        /// PMT/PAT leader
        /// </summary>
        public string b_ProjectLeader { get; set; }

        /// <summary>
        /// 项目经理Id
        /// </summary>
        public string b_ProjectManagerId { get; set; }

        /// <summary>
        /// 项目经理
        /// </summary>
        public string b_ProjectManager { get; set; }

        /// <summary>
        /// 项目总监Id
        /// </summary>
        public string b_ProjectDirectorId { get; set; }

        /// <summary>
        /// 项目总监
        /// </summary>
        public string b_ProjectDirector { get; set; }


        /// <summary>
        /// 预算来源及使用情况
        /// </summary>
        public string b_BudgetStatus { get; set; }


        /// <summary>
        /// 采购员Id
        /// </summary>
        public string b_BuyerId { get; set; }

        /// <summary>
        /// 采购员
        /// </summary>
        public string b_Buyer { get; set; }

        /// <summary>
        /// 紧急采购
        /// </summary>
        public bool b_UrgentPurchase { get; set; }

        /// <summary>
        /// 重复采购
        /// </summary>
        public bool b_RepetitivePurchase { get; set; }

        /// <summary>
        /// 授权采购
        /// </summary>
        public bool b_AuthorizedPurchase { get; set; }

        /// <summary>
        /// 选点供应商
        /// </summary>
        public string b_SourcedSupplier { get; set; }

        /// <summary>
        /// 合同价格
        /// </summary>
        public string b_ContractPrice { get; set; }

        /// <summary>
        /// 合同价格
        /// </summary>
        public decimal? b_ContractPriceStr { get; set; }


        /// <summary>
        /// 合同（订单）号
        /// </summary>
        public string b_PoNo { get; set; }


        /// <summary>
        /// 合同性质
        /// </summary>
        public string b_ContractProperty { get; set; }

        /// <summary>
        /// 采购内容
        /// </summary>
        public string b_PurchaseContent { get; set; }

        /// <summary>
        /// 签约方
        /// </summary>
        public string b_ContractParty { get; set; }

        /// <summary>
        /// 申请人地址
        /// </summary>
        public string b_ApplicantAddress { get; set; }

        /// <summary>
        /// 单一供应商（品牌）
        /// </summary>
        public bool b_IsSingleSupplier { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        public string b_ContractType { get; set; }


        /// <summary>
        /// 部门经理
        /// </summary>
        public string b_DeptManager { get; set; }

        /// <summary>
        /// 部门经理ID
        /// </summary>
        public string b_DeptManagerId { get; set; }

        /// <summary>
        /// 部门总监
        /// </summary>
        public string b_DeptDirector { get; set; }

        /// <summary>
        /// 部门总监ID
        /// </summary>
        public string b_DeptDirectorId { get; set; }

        /// <summary>
        /// 成本中心
        /// </summary>
        public string b_CostCenter { get; set; }

        /// <summary>
        /// 采购原因
        /// </summary>
        public string b_PurchasingReason { get; set; }

        /// <summary>
        /// 追加预算
        /// </summary>
        public string b_AdditionalBudget { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string b_Remark { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string b_VersionNo { get; set; }

        /// <summary>
        /// 部门VPId
        /// </summary>
        public string b_DeptVPId { get; set; }

        /// <summary>
        /// 部门VP
        /// </summary>
        public string b_DeptVP { get; set; }

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
        /// 单据状态
        /// </summary>
        public string State { get; set; }


        /// <summary>
        /// 操作
        /// </summary>
        public string operation { get; set; }

        /// <summary>
        /// 需求信息
        /// </summary>
        public List<PrManageItem> PrManageItems { get; set; }

        /// <summary>
        /// 采购询价信息
        /// </summary>
        public List<PrQuotationItem> PrQuotationItems { get; set; }

        /// <summary>
        /// 重复采购信息
        /// </summary>
        public List<PrRepeateItem> PrRepeateItems { get; set; }

        /// <summary>
        /// 选择供应商
        /// </summary>
        public List<PrChoiceSupplierItem> PrChoiceSupplierItems { get; set; }


        /// <summary>
        /// 文件集合
        /// </summary>
        public List<FileModel> Files { get; set; }


        /// <summary>
        /// 文件Id
        /// </summary>
        public List<string> fileIds { get; set; }

        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 是否拥有采购权限
        /// </summary>
        public bool IsPurchasingAuth { get; set; }


        /// <summary>
        /// 审核人
        /// </summary>
        public string AuditorStr { get; set; }


        public List<HistoryModel> HistoryList { get; set; }

    }

    public class PrManageItem
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string b_No { get; set; }

        /// <summary>
        /// 需求清单
        /// </summary>
        public string b_RequestList { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string b_SpecificationQuantity { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string b_ProjectNo { get; set; }

        /// <summary>
        /// 二级科目
        /// </summary>
        public string b_TaskNo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int b_Qty { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string b_Unit { get; set; }
    }



    public class PrQuotationItem
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string b_Supplier { get; set; }

        /// <summary>
        /// 报价
        /// </summary>
        public string b_Quotation { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string b_Remarks { get; set; }

    }

    public class PrRepeateItem
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// PR号/PR No.
        /// </summary>
        public string b_PrRecordNo { get; set; }

        /// <summary>
        /// 原供应商
        /// </summary>
        public string b_PreviousSupplier { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string b_ContractNo { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        //public string b_ContractPrice { get; set; }

        /// <summary>
        /// 原采购员
        /// </summary>
        public string b_PreviousBuyer { get; set; }


    }

    public class PrChoiceSupplierItem
    {

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string b_Supplier { get; set; }

        /// <summary>
        /// 合同价格
        /// </summary>
        public string b_ContractPrice { get; set; }

        /// <summary>
        /// 合同订单号
        /// </summary>
        public string b_PoNo { get; set; }

        /// <summary>
        /// 合同性质
        /// </summary>
        public string b_ContractProperty { get; set; }

        /// <summary>
        /// 付款条件
        /// </summary>
        public string b_PaymentClause { get; set; }

    }



    public class PrExportContractModel
    {
        /// <summary>
        /// 源ID
        /// </summary>
        public string source_id { get; set; }

        /// <summary>
        /// 合同订单号
        /// </summary>
        public string b_PoNo { get; set; }

        /// <summary>
        /// 采购员
        /// </summary>
        public string b_Buyer { get; set; }

        /// <summary>
        /// 采购内容
        /// </summary>
        public string b_PurchaseContent { get; set; }


        /// <summary>
        /// 需求部门
        /// </summary>
        public string b_BusinessDepartment { get; set; }


        /// <summary>
        /// 申请人
        /// </summary>
        public string b_Applicant { get; set; }

        /// <summary>
        /// 预算号
        /// </summary>
        public string b_BudgetCode { get; set; }

        /// <summary>
        /// 预算金额
        /// </summary>
        public decimal b_Budget { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string b_Supplier { get; set; }


        /// <summary>
        /// 是否单一供应商
        /// </summary>
        public string b_IsSingleSupplier { get; set; }

        /// <summary>
        /// 紧急采购
        /// </summary>
        public string b_UrgentPurchase { get; set; }


        /// <summary>
        /// 业务部门是否提出项目为单一供应商或紧急采购
        /// </summary>
        public string SingleSource { get; set; }


        /// <summary>
        /// 合同金额
        /// </summary>
        public decimal b_ContractPrice { get; set; }

        /// <summary>
        /// 追加预算
        /// </summary>
        public decimal b_AdditionalBudget { get; set; }


        /// <summary>
        /// 比预算节约金额
        /// </summary>
        public decimal SaveAmount { get; set; }

        /// <summary>
        /// 签约方
        /// </summary>
        public string b_ContractParty { get; set; }
    }





}