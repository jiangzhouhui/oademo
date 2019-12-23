using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinService.Entity;
using OABordrinService.MD5;
using OABordrinSystem.Controllers.ExpenseReimbursement;
using OABordrinSystem.Controllers.PrManage;
using OABordrinSystem.Controllers.TripReimbursement;
using OABordrinSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OABordrinService.ToDoServiceBll
{
    public class ToDoServiceBll
    {
        /// <summary>
        /// 获取代办列表
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="user"></param>
        public static List<TodoEntity> GetTodoList(string loginName, UserInfo user)
        {
            List<TodoEntity> list = new List<TodoEntity>();
            string auditorStr = "";
            SecurityBll.LogIn(loginName, user);
            if (user != null && user.Roles != null && user.inn!=null)
            {
                List<PrManageModel> prList = GetPrTodoList(user);
                List<ExpenseReimbursementModel> erList = GetErTodoList(user);
                List<TripReimbursementModel> trList = GetTrTodoList(user);
                if (prList != null && prList.Count > 0)
                {
                    foreach (var item in prList)
                    {
                        TodoEntity entity = new TodoEntity();
                        entity.RecordName = "PR单" + "(" + item.b_PrRecordNo + ")";
                        entity.Applicant = item.b_Applicant;
                        entity.FlowStatus = item.status;
                        entity.ApplicationDate = item.nb_RaisedDate.ToString("yyyy-MM-dd");
                        entity.AuditorStr = ActivityDA.GetActivityOperator(user.inn, item.activityId);
                        entity.LinkStr = "https://oa.bordrin.com/PrManage";
                        list.Add(entity);
                    }
                }

                if (erList != null && erList.Count > 0)
                {
                    foreach (var item in erList)
                    {
                        TodoEntity entity = new TodoEntity();
                        entity.RecordName = "费用报销单" + "(" + item.b_RecordNo + ")";
                        entity.Applicant = item.b_Employee;
                        entity.FlowStatus = item.status;
                        entity.ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        entity.AuditorStr = ActivityDA.GetActivityOperator(user.inn, item.activityId);
                        entity.LinkStr = "https://oa.bordrin.com/ExpenseReimbursement";

                        list.Add(entity);
                    }
                }

                if (trList != null && trList.Count > 0)
                {
                    foreach (var item in trList)
                    {
                        TodoEntity entity = new TodoEntity();
                        entity.RecordName = "差旅报销单" + "(" + item.b_RecordNo + ")";
                        entity.Applicant = item.b_Employee;
                        entity.FlowStatus = item.status;
                        entity.ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                        entity.AuditorStr = ActivityDA.GetActivityOperator(user.inn, item.activityId);
                        entity.LinkStr = "https://oa.bordrin.com/TripReimbursement";
                        list.Add(entity);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取PR代办列表
        /// </summary>
        /// <param name="user"></param>
        public static List<PrManageModel> GetPrTodoList(UserInfo user)
        {
            int total = 0;
            List<PrManageModel> list = PrManageController.GetPrManageList(user.Roles, out total, null, "", null, null, "");
            return list;
        }

        /// <summary>
        /// 获取Er代办列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<ExpenseReimbursementModel> GetErTodoList(UserInfo user)
        {
            int total = 0;
            //获取委托权限数据
            List<ExpenseReimbursementModel> returnList = new List<ExpenseReimbursementModel>();
            List<string> agentRoles = AgentSetBll.GetAgentRoles(user, "ExpenseReimbursement");
            List<ExpenseReimbursementModel> list = ExpenseReimbursementController.GetExpenseReimbursementList(user, out total, null, "", null, null, "", agentRoles);
            foreach (var item in list)
            {
                var result = ActivityDA.GetActivityAuditByLoginInfo(user.inn, item.Id, "innovator.B_EXPENSEREIMBURSEMENT", user.Roles, agentRoles);
                if (!result.isError() && result.getItemCount() > 0)
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }


        /// <summary>
        /// 获取Tr代办列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<TripReimbursementModel> GetTrTodoList(UserInfo user)
        {
            int total = 0;
            //获取委托权限数据
            List<TripReimbursementModel> returnList = new List<TripReimbursementModel>();
            List<string> agentRoles = AgentSetBll.GetAgentRoles(user, "TripReimbursement");
            List<TripReimbursementModel> list = TripReimbursementController.GetTripReimbursementList(user, out total, null, "", null, null, "", agentRoles);
            foreach (var item in list)
            {
                var result = ActivityDA.GetActivityAuditByLoginInfo(user.inn, item.id, "innovator.B_TRIPREIMBURSEMENTFORM", user.Roles, agentRoles);
                if (!result.isError() && result.getItemCount() > 0)
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }
    }
}