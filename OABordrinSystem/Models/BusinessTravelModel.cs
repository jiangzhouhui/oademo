using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class BusinessTravelModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string b_DocumentNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string b_CompanyCode { get; set; }

        /// <summary>
        /// 归属地
        /// </summary>
        public string b_Location { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public string b_ApplicationDate { get; set; }

        public DateTime? nb_ApplicationDate { get; set; }

        /// <summary>
        /// 出差类型
        /// </summary>
        public string b_TripType { get; set; }

        /// <summary>
        /// 是否项目
        /// </summary>
        public string b_Type { get; set; }

        /// <summary>
        /// 代申请人
        /// </summary>
        public string b_Preparer { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string b_Employee { get; set; }

        /// <summary>
        /// 工号
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
        /// 成本中心
        /// </summary>
        public string b_CostCenter { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string b_Mobile { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string b_ProjectName { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        public string b_Destination { get; set; }

        /// <summary>
        /// 高级经理
        /// </summary>
        public string b_SeniorManager { get; set; }

        /// <summary>
        /// 总监
        /// </summary>
        public string b_Director { get; set; }

        /// <summary>
        /// 中心VP
        /// </summary>
        public string b_VP { get; set; }

        /// <summary>
        /// 出差开始日期
        /// </summary>
        public string b_TravelDate { get; set; }

        /// <summary>
        /// 预计出差结束日期
        /// </summary>
        public string b_EstimatedReturnDate { get; set; }

        /// <summary>
        /// 出差事由
        /// </summary>
        public string b_Purpose { get; set; }

        /// <summary>
        /// 行程安排
        /// </summary>
        public string b_TravelSchedule { get; set; }

        /// <summary>
        /// 出差预算
        /// </summary>
        public decimal b_TravelBudget { get; set; }

        /// <summary>
        /// 需要行政支持事项
        /// </summary>
        public string b_NeedAdminSupport { get; set; }

        /// <summary>
        /// 机票审批
        /// </summary>
        public string b_FlightIsssue { get; set; }

        /// <summary>
        /// 机票代订
        /// </summary>
        public string b_FlightBooking { get; set; }

        /// <summary>
        /// 滴滴额度
        /// </summary>
        public string b_Didi { get; set; }

        /// <summary>
        /// 其他
        /// </summary>
        public string b_Others { get; set; }

        /// <summary>
        /// 其他内容
        /// </summary>
        public string b_OtherContent { get; set; }

        /// <summary>
        /// 酒店代订
        /// </summary>
        public string b_HotelBooking { get; set; }

        /// <summary>
        /// 滴滴额度
        /// </summary>
        public string Didi { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string b_Remark { get; set; }

        /// <summary>
        /// 历史备注
        /// </summary>
        public string OldRemark { get; set; }

        /// <summary>
        /// 是否挂起
        /// </summary>
        public bool b_IsHangUp { get; set; }

        /// <summary>
        /// 挂起活动的名称
        /// </summary>
        public string b_HangUpActivityName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string b_VersionNo { get; set; }

        /// <summary>
        /// 交通费
        /// </summary>
        public decimal b_TrafficExpense { get; set; }

        /// <summary>
        /// 住宿费
        /// </summary>
        public decimal b_HotelExpense { get; set; }

        /// <summary>
        /// 固定补贴
        /// </summary>
        public decimal b_FixedSubsidy { get; set; }

        /// <summary>
        /// 其他费用
        /// </summary>
        public decimal b_OtherExpenses { get; set; }


        /// <summary>
        /// 是否报销
        /// </summary>
        public int b_IsReimbursement { get; set; }

        /// <summary>
        /// 滴滴金额
        /// </summary>
        public decimal b_DidiMoney { get; set; }

        /// <summary>
        /// 滴滴追加额度
        /// </summary>
        public decimal b_DidiAddMoney { get; set; }


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
        /// 文件Id
        /// </summary>
        public List<string> fileIds { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileModel> Files { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public List<HistoryModel> HistoryList { get; set; }

        /// <summary>
        /// 机票代订
        /// </summary>
        public List<FlightBooking> FlightBookingItems { get; set; }

        /// <summary>
        /// 酒店代订
        /// </summary>
        public List<HotelBooking> HotelBookingItems { get; set; }
    }

    public class FlightBooking
    {
        /// <summary>
        /// Id
        /// </summary>
       public string Id { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string b_FirstName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string b_LastName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string b_IDType { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string b_IDCardNo { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        public string b_Nationality { get; set; }

        /// <summary>
        /// 护照号码
        /// </summary>
        public string b_PassportNumber { get; set; }

        /// <summary>
        /// 护照有效期
        /// </summary>
        public string b_Dateofexpiration { get; set; }

        /// <summary>
        /// 出生年月
        /// </summary>
        public string b_Dateofbirth { get; set; }

        /// <summary>
        /// 有效地址
        /// </summary>
        public string b_Address { get; set; }

        /// <summary>
        /// 出发时间
        /// </summary>
        public string b_Gooff { get; set; }

        /// <summary>
        /// 出发地点
        /// </summary>
        public string b_Goplace{ get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        public string b_Flightnumber { get; set; }
    }

    public class HotelBooking
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 入住日期
        /// </summary>
        public string b_Checkindate { get; set; }

        /// <summary>
        /// 离店日期
        /// </summary>
        public string b_Leavedate { get; set; }

        /// <summary>
        /// 具体地址
        /// </summary>
        public string b_Specificaddress { get; set; }
    }
}