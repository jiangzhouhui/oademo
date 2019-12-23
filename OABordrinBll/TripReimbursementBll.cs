using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OABordrinBll
{
    public class TripReimbursementBll
    {

        /// <summary>
        /// 编辑出差单
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="itemRoot"></param>
        /// <param name="operation"></param>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item EditTripReimbursement(Innovator inn,Item itemRoot,string operation,string status,string id,string b_BTRecordNo)
        {
            var result = inn.newItem();
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
            {
                if (status == "Start" && !string.IsNullOrEmpty(id))
                {
                    //删除住宿费
                    DelHotelExpenseItem(inn, id);

                    //删除交通费
                    DelTrafficExpenseItem(inn, id);

                    //删除餐费及固定补贴
                    DelMealsandfixedsubsidiesItem(inn, id);

                    //删除其他
                    DelOthersItem(inn, id);

                    //删除借款明细
                    DelLoanItemItem(inn, id);
                }

                result = itemRoot.apply();
                if (!result.isError()&& operation== "submit")
                {
                    if(string.IsNullOrEmpty(id))
                    {
                        id = result.getProperty("id");
                    }
                    BusinessTravelDA.UpdateBusinessTravelIsReimbursement(inn, "1", b_BTRecordNo);
                }
                //没有错误，提交事务
                ts.Complete();
            }
            return result;
        }

        /// <summary>
        /// 生成单据单号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string CreateRecordNo(Innovator inn, string id)
        {
            string b_recordno = "";
            if (!string.IsNullOrEmpty(id))
            {
                string sqlStr = "Declare @IndexNum int Select @IndexNum = isnull(Max(B_INDEXNUM), 0) + 1 from innovator.b_TripReimbursementForm where CREATED_ON &gt;= CONVERT(varchar(10),dateadd(dd,-day(getdate())+1,getdate()),120) and CREATED_ON &lt; CONVERT(CHAR(10),DATEADD(m,1,DATEADD(dd,-DAY(GETDATE())+1,GETDATE())),111) update innovator.b_TripReimbursementForm set b_RecordNo = b_RecordNo + RIGHT('00000' + CAST(@IndexNum as varchar), 5),b_indexnum=@IndexNum where id = '" + id + "'";
                var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
                b_recordno = GetRecordNoById(inn, id);
            }
            return b_recordno;
        }

        /// <summary>
        /// 根据ID获取单号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetRecordNoById(Innovator inn, string id)
        {
            var item = inn.newItem("b_TripReimbursementForm", "get");
            item.setAttribute("id", id);
            item.setAttribute("select", "b_recordno");
            var result = item.apply();
            if (!result.isError())
            {
                return result.getProperty("b_recordno");
            }

            return "";
        }

        /// <summary>
        /// 删除主表信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteTripReimbursement(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_TripReimbursementForm' action='delete' id='" + id + "'></Item></AML>";
            var result = inn.applyAML(strAml);

        }

        /// <summary>
        /// 删除住宿费
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DelHotelExpenseItem(Innovator inn, string id)
        {
            //询价信息
            var R_HotelExpense = inn.newItem("R_HotelExpense", "get");
            R_HotelExpense.setAttribute("where", "source_id='" + id + "'");
            R_HotelExpense.setAttribute("select", "related_id");
            var oldItems = R_HotelExpense.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='R_HotelExpense' action='purge' idlist='" + requestIds + "'></Item><Item type='b_HotelExpense' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除交通费
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DelTrafficExpenseItem(Innovator inn, string id)
        {
            //询价信息
            var R_TrafficExpense = inn.newItem("R_TrafficExpense", "get");
            R_TrafficExpense.setAttribute("where", "source_id='" + id + "'");
            R_TrafficExpense.setAttribute("select", "related_id");
            var oldItems = R_TrafficExpense.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='R_TrafficExpense' action='purge' idlist='" + requestIds + "'></Item><Item type='b_TrafficExpense' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除餐费及固定补贴
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DelMealsandfixedsubsidiesItem(Innovator inn, string id)
        {
            //询价信息
            var R_Mealsandfixedsubsidies = inn.newItem("R_Mealsandfixedsubsidies", "get");
            R_Mealsandfixedsubsidies.setAttribute("where", "source_id='" + id + "'");
            R_Mealsandfixedsubsidies.setAttribute("select", "related_id");
            var oldItems = R_Mealsandfixedsubsidies.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='R_Mealsandfixedsubsidies' action='purge' idlist='" + requestIds + "'></Item><Item type='b_Mealsandfixedsubsidies' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除其他
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DelOthersItem(Innovator inn, string id)
        {
            //询价信息
            var R_Others = inn.newItem("R_Others", "get");
            R_Others.setAttribute("where", "source_id='" + id + "'");
            R_Others.setAttribute("select", "related_id");
            var oldItems = R_Others.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='R_Others' action='purge' idlist='" + requestIds + "'></Item><Item type='b_Others' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除借款明细
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DelLoanItemItem(Innovator inn, string id)
        {
            //询价信息
            var R_LoanItems = inn.newItem("R_LoanItems", "get");
            R_LoanItems.setAttribute("where", "source_id='" + id + "'");
            R_LoanItems.setAttribute("select", "related_id");
            var oldItems = R_LoanItems.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='R_LoanItems' action='purge' idlist='" + requestIds + "'></Item><Item type='b_LoanItem' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除对应的附件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteFile(Innovator inn, string id)
        {
            var R_TripReimbursementFile = inn.newItem("R_TripReimbursementFile", "get");
            R_TripReimbursementFile.setAttribute("where", "source_id='" + id + "'");
            R_TripReimbursementFile.setAttribute("select", "related_id");
            var oldItems = R_TripReimbursementFile.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='R_TripReimbursementFile' action='purge' idlist='" + requestIds + "'></Item><Item type='File' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 根据ID获取差旅报销单
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetTripReimbursementObjById(Innovator inn, string id)
        {
            var expenseTripReimbursement = inn.newItem("b_TripReimbursementForm", "get");
            expenseTripReimbursement.setAttribute("id", id);

            //住宿费
            var R_HotelExpense = inn.newItem("R_HotelExpense", "get");
            R_HotelExpense.setAttribute("where", "source_id='" + id + "'");
            R_HotelExpense.setAttribute("select", "related_id");
            expenseTripReimbursement.addRelationship(R_HotelExpense);

            //交通费
            var R_TrafficExpense = inn.newItem("R_TrafficExpense", "get");
            R_TrafficExpense.setAttribute("where", "source_id='" + id + "'");
            R_TrafficExpense.setAttribute("select", "related_id");
            expenseTripReimbursement.addRelationship(R_TrafficExpense);

            //餐费及固定补贴
            var R_Mealsandfixedsubsidies = inn.newItem("R_Mealsandfixedsubsidies", "get");
            R_Mealsandfixedsubsidies.setAttribute("where", "source_id='" + id + "'");
            R_Mealsandfixedsubsidies.setAttribute("select", "related_id");
            expenseTripReimbursement.addRelationship(R_Mealsandfixedsubsidies);

            //其他
            var R_Others = inn.newItem("R_Others", "get");
            R_Others.setAttribute("where", "source_id='" + id + "'");
            R_Others.setAttribute("select", "related_id");
            expenseTripReimbursement.addRelationship(R_Others);

            //借款明细
            var R_LoanItems = inn.newItem("R_LoanItems", "get");
            R_LoanItems.setAttribute("where", "source_id='" + id + "'");
            R_LoanItems.setAttribute("select", "related_id");
            expenseTripReimbursement.addRelationship(R_LoanItems);


            //附件
            var R_TripReimbursement = inn.newItem("R_TripReimbursementFile", "get");
            R_TripReimbursement.setAttribute("where", "source_id='" + id + "'");
            R_TripReimbursement.setAttribute("select", "related_id");
            expenseTripReimbursement.addRelationship(R_TripReimbursement);

            var result = expenseTripReimbursement.apply();
            return result;
        }

        /// <summary>
        /// 获取老备注
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetOldRemarkById(Innovator inn, string id)
        {
            string strSql = "select b_Remark from innovator.b_TripReimbursementForm where id='" + id + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            if (!returnItem.isError() && returnItem.getItemCount() > 0)
            {
                return returnItem.getProperty("b_remark");
            }
            return "";
        }


        /// <summary>
        /// 自动完成
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="b_TotalExpense"></param>
        /// <param name="b_IsBudgetary"></param>
        /// <param name="b_Type"></param>
        /// <param name="LineLeader"></param>
        /// <param name="DeptLeader"></param>
        /// <param name="ChoicePath"></param>
        public static void AutomaticCompletionTask(Innovator inn, string id, decimal b_TotalExpense, bool b_IsBudgetary, string b_Type, bool b_IntalBusiness, string LineLeader, string DeptLeader, ref WORKFLOW_PROCESS_PATH ChoicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.b_TripReimbursementForm");
            if (!activityItem.isError() && activityItem.getItemCount() > 0)
            {
                string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
                string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");

                //获取当前操作
                bool isOperate = false;
                Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId);
                if (!identitys.isError() && identitys.getItemCount() == 1)
                {
                    isOperate = true;
                }
                if (isOperate)
                {
                    string lineName = GetLineNameByActivityName(inn, activityId, keyedName, b_TotalExpense, b_IsBudgetary, DeptLeader);
                    WORKFLOW_PROCESS_PATH newChoicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.b_TripReimbursementForm", lineName);
                    if (newChoicePath != null)
                    {
                        ChoicePath = newChoicePath;
                        AutomaticCompletionTask(inn, id, b_TotalExpense, b_IsBudgetary, b_Type, b_IntalBusiness, LineLeader, DeptLeader, ref ChoicePath);
                    }
                }
            }
        }

        /// <summary>
        /// 根据活动名称选择附件
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="b_TotalExpense"></param>
        /// <param name="b_IsBudgetary"></param>
        /// <param name="b_Type"></param>
        /// <returns></returns>
        public static string GetLineNameByActivityName(Innovator inn, string activityId, string activityName, decimal b_TotalExpense, bool b_IsBudgetary, string DeptLeader)
        {
            string lineName = "agree";


            //部门总监审批
            if (activityName == "Dept.Director" || activityName == "Project Director")
            {
                //获取当前活动的的审核操作
                int auditPersonCount = 0;
                Item activityItem = ActivityAssignmentDA.GetActivityAssignment(inn, activityId);
                if (!activityItem.isError() && activityItem.getItemCount() > 0)
                {
                    auditPersonCount = activityItem.getItemCount();
                }

                //总金额大于5千 或者 在预算外
                if (b_TotalExpense > 5000 || b_IsBudgetary == false || auditPersonCount == 0)
                {
                    lineName = "lineA";
                }
                //预算内  并且 总金额小于等于5千
                else if (b_IsBudgetary && b_TotalExpense <= 5000)
                {
                    lineName = "lineB";
                }
            }

            //部门VP审批
            if (activityName == "Division VP" || activityName == "Project VP")
            {
                //预算外  或者 总金额大于1万
                if (b_IsBudgetary == false || b_TotalExpense > 10000)
                {
                    lineName = "lineA";
                }
                //预算内 并且 总金额小于1万
                else if (b_IsBudgetary && b_TotalExpense <= 10000)
                {
                    lineName = "lineB";
                }
            }
            return lineName;
        }

        /// <summary>
        /// 挂机自动完成审核
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="b_HangUpActivityName"></param>
        /// <param name="b_TotalExpense"></param>
        /// <param name="b_IsBudgetary"></param>
        /// <param name="b_Type"></param>
        /// <param name="choicePath"></param>
        public static void HangUpAutoAudit(Innovator inn, string id, string b_HangUpActivityName, decimal b_TotalExpense, bool b_IsBudgetary, bool b_IntalBusiness, string DeptLeader, string b_Type, ref WORKFLOW_PROCESS_PATH choicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.b_TripReimbursementForm");

            //获取当前活动的流程名称
            string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
            string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");

            //当前流程节点的名称，不等于挂机的流程名称时，执行自动完成！
            if (b_HangUpActivityName != keyedName)
            {
                string lineName = GetLineNameByActivityName(inn, activityId, keyedName, b_TotalExpense, b_IsBudgetary, DeptLeader);
                choicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.b_TripReimbursementForm", lineName);
                HangUpAutoAudit(inn, id, b_HangUpActivityName, b_TotalExpense, b_IsBudgetary, b_IntalBusiness, DeptLeader, b_Type, ref choicePath);
            }
        }

        /// <summary>
        /// 添加审核权限
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="region"></param>
        public static void AddTripReimbursementAudit(Innovator inn, string id, string region, List<string> analysisAuditStr, List<string> accountAuditStr, string B_CENTRE)
        {

            List<string> activityids = new List<string> { "Financial Analyst", "Expense Accountant Check", "GM", "Financial Director", "Expense Accountant Creation" };
            ActivityBll.DeleteActivityAuthById(inn, id, "innovator.b_TripReimbursementForm", activityids);

            //添加财务员审批
            ActivityBll.AddActivityAudit(inn, id, "Financial Analyst", "innovator.b_TripReimbursementForm", analysisAuditStr);

            if (B_CENTRE.Trim() == "盛和")
            {
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "GMSH", "", "GM", "innovator.b_TripReimbursementForm");
            }
            else if (B_CENTRE.Trim() == "骏盛")
            {
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "GMJS", "", "GM", "innovator.b_TripReimbursementForm");
            }
            else
            {
                //添加CEO审批
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "CEO", "", "GM", "innovator.b_TripReimbursementForm");
            }

            //费用会计审核
            ActivityBll.AddActivityAudit(inn, id, "Expense Accountant Check", "innovator.b_TripReimbursementForm", accountAuditStr);

            //财务总监
            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "财务总监（" + region + "）", region, "Financial Director", "innovator.b_TripReimbursementForm");

            //费用会计制证
            ActivityBll.AddActivityAudit(inn, id, "Expense Accountant Creation", "innovator.b_TripReimbursementForm", accountAuditStr);
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="emailEntity"></param>
        /// <param name="choicePath"></param>
        public static void SendEmailByOperation(Innovator inn, EmailEntity emailEntity, WORKFLOW_PROCESS_PATH choicePath)
        {
            //邮箱列表
            List<string> listEmail = new List<string>();
            List<string> names = new List<string>();
            string nameStr = "";
            string subject = "";
            string body = "";

            Item activity = ActivityDA.GetActivityById(inn, choicePath.RELATED_ID);
            string keyedName = activity.getProperty("keyed_name").Trim();
            //获取邮件需要发送的人员信息
            Item identitys = IdentityDA.GetIdentityByActivityId(inn, choicePath.RELATED_ID);
            //获取邮箱信息
            UserDA.GetEmailByIdentitys(inn, identitys, listEmail, names);

            if (names != null && names.Count > 0)
            {
                for (int i = 0; i < names.Count; i++)
                {
                    if (i != names.Count - 1)
                    {
                        nameStr += names[i] + "、";
                    }
                    else
                    {
                        nameStr += names[i];
                    }
                }
            }
            if (choicePath.NAME.Contains("Return"))
            {
                subject = "Your trip reimbursement application< " + emailEntity.RecordNo + " > has been rejected.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your trip reimbursement application < " + emailEntity.RecordNo + " > has been rejected.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/TripReimbursement/Index";
            }
            else if (keyedName == "End")
            {
                BusinessTravelBll.UpdateBusinessTravelIsReimbursement(inn, "2", emailEntity.BTRecordNo);
                //获取申请人邮箱
                Item applicantIdentity = IdentityDA.GetIdentityByKeyedName(inn, emailEntity.ApplicantName);
                if (!applicantIdentity.isError())
                {
                    UserDA.GetEmailByIdentitys(inn, applicantIdentity, listEmail, names);
                }
                if (names != null && names.Count > 0)
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        if (i != names.Count - 1)
                        {
                            nameStr += names[i] + "、";
                        }
                        else
                        {
                            nameStr += names[i];
                        }
                    }
                }
                subject = "Your trip reimbursement application< " + emailEntity.RecordNo + " > has been approved.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your trip reimbursement application < " + emailEntity.RecordNo + " > has been approved.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/TripReimbursement/Index";
            }
            else
            {
                subject = "<" + emailEntity.ApplicantDepartment + "> " + emailEntity.ApplicantName + " has submitted a trip reimbursement application< " + emailEntity.RecordNo + " > for your approval.";
                body += "Dear " + nameStr + ",<br/><br/>";
                body += "<" + emailEntity.ApplicantDepartment + "> " + emailEntity.ApplicantName + " has submitted a trip reimbursement application< " + emailEntity.RecordNo + " > for your approval.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/TripReimbursement/Index";
            }

            listEmail = new List<string>();
            listEmail.Add("ping.chen@bordrin.com");
            listEmail.Add("zhouhui.jiang@bordrin.com");
            //listEmail.Add("Jingjian.Gong@Bordrin.com");

            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }

        /// <summary>
        /// 当财务总监审核过后，发送邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_Employee"></param>
        /// <param name="b_RecordNo"></param>
        public static void SendEmailToProposer(Innovator inn, string proposer, string recordNo)
        {
            //邮箱列表
            List<string> listEmail = new List<string>();
            List<string> names = new List<string>();
            string subject = "";
            string body = "";

            //获取申请人邮箱
            Item applicantIdentity = IdentityDA.GetIdentityByKeyedName(inn, proposer);
            if (!applicantIdentity.isError())
            {
                UserDA.GetEmailByIdentitys(inn, applicantIdentity, listEmail, names);
            }

            subject = "Your trip reimbursement application <" + recordNo + "> has been approved by financial analyst, Please hand in your application and receipts to finance department.";

            body = "您单号为[" + recordNo + "]的差旅报销单已通过财务分析员审核，请将报销单打印并附上报销凭证，交到财务部门进行费用审核。<br/>";
            body += "Your trip reimbursement application < " + recordNo + " > has been approved by financial analyst, Please hand in your application and receipts to finance department.";

            listEmail = new List<string>();
            listEmail.Add("ping.chen@bordrin.com");
            listEmail.Add("zhouhui.jiang@bordrin.com");
            //listEmail.Add("Jingjian.Gong@Bordrin.com");

            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }

        /// <summary>
        /// 管理人员重发凭证提交邮件
        /// </summary>
        /// <param name="b_Employee"></param>
        /// <param name="b_RecordNo"></param>
        public static void ExpenseAccountantCheckSendEmail(Innovator inn, string b_Employee, string b_RecordNo)
        {
            //邮箱列表
            List<string> listEmail = new List<string>();
            List<string> names = new List<string>();
            string subject = "";
            string body = "";

            //获取申请人邮箱
            Item applicantIdentity = IdentityDA.GetIdentityByKeyedName(inn, b_Employee);
            if (!applicantIdentity.isError())
            {
                UserDA.GetEmailByIdentitys(inn, applicantIdentity, listEmail, names);
            }
            subject = "您单号为[" + b_RecordNo + "]的差旅报销单已通过财务分析员审核，请将报销单打印并附上报销凭证，交到财务部门进行费用审核。——如已打印并交到财务部，请忽略此邮件。";
            body = "您单号为[" + b_RecordNo + "]的差旅报销单已通过财务分析员审核，请将报销单打印并附上报销凭证，交到财务部门进行费用审核。——如已打印并交到财务部，请忽略此邮件。<br/>";
            body += "打印方式：进入 https://oa.bordrin.com ,打开菜单 报销管理->查询差旅报销，找到对应申请单，单击单号后面的打印按钮打印申请单。";

            listEmail = new List<string>();
            listEmail.Add("ping.chen@bordrin.com");
            listEmail.Add("zhouhui.jiang@bordrin.com");
            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }


        //调用相关接口，发起会计制证
        public static List<ApiExpenseReimbursementEntity> SendTripReimbursementCreation(Innovator inn, string id)
        {
            List<ApiExpenseReimbursementEntity> datas = new List<ApiExpenseReimbursementEntity>();
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                //酒店住宿费
                List<ApiExpenseReimbursementEntity> hotelDatas = (from g in db.B_HOTELEXPENSE
                                                                  join t in db.R_HOTELEXPENSE on g.id equals t.RELATED_ID
                                                                  join y in db.B_TRIPREIMBURSEMENTFORM on t.SOURCE_ID equals y.id
                                                                  join u in db.WORKFLOW on y.id equals u.SOURCE_ID
                                                                  join i in db.WORKFLOW_PROCESS on u.RELATED_ID equals i.ID
                                                                  where y.id == id
                                                                  select new ApiExpenseReimbursementEntity
                                                                  {
                                                                      BUKRS = y.B_COMPANYCODE,
                                                                      XBLNR = y.B_RECORDNO,
                                                                      closed_date = i.CLOSED_DATE,
                                                                      NUMPG = y.B_ATTACHMENTSQUANTITY,
                                                                      PROTYP = y.B_TYPE,
                                                                      DMBTR = g.B_CNYSUBTOTAL,
                                                                      KOSTL = y.B_COSTCENTER,
                                                                      SGTXT = "差旅费",
                                                                      AUFNR = g.B_PROJECTNAME,
                                                                      POSID = y.B_BUDGETNUMBER,
                                                                      b_TaxRate = g.B_TAXRATE,
                                                                      b_Tax = g.B_TAX,
                                                                      b_TaxFreeAmount = g.B_TAXFREEAMOUNT,
                                                                      b_StaffNo = y.B_STAFFNO
                                                                  }).ToList();

                if (hotelDatas != null)
                {
                    datas.AddRange(hotelDatas);
                }


                //交通费
                List<ApiExpenseReimbursementEntity> trafficDatas = (from g in db.B_TRAFFICEXPENSE
                                                                    join t in db.R_TRAFFICEXPENSE on g.id equals t.RELATED_ID
                                                                    join y in db.B_TRIPREIMBURSEMENTFORM on t.SOURCE_ID equals y.id
                                                                    join u in db.WORKFLOW on y.id equals u.SOURCE_ID
                                                                    join i in db.WORKFLOW_PROCESS on u.RELATED_ID equals i.ID
                                                                    where y.id == id
                                                                    select new ApiExpenseReimbursementEntity
                                                                    {
                                                                        BUKRS = y.B_COMPANYCODE,
                                                                        XBLNR = y.B_RECORDNO,
                                                                        closed_date = i.CLOSED_DATE,
                                                                        NUMPG = y.B_ATTACHMENTSQUANTITY,
                                                                        PROTYP = y.B_TYPE,
                                                                        DMBTR = g.B_CNYSUBTOTAL,
                                                                        KOSTL = y.B_COSTCENTER,
                                                                        SGTXT = "差旅费",
                                                                        AUFNR = g.B_PROJECTNAME,
                                                                        POSID = y.B_BUDGETNUMBER,
                                                                        b_TaxRate = g.B_TAXRATE,
                                                                        b_Tax = g.B_TAX,
                                                                        b_TaxFreeAmount = g.B_TAXFREEAMOUNT,
                                                                        b_StaffNo = y.B_STAFFNO
                                                                    }).ToList();

                if (trafficDatas != null)
                {
                    datas.AddRange(trafficDatas);
                }

                //餐费及固定补贴
                List<ApiExpenseReimbursementEntity> mealsandDatas = (from g in db.B_MEALSANDFIXEDSUBSIDIES
                                                                     join t in db.R_MEALSANDFIXEDSUBSIDIES on g.id equals t.RELATED_ID
                                                                     join y in db.B_TRIPREIMBURSEMENTFORM on t.SOURCE_ID equals y.id
                                                                     join u in db.WORKFLOW on y.id equals u.SOURCE_ID
                                                                     join i in db.WORKFLOW_PROCESS on u.RELATED_ID equals i.ID
                                                                     where y.id == id
                                                                     select new ApiExpenseReimbursementEntity
                                                                     {
                                                                         BUKRS = y.B_COMPANYCODE,
                                                                         XBLNR = y.B_RECORDNO,
                                                                         closed_date = i.CLOSED_DATE,
                                                                         NUMPG = y.B_ATTACHMENTSQUANTITY,
                                                                         PROTYP = y.B_TYPE,
                                                                         DMBTR = g.B_CNYSUBTOTAL,
                                                                         KOSTL = y.B_COSTCENTER,
                                                                         SGTXT = "差旅费",
                                                                         AUFNR = g.B_PROJECTNAME,
                                                                         POSID = y.B_BUDGETNUMBER,
                                                                         b_TaxRate = g.B_TAXRATE,
                                                                         b_Tax = g.B_TAX,
                                                                         b_TaxFreeAmount = g.B_TAXFREEAMOUNT,
                                                                         b_StaffNo = y.B_STAFFNO
                                                                     }).ToList();

                if (mealsandDatas != null)
                {
                    datas.AddRange(mealsandDatas);
                }

                //其他
                List<ApiExpenseReimbursementEntity> others = (from g in db.B_OTHERS
                                                              join t in db.R_OTHERS on g.id equals t.RELATED_ID
                                                              join y in db.B_TRIPREIMBURSEMENTFORM on t.SOURCE_ID equals y.id
                                                              join u in db.WORKFLOW on y.id equals u.SOURCE_ID
                                                              join i in db.WORKFLOW_PROCESS on u.RELATED_ID equals i.ID
                                                              where y.id == id
                                                              select new ApiExpenseReimbursementEntity
                                                              {
                                                                  BUKRS = y.B_COMPANYCODE,
                                                                  XBLNR = y.B_RECORDNO,
                                                                  closed_date = i.CLOSED_DATE,
                                                                  NUMPG = y.B_ATTACHMENTSQUANTITY,
                                                                  PROTYP = y.B_TYPE,
                                                                  DMBTR = g.B_CNYSUBTOTAL,
                                                                  KOSTL = y.B_COSTCENTER,
                                                                  SGTXT = "差旅费",
                                                                  AUFNR = g.B_PROJECTNAME,
                                                                  POSID = y.B_BUDGETNUMBER,
                                                                  b_TaxRate = g.B_TAXRATE,
                                                                  b_Tax = g.B_TAX,
                                                                  b_TaxFreeAmount = g.B_TAXFREEAMOUNT,
                                                                  b_StaffNo = y.B_STAFFNO
                                                              }).ToList();

                if (others != null)
                {
                    datas.AddRange(others);
                }
            }

            if (datas != null && datas.Count > 0)
            {
                foreach (var entity in datas)
                {
                    if (!string.IsNullOrEmpty(entity.BUKRS))
                    {
                        entity.BUKRS = entity.BUKRS.Substring(0, entity.BUKRS.IndexOf('('));
                    }

                    if (entity.closed_date != null)
                    {
                        entity.BLDAT = entity.closed_date.GetValueOrDefault().ToString("yyyyMMdd");
                        entity.BUDAT = entity.closed_date.GetValueOrDefault().ToString("yyyyMMdd");
                    }
                    else
                    {
                        entity.BLDAT = DateTime.Now.ToString("yyyyMMdd");
                        entity.BUDAT = DateTime.Now.ToString("yyyyMMdd");
                    }

                    if (entity.PROTYP == "Project")
                    {
                        entity.PROTYP = "P";
                    }
                    else
                    {
                        entity.PROTYP = "E";
                    }

                    //根据费用名称获取 科目编号
                    var categoryItem = ExpenseCategoryBll.GetExpenseCategoryByName(inn, entity.SGTXT);
                    if (!categoryItem.isError() && categoryItem.getItemCount() > 0)
                    {
                        //科目代码
                        string hkont = categoryItem.getItemByIndex(0).getProperty("b_corresponsubject");
                        entity.HKONT = hkont;
                    }

                    //根据项目名称获取项目编号
                    var b_ProjectItem = ProjectManageDA.GetProjectManageByName(inn, entity.AUFNR);
                    if (!b_ProjectItem.isError())
                    {
                        string b_projectrecordno = b_ProjectItem.getItemByIndex(0).getProperty("b_projectrecordno");
                        entity.AUFNR = b_projectrecordno;
                    }
                }
            }
            return datas;
        }
    }
}
