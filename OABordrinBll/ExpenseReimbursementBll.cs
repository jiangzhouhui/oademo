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

namespace OABordrinBll
{
    public class ExpenseReimbursementBll
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
                string sqlStr = "Declare @IndexNum int Select @IndexNum = isnull(Max(B_INDEXNUM), 0) + 1 from innovator.b_ExpenseReimbursement where CREATED_ON &gt;= CONVERT(varchar(10),dateadd(dd,-day(getdate())+1,getdate()),120) and CREATED_ON &lt; CONVERT(CHAR(10),DATEADD(m,1,DATEADD(dd,-DAY(GETDATE())+1,GETDATE())),111) update innovator.b_ExpenseReimbursement set b_RecordNo = b_RecordNo + RIGHT('00000' + CAST(@IndexNum as varchar), 5),b_indexnum=@IndexNum where id = '" + id + "'";
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
            var item = inn.newItem("b_ExpenseReimbursement", "get");
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
        public static void DeleteExpenseReimbursement(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_ExpenseReimbursement' action='delete' id='" + id + "'></Item></AML>";
            var result = inn.applyAML(strAml);

        }


        /// <summary>
        /// 删除报销明细
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteExpenseReimbursementItem(Innovator inn, string id)
        {
            //询价信息
            var R_ReimbursementItem = inn.newItem("R_ReimbursementItem", "get");
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
                string amlStr = "<AML><Item type='R_ReimbursementItem' action='purge' idlist='" + requestIds + "'></Item><Item type='b_ReimbursementItem' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除借款明细
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteLoanItem(Innovator inn, string id)
        {
            //询价信息
            var R_LoanItem = inn.newItem("R_LoanItem", "get");
            R_LoanItem.setAttribute("where", "source_id='" + id + "'");
            R_LoanItem.setAttribute("select", "related_id");
            var oldItems = R_LoanItem.apply();
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
                string amlStr = "<AML><Item type='R_LoanItem' action='purge' idlist='" + requestIds + "'></Item><Item type='b_LoanItem' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除对应的附件信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteFile(Innovator inn, string id)
        {
            var R_ExpenseReimbursementFile = inn.newItem("R_ExpenseReimbursementFile", "get");
            R_ExpenseReimbursementFile.setAttribute("where", "source_id='" + id + "'");
            R_ExpenseReimbursementFile.setAttribute("select", "related_id");
            var oldItems = R_ExpenseReimbursementFile.apply();
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
                string amlStr = "<AML><Item type='R_ExpenseReimbursementFile' action='purge' idlist='" + requestIds + "'></Item><Item type='File' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }




        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="emailEntity"></param>
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
                subject = "Your expense reimbursement application < " + emailEntity.RecordNo + " > has been rejected.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your expense reimbursement application < " + emailEntity.RecordNo + " > has been rejected.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/ExpenseReimbursement/Index";
            }
            else if (keyedName == "End")
            {

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
                subject = "Your expense reimbursement application < " + emailEntity.RecordNo + " > has been approved.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your expense reimbursement application < " + emailEntity.RecordNo + " > has been approved.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/ExpenseReimbursement/Index";
            }
            else
            {
                subject = "<" + emailEntity.ApplicantDepartment + "> " + emailEntity.ApplicantName + " has submitted an expense reimbursement application < " + emailEntity.RecordNo + " > for your approval.";
                body += "Dear " + nameStr + ",<br/><br/>";
                body += "<" + emailEntity.ApplicantDepartment + "> " + emailEntity.ApplicantName + " has submitted an expense reimbursement application < " + emailEntity.RecordNo + " > for your approval.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/ExpenseReimbursement/Index";
            }
            listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");
            //listEmail.Add("Jingjian.Gong@Bordrin.com");
            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }


        /// <summary>
        /// 当财务总监审核通过后，发送邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="proposer"></param>
        /// <param name="recordNo"></param>
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

            subject = "Your expense reimbursement application <" + recordNo + "> has been approved by financial analyst, Please hand in your application and receipts to finance department.";

            body = "您单号为< " + recordNo + " >的费用报销单已通过财务分析员审核，请将报销单打印并附上报销凭证，交到财务部门进行费用审核。<br/>";
            body += "Your expense reimbursement application < " + recordNo + " > has been approved by financial analyst, Please hand in your application and receipts to finance department.";

            listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");
            //listEmail.Add("Jingjian.Gong@Bordrin.com");

            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }

        /// <summary>
        /// 管理员重发凭证邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_Employee"></param>
        /// <param name="recordNo"></param>
        public static void ExpenseAccountantCheckSendEmail(Innovator inn,string b_Employee,string recordNo)
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
            subject = "您单号为["+ recordNo + "]的费用报销单已通过财务分析员审核，请将报销单打印并附上报销凭证，交到财务部门进行费用审核。——如已打印并交到财务部，请忽略此邮件。";

            body = "您单号为["+ recordNo + "]的费用报销单已通过财务分析员审核，请将报销单打印并附上报销凭证，交到财务部门进行费用审核。——如已打印并交到财务部，请忽略此邮件。<br/>";
            body += "打印方式：进入 https://oa.bordrin.com ,打开菜单 报销管理->查询费用报销，找到对应申请单，单击单号后面的打印按钮打印申请单。";

            listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");
            listEmail.Add("Kai.Feng @bordrin.com");
            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }



        /// <summary>
        /// 根据ID获取报销单
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetExpenseReimbursementObjById(Innovator inn, string id)
        {
            var expenseReimbursement = inn.newItem("b_ExpenseReimbursement", "get");
            expenseReimbursement.setAttribute("id", id);

            //报销明细
            var R_ReimbursementItem = inn.newItem("R_ReimbursementItem", "get");
            R_ReimbursementItem.setAttribute("where", "source_id='" + id + "'");
            R_ReimbursementItem.setAttribute("select", "related_id");
            expenseReimbursement.addRelationship(R_ReimbursementItem);

            //借款明细
            var R_LoanItem = inn.newItem("R_LoanItem", "get");
            R_LoanItem.setAttribute("where", "source_id='" + id + "'");
            R_LoanItem.setAttribute("select", "related_id");
            expenseReimbursement.addRelationship(R_LoanItem);

            //附件
            var R_ExpenseReimbursement = inn.newItem("R_ExpenseReimbursementFile", "get");
            R_ExpenseReimbursement.setAttribute("where", "source_id='" + id + "'");
            R_ExpenseReimbursement.setAttribute("select", "related_id");
            expenseReimbursement.addRelationship(R_ExpenseReimbursement);
            var result = expenseReimbursement.apply();
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
            string strSql = "select b_Remark from innovator.b_ExpenseReimbursement where id='" + id + "'";

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
        /// <param name="ChoicePath"></param>
        public void AutomaticCompletionTask(Innovator inn, string id, decimal totalAmount, bool b_IsBudgetary, string b_Type, string lineLeader, string departmentLeader, ref WORKFLOW_PROCESS_PATH ChoicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.b_ExpenseReimbursement");
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
                    string lineName = GetLineNameByActivityName(inn, activityId, keyedName, totalAmount, b_IsBudgetary);
                    WORKFLOW_PROCESS_PATH newChoicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.b_ExpenseReimbursement", lineName);
                    if (newChoicePath != null)
                    {
                        ChoicePath = newChoicePath;
                        AutomaticCompletionTask(inn, id, totalAmount, b_IsBudgetary, b_Type, lineLeader, departmentLeader, ref ChoicePath);
                    }
                }
            }
        }


        /// <summary>
        /// 挂起自动完成审核
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id">数据ID</param>
        /// <param name="b_HangUpActivityName">挂起所在的流程节点</param>
        /// <param name="b_TotalAmount">总费用</param>
        /// <param name="b_IsBudgetary">是否在预算内</param>
        /// <param name="b_Type">项目类型</param>
        /// <param name="choicePath">选择的路线</param>
        public static void HangUpAutoAudit(Innovator inn, string id, string b_HangUpActivityName, decimal b_TotalAmount, bool b_IsBudgetary, string b_Type, string departmentLeader, ref WORKFLOW_PROCESS_PATH choicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.b_ExpenseReimbursement");
            //获取当前活动的流程名称
            string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
            string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");

            //当前流程节点的名称，不等于挂机的流程名称时，执行自动完成！
            if (b_HangUpActivityName != keyedName)
            {
                string lineName = GetLineNameByActivityName(inn, activityId, keyedName, b_TotalAmount, b_IsBudgetary);
                choicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.b_ExpenseReimbursement", lineName);
                HangUpAutoAudit(inn, id, b_HangUpActivityName, b_TotalAmount, b_IsBudgetary, b_Type, departmentLeader, ref choicePath);
            }
        }


        /// <summary>
        /// 根据活动名称选择附件
        /// </summary>
        /// <param name="activityName"></param>
        /// <param name="b_TotalAmount"></param>
        /// <param name="b_IsBudgetary"></param>
        /// <param name="b_Type"></param>
        /// <returns></returns>
        public static string GetLineNameByActivityName(Innovator inn, string activityId, string activityName, decimal b_TotalAmount, bool b_IsBudgetary)
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
                if (b_TotalAmount > 5000 || b_IsBudgetary == false || auditPersonCount == 0)
                {
                    lineName = "lineA";
                }
                //预算内  并且 总金额小于等于5千
                else if (b_IsBudgetary && b_TotalAmount <= 5000)
                {
                    lineName = "lineB";
                }
            }

            //部门VP审批
            if (activityName == "Division VP" || activityName == "Project VP")
            {
                //预算外  或者 总金额大于1万
                if (b_IsBudgetary == false || b_TotalAmount > 10000)
                {
                    lineName = "lineA";
                }
                //预算内 并且 总金额小于等于1万
                else if (b_IsBudgetary && b_TotalAmount <= 10000)
                {
                    lineName = "lineB";
                }
            }
            return lineName;
        }


        /// <summary>
        ///  添加审核权限
        /// </summary>
        public static void AddExpenseReimbursementAudit(Innovator inn, string id, string region, List<string> analysisAuditStr, List<string> accountAuditStr, string B_CENTRE)
        {
            //删除财务分析员
            List<string> activitys = new List<string> { "Financial Analyst", "Expense Accountant Check", "GM", "Financial Director", "Expense Accountant Creation" };
            ActivityBll.DeleteActivityAuthById(inn, id, "innovator.b_ExpenseReimbursement", activitys);

            //添加财务员审批
            //WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "财务分析员（" + region + "）", region, "Financial Analyst", "innovator.b_ExpenseReimbursement");
            ActivityBll.AddActivityAudit(inn, id, "Financial Analyst", "innovator.b_ExpenseReimbursement", analysisAuditStr);

            //添加CEO审核
            if (B_CENTRE == "盛和")
            {
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "GMSH", "", "GM", "innovator.b_ExpenseReimbursement");
            }
            else if (B_CENTRE == "骏盛")
            {
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "GMJS", "", "GM", "innovator.b_ExpenseReimbursement");
            }
            else
            {
                WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "CEO", "", "GM", "innovator.b_ExpenseReimbursement");
            }

            //费用会计审核
            //WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "费用会计（" + region + "）", region, "Expense Accountant Check", "innovator.b_ExpenseReimbursement");
            ActivityBll.AddActivityAudit(inn, id, "Expense Accountant Check", "innovator.b_ExpenseReimbursement", accountAuditStr);

            //财务总监
            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "财务总监（" + region + "）", region, "Financial Director", "innovator.b_ExpenseReimbursement");

            //费用会计制证 
            //WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, id, "费用会计（" + region + "）", region, "Expense Accountant Creation", "innovator.b_ExpenseReimbursement");
            ActivityBll.AddActivityAudit(inn, id, "Expense Accountant Creation", "innovator.b_ExpenseReimbursement", accountAuditStr);
        }

        //调用相关接口，发起会计制证
        public static List<ApiExpenseReimbursementEntity> SendExpenseAccountantCreation(Innovator inn, string id)
        {
            List<ApiExpenseReimbursementEntity> datas = new List<ApiExpenseReimbursementEntity>();
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                datas = (from g in db.B_REIMBURSEMENTITEM
                         join t in db.R_REIMBURSEMENTITEM on g.id equals t.RELATED_ID
                         join y in db.B_EXPENSEREIMBURSEMENT on t.SOURCE_ID equals y.id
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
                             SGTXT = g.B_CATEGORYNUMBER,
                             AUFNR = g.B_PROJECTNAME,
                             POSID = g.B_BUDGETNUMBER,
                             b_TaxRate = g.B_TAXRATE,
                             b_Tax = g.B_TAX,
                             b_TaxFreeAmount = g.B_TAXFREEAMOUNT,
                             b_StaffNo = y.B_STAFFNO
                         }).ToList();
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
                    if (!categoryItem.isError())
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
