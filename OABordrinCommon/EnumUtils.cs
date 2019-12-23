using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinCommon
{
    public enum ContractParty
    {
        //[EnumDescription("思致")]
        //PowerThink = 0,
        //[EnumDescription("盛和")]
        //No = 0
    }

    [EnumDescription("菜单")]
    public enum SystemMenuList
    {
        [EnumDescription("项目管理")]
        b_ProjectManage = 0,
        [EnumDescription("角色管理")]
        b_RoleManage = 1,
        [EnumDescription("菜单权限管理")]
        b_MenuAuthManage = 2,
        [EnumDescription("组织架构")]
        b_OrganizationalStructure = 3,
        [EnumDescription("人员信息")]
        b_User = 4,
        [EnumDescription("费用类别")]
        b_ExpenseCategory = 5,
        [EnumDescription("审核人配置表")]
        b_ExpenseAuditConfiguration = 6,
        [EnumDescription("代理设置")]
        b_AgentSet=7
    }

    [EnumDescription("授权模块")]
    public enum AgentModule
    {
        //[EnumDescription("Pr单管理")]
        //PrManage = 0,
        [EnumDescription("费用报销")]
        ExpenseReimbursement = 1,
        [EnumDescription("差旅报销")]
        TripReimbursement = 2
    }




    /// <summary>
    /// 税码列表
    /// </summary>
    public enum TaxCodeTypeList
    {
        [EnumDescription("普通发票0%")]
        ordinary = 0,
        [EnumDescription("专票17%")]
        SpecialTicketOne = 17,
        [EnumDescription("专票11%")]
        SpecialTicketTwo = 11,
        [EnumDescription("专票6%")]
        SpecialTicketThree = 6,
        [EnumDescription("专票3%")]
        SpecialTicketFour = 3
    }


}
