using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class BusinessTravelBll
    {
        /// <summary>
        /// 生成单据编号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static string CreateRecordNo(Innovator inn, string id)
        {
            string b_recordno = "";
            if (!string.IsNullOrEmpty(id))
            {
                string sqlStr = "Declare @IndexNum int Select @IndexNum = isnull(Max(B_INDEXNUM), 0) + 1 from innovator.b_BusinessTravel where CREATED_ON &gt;= CONVERT(varchar(10),dateadd(dd,-day(getdate())+1,getdate()),120) and CREATED_ON &lt; CONVERT(CHAR(10),DATEADD(m,1,DATEADD(dd,-DAY(GETDATE())+1,GETDATE())),111) update innovator.b_BusinessTravel set b_DocumentNo = b_DocumentNo + RIGHT('00000' + CAST(@IndexNum as varchar), 5),b_indexnum=@IndexNum where id = '" + id + "'";
                var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");

                b_recordno = GetRecordNoById(inn, id);
            }
            return b_recordno;
        }

        /// <summary>
        /// 根据Id获取单据编号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetRecordNoById(Innovator inn, string id)
        {
            var item = inn.newItem("b_BusinessTravel", "get");
            item.setAttribute("id", id);
            item.setAttribute("select", "b_documentno");
            var result = item.apply();
            if (!result.isError())
            {
                return result.getProperty("b_documentno");
            }
            return "";
        }

        /// <summary>
        /// 删除主表信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteBusinessTravel(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_BusinessTravel' action='delete' id='" + id + "'></Item></AML>";
            var result = inn.applyAML(strAml);

        }

        /// <summary>
        /// 删除机票代订
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteFlightBookingItem(Innovator inn, string id)
        {
            //询价信息
            var R_ReimbursementItem = inn.newItem("R_FlightBooking", "get");
            R_ReimbursementItem.setAttribute("where", "source_id='" + id + "'");
            R_ReimbursementItem.setAttribute("select", "related_id");
            var oldItems = R_ReimbursementItem.apply();
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
                string amlStr = "<AML><Item type='R_FlightBooking' action='purge' idlist='" + requestIds + "'></Item><Item type='R_FlightBooking' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除酒店代订
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteHotelBookingItem(Innovator inn, string id)
        {
            //询价信息
            var R_ReimbursementItem = inn.newItem("R_HotelBooking", "get");
            R_ReimbursementItem.setAttribute("where", "source_id='" + id + "'");
            R_ReimbursementItem.setAttribute("select", "related_id");
            var oldItems = R_ReimbursementItem.apply();
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
                string amlStr = "<AML><Item type='R_HotelBooking' action='purge' idlist='" + requestIds + "'></Item><Item type='R_HotelBooking' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除对应附件信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteFile(Innovator inn, string id)
        {
            var R_File = inn.newItem("R_File", "get");
            R_File.setAttribute("where", "source_id='" + id + "'");
            R_File.setAttribute("select", "related_id");
            var oldItems = R_File.apply();
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
                string amlStr = "<AML><Item type='R_File' action='purge' idlist='" + requestIds + "'></Item><Item type='File' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }


        /// <summary>
        /// 根据ID获取报销单
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetBusinessTravelObjById(Innovator inn, string id)
        {
            var expenseBusinessTravel = inn.newItem("b_BusinessTravel", "get");
            expenseBusinessTravel.setAttribute("id", id);

            //机票代订明细
            var R_FlightBooking = inn.newItem("R_FlightBooking", "get");
            R_FlightBooking.setAttribute("where", "source_id='" + id + "'");
            R_FlightBooking.setAttribute("select", "related_id");
            expenseBusinessTravel.addRelationship(R_FlightBooking);

            //酒店代订明细
            var R_HotelBooking = inn.newItem("R_HotelBooking", "get");
            R_HotelBooking.setAttribute("where", "source_id='" + id + "'");
            R_HotelBooking.setAttribute("select", "related_id");
            expenseBusinessTravel.addRelationship(R_HotelBooking);

            //附件
            var R_File = inn.newItem("R_File", "get");
            R_File.setAttribute("where", "source_id='" + id + "'");
            R_File.setAttribute("select", "related_id");
            expenseBusinessTravel.addRelationship(R_File);
            var result = expenseBusinessTravel.apply();
            return result;
        }

        /// <summary>
        /// 获取老的备注
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetOldRemarkById(Innovator inn, string id)
        {
            string strSql = "select b_Remark from innovator.b_BusinessTravel where id='" + id + "'";

            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            if (!returnItem.isError() && returnItem.getItemCount() > 0)
            {
                return returnItem.getProperty("b_remark");
            }
            return "";
        }

        /// <summary>
        /// 自动完成任务
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="b_TripType"></param>
        /// <param name="b_Type"></param>
        /// <param name="lineleader"></param>
        /// <param name="deptleader"></param>
        /// <param name="ChoicePath"></param>
        public void AutomaticCompletionTask(Innovator inn, string id, ref WORKFLOW_PROCESS_PATH ChoicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.b_BusinessTravel");
            if (!activityItem.isError() && activityItem.getItemCount() > 0)
            {
                string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
                string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");
                //获取当前任务的操作权限
                bool isOperate = false;
                Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId);
                if (!identitys.isError() && identitys.getItemCount() == 1)
                {
                    isOperate = true;
                }
                if (isOperate)
                {
                    string lineName = GetLineNameByActivityName(inn, keyedName);
                    WORKFLOW_PROCESS_PATH newChoicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.b_BusinessTravel", lineName);
                    if (newChoicePath != null)
                    {
                        ChoicePath = newChoicePath;
                        AutomaticCompletionTask(inn, id, ref ChoicePath);
                    }
                }
            }
        }


        /// <summary>
        /// 挂起自动完成审核
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="b_HangUpActivityName"></param>
        /// <param name="b_TripType"></param>
        /// <param name="b_Type"></param>
        /// <param name="b_Director"></param>
        /// <param name="choicePath"></param>
        public static void HangUpAutoAudit(Innovator inn, string id, string b_HangUpActivityName, ref WORKFLOW_PROCESS_PATH choicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.b_BusinessTravel");
            //获取当前活动的流程名称
            string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
            string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");

            //当前流程节点的名称，不等于挂机的流程名称时，执行自动完成！
            if (b_HangUpActivityName != keyedName)
            {
                string lineName = GetLineNameByActivityName(inn, keyedName);
                choicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.b_BusinessTravel", lineName);
                HangUpAutoAudit(inn, id, b_HangUpActivityName, ref choicePath);
            }
        }


        /// <summary>
        /// 根据活动名称选择附件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="activityId"></param>
        /// <param name="keyedName"></param>
        /// <param name="b_Type"></param>
        /// <returns></returns>
        public static string GetLineNameByActivityName(Innovator inn, string activityName)
        {
            string lineName = "agree";
            return lineName;
        }

        public static void AddBusinessTraveAudit(Innovator inn, string id, string region, string B_CENTRE, bool isAdministrativeSupport, string b_TripType)
        {
            //删除财务分析员
            List<string> activitys = new List<string> { "Administrative approval", "GM" };
            ActivityBll.DeleteActivityAuthById(inn, id, "innovator.b_BusinessTravel", activitys);

            if (b_TripType == "International")
            {
                //添加CEO审核
                if (B_CENTRE == "盛和")
                {
                    WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "GMSH", "", "GM", "innovator.b_BusinessTravel");
                }
                else if (B_CENTRE == "骏盛")
                {
                    WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "GMJS", "", "GM", "innovator.b_BusinessTravel");
                }
                else
                {
                    WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "CEO", "", "GM", "innovator.b_BusinessTravel");
                }
            }
            if (isAdministrativeSupport)
            {
                //行政人员
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "行政人员（" + region + "）", region, "Administrative approval", "innovator.b_BusinessTravel");
            }
        }


        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="emailEntity"></param>
        /// <param name="choicePath"></param>
        public static void SendEmailByOperation(Innovator inn, EmailEntity emailEntity, WORKFLOW_PROCESS_PATH choicePath, string region = "")
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
                subject = "Your business travel application< " + emailEntity.RecordNo + " > has been rejected.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your business travel application < " + emailEntity.RecordNo + " > has been rejected.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/BusinessTravel/Index";
            }
            else if (keyedName == "End")
            {

                //获取申请人邮箱
                Item applicantIdentity = IdentityDA.GetIdentityByKeyedName(inn, emailEntity.ApplicantName);
                if (!applicantIdentity.isError())
                {
                    UserDA.GetEmailByIdentitys(inn, applicantIdentity, listEmail, names);
                }

                //根据地区获取行政人员
                List<IDENTITY> administrationIdentitys = IdentityDA.GetMemberByIdentityName("行政人员（" + region + "）");
                List<string> listNames = new List<string>();
                foreach (var item in administrationIdentitys)
                {
                    Item identity = IdentityDA.GetIdentityByKeyedName(inn, emailEntity.ApplicantName);

                    UserDA.GetEmailByIdentitys(inn, identity, listEmail, listNames);
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
                subject = "Your business travel application< " + emailEntity.RecordNo + " > has been approved.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your business travel application < " + emailEntity.RecordNo + " > has been approved.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/BusinessTravel/Index";
            }
            else
            {
                subject = "<" + emailEntity.ApplicantDepartment + "> " + emailEntity.ApplicantName + " has submitted a business travel application< " + emailEntity.RecordNo + " > for your approval.";
                body += "Dear " + nameStr + ",<br/><br/>";
                body += "<" + emailEntity.ApplicantDepartment + "> " + emailEntity.ApplicantName + " has submitted a business travel application< " + emailEntity.RecordNo + " > for your approval.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/BusinessTravel/Index";
            }

            listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");

            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }


        /// <summary>
        /// 修改出差单报销状态
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_IsReimbursement"></param>
        /// <param name="b_BTRecordNo"></param>
        public static void UpdateBusinessTravelIsReimbursement(Innovator inn, string b_IsReimbursement, string b_BTRecordNo)
        {
            if(!string.IsNullOrEmpty(b_BTRecordNo))
            {
                BusinessTravelDA.UpdateBusinessTravelIsReimbursement(inn, b_IsReimbursement, b_BTRecordNo);
            }
        }

    }
}
