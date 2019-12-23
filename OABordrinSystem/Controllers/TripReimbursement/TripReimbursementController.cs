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

namespace OABordrinSystem.Controllers.TripReimbursement
{
    public class TripReimbursementController : BaseController
    {
        // GET: TripReimbursement
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("差旅报销申请", "TRCommon", "TRItemType", ViewBag.language);
            ViewBag.UserName = Userinfo.UserName;
            ViewBag.UserEmail = Userinfo.Email;
            ViewBag.department = Userinfo.department;
            ViewBag.b_JobNumber = Userinfo.b_JobNumber;
            return View();
        }

        public JsonResult GetTripReimbursementList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {

            //获取委托权限数据
            List<string> agentRoles = AgentSetBll.GetAgentRoles(Userinfo, "TripReimbursement");

            int total = 0;
            var dataList = GetTripReimbursementList(Userinfo, out total, para, searchValue, startTime, endTime, status, agentRoles);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {

                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "";

                    //获取当前活动的数据
                    var result = ActivityDA.GetActivityAuditByLoginInfo(inn, item.id, "innovator.B_TRIPREIMBURSEMENTFORM", Userinfo.Roles, agentRoles);
                    if (!result.isError() && result.getItemCount() > 0)
                    {
                        //  item.activityId = result.getItemByIndex(0).getProperty("activityid");
                        item.activityAssignmentId = result.getItemByIndex(0).getProperty("activityassignmentid");
                    }

                    if (item.status == "Start")
                    {
                        linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' activityId='" + item.activityId + "' activityAssignmentId='" + item.activityAssignmentId + "' Id='" + item.id + "' ItemStatus='" + item.status.Trim() + "'></a>";
                    }
                    else
                    {
                        if (!result.isError() && result.getItemCount() > 0)
                        {
                            linkAList += "<a class='glyphicon glyphicon-user audit' title='审核' activityId='" + item.activityId + "' activityAssignmentId='" + item.activityAssignmentId + "' Id='" + item.id + "' ItemStatus='" + item.status.Trim() + "'></a>";
                        }
                    }
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-list-alt history' title='日志' id='" + item.id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-asterisk workflow' title='流程' ItemStatus='" + item.status + "' Id='" + item.id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' Id='" + item.id + "' ></a>";
                    if (item.b_Preparer == Userinfo.UserName && item.status != "Start" && item.status != "Financial Director" && item.status != "Expense Accountant Creation")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='fa fa-mail-reply recall' title='撤回' Id='" + item.id + "' activityId='" + item.activityId + "' btrecordno='"+ item.b_BTRecordNo +"' ></a>";
                    }
                    if (item.status == "Start")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' Id='" + item.id + "' ></a>";
                    }
                    strHtml = string.Format(strHtml, item.b_RecordNo, linkAList);
                    item.b_ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    item.b_RecordNo = strHtml;
                    item.AuditorStr = ActivityDA.GetActivityOperator(inn, item.activityId, false);
                    item.AuditorStr = "<div style='width:150px;word-wrap:break-word;'>" + item.AuditorStr + "</div>";
                    item.status = Common.GetChineseValueByParam(item.status, "TRManageWorkFlow", "WorkFlow", Userinfo.language);
                }
            }
            return Json(new
            {
                sEcho = para.sEcho,                                                              //服务器请求次数
                iTotalRecords = total,                                                           //记录总数
                iTotalDisplayRecords = total,                                                    //显示记录的总数
                aaData = dataList                                                                //dataList对象集合
            }, JsonRequestBehavior.AllowGet);
        }

        public static List<TripReimbursementModel> GetTripReimbursementList(UserInfo userInfo, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status, List<string> agentRoles)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<TripReimbursementModel> datas = (from g in db.B_TRIPREIMBURSEMENTFORM
                                                            join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                            join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                            join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                            join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                            join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                            join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                            where (i.STATE == "active" && o.VOTING_WEIGHT > 0 && (userInfo.Roles.Contains(p.ID) || g.B_PREPARER == userInfo.UserName || (agentRoles.Contains(p.ID) && i.KEYED_NAME != "Start"))) && (g.B_RECORDNO.Contains(searchValue) || g.B_DEPT.Contains(searchValue) || g.B_EMPLOYEE.Contains(searchValue)) && o.CLOSED_BY == null
                                                            select new TripReimbursementModel
                                                            {
                                                                id = g.id,
                                                                b_RecordNo = g.B_RECORDNO,
                                                                nb_ApplicationDate = g.CREATED_ON,
                                                                b_Dept = g.B_DEPT,
                                                                b_Employee = g.B_EMPLOYEE,
                                                                b_Preparer = g.B_PREPARER,
                                                                b_AmountInTotal = g.B_AMOUNTINTOTAL,
                                                                status = i.KEYED_NAME,
                                                                activityId = i.ID,
                                                                //activityAssignmentId = o.ID,
                                                                b_BTRecordNo=g.B_BTRECORDNO
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
                //总条数
                total = datas.Count();
                //排序
                if (para != null)
                {
                    if (para.sSortType == "asc")
                    {
                        datas = Common.OrderBy(datas, para.iSortTitle, false);                          //点击你选中的列名称排序
                    }
                    else                                                                                //else (desc 降序)
                    {
                        datas = Common.OrderBy(datas, para.iSortTitle, true);                             //点击你选中的列名称排序
                    }
                    //分页
                    datas = datas.Skip(para.iDisplayStart).Take(para.iDisplayLength);
                }

                //每页显示的页数和页数的条数
                return datas.ToList();
            }
        }

        //差旅报销单
        public JsonResult SaveTripReimbursement(TripReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item lineleader = null;
                Item deptleader = null;
                Item divisionvp = null;

                //申请人
                if (!string.IsNullOrEmpty(model.b_Employee))
                {
                    var employee = IdentityDA.GetIdentityByKeyedName(inn, model.b_Employee);
                    if (employee.isError())
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("找不到填写的申请人！", "TRCommon", "TRItemType", ViewBag.language));
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
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "TRCommon", "TRItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (!isCEO && string.IsNullOrEmpty(model.b_LineLeader) && string.IsNullOrEmpty(model.b_DeptLeader) && string.IsNullOrEmpty(model.b_DivisionVP))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "TRCommon", "TRItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }


                //验证高级经理
                if (!string.IsNullOrEmpty(model.b_LineLeader))
                {
                    lineleader = IdentityDA.GetIdentityByKeyedName(inn, model.b_LineLeader);
                    if (lineleader.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的高级经理！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }
                //部门总监
                if (!string.IsNullOrEmpty(model.b_DeptLeader))
                {
                    deptleader = IdentityDA.GetIdentityByKeyedName(inn, model.b_DeptLeader);
                    if (deptleader.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的部门总监！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }
                //验证中心领导
                if (!string.IsNullOrEmpty(model.b_DivisionVP))
                {
                    divisionvp = IdentityDA.GetIdentityByKeyedName(inn, model.b_DivisionVP);
                    if (divisionvp.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的VP！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                if (model.HotelExpenseItems == null && model.TrafficExpenseItems == null && model.MealsSubsidiesItems == null && model.OthersItems == null)
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("报销明细必须填写!", "TRCommon", "TRItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }


                //判断出差单是否为未报销
                if (!string.IsNullOrEmpty(model.b_BTRecordNo))
                {
                    var businessTravel = BusinessTravelDA.GetBusinessTravelByParam(model.b_BTRecordNo, model.b_Employee);
                    if(businessTravel==null)
                    {
                        retModel.AddError("errorMessage", "该出差单已经在报销中!");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //验证借款人是否存在
                if (model.LoanItems != null && model.LoanItems.Count > 0)
                {
                    for (int i = 0; i < model.LoanItems.Count; i++)
                    {
                        Item Borrower = IdentityDA.GetIdentityByKeyedName(inn, model.LoanItems[i].b_Borrower);
                        if (Borrower.isError())
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("找不到对应的借款人!", "TRCommon", "TRItemType", ViewBag.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                //验证输入的项目名称是否正确
                List<string> projectNameList = new List<string>();
                if (model.HotelExpenseItems != null)
                {
                    projectNameList.AddRange(model.HotelExpenseItems.Where(x => x.b_ProjectName != null && x.b_ProjectName != "").Select(x => x.b_ProjectName));
                }
                if (model.TrafficExpenseItems != null)
                {
                    projectNameList.AddRange(model.TrafficExpenseItems.Where(x => x.b_ProjectName != null && x.b_ProjectName != "").Select(x => x.b_ProjectName));
                }
                if (model.MealsSubsidiesItems != null)
                {
                    projectNameList.AddRange(model.MealsSubsidiesItems.Where(x => x.b_ProjectName != null && x.b_ProjectName != "").Select(x => x.b_ProjectName));
                }
                if (model.OthersItems != null)
                {
                    projectNameList.AddRange(model.OthersItems.Where(x => x.b_ProjectName != null && x.b_ProjectName != "").Select(x => x.b_ProjectName));
                }
                projectNameList = projectNameList.Where(x => x.Trim() != "").Distinct().ToList();

                for (int index = 0; index < projectNameList.Count; index++)
                {
                    string b_ProjectName = projectNameList[index];
                    var projectItem = ProjectManageDA.GetProjectManageByName(inn, b_ProjectName);
                    if (!string.IsNullOrEmpty(projectItem.getErrorString()) && projectItem.getItemCount() <= 0)
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("请输入正确的项目名称!", "TRCommon", "TRItemType", ViewBag.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                List<string> analysisAuditStr = new List<string>();
                List<string> accountAuditStr = new List<string>();
                //查询是否配置了对应的  财务分析员和费用会计
                List<B_EXPENSEAUDITCONFIGURATION> datalist = ExpenseAuditConfigurationBll.GetAllExpenseAuditConfiguration();
                if (model.b_Type == "Project")
                {
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
                    string dataStr = DateTime.Now.ToString("yyyyMM");
                    model.b_RecordNo = "TR" + "-" + dataStr + "-";
                    itemRoot = inn.newItem("b_TripReimbursementForm", "add");
                    itemRoot = OperationData(model, itemRoot);
                }
                else if (model.status == "Start")
                {
                    ////删除住宿费
                    //TripReimbursementBll.DelHotelExpenseItem(inn, model.id);

                    ////删除交通费
                    //TripReimbursementBll.DelTrafficExpenseItem(inn, model.id);

                    ////删除餐费及固定补贴
                    //TripReimbursementBll.DelMealsandfixedsubsidiesItem(inn, model.id);

                    ////删除其他
                    //TripReimbursementBll.DelOthersItem(inn, model.id);

                    ////删除借款明细
                    //TripReimbursementBll.DelLoanItemItem(inn, model.id);

                    //修改编辑数据
                    itemRoot = inn.newItem("b_TripReimbursementForm", "edit");
                    itemRoot.setAttribute("id", model.id);
                    itemRoot = OperationData(model, itemRoot);
                }
                //备注
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    if (!string.IsNullOrEmpty(model.id))
                    {
                        string oldRemark = TripReimbursementBll.GetOldRemarkById(inn, model.id);
                        if (!string.IsNullOrEmpty(oldRemark))
                        {
                            model.b_Remark = oldRemark + "</br>" + Userinfo.UserName + ":" + model.b_Remark;
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

                //var result = itemRoot.apply();
                var result = TripReimbursementBll.EditTripReimbursement(inn, itemRoot, model.operation,model.status,model.id,model.b_BTRecordNo);
                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.status))
                    {
                        model.id = result.getProperty("id");
                        model.b_RecordNo = TripReimbursementBll.CreateRecordNo(inn, model.id);
                    }
                }

                //推动流程状态
                if (model.operation == "submit")
                {
                    if (model.b_Type == "Non Project")
                    {
                        //添加高级经理
                        List<string> lineLeaders = new List<string>() { "Dept.Manager" };
                        ActivityBll.DeleteActivityAuthById(inn, model.id, "innovator.b_TripReimbursementForm", lineLeaders);
                        if (lineleader != null)
                        {
                            string lineLeaderId = lineleader.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.id, lineLeaderId, lineLeaders, "innovator.b_TripReimbursementForm");
                        }


                        //添加部门总监
                        List<string> departmentLeaders = new List<string>() { "Dept.Director" };
                        ActivityBll.DeleteActivityAuthById(inn, model.id, "innovator.b_TripReimbursementForm", departmentLeaders);
                        if (deptleader != null)
                        {
                            string departmentLeaderId = deptleader.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.id, departmentLeaderId, departmentLeaders, "innovator.b_TripReimbursementForm");
                        }

                        //添加中心VP
                        List<string> divisionVps = new List<string>() { "Division VP" };
                        ActivityBll.DeleteActivityAuthById(inn, model.id, "innovator.b_TripReimbursementForm", divisionVps);
                        if (divisionvp != null)
                        {
                            string departmentVPId = divisionvp.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.id, departmentVPId, divisionVps, "innovator.b_TripReimbursementForm");
                        }
                    }
                    else
                    {
                        //当为项目类型时   添加项目审核人员
                        List<string> projectDirectors = new List<string> { "Project Manager", "Project Director", "Project VP" };
                        //删除项目审核人员
                        ActivityBll.DeleteActivityAuthById(inn, model.id, "innovator.b_TripReimbursementForm", projectDirectors);

                        AddProjectAudit(inn, model, projectNameList);
                    }

                    //根据地区添加流程节点 角色审核
                    TripReimbursementBll.AddTripReimbursementAudit(inn, model.id, model.b_ReimbursementPlace, analysisAuditStr, accountAuditStr, employeeInfo.B_CENTRE);

                    //如果activityId为空时，任务ID
                    if (string.IsNullOrEmpty(model.activityId))
                    {
                        Item activityItem = ActivityDA.GetActivityByItemId(inn, model.id, "innovator.b_TripReimbursementForm");
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

                        //判断是否被挂起
                        if (model.b_IsHangUp && !string.IsNullOrEmpty(model.b_HangUpActivityName))
                        {
                            TripReimbursementBll.HangUpAutoAudit(inn, model.id, model.b_HangUpActivityName, model.b_TotalExpense, model.b_IsBudgetary, model.b_IntalBusiness, model.b_DeptLeader, model.b_Type, ref choicePath);
                        }
                        else
                        {
                            //判断下一步骤如果无人审核   直接过掉
                            TripReimbursementBll.AutomaticCompletionTask(inn, model.id, model.b_TotalExpense, model.b_IsBudgetary, model.b_Type, model.b_IntalBusiness, model.b_LineLeader, model.b_DeptLeader, ref choicePath);
                        }

                        if (choicePath != null)
                        {
                            EmailEntity emailEntity = new EmailEntity();
                            emailEntity.ItemId = model.id;
                            emailEntity.ApplicantDepartment = model.b_Dept;
                            emailEntity.ApplicantName = model.b_Employee;
                            emailEntity.RecordNo = model.b_RecordNo;
                            TripReimbursementBll.SendEmailByOperation(inn, emailEntity, choicePath);
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
        public Item OperationData(TripReimbursementModel model, Item itemRoot)
        {
            itemRoot.setProperty("b_recordno", model.b_RecordNo);
            itemRoot.setProperty("b_companycode", model.b_CompanyCode);
            itemRoot.setProperty("b_reimbursementplace", model.b_ReimbursementPlace);
            itemRoot.setProperty("b_isbudgetary", model.b_IsBudgetary ? "1" : "0");
            itemRoot.setProperty("b_budgetnumber", model.b_BudgetNumber);
            itemRoot.setProperty("b_type", model.b_Type);
            itemRoot.setProperty("b_applicationdate", model.b_ApplicationDate.ToString());
            itemRoot.setProperty("b_intalbusiness", model.b_IntalBusiness ? "1" : "0");
            itemRoot.setProperty("b_preparer", model.b_Preparer);
            itemRoot.setProperty("b_preparerno", model.b_PreparerNo);
            itemRoot.setProperty("b_employee", model.b_Employee);
            itemRoot.setProperty("b_staffno", model.b_StaffNo);
            itemRoot.setProperty("b_dept", model.b_Dept);
            itemRoot.setProperty("b_costcenter", model.b_CostCenter);
            itemRoot.setProperty("b_tel", model.b_Tel);
            itemRoot.setProperty("b_lineleader", model.b_LineLeader);
            itemRoot.setProperty("b_deptleader", model.b_DeptLeader);
            itemRoot.setProperty("b_divisionvp", model.b_DivisionVP);
            itemRoot.setProperty("b_advancedamount", model.b_AdvancedAmount.ToString());
            itemRoot.setProperty("b_totalexpense", model.b_TotalExpense.ToString());
            itemRoot.setProperty("b_amountintotal", model.b_AmountInTotal);
            itemRoot.setProperty("b_attachmentsquantity", model.b_AttachmentsQuantity.ToString());
            itemRoot.setProperty("b_amountinwords", model.b_AmountInWords);
            itemRoot.setProperty("b_totalamount", model.b_TotalAmount.ToString());
            itemRoot.setProperty("b_hotelinwords", model.b_HotelInWords);
            itemRoot.setProperty("b_hotelamount", model.b_HotelAmount.ToString());
            itemRoot.setProperty("b_trafinwords", model.b_TrafInWords);
            itemRoot.setProperty("b_trafamount", model.b_TrafAmount.ToString());
            itemRoot.setProperty("b_mealinwords", model.b_MealInWords);
            itemRoot.setProperty("b_mealamount", model.b_MealAmount.ToString());
            itemRoot.setProperty("b_othinwords", model.b_OthInWords);
            itemRoot.setProperty("b_othamount", model.b_OthAmount.ToString());
            itemRoot.setProperty("b_btrecordno", model.b_BTRecordNo);
            itemRoot.setProperty("b_travelbudget", model.b_TravelBudget.ToString());
            model.b_IsBeyondBudget = model.b_IsBeyondBudget == "是" ? "1" : "0";
            itemRoot.setProperty("b_isbeyondbudget", model.b_IsBeyondBudget);
            itemRoot.setProperty("b_beyondreason", model.b_BeyondReason);
            if (model.operation == "submit")
            {
                itemRoot.setProperty("b_ishangup", "0");
                itemRoot.setProperty("b_hangupactivityname", "");
            }

            //住宿费
            if (model.HotelExpenseItems != null)
            {
                for (int i = 0; i < model.HotelExpenseItems.Count; i++)
                {
                    var item = model.HotelExpenseItems[i];
                    var R_HotelExpenseItem = inn.newItem("R_HotelExpense", "add");
                    var HotelExpenseItem = inn.newItem("b_HotelExpense", "add");
                    HotelExpenseItem.setProperty("b_startdate", item.b_StartDate.ToString());
                    HotelExpenseItem.setProperty("b_enddate", item.b_EndDate.ToString());
                    HotelExpenseItem.setProperty("b_projectname", item.b_ProjectName);
                    HotelExpenseItem.setProperty("b_city", item.b_City);
                    HotelExpenseItem.setProperty("b_hotel", item.b_Hotel);
                    HotelExpenseItem.setProperty("b_currency", item.b_Currency);
                    HotelExpenseItem.setProperty("b_rate", item.b_Rate.ToString());
                    HotelExpenseItem.setProperty("b_originalcurrency", item.b_OriginalCurrency.ToString());
                    HotelExpenseItem.setProperty("b_days", item.b_Count.ToString());
                    HotelExpenseItem.setProperty("b_taxrate", item.b_TaxRate.ToString());
                    HotelExpenseItem.setProperty("b_tax", item.b_Tax.ToString());
                    HotelExpenseItem.setProperty("b_taxfreeamount", item.b_TaxFreeAmount.ToString());
                    HotelExpenseItem.setProperty("b_cnysubtotal", item.b_CNYSubtotal.ToString());
                    R_HotelExpenseItem.setRelatedItem(HotelExpenseItem);
                    itemRoot.addRelationship(R_HotelExpenseItem);
                }
            }

            //交通费
            if (model.TrafficExpenseItems != null)
            {
                for (int i = 0; i < model.TrafficExpenseItems.Count; i++)
                {
                    var item = model.TrafficExpenseItems[i];
                    var R_TrafficExpenseItem = inn.newItem("R_TrafficExpense", "add");
                    var TrafficExpenseItem = inn.newItem("b_TrafficExpense", "add");
                    TrafficExpenseItem.setProperty("b_startdate", item.b_StartDate.ToString());
                    TrafficExpenseItem.setProperty("b_enddate", item.b_EndDate.ToString());
                    TrafficExpenseItem.setProperty("b_projectname", item.b_ProjectName);
                    TrafficExpenseItem.setProperty("b_type", item.b_Type);
                    TrafficExpenseItem.setProperty("b_startpoint", item.b_StartPoint);
                    TrafficExpenseItem.setProperty("b_endpoint", item.b_EndPoint);
                    TrafficExpenseItem.setProperty("b_currency", item.b_Currency);
                    TrafficExpenseItem.setProperty("b_rate", item.b_Rate.ToString());
                    TrafficExpenseItem.setProperty("b_originalcurrency", item.b_OriginalCurrency.ToString());
                    TrafficExpenseItem.setProperty("b_kilometre", item.b_Count.ToString());
                    TrafficExpenseItem.setProperty("b_taxrate", item.b_TaxRate.ToString());
                    TrafficExpenseItem.setProperty("b_tax", item.b_Tax.ToString());
                    TrafficExpenseItem.setProperty("b_taxfreeamount", item.b_TaxFreeAmount.ToString());
                    TrafficExpenseItem.setProperty("b_cnysubtotal", item.b_CNYSubtotal.ToString());
                    R_TrafficExpenseItem.setRelatedItem(TrafficExpenseItem);
                    itemRoot.addRelationship(R_TrafficExpenseItem);
                }
            }

            //餐费及固定补贴
            if (model.MealsSubsidiesItems != null)
            {
                for (int i = 0; i < model.MealsSubsidiesItems.Count; i++)
                {
                    var item = model.MealsSubsidiesItems[i];
                    var R_MealsandfixedsubsidiesItem = inn.newItem("R_Mealsandfixedsubsidies", "add");
                    var MealsandfixedsubsidiesItem = inn.newItem("b_Mealsandfixedsubsidies", "add");
                    MealsandfixedsubsidiesItem.setProperty("b_startdate", item.b_StartDate.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_enddate", item.b_EndDate.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_projectname", item.b_ProjectName);
                    MealsandfixedsubsidiesItem.setProperty("b_place", item.b_Place);
                    MealsandfixedsubsidiesItem.setProperty("b_companionamount", item.b_CompanionAmount);
                    MealsandfixedsubsidiesItem.setProperty("b_companionname", item.b_CompanionName);
                    MealsandfixedsubsidiesItem.setProperty("b_currency", item.b_Currency);
                    MealsandfixedsubsidiesItem.setProperty("b_rate", item.b_Rate.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_fixedsubsidy", item.b_FixedSubsidy.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_taxrate", item.b_TaxRate.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_tax", item.b_Tax.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_taxfreeamount", item.b_TaxFreeAmount.ToString());
                    MealsandfixedsubsidiesItem.setProperty("b_cnysubtotal", item.b_CNYSubtotal.ToString());
                    R_MealsandfixedsubsidiesItem.setRelatedItem(MealsandfixedsubsidiesItem);
                    itemRoot.addRelationship(R_MealsandfixedsubsidiesItem);
                }
            }

            //其他
            if (model.OthersItems != null)
            {
                for (int i = 0; i < model.OthersItems.Count; i++)
                {
                    var item = model.OthersItems[i];
                    var R_OthersItem = inn.newItem("R_Others", "add");
                    var OthersItem = inn.newItem("b_Others", "add");
                    OthersItem.setProperty("b_startdate", item.b_StartDate.ToString());
                    OthersItem.setProperty("b_enddate", item.b_EndDate.ToString());
                    OthersItem.setProperty("b_projectname", item.b_ProjectName);
                    OthersItem.setProperty("b_place", item.b_Place);
                    OthersItem.setProperty("b_type", item.b_Type);
                    OthersItem.setProperty("b_reason", item.b_Reason);
                    OthersItem.setProperty("b_currency", item.b_Currency);
                    OthersItem.setProperty("b_rate", item.b_Rate.ToString());
                    OthersItem.setProperty("b_originalcurrency", item.b_OriginalCurrency.ToString());
                    OthersItem.setProperty("b_count", item.b_Count.ToString());
                    OthersItem.setProperty("b_taxrate", item.b_TaxRate.ToString());
                    OthersItem.setProperty("b_tax", item.b_Tax.ToString());
                    OthersItem.setProperty("b_taxfreeamount", item.b_TaxFreeAmount.ToString());
                    OthersItem.setProperty("b_cnysubtotal", item.b_CNYSubtotal.ToString());
                    R_OthersItem.setRelatedItem(OthersItem);
                    itemRoot.addRelationship(R_OthersItem);
                }
            }

            //借款明细
            if (model.LoanItems != null)
            {
                for (int i = 0; i < model.LoanItems.Count; i++)
                {
                    var item = model.LoanItems[i];
                    var R_LoanItem = inn.newItem("R_LoanItems", "add");
                    var LoanItem = inn.newItem("b_LoanItem", "add");
                    LoanItem.setProperty("b_loanorderno", item.b_LoanOrderNo);
                    LoanItem.setProperty("b_date", item.b_Date.ToString());
                    LoanItem.setProperty("b_borrower", item.b_Borrower);
                    LoanItem.setProperty("b_loanamount", item.b_LoanAmount.ToString());
                    LoanItem.setProperty("b_loanreason", item.b_LoanReason);
                    R_LoanItem.setRelatedItem(LoanItem);
                    itemRoot.addRelationship(R_LoanItem);
                }
            }

            //文件保存
            if (string.IsNullOrEmpty(model.status))
            {
                if (model.fileIds != null)
                {
                    for (int index = 0; index < model.fileIds.Count; index++)
                    {
                        var fileId = model.fileIds[index];
                        var trFile = inn.newItem("R_TripReimbursementFile", "add");
                        var fileItem = inn.newItem("File", "get");
                        fileItem.setAttribute("id", fileId);
                        trFile.setRelatedItem(fileItem);
                        itemRoot.addRelationship(trFile);
                    }
                }
            }

            return itemRoot;
        }


        /// <summary>
        /// 审核出差报销
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult AuditTripReimbursement(TripReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                EmailEntity emailEntity = new EmailEntity();
                emailEntity.ItemId = model.id;
                emailEntity.ApplicantDepartment = model.b_Dept;
                emailEntity.ApplicantName = model.b_Employee;
                emailEntity.RecordNo = model.b_RecordNo;
                emailEntity.BTRecordNo = model.b_BTRecordNo;


                WORKFLOW_PROCESS_PATH choicePath = AuditChoiceLine(model, inn);

                string presentRemark = model.b_Remark;

                //修改编辑数据
                var itemRoot = inn.newItem("b_TripReimbursementForm", "edit");
                itemRoot.setAttribute("id", model.id);
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    string oldRemark = TripReimbursementBll.GetOldRemarkById(inn, model.id);
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

                    //删除住宿费
                    TripReimbursementBll.DelHotelExpenseItem(inn, model.id);

                    //删除交通费
                    TripReimbursementBll.DelTrafficExpenseItem(inn, model.id);

                    //删除餐费及固定补贴
                    TripReimbursementBll.DelMealsandfixedsubsidiesItem(inn, model.id);

                    //删除其他
                    TripReimbursementBll.DelOthersItem(inn, model.id);

                    //删除借款明细
                    TripReimbursementBll.DelLoanItemItem(inn, model.id);

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
                    }

                    //当活动无人审核时  自动完成
                    TripReimbursementBll.AutomaticCompletionTask(inn, model.id, model.b_TotalExpense, model.b_IsBudgetary, model.b_Type, model.b_IntalBusiness, model.b_LineLeader, model.b_DeptLeader, ref choicePath);

                    //判断活动是否关闭
                    if (ActivityBll.ActivityIsClosed(inn, choicePath.SOURCE_ID))
                    {

                        var activityItem = ActivityDA.GetActivityById(inn, choicePath.RELATED_ID);
                        if (!activityItem.isError() && activityItem.getItemCount() > 0)
                        {
                            string name = activityItem.getProperty("name");
                            //获取CEO名称
                            if (name == "GM" && !string.IsNullOrEmpty(name))
                            {
                                //判断CEO之前是否审核过
                                if (model.b_Type == "Non Project" && UserBll.CeoBeforeIsAudit(inn, model.b_LineLeader, model.b_DeptLeader, model.b_DivisionVP, model.b_Employee))
                                {
                                    choicePath = WorkFlowBll.AutoCompleteActivityByParam(model.id, "innovator.b_TripReimbursementForm", "agree");
                                }
                            }
                        }
                    }

                    //判断活动是否结束，如果结束发送邮件到下一环节
                    if (ActivityBll.ActivityIsClosed(inn, choicePath.SOURCE_ID))
                    {
                        TripReimbursementBll.SendEmailByOperation(inn, emailEntity, choicePath);
                        if (model.status == "Financial Analyst")
                        {
                            TripReimbursementBll.SendEmailToProposer(inn, model.b_Employee, model.b_RecordNo);
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
        /// 审核选择路线
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static WORKFLOW_PROCESS_PATH AuditChoiceLine(TripReimbursementModel model, Innovator inn)
        {
            //获取路线
            var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
            string lineName = TripReimbursementBll.GetLineNameByActivityName(inn, model.activityId, model.status, model.b_TotalExpense, model.b_IsBudgetary, model.b_DeptLeader);
            WORKFLOW_PROCESS_PATH choicePath = listActivity.Where(x => x.NAME == lineName).FirstOrDefault();
            return choicePath;
        }

        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public JsonResult RecallTripReimbursement(string id, string activityId,string btrecordno)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(activityId);
                WORKFLOW_PROCESS_PATH choicePath = null;
                choicePath = listActivity.Where(x => x.NAME == "ReturnStart").FirstOrDefault();

                //获取Admin 登录连接
                var adminInn = WorkFlowBll.GetAdminInnovator();
                if (adminInn != null && choicePath != null)
                {
                    //获取Admin 对当前任务权限数据
                    Item activityItem = ActivityDA.GetActivityByItemId(adminInn, id, "Administrators", "b_TripReimbursementForm");
                    if (!activityItem.isError())
                    {
                        string nactivityId = activityItem.getProperty("activityid");
                        string activityAssignmentId = activityItem.getProperty("activityassignmentid");
                        ActivityDA.CompleteActivity(adminInn, nactivityId, activityAssignmentId, choicePath.ID, choicePath.NAME, "", Userinfo.UserName + "对单据进行撤回操作");
                    }
                    BusinessTravelBll.UpdateBusinessTravelIsReimbursement(inn, "0", btrecordno);
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="model"></param>
        public JsonResult RefuseTripReimbursement(TripReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    //修改编辑数据
                    var itemRoot = inn.newItem("b_TripReimbursementForm", "edit");
                    itemRoot.setAttribute("id", model.id);
                    string oldRemark = TripReimbursementBll.GetOldRemarkById(inn, model.id);
                    if (!string.IsNullOrEmpty(oldRemark))
                    {
                        model.b_Remark = oldRemark + "</br>" + Userinfo.UserName + ":" + model.b_Remark;
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
        public JsonResult HangUpTripReimbursement(TripReimbursementModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var itemRoot = inn.newItem("b_TripReimbursementForm", "edit");
                itemRoot.setAttribute("id", model.id);
                itemRoot.setProperty("b_ishangup", "1");
                itemRoot.setProperty("b_hangupactivityname", model.status);
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    string oldRemark = TripReimbursementBll.GetOldRemarkById(inn, model.id);
                    if (!string.IsNullOrEmpty(oldRemark))
                    {
                        model.b_Remark = oldRemark + "</br>" + Userinfo.UserName + ":" + model.b_Remark;
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
        /// 将流程退回到开始状态
        /// </summary>
        /// <param name="model"></param>
        private void ReturnToStart(TripReimbursementModel model, string presentRemark)
        {
            EmailEntity emailEntity = new EmailEntity();
            emailEntity.ItemId = model.id;
            emailEntity.ApplicantDepartment = model.b_Dept;
            emailEntity.ApplicantName = model.b_Employee;
            emailEntity.RecordNo = model.b_RecordNo;

            //获取路线
            var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
            WORKFLOW_PROCESS_PATH choicePath = null;
            if (!string.IsNullOrEmpty(model.status))
            {
                choicePath = listActivity.Where(x => x.NAME == "ReturnStart").FirstOrDefault();

                string errorStr = "";
                if (model.status == "Project Director" || model.status == "Project Manager" || model.status == "Project VP")
                {
                    //获取Admin的Aras 连接
                    var adminInn = WorkFlowBll.GetAdminInnovator();
                    if (adminInn != null)
                    {
                        //获取Admin 对当前任务权限数据
                        Item activityItem = ActivityDA.GetActivityByItemId(adminInn, model.id, "Administrators", "b_TripReimbursementForm");
                        if (!activityItem.isError())
                        {
                            string activityId = activityItem.getProperty("activityid");
                            string activityAssignmentId = activityItem.getProperty("activityassignmentid");
                            errorStr = ActivityDA.CompleteActivity(adminInn, activityId, activityAssignmentId, choicePath.ID, choicePath.NAME, "", Userinfo.UserName + "对单据进行了拒绝操作！" + presentRemark);
                        }
                    }
                }
                else
                {
                    errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark, Userinfo);
                }
                if (string.IsNullOrEmpty(errorStr))
                {
                    BusinessTravelBll.UpdateBusinessTravelIsReimbursement(inn, "0", model.b_BTRecordNo);
                    TripReimbursementBll.SendEmailByOperation(inn, emailEntity, choicePath);
                }
            }
        }


        /// <summary>
        /// 根据ID获取差旅报销申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetTripReimbursementById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result = TripReimbursementBll.GetTripReimbursementObjById(inn, id);

                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                TripReimbursementModel model = new TripReimbursementModel();
                model.id = result.getProperty("id");
                model.b_RecordNo = result.getProperty("b_recordno");
                model.b_CompanyCode = result.getProperty("b_companycode");
                model.b_ReimbursementPlace = result.getProperty("b_reimbursementplace");
                model.b_IsBudgetary = result.getProperty("b_isbudgetary") == "1" ? true : false;
                model.b_BudgetNumber = result.getProperty("b_budgetnumber");
                model.b_Type = result.getProperty("b_type");
                model.b_ApplicationDate = DateTime.Parse(result.getProperty("b_applicationdate")).ToString("yyyy-MM-dd");
                model.b_IntalBusiness = result.getProperty("b_intalbusiness") == "1" ? true : false;
                model.b_Preparer = result.getProperty("b_preparer");
                model.b_PreparerNo = result.getProperty("b_preparerno");
                model.b_Employee = result.getProperty("b_employee");
                model.b_StaffNo = result.getProperty("b_staffno");
                // model.b_Position = result.getProperty("b_position");
                model.b_Dept = result.getProperty("b_dept");
                model.b_CostCenter = result.getProperty("b_costcenter");
                model.b_Tel = result.getProperty("b_tel");
                model.b_AmountInWords = result.getProperty("b_amountinwords");
                model.b_TotalAmount = !string.IsNullOrEmpty(result.getProperty("b_totalamount")) ? decimal.Parse(result.getProperty("b_totalamount")) : 0;

                model.b_HotelInWords = result.getProperty("b_hotelinwords");
                model.b_HotelAmount = !string.IsNullOrEmpty(result.getProperty("b_hotelamount")) ? decimal.Parse(result.getProperty("b_hotelamount")) : 0;
                model.b_TrafInWords = result.getProperty("b_trafinwords");
                model.b_TrafAmount = !string.IsNullOrEmpty(result.getProperty("b_trafamount")) ? decimal.Parse(result.getProperty("b_trafamount")) : 0;
                model.b_MealInWords = result.getProperty("b_mealinwords");
                model.b_MealAmount = !string.IsNullOrEmpty(result.getProperty("b_mealamount")) ? decimal.Parse(result.getProperty("b_mealamount")) : 0;
                model.b_OthInWords = result.getProperty("b_othinwords");
                model.b_OthAmount = !string.IsNullOrEmpty(result.getProperty("b_othamount")) ? decimal.Parse(result.getProperty("b_othamount")) : 0;
                model.OldRemark = result.getProperty("b_remark");
                model.b_LineLeader = result.getProperty("b_lineleader");
                model.b_DeptLeader = result.getProperty("b_deptleader");
                model.b_DivisionVP = result.getProperty("b_divisionvp");
                model.b_AdvancedAmount = !string.IsNullOrEmpty(result.getProperty("b_advancedamount")) ? decimal.Parse(result.getProperty("b_advancedamount")) : 0;
                model.b_TotalExpense = !string.IsNullOrEmpty(result.getProperty("b_totalexpense")) ? decimal.Parse(result.getProperty("b_totalexpense")) : 0;
                model.b_AmountInTotal = result.getProperty("b_amountintotal");
                model.b_AttachmentsQuantity = !string.IsNullOrEmpty(result.getProperty("b_attachmentsquantity")) ? int.Parse(result.getProperty("b_attachmentsquantity")) : 0;
                model.b_HangUpActivityName = result.getProperty("b_hangupactivityname");
                model.b_IsHangUp = result.getProperty("b_ishangup") == "1" ? true : false;
                model.b_BTRecordNo = result.getProperty("b_btrecordno");
                model.b_TravelBudget = !string.IsNullOrEmpty(result.getProperty("b_travelbudget")) ? decimal.Parse(result.getProperty("b_travelbudget")) : 0;
                model.b_IsBeyondBudget = result.getProperty("b_isbeyondbudget");
                model.b_IsBeyondBudget = model.b_IsBeyondBudget == "1" ? "是" : "否";
                model.b_BeyondReason = result.getProperty("b_beyondreason");

                //if (model.OldRemark.Contains("\r\n"))
                //{
                //    model.OldRemark = model.OldRemark.Replace("\r\n", "<br/>");
                //}


                //住宿费
                Item HeRelation = result.getRelationships("R_HotelExpense");
                if (HeRelation.getItemCount() > 0)
                {
                    model.HotelExpenseItems = new List<HotelExpense>();

                    for (int i = 0; i < HeRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = HeRelation.getItemByIndex(i).getRelatedItem();
                        HotelExpense HeModel = new HotelExpense();
                        HeModel.id = ItemObJ.getProperty("id");
                        HeModel.b_StartDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_startdate")) ? DateTime.Parse(ItemObJ.getProperty("b_startdate")).ToString("yyyy-MM-dd") : "";
                        HeModel.b_EndDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_enddate")) ? DateTime.Parse(ItemObJ.getProperty("b_enddate")).ToString("yyyy-MM-dd") : "";
                        HeModel.b_ProjectName = ItemObJ.getProperty("b_projectname");
                        HeModel.b_City = ItemObJ.getProperty("b_city");
                        HeModel.b_Hotel = ItemObJ.getProperty("b_hotel");
                        HeModel.b_Currency = ItemObJ.getProperty("b_currency");
                        HeModel.b_Rate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_rate")) ? float.Parse(ItemObJ.getProperty("b_rate")) : 0;
                        HeModel.b_OriginalCurrency = !string.IsNullOrEmpty(ItemObJ.getProperty("b_originalcurrency")) ? decimal.Parse(ItemObJ.getProperty("b_originalcurrency")) : 0;
                        HeModel.b_Count = !string.IsNullOrEmpty(ItemObJ.getProperty("b_days")) ? int.Parse(ItemObJ.getProperty("b_days")) : 0;
                        HeModel.b_TaxRate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxrate")) ? float.Parse(ItemObJ.getProperty("b_taxrate")) : 0;
                        HeModel.b_Tax = !string.IsNullOrEmpty(ItemObJ.getProperty("b_tax")) ? decimal.Parse(ItemObJ.getProperty("b_tax")) : 0;
                        HeModel.b_TaxFreeAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxfreeamount")) ? decimal.Parse(ItemObJ.getProperty("b_taxfreeamount")) : 0;
                        HeModel.b_CNYSubtotal = !string.IsNullOrEmpty(ItemObJ.getProperty("b_cnysubtotal")) ? decimal.Parse(ItemObJ.getProperty("b_cnysubtotal")) : 0;
                        model.HotelExpenseItems.Add(HeModel);
                    }
                }

                //交通费
                Item TeRelation = result.getRelationships("R_TrafficExpense");
                if (TeRelation.getItemCount() > 0)
                {
                    model.TrafficExpenseItems = new List<TrafficExpense>();

                    for (int i = 0; i < TeRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = TeRelation.getItemByIndex(i).getRelatedItem();
                        TrafficExpense TeModel = new TrafficExpense();
                        TeModel.id = ItemObJ.getProperty("id");
                        TeModel.b_StartDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_startdate")) ? DateTime.Parse(ItemObJ.getProperty("b_startdate")).ToString("yyyy-MM-dd") : "";
                        TeModel.b_EndDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_enddate")) ? DateTime.Parse(ItemObJ.getProperty("b_enddate")).ToString("yyyy-MM-dd") : "";
                        TeModel.b_ProjectName = ItemObJ.getProperty("b_projectname");
                        TeModel.b_Type = ItemObJ.getProperty("b_type");
                        TeModel.b_StartPoint = ItemObJ.getProperty("b_startpoint");
                        TeModel.b_EndPoint = ItemObJ.getProperty("b_endpoint");
                        TeModel.b_Currency = ItemObJ.getProperty("b_currency");
                        TeModel.b_Rate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_rate")) ? float.Parse(ItemObJ.getProperty("b_rate")) : 0;
                        TeModel.b_OriginalCurrency = !string.IsNullOrEmpty(ItemObJ.getProperty("b_originalcurrency")) ? decimal.Parse(ItemObJ.getProperty("b_originalcurrency")) : 0;
                        TeModel.b_Count = !string.IsNullOrEmpty(ItemObJ.getProperty("b_kilometre")) ? int.Parse(ItemObJ.getProperty("b_kilometre")) : 0;
                        TeModel.b_TaxRate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxrate")) ? float.Parse(ItemObJ.getProperty("b_taxrate")) : 0;
                        TeModel.b_Tax = !string.IsNullOrEmpty(ItemObJ.getProperty("b_tax")) ? decimal.Parse(ItemObJ.getProperty("b_tax")) : 0;
                        TeModel.b_TaxFreeAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxfreeamount")) ? decimal.Parse(ItemObJ.getProperty("b_taxfreeamount")) : 0;
                        TeModel.b_CNYSubtotal = !string.IsNullOrEmpty(ItemObJ.getProperty("b_cnysubtotal")) ? decimal.Parse(ItemObJ.getProperty("b_cnysubtotal")) : 0;
                        model.TrafficExpenseItems.Add(TeModel);
                    }
                }

                //餐费及固定补贴
                Item MbRelation = result.getRelationships("R_Mealsandfixedsubsidies");
                if (MbRelation.getItemCount() > 0)
                {
                    model.MealsSubsidiesItems = new List<Mealsandfixedsubsidies>();

                    for (int i = 0; i < MbRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = MbRelation.getItemByIndex(i).getRelatedItem();
                        Mealsandfixedsubsidies MbModel = new Mealsandfixedsubsidies();
                        MbModel.id = ItemObJ.getProperty("id");
                        MbModel.b_StartDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_startdate")) ? DateTime.Parse(ItemObJ.getProperty("b_startdate")).ToString("yyyy-MM-dd") : "";
                        MbModel.b_EndDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_enddate")) ? DateTime.Parse(ItemObJ.getProperty("b_enddate")).ToString("yyyy-MM-dd") : "";
                        MbModel.b_ProjectName = ItemObJ.getProperty("b_projectname");
                        MbModel.b_Place = ItemObJ.getProperty("b_place");
                        MbModel.b_CompanionAmount = ItemObJ.getProperty("b_companionamount");
                        MbModel.b_CompanionName = ItemObJ.getProperty("b_companionname");
                        MbModel.b_Currency = ItemObJ.getProperty("b_currency");
                        MbModel.b_Rate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_rate")) ? float.Parse(ItemObJ.getProperty("b_rate")) : 0;
                        MbModel.b_FixedSubsidy = !string.IsNullOrEmpty(ItemObJ.getProperty("b_fixedsubsidy")) ? decimal.Parse(ItemObJ.getProperty("b_fixedsubsidy")) : 0;
                        MbModel.b_TaxRate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxrate")) ? float.Parse(ItemObJ.getProperty("b_taxrate")) : 0;
                        MbModel.b_Tax = !string.IsNullOrEmpty(ItemObJ.getProperty("b_tax")) ? decimal.Parse(ItemObJ.getProperty("b_tax")) : 0;
                        MbModel.b_TaxFreeAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxfreeamount")) ? decimal.Parse(ItemObJ.getProperty("b_taxfreeamount")) : 0;
                        MbModel.b_CNYSubtotal = !string.IsNullOrEmpty(ItemObJ.getProperty("b_cnysubtotal")) ? decimal.Parse(ItemObJ.getProperty("b_cnysubtotal")) : 0;
                        model.MealsSubsidiesItems.Add(MbModel);
                    }
                }

                //其他
                Item OsRelation = result.getRelationships("R_Others");
                if (OsRelation.getItemCount() > 0)
                {
                    model.OthersItems = new List<Others>();

                    for (int i = 0; i < OsRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = OsRelation.getItemByIndex(i).getRelatedItem();
                        Others OsModel = new Others();
                        OsModel.id = ItemObJ.getProperty("id");
                        OsModel.b_StartDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_startdate")) ? DateTime.Parse(ItemObJ.getProperty("b_startdate")).ToString("yyyy-MM-dd") : "";
                        OsModel.b_EndDate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_enddate")) ? DateTime.Parse(ItemObJ.getProperty("b_enddate")).ToString("yyyy-MM-dd") : "";
                        OsModel.b_ProjectName = ItemObJ.getProperty("b_projectname");
                        OsModel.b_Place = ItemObJ.getProperty("b_place");
                        OsModel.b_Type = ItemObJ.getProperty("b_type");
                        OsModel.b_Reason = ItemObJ.getProperty("b_reason");
                        OsModel.b_Currency = ItemObJ.getProperty("b_currency");
                        OsModel.b_Rate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_rate")) ? float.Parse(ItemObJ.getProperty("b_rate")) : 0;
                        OsModel.b_OriginalCurrency = !string.IsNullOrEmpty(ItemObJ.getProperty("b_originalcurrency")) ? decimal.Parse(ItemObJ.getProperty("b_originalcurrency")) : 0;
                        OsModel.b_Count = !string.IsNullOrEmpty(ItemObJ.getProperty("b_count")) ? int.Parse(ItemObJ.getProperty("b_count")) : 0;
                        OsModel.b_TaxRate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxrate")) ? float.Parse(ItemObJ.getProperty("b_taxrate")) : 0;
                        OsModel.b_Tax = !string.IsNullOrEmpty(ItemObJ.getProperty("b_tax")) ? decimal.Parse(ItemObJ.getProperty("b_tax")) : 0;
                        OsModel.b_TaxFreeAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxfreeamount")) ? decimal.Parse(ItemObJ.getProperty("b_taxfreeamount")) : 0;
                        OsModel.b_CNYSubtotal = !string.IsNullOrEmpty(ItemObJ.getProperty("b_cnysubtotal")) ? decimal.Parse(ItemObJ.getProperty("b_cnysubtotal")) : 0;
                        model.OthersItems.Add(OsModel);
                    }
                }

                //借款明细
                Item LnRelation = result.getRelationships("R_LoanItems");
                if (LnRelation.getItemCount() > 0)
                {
                    model.LoanItems = new List<LoanItems>();

                    for (int i = 0; i < LnRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = LnRelation.getItemByIndex(i).getRelatedItem();
                        LoanItems LnModel = new LoanItems();
                        LnModel.id = ItemObJ.getProperty("id");
                        LnModel.b_LoanOrderNo = ItemObJ.getProperty("b_loanorderno");
                        LnModel.b_Date = !string.IsNullOrEmpty(ItemObJ.getProperty("b_date")) ? DateTime.Parse(ItemObJ.getProperty("b_date")).ToString("yyyy-MM-dd") : "";
                        LnModel.b_Borrower = ItemObJ.getProperty("b_borrower");
                        LnModel.b_LoanAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_loanamount")) ? decimal.Parse(ItemObJ.getProperty("b_loanamount")) : 0;
                        LnModel.b_LoanReason = ItemObJ.getProperty("b_loanreason");
                        model.LoanItems.Add(LnModel);
                    }
                }

                //获取文件信息
                Item nFiles = result.getRelationships("R_TripReimbursementFile");
                if (nFiles.getItemCount() > 0)
                {
                    model.Files = new List<FileModel>();
                    for (int i = 0; i < nFiles.getItemCount(); i++)
                    {
                        Item itemObj = nFiles.getItemByIndex(i).getRelatedItem();
                        FileModel itemFile = new FileModel();
                        itemFile.id = itemObj.getProperty("id");
                        itemFile.fileName = itemObj.getProperty("filename");
                        itemFile.source_id = nFiles.getItemByIndex(i).getProperty("source_id");
                        itemFile.relationId = nFiles.getItemByIndex(i).getProperty("id");
                        itemFile.mimeType = itemObj.getProperty("mimetype");
                        itemFile.comments = itemObj.getProperty("comments");
                        model.Files.Add(itemFile);
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
        /// 删除差旅报销单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelTripReimbursement(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //删除住宿费
                TripReimbursementBll.DelHotelExpenseItem(inn, id);

                //删除交通费
                TripReimbursementBll.DelTrafficExpenseItem(inn, id);

                //删除餐费及固定补贴
                TripReimbursementBll.DelMealsandfixedsubsidiesItem(inn, id);

                //删除其他
                TripReimbursementBll.DelOthersItem(inn, id);

                //删除借款明细
                TripReimbursementBll.DelLoanItemItem(inn, id);

                //删除附件
                TripReimbursementBll.DeleteFile(inn, id);

                //删除主表
                TripReimbursementBll.DeleteTripReimbursement(inn, id);
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
        /// <param name="item_id"></param>
        /// <returns></returns>
        public JsonResult GetTripReimbursementHistoryList(DataTableParameter para, string item_id)
        {
            int total = 0;
            List<HistoryModel> listDatas = GetTripReimbursementHistoryList(out total, para, item_id);
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

        public static List<HistoryModel> GetTripReimbursementHistoryList(out int total, DataTableParameter para, string item_id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_TRIPREIMBURSEMENTFORM
                                                  join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                  join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                  join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                  join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                  join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                  join p in db.USER on o.CLOSED_BY equals p.ID
                                                  where g.id == item_id && o.CLOSED_BY != null && o.COMMENTS != "AutoComplete"
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

        /// <summary>
        /// 获取Select流程数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkflowStatusList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<SelectModel> list = new List<SelectModel>();
                var result = WorkFlowBll.GetWorkflowStatusList(inn, "b_TripReimbursementWorkFlow");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (int i = 0; i < result.getItemCount(); i++)
                    {
                        SelectModel model = new SelectModel();
                        var item = result.getItemByIndex(i);
                        model.value = item.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(model.value))
                        {
                            model.text = Common.GetChineseValueByParam(model.value, "TRManageWorkFlow", "WorkFlow", Userinfo.language);
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
        /// 添加项目总监
        /// </summary>
        /// <param name="model"></param>
        public static void AddProjectAudit(Innovator inn, TripReimbursementModel model, List<string> projectNameList)
        {
            List<string> managerIds = new List<string>();
            List<string> directorIds = new List<string>();
            List<string> vpIds = new List<string>();
            List<string> projectManagers = new List<string> { "Project Manager" };
            List<string> projectDirectors = new List<string> { "Project Director" };
            List<string> projectVps = new List<string> { "Project VP" };

            if (projectNameList != null && projectNameList.Count > 0)
            {
                foreach (var item in projectNameList)
                {
                    Item projectItem = ProjectManageDA.GetProjectManageByName(inn, item);
                    if (!projectItem.isError())
                    {
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

                        //获取项目VP
                        if (projectVpName != model.b_Employee)
                        {
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
                        if (directorId == vpId && model.b_TotalExpense > 3000)
                        {
                            directorIds.Remove(directorId);
                            index--;
                            break;
                        }
                        else if (directorId == vpId && model.b_TotalExpense <= 3000)
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
                        if (managerId == vpId && model.b_TotalExpense > 3000)
                        {
                            managerIds.Remove(managerId);
                            index--;
                            break;
                        }
                        else if (managerId == vpId && model.b_TotalExpense <= 3000)
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
                    ActivityBll.AddActivityAuth(inn, model.id, id, projectManagers, "innovator.b_TripReimbursementForm", voting_weight);
                }
            }

            //将部门总监审核人  添加到流程审批节点
            if (directorIds.Count > 0)
            {
                int voting_weight = Common.CalculationWeight(directorIds.Count);
                foreach (var id in directorIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.id, id, projectDirectors, "innovator.b_TripReimbursementForm", voting_weight);
                }
            }

            //将部门VP审核人添加到流程审核节点
            if (vpIds.Count > 0)
            {
                int voting_weight = Common.CalculationWeight(vpIds.Count);
                foreach (var id in vpIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.id, id, projectVps, "innovator.b_TripReimbursementForm", voting_weight);
                }
            }
        }

        /// <summary>
        /// 验证填单人信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult ValidUserInfo(string userName)
        {
            var retModel = new JsonReturnModel();
            try
            {
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
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "TRCommon", "TRItemType", ViewBag.language));
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


        /// <summary>
        /// 获取出差单根据条件
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public JsonResult GetBusinessTravelByParam(string b_BTRecordNo, string b_Employee)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var businessTravel = BusinessTravelDA.GetBusinessTravelByParam(b_BTRecordNo, b_Employee);
                retModel.data = businessTravel;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }




    }
}