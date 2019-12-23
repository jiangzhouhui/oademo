using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.ExpenseReimbursement
{
    public class ExpenseReimbursementController : BaseController
    {
        // GET: ExpenseReimbursement
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("费用报销申请", "ERCommon", "ERItemType", ViewBag.language);
            ViewBag.UserName = Userinfo.UserName;
            ViewBag.UserEmail = Userinfo.Email;
            ViewBag.department = Userinfo.department;
            ViewBag.b_JobNumber = Userinfo.b_JobNumber;
            return View();
        }

        public JsonResult GetExpenseReimbursementList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            //获取委托权限数据
            List<string> agentRoles = AgentSetBll.GetAgentRoles(Userinfo, "ExpenseReimbursement");

            int total = 0;
            var dataList = GetExpenseReimbursementList(Userinfo, out total, para, searchValue, startTime, endTime, status, agentRoles);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "";

                    //获取当前活动的数据
                    var result = ActivityDA.GetActivityAuditByLoginInfo(inn, item.Id, "innovator.B_EXPENSEREIMBURSEMENT", Userinfo.Roles, agentRoles);
                    if (!result.isError() && result.getItemCount() > 0)
                    {
                        item.activityAssignmentId = result.getItemByIndex(0).getProperty("activityassignmentid");
                    }

                    if (item.status == "Start")
                    {
                        linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' activityId='" + item.activityId + "' activityAssignmentId='" + item.activityAssignmentId + "' Id='" + item.Id + "' ItemStatus='" + item.status.Trim() + "'></a>";
                    }
                    else
                    {
                        if (!result.isError() && result.getItemCount() > 0)
                        {
                            linkAList += "<a class='glyphicon glyphicon-user audit' title='审核' activityId='" + item.activityId + "' activityAssignmentId='" + item.activityAssignmentId + "' Id='" + item.Id + "' ItemStatus='" + item.status.Trim() + "' ></a>";
                        }
                    }
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-list-alt history' title='日志' Id='" + item.Id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-asterisk workflow' title='流程' ItemStatus='" + item.status + "' Id='" + item.Id + "'  ></a>";
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' Id='" + item.Id + "' ></a>";

                    if ((item.b_Preparer == Userinfo.UserName) && item.status != "Start" && item.status != "Financial Director" && item.status != "Expense Accountant Creation")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='fa fa-mail-reply recall' title='撤回' Id='" + item.Id + "' activityId='" + item.activityId + "' ></a>";
                    }
                    if (item.status == "Start")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' Id='" + item.Id + "' ></a>";
                    }
                    strHtml = string.Format(strHtml, item.b_RecordNo, linkAList);
                    item.b_ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    item.b_RecordNo = strHtml;
                    item.AuditorStr = ActivityDA.GetActivityOperator(inn, item.activityId, false);
                    item.AuditorStr = "<div style='width:150px;word-wrap:break-word;'>" + item.AuditorStr + "</div>";
                    item.status = Common.GetChineseValueByParam(item.status, "ERManageWorkFlow", "WorkFlow", Userinfo.language);
                }
            }
            return Json(new
            {
                sEcho = para.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = dataList
            }, JsonRequestBehavior.AllowGet);
        }


        public static List<ExpenseReimbursementModel> GetExpenseReimbursementList(UserInfo userInfo, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status, List<string> agentRoles)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<ExpenseReimbursementModel> datas = (from g in db.B_EXPENSEREIMBURSEMENT
                                                               join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                               join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                               join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                               join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                               join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                               join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                               where (i.STATE == "active" && o.VOTING_WEIGHT>0 && (userInfo.Roles.Contains(p.ID) || g.B_PREPARER == userInfo.UserName || (agentRoles.Contains(p.ID) && i.KEYED_NAME != "Start"))) && (g.B_RECORDNO.Contains(searchValue) || g.B_DEPT.Contains(searchValue) || g.B_EMPLOYEE.Contains(searchValue)) && o.CLOSED_BY == null
                                                               select new ExpenseReimbursementModel
                                                               {
                                                                   Id = g.id,
                                                                   b_RecordNo = g.B_RECORDNO,
                                                                   nb_ApplicationDate = g.CREATED_ON,
                                                                   b_Dept = g.B_DEPT,
                                                                   b_Employee = g.B_EMPLOYEE,
                                                                   b_Preparer = g.B_PREPARER,
                                                                   b_DueCompany = g.B_DUECOMPANY,
                                                                   status = i.KEYED_NAME,
                                                                   activityId = i.ID
                                                               });
                // 时间查询
                if (startTime != null)
                {
                    datas = datas.Where(x => x.nb_ApplicationDate >= startTime);
                }

                if (endTime != null)
                {
                    datas = datas.Where(x => x.nb_ApplicationDate <= endTime);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    datas = datas.Where(x => x.status == status);
                }
                datas = datas.Distinct();
                total = datas.Count();
                //排序
                if (para!=null)
                {
                    if (para.sSortType == "asc")
                    {
                        datas = Common.OrderBy(datas, para.iSortTitle, false);
                    }
                    else
                    {
                        datas = Common.OrderBy(datas, para.iSortTitle, true);
                    }
                    //分页
                    datas = datas.Skip(para.iDisplayStart).Take(para.iDisplayLength);
                }
                return datas.ToList();
            }
        }

        /// <summary>
        /// 保存费用报销申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveExpenseReimbursement(ExpenseReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item lineLeader = null;
                Item departmentLeader = null;
                Item divisionVP = null;
                //申请人
                if (!string.IsNullOrEmpty(model.b_Employee))
                {
                    var employee = IdentityDA.GetIdentityByKeyedName(inn, model.b_Employee);
                    if (employee.isError())
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("找不到填写的申请人！", "ERCommon", "ERItemType", ViewBag.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //查看当前人是否为CEO
                bool isCEO = false;
                USER employeeInfo = UserDA.GetUserByFirstName(model.b_Employee);
                if (employeeInfo != null)
                {
                    //List<IDENTITY> identityList = IdentityDA.GetMemberByIdentityName("CEO");
                    //int ncount = identityList.Where(x => x.KEYED_NAME.Trim() == Userinfo.UserName.Trim()).Count();
                    if (UserBll.IsCEObyUserName(inn, model.b_Employee))
                    {
                        isCEO = true;
                        model.b_IsHangUp = true;
                        model.b_HangUpActivityName = "Financial Analyst";
                    }
                }

                //查看人员信息是否完整
                if (!isCEO && string.IsNullOrEmpty(employeeInfo.B_CENTRE))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "ERCommon", "ERItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (!isCEO && string.IsNullOrEmpty(model.b_LineLeader) && string.IsNullOrEmpty(model.b_DepartmentLeader) && string.IsNullOrEmpty(model.b_DivisionVP))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "ERCommon", "ERItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //验证高级经理
                if (!string.IsNullOrEmpty(model.b_LineLeader))
                {
                    lineLeader = IdentityDA.GetIdentityByKeyedName(inn, model.b_LineLeader);
                    if (lineLeader.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的高级经理！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //部门领导
                if (!string.IsNullOrEmpty(model.b_DepartmentLeader))
                {
                    departmentLeader = IdentityDA.GetIdentityByKeyedName(inn, model.b_DepartmentLeader);
                    if (departmentLeader.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的部门总监！");
                    }
                }

                //验证中心领导
                if (!string.IsNullOrEmpty(model.b_DivisionVP))
                {
                    divisionVP = IdentityDA.GetIdentityByKeyedName(inn, model.b_DivisionVP);
                    if (divisionVP.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的VP！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //验证报销申请明细是否填写
                if (model.ReimbursementItems == null || model.ReimbursementItems.Count <= 0)
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("报销明细必须填写!", "ERCommon", "ERItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //验证输入的借款人是否存在
                if (model.LoanItems != null && model.LoanItems.Count > 0)
                {
                    for (int i = 0; i < model.LoanItems.Count; i++)
                    {
                        Item Borrower = IdentityDA.GetIdentityByKeyedName(inn, model.LoanItems[i].b_Borrower);
                        if (Borrower.isError())
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("找不到对应的借款人!", "ERCommon", "ERItemType", ViewBag.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                }


                //验证输入的项目名称是否正确
                for (int index = 0; index < model.ReimbursementItems.Count; index++)
                {
                    var item = model.ReimbursementItems[index];
                    if (!string.IsNullOrEmpty(item.b_ProjectName))
                    {
                        var projectItem = ProjectManageDA.GetProjectManageByName(inn, item.b_ProjectName);
                        if (!string.IsNullOrEmpty(projectItem.getErrorString()) && projectItem.getItemCount() <= 0)
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("请输入正确的项目名称!", "ERCommon", "ERItemType", ViewBag.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                List<string> analysisAuditStr = new List<string>();
                List<string> accountAuditStr = new List<string>();
                List<string> projectNameList = new List<string>();

                //查询是否配置了对应的  财务分析员和费用会计
                List<B_EXPENSEAUDITCONFIGURATION> datalist = ExpenseAuditConfigurationBll.GetAllExpenseAuditConfiguration();
                if (model.b_Type == "Project")
                {
                    projectNameList.AddRange(model.ReimbursementItems.Select(x => x.b_ProjectName));
                    projectNameList = projectNameList.Distinct().ToList();

                    //验证是否配置了对应的财务分析员
                    List<B_EXPENSEAUDITCONFIGURATION> analysisObj = ExpenseAuditConfigurationBll.GetExpenseAuditConfigurationByProject("财务分析员", model.b_CompanyCode, projectNameList, datalist);
                    if (analysisObj != null && analysisObj.Count > 0)
                    {
                        for (int i = 0; i < analysisObj.Count; i++)
                        {
                            analysisAuditStr.AddRange(analysisObj[i].B_HANDLEPERSONS.Split(';').Where(x => x != "").ToList());
                        }
                    }

                    //验证是否配置了对应的费用会计
                    List<B_EXPENSEAUDITCONFIGURATION> accountiObj = ExpenseAuditConfigurationBll.GetExpenseAuditConfigurationByProject("费用会计", model.b_CompanyCode, projectNameList, datalist);
                    if (accountiObj != null && accountiObj.Count > 0)
                    {
                        for (int i = 0; i < accountiObj.Count; i++)
                        {
                            accountAuditStr.AddRange(accountiObj[i].B_HANDLEPERSONS.Split(';').Where(x => x != "").ToList());
                        }
                    }
                }
                else
                {
                    //验证是否匹配了对应的财务分析员
                    B_EXPENSEAUDITCONFIGURATION analysisObj = ExpenseAuditConfigurationBll.GetExpenseAuditConfigurationByNonProject("财务分析员", model.b_CompanyCode, model.b_CostCenter, datalist);

                    if (analysisObj != null && !string.IsNullOrEmpty(analysisObj.B_HANDLEPERSONS))
                    {
                        analysisAuditStr = analysisObj.B_HANDLEPERSONS.Split(';').Where(x => x != "").ToList();
                    }

                    //验证是否匹配了对应的费用会计
                    B_EXPENSEAUDITCONFIGURATION accountiObj = ExpenseAuditConfigurationBll.GetExpenseAuditConfigurationByNonProject("费用会计", model.b_CompanyCode, model.b_CostCenter, datalist);

                    if (accountiObj != null && !string.IsNullOrEmpty(accountiObj.B_HANDLEPERSONS))
                    {
                        accountAuditStr = accountiObj.B_HANDLEPERSONS.Split(';').Where(x => x != "").ToList();
                    }
                }

                analysisAuditStr = analysisAuditStr.Distinct().ToList();
                accountAuditStr = accountAuditStr.Distinct().ToList();
                if (analysisAuditStr.Count == 0)
                {
                    retModel.AddError("errorMessage", "未配置对应的财务分析审核人！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (accountAuditStr.Count == 0)
                {
                    retModel.AddError("errorMessage", "未配置对应的费用会计审核人！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //数据准备
                Item itemRoot = inn.newItem();
                if (string.IsNullOrEmpty(model.status))
                {
                    //设置单据编号
                    string dateStr = DateTime.Now.ToString("yyyyMM");
                    model.b_RecordNo = "ER" + "-" + dateStr + "-";
                    itemRoot = inn.newItem("b_ExpenseReimbursement", "add");
                    itemRoot = OperationData(model, itemRoot);
                }
                else if (model.status == "Start")
                {
                    //删除报销明细
                    ExpenseReimbursementBll.DeleteExpenseReimbursementItem(inn, model.Id);

                    //删除借款明细
                    ExpenseReimbursementBll.DeleteLoanItem(inn, model.Id);

                    //修改编辑数据
                    itemRoot = inn.newItem("b_ExpenseReimbursement", "edit");
                    itemRoot.setAttribute("id", model.Id);
                    itemRoot = OperationData(model, itemRoot);
                }

                //备注
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    if (!string.IsNullOrEmpty(model.Id))
                    {
                        string oldRemark = ExpenseReimbursementBll.GetOldRemarkById(inn, model.Id);
                        if (!string.IsNullOrEmpty(oldRemark))
                        {
                            model.b_Remark = oldRemark + "<br/>" + Userinfo.UserName + ":" + model.b_Remark;
                        }
                        else
                        {
                            model.b_Remark = Userinfo.UserName + ":" + model.b_Remark;
                        }
                    }
                    else
                    {
                        model.b_Remark = Userinfo.UserName + ":" + model.b_Remark;
                    }
                    itemRoot.setProperty("b_remark", model.b_Remark);
                }
                var result = itemRoot.apply();
                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.status))
                    {
                        model.Id = result.getProperty("id");
                        model.b_RecordNo = ExpenseReimbursementBll.CreateRecordNo(inn, model.Id);
                    }
                }

                //推动流程状态
                if (model.operation == "submit")
                {
                    if (model.b_Type == "Non Project")
                    {
                        //添加高级经理
                        List<string> lineLeaders = new List<string>() { "Dept.Manager" };
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_ExpenseReimbursement", lineLeaders);
                        if (lineLeader != null)
                        {
                            string lineLeaderId = lineLeader.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.Id, lineLeaderId, lineLeaders, "innovator.b_ExpenseReimbursement");
                        }

                        //添加部门总监
                        List<string> departmentLeaders = new List<string> { "Dept.Director" };
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_ExpenseReimbursement", departmentLeaders);
                        if (departmentLeader != null)
                        {
                            string departmentLeaderId = departmentLeader.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.Id, departmentLeaderId, departmentLeaders, "innovator.b_ExpenseReimbursement");
                        }
                        //添加中心VP
                        List<string> divisionVps = new List<string>() { "Division VP" };
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_ExpenseReimbursement", divisionVps);
                        if (divisionVP != null)
                        {
                            string divisionVpId = divisionVP.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.Id, divisionVpId, divisionVps, "innovator.b_ExpenseReimbursement");
                        }
                    }
                    else
                    {
                        //当为项目类型时   添加项目审核人员
                        List<string> projectAudits = new List<string> { "Project Manager", "Project Director", "Project VP" };
                        //删除项目审核人员
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_ExpenseReimbursement", projectAudits);
                        //添加项目审核人员
                        AddProjectAudit(inn, model);

                    }

                    //根据地区添加流程节点，角色审核
                    //AddWorkFlowRoleAuditByRegion();
                    ExpenseReimbursementBll.AddExpenseReimbursementAudit(inn, model.Id, model.b_ReimbursementPlace, analysisAuditStr, accountAuditStr, employeeInfo.B_CENTRE);

                    //如果activityId为空时，任务ID
                    if (string.IsNullOrEmpty(model.activityId))
                    {
                        Item activityItem = ActivityDA.GetActivityByItemId(inn, model.Id, "innovator.b_ExpenseReimbursement");
                        if (!activityItem.isError())
                        {
                            model.activityId = activityItem.getProperty("activityid");
                            model.activityAssignmentId = activityItem.getProperty("activityassignmentid");
                        }
                    }
                    //获取路线
                    var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
                    string submitLineName = model.b_Type == "Non Project" ? "Dept" : "Project";
                    WORKFLOW_PROCESS_PATH choicePath = listActivity.Where(x => x.NAME == submitLineName).FirstOrDefault();
                    if (choicePath != null)
                    {
                        string errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark);
                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            retModel.AddError("errorMessage", errorStr);
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }

                        //判断当前单据是否挂起
                        if (model.b_IsHangUp && !string.IsNullOrEmpty(model.b_HangUpActivityName))
                        {
                            //挂起的自动审核
                            ExpenseReimbursementBll.HangUpAutoAudit(inn, model.Id, model.b_HangUpActivityName, model.b_TotalAmount, model.b_IsBudgetary, model.b_Type, model.b_DepartmentLeader, ref choicePath);
                        }
                        else
                        {
                            ExpenseReimbursementBll bll = new ExpenseReimbursementBll();
                            //判断下一步骤如果无人审核   直接过掉
                            bll.AutomaticCompletionTask(inn, model.Id, model.b_TotalAmount, model.b_IsBudgetary, model.b_Type, model.b_LineLeader, model.b_DepartmentLeader, ref choicePath);
                        }
                        if (choicePath != null)
                        {
                            EmailEntity emailEntity = new EmailEntity();
                            emailEntity.ItemId = model.Id;
                            emailEntity.ApplicantDepartment = model.b_Dept;
                            emailEntity.ApplicantName = model.b_Employee;
                            emailEntity.RecordNo = model.b_RecordNo;
                            ExpenseReimbursementBll.SendEmailByOperation(inn, emailEntity, choicePath);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 操作数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="itemRoot"></param>
        /// <returns></returns>
        private Item OperationData(ExpenseReimbursementModel model, Item itemRoot)
        {
            itemRoot.setProperty("b_recordno", model.b_RecordNo);
            itemRoot.setProperty("b_companycode", model.b_CompanyCode);
            itemRoot.setProperty("b_reimbursementplace", model.b_ReimbursementPlace);
            itemRoot.setProperty("b_isbudgetary", model.b_IsBudgetary ? "1" : "0");
            itemRoot.setProperty("b_preparer", model.b_Preparer);
            itemRoot.setProperty("b_preparerno", model.b_PreparerNo);
            itemRoot.setProperty("b_applicationdate", model.b_ApplicationDate);
            itemRoot.setProperty("b_employee", model.b_Employee);
            itemRoot.setProperty("b_staffno", model.b_StaffNo);
            itemRoot.setProperty("b_position", model.b_Position);
            itemRoot.setProperty("b_dept", model.b_Dept);
            itemRoot.setProperty("b_costcenter", model.b_CostCenter);
            itemRoot.setProperty("b_tel", model.b_Tel);
            itemRoot.setProperty("b_advancedamount", model.b_AdvancedAmount.ToString());
            itemRoot.setProperty("b_totalexpense", model.b_TotalExpense.ToString());
            itemRoot.setProperty("b_duecompany", model.b_DueCompany);
            itemRoot.setProperty("b_amountinwords", model.b_AmountInWords);
            itemRoot.setProperty("b_totalamount", model.b_TotalAmount.ToString());
            itemRoot.setProperty("b_type", model.b_Type);
            itemRoot.setProperty("b_attachmentsquantity", model.b_AttachmentsQuantity.ToString());
            itemRoot.setProperty("b_lineleader", model.b_LineLeader);
            itemRoot.setProperty("b_departmentleader", model.b_DepartmentLeader);
            itemRoot.setProperty("b_divisionvp", model.b_DivisionVP);
            if (model.operation == "submit")
            {
                itemRoot.setProperty("b_ishangup", "0");
                itemRoot.setProperty("b_hangupactivityname", "");
            }


            //报销明细
            if (model.ReimbursementItems != null)
            {
                for (int i = 0; i < model.ReimbursementItems.Count; i++)
                {
                    var item = model.ReimbursementItems[i];
                    var r_ReimbursementItem = inn.newItem("R_ReimbursementItem", "add");
                    var ReimbursementItem = inn.newItem("b_ReimbursementItem", "add");
                    ReimbursementItem.setProperty("b_date", item.b_Date);
                    ReimbursementItem.setProperty("b_categorynumber", item.b_CategoryNumber);
                    ReimbursementItem.setProperty("b_projectname", item.b_ProjectName);
                    ReimbursementItem.setProperty("b_budgetnumber", item.b_BudgetNumber);
                    ReimbursementItem.setProperty("b_currency", item.b_Currency);
                    ReimbursementItem.setProperty("b_rate", item.b_Rate.ToString());
                    ReimbursementItem.setProperty("b_originalcurrency", item.b_OriginalCurrency.ToString());
                    ReimbursementItem.setProperty("b_count", item.b_Count.ToString());
                    ReimbursementItem.setProperty("b_taxrate", item.b_TaxRate.ToString());
                    ReimbursementItem.setProperty("b_tax", item.b_Tax.ToString());
                    ReimbursementItem.setProperty("b_taxfreeamount", item.b_TaxFreeAmount.ToString());
                    ReimbursementItem.setProperty("b_cnysubtotal", item.b_CNYSubtotal.ToString());
                    r_ReimbursementItem.setRelatedItem(ReimbursementItem);
                    itemRoot.addRelationship(r_ReimbursementItem);
                }
            }

            //借款明细
            if (model.LoanItems != null)
            {
                for (int i = 0; i < model.LoanItems.Count; i++)
                {
                    var item = model.LoanItems[i];
                    var r_LoanItem = inn.newItem("R_LoanItem", "add");
                    var LoanItem = inn.newItem("b_LoanItem", "add");
                    LoanItem.setProperty("b_loanorderno", item.b_LoanOrderNo);
                    LoanItem.setProperty("b_date", item.b_Date.ToString());
                    LoanItem.setProperty("b_borrower", item.b_Borrower);
                    LoanItem.setProperty("b_loanamount", item.b_LoanAmount.ToString());
                    LoanItem.setProperty("b_loanreason", item.b_LoanReason);
                    r_LoanItem.setRelatedItem(LoanItem);
                    itemRoot.addRelationship(r_LoanItem);
                }
            }

            if (string.IsNullOrEmpty(model.status))
            {
                //文件保存
                if (model.fileIds != null)
                {
                    for (int index = 0; index < model.fileIds.Count; index++)
                    {
                        var fileId = model.fileIds[index];
                        var erFile = inn.newItem("R_ExpenseReimbursementFile", "add");
                        var fileItem = inn.newItem("File", "get");
                        fileItem.setAttribute("id", fileId);
                        erFile.setRelatedItem(fileItem);
                        itemRoot.addRelationship(erFile);
                    }
                }
            }
            return itemRoot;
        }


        /// <summary>
        /// 审核费用报销
        /// </summary>
        /// <returns></returns>
        public JsonResult AuditExpenseReimbursement(ExpenseReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                EmailEntity emailEntity = new EmailEntity();
                emailEntity.ItemId = model.Id;
                emailEntity.ApplicantDepartment = model.b_Dept;
                emailEntity.ApplicantName = model.b_Employee;
                emailEntity.RecordNo = model.b_RecordNo;

                WORKFLOW_PROCESS_PATH choicePath = AuditChoiceLine(model, inn);
                string presentRemark = model.b_Remark;

                //修改编辑数据
                var itemRoot = inn.newItem("b_ExpenseReimbursement", "edit");
                itemRoot.setAttribute("id", model.Id);
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    string oldRemark = ExpenseReimbursementBll.GetOldRemarkById(inn, model.Id);
                    if (!string.IsNullOrEmpty(oldRemark))
                    {
                        model.b_Remark = oldRemark + "<br/>" + Userinfo.UserName + ":" + model.b_Remark;
                    }
                    else
                    {
                        model.b_Remark = Userinfo.UserName + ":" + model.b_Remark;
                    }
                    itemRoot.setProperty("b_remark", model.b_Remark);
                }

                if (model.status == "Expense Accountant Check" || model.status == "Expense Accountant Creation" || model.status == "Financial Analyst")
                {
                    //验证报销申请明细是否填写
                    if (model.ReimbursementItems == null || model.ReimbursementItems.Count <= 0)
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("报销明细必须填写!", "ERCommon", "ERItemType", ViewBag.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    //删除报销明细
                    ExpenseReimbursementBll.DeleteExpenseReimbursementItem(inn, model.Id);

                    //删除借款明细
                    ExpenseReimbursementBll.DeleteLoanItem(inn, model.Id);

                    itemRoot = OperationData(model, itemRoot);
                }
                var result = itemRoot.apply();
                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (choicePath != null)
                {
                    string errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark, Userinfo);
                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        retModel.AddError("errorMessage", errorStr);
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    //当活动无人审核时  自动完成
                    ExpenseReimbursementBll bll = new ExpenseReimbursementBll();
                    bll.AutomaticCompletionTask(inn, model.Id, model.b_TotalAmount, model.b_IsBudgetary, model.b_Type, model.b_LineLeader, model.b_DepartmentLeader, ref choicePath);


                    //判断当前环节是否关闭
                    if (ActivityBll.ActivityIsClosed(inn, choicePath.SOURCE_ID))
                    {
                        //判断下环节是否为CEO审核
                        var activityItem = ActivityDA.GetActivityById(inn, choicePath.RELATED_ID);
                        if (!activityItem.isError() && activityItem.getItemCount() > 0)
                        {
                            string name = activityItem.getProperty("name");
                            //获取CEO的名称
                            if (name == "GM" && !string.IsNullOrEmpty(name))
                            {
                                //判断CEO在之前是否审核过
                                if (model.b_Type == "Non Project" && UserBll.CeoBeforeIsAudit(inn, model.b_LineLeader, model.b_DepartmentLeader, model.b_DivisionVP,model.b_Employee))
                                {
                                    choicePath = WorkFlowBll.AutoCompleteActivityByParam(model.Id, "innovator.b_ExpenseReimbursement", "agree");
                                }
                            }
                        }
                    }

                    //判断活动是否结束，如果结束发送邮件到下一环节
                    if (ActivityBll.ActivityIsClosed(inn, choicePath.SOURCE_ID))
                    {
                        ExpenseReimbursementBll.SendEmailByOperation(inn, emailEntity, choicePath);
                        if (model.status == "Financial Analyst")
                        {
                            ExpenseReimbursementBll.SendEmailToProposer(inn, model.b_Employee, model.b_RecordNo);
                        }
                    }
                    ////发起Sap制证
                    //if (model.status == "Expense Accountant Creation")
                    //{
                    //    ExpenseReimbursementBll.SendExpenseAccountantCreation(inn, model.Id);
                    //}
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 审核选择的路线
        /// </summary>
        private static WORKFLOW_PROCESS_PATH AuditChoiceLine(ExpenseReimbursementModel model, Innovator inn)
        {
            //获取路线
            var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
            string lineName = ExpenseReimbursementBll.GetLineNameByActivityName(inn, model.activityId, model.status, model.b_TotalAmount, model.b_IsBudgetary);
            WORKFLOW_PROCESS_PATH choicePath = listActivity.Where(x => x.NAME == lineName).FirstOrDefault();
            return choicePath;
        }


        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult RefuseExpenseReimbursement(ExpenseReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    //修改编辑数据
                    var itemRoot = inn.newItem("b_ExpenseReimbursement", "edit");
                    itemRoot.setAttribute("id", model.Id);
                    string oldRemark = ExpenseReimbursementBll.GetOldRemarkById(inn, model.Id);
                    if (!string.IsNullOrEmpty(oldRemark))
                    {
                        model.b_Remark = oldRemark + "<br/>" + Userinfo.UserName + ":" + model.b_Remark;
                    }
                    else
                    {
                        model.b_Remark = Userinfo.UserName + ":" + model.b_Remark;
                    }
                    itemRoot.setProperty("b_remark", model.b_Remark);
                    var result = itemRoot.apply();
                }

                ReturnToStart(model, presentRemark);
            }
            catch (Exception ex)
            {

                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 挂起
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult HangUpExpenseReimbursement(ExpenseReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var itemRoot = inn.newItem("b_ExpenseReimbursement", "edit");
                itemRoot.setAttribute("id", model.Id);
                itemRoot.setProperty("b_ishangup", "1");
                itemRoot.setProperty("b_hangupactivityname", model.status);
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    string oldRemark = ExpenseReimbursementBll.GetOldRemarkById(inn, model.Id);
                    if (!string.IsNullOrEmpty(oldRemark))
                    {
                        model.b_Remark = oldRemark + "<br/>" + Userinfo.UserName + ":" + model.b_Remark;
                    }
                    else
                    {
                        model.b_Remark = Userinfo.UserName + ":" + model.b_Remark;
                    }
                }
                itemRoot.setProperty("b_remark", model.b_Remark);
                var result = itemRoot.apply();
                if (!result.isError())
                {
                    ReturnToStart(model, presentRemark);
                }
                else
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public JsonResult RecallExpenseReimbursement(string id, string activityId)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(activityId);
                WORKFLOW_PROCESS_PATH choicePath = null;
                choicePath = listActivity.Where(x => x.NAME == "ReturnStart").FirstOrDefault();
                //获取Admin的Aras 连接
                var adminInn = WorkFlowBll.GetAdminInnovator();
                if (adminInn != null && choicePath != null)
                {
                    //获取Admin 对当前任务权限数据
                    Item activityItem = ActivityDA.GetActivityByItemId(adminInn, id, "Administrators", "b_ExpenseReimbursement");
                    if (!activityItem.isError())
                    {
                        string nactivityId = activityItem.getProperty("activityid");
                        string activityAssignmentId = activityItem.getProperty("activityassignmentid");
                        ActivityDA.CompleteActivity(adminInn, nactivityId, activityAssignmentId, choicePath.ID, choicePath.NAME, "", Userinfo.UserName + Common.GetLanguageValueByParam("对单据进行了撤回操作！", "ERCommon", "ERItemType", ViewBag.language));
                    }
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 将流程退回到开始状态
        /// </summary>
        /// <param name="model"></param>
        private void ReturnToStart(ExpenseReimbursementModel model, string presentRemark)
        {
            EmailEntity emailEntity = new EmailEntity();
            emailEntity.ItemId = model.Id;
            emailEntity.ApplicantDepartment = model.b_Dept;
            emailEntity.ApplicantName = model.b_Employee;
            emailEntity.RecordNo = model.b_RecordNo;

            //获取路线
            var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
            WORKFLOW_PROCESS_PATH choicePath = null;
            if (!string.IsNullOrEmpty(model.status))
            {
                choicePath = listActivity.Where(x => x.NAME == "ReturnStart").FirstOrDefault();
            }
            string errorStr = "";
            if (model.status == "Project Director" || model.status == "Project Manager" || model.status == "Project VP")
            {
                //获取Admin的Aras 连接
                var adminInn = WorkFlowBll.GetAdminInnovator();
                if (adminInn != null)
                {
                    //获取Admin 对当前任务权限数据
                    Item activityItem = ActivityDA.GetActivityByItemId(adminInn, model.Id, "Administrators", "b_ExpenseReimbursement");
                    if (!activityItem.isError())
                    {
                        string activityId = activityItem.getProperty("activityid");
                        string activityAssignmentId = activityItem.getProperty("activityassignmentid");
                        errorStr = ActivityDA.CompleteActivity(adminInn, activityId, activityAssignmentId, choicePath.ID, choicePath.NAME, "", Userinfo.UserName + Common.GetLanguageValueByParam("对单据进行了拒绝操作！ 备注：", "ERCommon", "ERItemType", ViewBag.language) + presentRemark);
                    }
                }
            }
            else
            {
                errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark,Userinfo);
            }

            if (string.IsNullOrEmpty(errorStr))
            {
                ExpenseReimbursementBll.SendEmailByOperation(inn, emailEntity, choicePath);
            }
        }


        /// <summary>
        /// 根据ID获取报销申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetExpenseReimbursementById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result = ExpenseReimbursementBll.GetExpenseReimbursementObjById(inn, id);

                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                ExpenseReimbursementModel model = new ExpenseReimbursementModel();
                model.Id = result.getProperty("id");
                model.b_RecordNo = result.getProperty("b_recordno");
                model.b_CompanyCode = result.getProperty("b_companycode");
                model.b_ReimbursementPlace = result.getProperty("b_reimbursementplace");
                model.b_IsBudgetary = result.getProperty("b_isbudgetary") == "1" ? true : false;
                model.b_Preparer = result.getProperty("b_preparer");
                model.b_PreparerNo = result.getProperty("b_preparerno");
                model.b_ApplicationDate = DateTime.Parse(result.getProperty("b_applicationdate")).ToString("yyyy-MM-dd");
                model.b_Employee = result.getProperty("b_employee");
                model.b_StaffNo = result.getProperty("b_staffno");
                model.b_Position = result.getProperty("b_position");
                model.b_Dept = result.getProperty("b_dept");
                model.b_CostCenter = result.getProperty("b_costcenter");
                model.b_Tel = result.getProperty("b_tel");
                model.b_AdvancedAmount = !string.IsNullOrEmpty(result.getProperty("b_advancedamount")) ? decimal.Parse(result.getProperty("b_advancedamount")) : 0;
                model.b_TotalExpense = !string.IsNullOrEmpty(result.getProperty("b_totalexpense")) ? decimal.Parse(result.getProperty("b_totalexpense")) : 0;
                model.b_DueCompany = result.getProperty("b_duecompany");
                model.b_IsHangUp = result.getProperty("b_ishangup") == "1" ? true : false;
                model.b_AmountInWords = result.getProperty("b_amountinwords");
                model.b_TotalAmount = !string.IsNullOrEmpty(result.getProperty("b_totalamount")) ? decimal.Parse(result.getProperty("b_totalamount")) : 0;
                model.b_Type = result.getProperty("b_type");
                model.b_AttachmentsQuantity = !string.IsNullOrEmpty(result.getProperty("b_attachmentsquantity")) ? int.Parse(result.getProperty("b_attachmentsquantity")) : 0;
                model.b_LineLeader = result.getProperty("b_lineleader");
                model.b_DepartmentLeader = result.getProperty("b_departmentleader");
                model.b_DivisionVP = result.getProperty("b_divisionvp");
                model.b_HangUpActivityName = result.getProperty("b_hangupactivityname");
                model.OldRemark = result.getProperty("b_remark");
                if (model.OldRemark != null && model.OldRemark.Contains("\r\n"))
                {
                    model.OldRemark = model.OldRemark.Replace("\r\n", "<br/>");
                }

                //报销明细
                Item Relation = result.getRelationships("R_ReimbursementItem");
                if (Relation.getItemCount() > 0)
                {
                    model.ReimbursementItems = new List<ReimbursementItem>();
                    for (int i = 0; i < Relation.getItemCount(); i++)
                    {
                        Item ItemObJ = Relation.getItemByIndex(i).getRelatedItem();
                        ReimbursementItem itemModel = new ReimbursementItem();
                        itemModel.Id = ItemObJ.getProperty("id");
                        itemModel.b_Date = !string.IsNullOrEmpty(ItemObJ.getProperty("b_date")) ? DateTime.Parse(ItemObJ.getProperty("b_date")).ToString("yyyy-MM-dd") : "";
                        itemModel.b_CategoryNumber = ItemObJ.getProperty("b_categorynumber");
                        itemModel.b_ProjectName = ItemObJ.getProperty("b_projectname");
                        itemModel.b_BudgetNumber = ItemObJ.getProperty("b_budgetnumber");
                        itemModel.b_Currency = ItemObJ.getProperty("b_currency");
                        itemModel.b_Rate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_rate")) ? float.Parse(ItemObJ.getProperty("b_rate")) : 0;
                        itemModel.b_OriginalCurrency = !string.IsNullOrEmpty(ItemObJ.getProperty("b_originalcurrency")) ? decimal.Parse(ItemObJ.getProperty("b_originalcurrency")) : 0;
                        itemModel.b_Count = !string.IsNullOrEmpty(ItemObJ.getProperty("b_count")) ? int.Parse(ItemObJ.getProperty("b_count")) : 0;
                        itemModel.b_TaxRate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxrate")) ? float.Parse(ItemObJ.getProperty("b_taxrate")) : 0;
                        itemModel.b_Tax = !string.IsNullOrEmpty(ItemObJ.getProperty("b_tax")) ? decimal.Parse(ItemObJ.getProperty("b_tax")) : 0;
                        itemModel.b_TaxFreeAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxfreeamount")) ? decimal.Parse(ItemObJ.getProperty("b_taxfreeamount")) : 0;
                        itemModel.b_CNYSubtotal = !string.IsNullOrEmpty(ItemObJ.getProperty("b_cnysubtotal")) ? decimal.Parse(ItemObJ.getProperty("b_cnysubtotal")) : 0;
                        model.ReimbursementItems.Add(itemModel);
                    }
                }

                //借款明细
                Item loanItemRelation = result.getRelationships("R_LoanItem");
                if (loanItemRelation.getItemCount() > 0)
                {
                    model.LoanItems = new List<LoanItem>();
                    for (int i = 0; i < loanItemRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = loanItemRelation.getItemByIndex(i).getRelatedItem();
                        LoanItem item = new LoanItem();
                        item.id = ItemObJ.getProperty("id");
                        item.b_LoanOrderNo = ItemObJ.getProperty("b_loanorderno");
                        item.b_Date = !string.IsNullOrEmpty(ItemObJ.getProperty("b_date")) ? DateTime.Parse(ItemObJ.getProperty("b_date")).ToString("yyyy-MM-dd") : "";
                        item.b_Borrower = ItemObJ.getProperty("b_borrower");
                        item.b_LoanAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_loanamount")) ? decimal.Parse(ItemObJ.getProperty("b_loanamount")) : 0;
                        item.b_LoanReason = ItemObJ.getProperty("b_loanreason");
                        model.LoanItems.Add(item);
                    }
                }

                //获取文件信息
                Item nFiles = result.getRelationships("R_ExpenseReimbursementFile");
                if (nFiles.getItemCount() > 0)
                {
                    model.Files = new List<FileModel>();
                    for (int i = 0; i < nFiles.getItemCount(); i++)
                    {
                        Item itemObj = nFiles.getItemByIndex(i).getRelatedItem();
                        FileModel itemModel = new FileModel();
                        itemModel.id = itemObj.getProperty("id");
                        itemModel.fileName = itemObj.getProperty("filename");
                        itemModel.source_id = nFiles.getItemByIndex(i).getProperty("source_id");
                        itemModel.relationId = nFiles.getItemByIndex(i).getProperty("id");
                        itemModel.mimeType = itemObj.getProperty("mimetype");
                        itemModel.comments = itemObj.getProperty("comments");
                        model.Files.Add(itemModel);
                    }
                }
                retModel.data = model;

            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除报销单
        /// </summary>
        /// <returns></returns>
        public JsonResult DeleteExpenseReimbursement(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //删除文件
                ExpenseReimbursementBll.DeleteFile(inn, id);
                //删除报销明细
                ExpenseReimbursementBll.DeleteExpenseReimbursementItem(inn, id);
                //删除借款明细
                ExpenseReimbursementBll.DeleteLoanItem(inn, id);
                //删除报销基础信息
                ExpenseReimbursementBll.DeleteExpenseReimbursement(inn, id);
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="para"></param>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        public JsonResult GetExpenseReimbursementHistoryList(DataTableParameter para, string item_Id)
        {
            int total = 0;
            List<HistoryModel> listDatas = GetExpenseReimbursementHistoryList(out total, para, item_Id);
            foreach (var model in listDatas)
            {
                model.Create_onStr = model.Create_onStr.GetValueOrDefault().AddHours(8);
                model.Created_on = model.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                model.Comments = "<div style='width:200px;word-wrap:break-word;'>" + model.Comments + "</div>";
            }
            return Json(new
            {
                sEcho = para.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = listDatas
            }, JsonRequestBehavior.AllowGet);
        }


        private static List<HistoryModel> GetExpenseReimbursementHistoryList(out int total, DataTableParameter para, string item_Id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_EXPENSEREIMBURSEMENT
                                                  join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                  join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                  join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                  join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                  join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                  join p in db.USER on o.CLOSED_BY equals p.ID
                                                  where g.id == item_Id && o.CLOSED_BY != null && o.COMMENTS != "AutoComplete"
                                                  select new HistoryModel
                                                  {
                                                      OperateStr = o.PATH,
                                                      Comments = o.COMMENTS,
                                                      Create_onStr = o.CLOSED_ON,
                                                      CreateName = p.KEYED_NAME,
                                                      ItemStatus = i.KEYED_NAME
                                                  });
                if (para.sEcho == "2")
                {
                    para.sSortType = "desc";
                    para.iSortTitle = "Create_onStr";
                }
                //排序
                if (para.sSortType == "asc")
                {
                    datas = Common.OrderBy(datas, para.iSortTitle, false);
                }
                else
                {
                    datas = Common.OrderBy(datas, para.iSortTitle, true);
                }
                total = datas.Count();
                //分页
                datas = datas.Skip(para.iDisplayStart).Take(para.iDisplayLength);
                return datas.ToList();
            }
        }


        //获取查询Select流程状态数据
        public JsonResult GetWorkflowStatusList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<SelectModel> list = new List<SelectModel>();
                var result = WorkFlowBll.GetWorkflowStatusList(inn, "b_ExpenseReimbursementWorkFlow");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (int i = 0; i < result.getItemCount(); i++)
                    {
                        SelectModel model = new SelectModel();
                        var item = result.getItemByIndex(i);
                        model.value = item.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(model.value))
                        {
                            model.text = Common.GetChineseValueByParam(model.value, "ERManageWorkFlow", "WorkFlow", Userinfo.language);
                        }
                        list.Add(model);
                    }
                }
                retModel.data = list;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加项目总监审核
        /// </summary>
        private static void AddProjectAudit(Innovator inn, ExpenseReimbursementModel model)
        {
            List<string> managerIds = new List<string>();
            List<string> directorIds = new List<string>();
            List<string> vpIds = new List<string>();
            List<string> projectManagers = new List<string> { "Project Manager" };
            List<string> projectDirectors = new List<string> { "Project Director" };
            List<string> projectVps = new List<string> { "Project VP" };

            if (model.ReimbursementItems != null && model.ReimbursementItems.Count > 0)
            {
                foreach (var item in model.ReimbursementItems)
                {
                    //根据项目名称获取项目总监
                    if (!string.IsNullOrEmpty(item.b_ProjectName))
                    {
                        Item projectItem = ProjectManageDA.GetProjectManageByName(inn, item.b_ProjectName);
                        //获取项目经理
                        string projectManagerName = projectItem.getProperty("b_projectmanager");
                        //获取项目总监
                        string projectDirectorName = projectItem.getProperty("b_projectdirector");
                        //获取项目VP
                        string projectVpName = projectItem.getProperty("b_projectvp");

                        //获取项目经理的Identity的主键
                        if (projectManagerName != model.b_Employee)
                        {
                            Item projectManager = IdentityDA.GetIdentityByKeyedName(inn, projectManagerName);
                            if (!projectManager.isError() && projectManager.getItemCount() > 0)
                            {
                                string projectManagerId = projectManager.getProperty("id");
                                managerIds.Add(projectManagerId);
                            }
                        }

                        //获取项目总监的Identity的主键
                        if (projectDirectorName != model.b_Employee)
                        {
                            Item projectDirector = IdentityDA.GetIdentityByKeyedName(inn, projectDirectorName);
                            if (!projectDirector.isError() && projectDirector.getItemCount() > 0)
                            {
                                string projectDirectorId = projectDirector.getProperty("id");
                                directorIds.Add(projectDirectorId);
                            }
                        }

                        if (projectVpName != model.b_Employee)
                        {
                            //获取项目VP
                            Item projectVp = IdentityDA.GetIdentityByKeyedName(inn, projectVpName);
                            if (!projectVp.isError() && projectVp.getItemCount() > 0)
                            {
                                string projectVpId = projectVp.getProperty("id");
                                vpIds.Add(projectVpId);
                            }
                        }
                        else
                        {
                            //获取人员汇报关系的VP
                            USER user = UserDA.GetUserByFirstName(model.b_Employee);
                            Item projectVp = IdentityDA.GetIdentityByKeyedName(inn, user.B_VP);
                            if (!projectVp.isError() && projectVp.getItemCount() > 0)
                            {
                                string projectVpId = projectVp.getProperty("id");
                                vpIds.Add(projectVpId);
                            }
                        }
                    }
                }
            }


            managerIds = managerIds.Distinct().ToList();
            directorIds = directorIds.Distinct().ToList();
            vpIds = vpIds.Distinct().ToList();

            //经理集合跟总监集合对比，保留最大的职位
            if (managerIds.Count > 0 && directorIds.Count > 0)
            {
                for (var index = 0; index < managerIds.Count; index++)
                {
                    string managerId = managerIds[index];
                    foreach (var directorId in directorIds)
                    {
                        if (managerId == directorId)
                        {
                            managerIds.Remove(managerId);
                            index--;
                            break;
                        }
                    }
                }
            }

            //总监集合与VP集合对比
            if (directorIds.Count > 0 && vpIds.Count > 0)
            {
                for (int index = 0; index < directorIds.Count; index++)
                {
                    string directorId = directorIds[index];
                    for (int nindex = 0; nindex < vpIds.Count; nindex++)
                    {
                        string vpId = vpIds[nindex];
                        if (directorId == vpId && model.b_TotalAmount > 3000)
                        {
                            directorIds.Remove(directorId);
                            index--;
                            break;
                        }
                        else if (directorId == vpId && model.b_TotalAmount <= 3000)
                        {
                            vpIds.Remove(vpId);
                            nindex--;
                            break;
                        }
                    }
                }
            }

            //经理集合与   VP集合对比
            if (managerIds.Count > 0 && vpIds.Count > 0)
            {
                for (int index = 0; index < managerIds.Count; index++)
                {
                    string managerId = managerIds[index];
                    for (int nindex = 0; nindex < vpIds.Count; nindex++)
                    {
                        string vpId = vpIds[nindex];
                        if (managerId == vpId && model.b_TotalAmount > 3000)
                        {
                            managerIds.Remove(managerId);
                            index--;
                            break;
                        }
                        else if (managerId == vpId && model.b_TotalAmount <= 3000)
                        {
                            vpIds.Remove(vpId);
                            nindex--;
                            break;
                        }
                    }
                }
            }

            //将项目经理审核人 添加到流程审批节点
            if (managerIds.Count > 0)
            {
                int voting_weight = Common.CalculationWeight(managerIds.Count);
                foreach (var id in managerIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.Id, id, projectManagers, "innovator.b_ExpenseReimbursement", voting_weight);
                }
            }

            //将部门总监审核人  添加到流程审批节点
            if (directorIds.Count > 0)
            {
                int voting_weight = Common.CalculationWeight(directorIds.Count);
                foreach (var id in directorIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.Id, id, projectDirectors, "innovator.b_ExpenseReimbursement", voting_weight);
                }
            }

            //将部门VP审核人   添加到流程审核节点
            if (vpIds.Count > 0)
            {
                int voting_weight = Common.CalculationWeight(vpIds.Count);
                foreach (var id in vpIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.Id, id, projectVps, "innovator.b_ExpenseReimbursement", voting_weight);
                }
            }

        }

        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult ValidUserInfo(string userName)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //查看当前人是否为CEO
                bool isCEO = false;
                USER employeeInfo = UserDA.GetUserByFirstName(userName);
                if (employeeInfo != null)
                {
                    var ItemStructure = OrganizationalStructureDA.GetOrganizationalStructureByLeader(inn, employeeInfo.LOGIN_NAME, 1);
                    if (!ItemStructure.isError() && ItemStructure.getItemCount() > 0)
                    {
                        isCEO = true;
                    }

                    //查看人员信息是否完整
                    if (!isCEO && string.IsNullOrEmpty(employeeInfo.B_CENTRE))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "ERCommon", "ERItemType", ViewBag.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }
    }
}