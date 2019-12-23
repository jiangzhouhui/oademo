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
using System.Transactions;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.BusinessTravel
{
    public class BusinessTravelController : BaseController
    {
        // GET: BusinessTravel
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("出差申请", "b_BusinessTravel", "BTItemType", ViewBag.language);
            ViewBag.UserName = Userinfo.UserName;
            ViewBag.UserEmail = Userinfo.Email;
            ViewBag.department = Userinfo.department;
            ViewBag.b_JobNumber = Userinfo.b_JobNumber;
            return View();
        }

        public JsonResult GetBusinessTravelList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            //获取委托权限数据
            List<string> agentRoles = AgentSetBll.GetAgentRoles(Userinfo, "BusinessTravel");

            int total = 0;
            var dataList = GetBusinessTravelList(Userinfo, out total, para, searchValue, startTime, endTime, status, agentRoles);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "";

                    //获取当前活动的数据
                    var result = ActivityDA.GetActivityAuditByLoginInfo(inn, item.Id, "innovator.B_BUSINESSTRAVEL", Userinfo.Roles, agentRoles);
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

                    if ((item.b_Preparer == Userinfo.UserName) && item.status != "Start" && item.status != "Administrative approval")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='fa fa-mail-reply recall' title='撤回' Id='" + item.Id + "' activityId='" + item.activityId + "' ></a>";
                    }

                    if (item.status == "Start")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' Id='" + item.Id + "' ></a>";
                    }
                    strHtml = string.Format(strHtml, item.b_DocumentNo, linkAList);
                    item.b_ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    item.b_DocumentNo = strHtml;
                    item.b_ProjectName = "<div style='width:100px;word-wrap:break-word;'>" + item.b_ProjectName + "</div>";
                    item.AuditorStr = ActivityDA.GetActivityOperator(inn, item.activityId, false);
                    item.AuditorStr = "<div style='width:100px;word-wrap:break-word;'>" + item.AuditorStr + "</div>";
                    item.status = Common.GetChineseValueByParam(item.status, "BTManageWorkFlow", "WorkFlow", Userinfo.language);
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


        public static List<BusinessTravelModel> GetBusinessTravelList(UserInfo userInfo, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status, List<string> agentRoles)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<BusinessTravelModel> datas = (from g in db.B_BUSINESSTRAVEL
                                                         join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                         join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                         join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                         join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                         join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                         join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                         where (i.STATE == "active" && o.VOTING_WEIGHT > 0 && (userInfo.Roles.Contains(p.ID) || g.B_PREPARER == userInfo.UserName || (agentRoles.Contains(p.ID) && i.KEYED_NAME != "Start"))) && (g.B_DOCUMENTNO.Contains(searchValue) || g.B_DEPT.Contains(searchValue) || g.B_EMPLOYEE.Contains(searchValue)) && o.CLOSED_BY == null
                                                         select new BusinessTravelModel
                                                         {
                                                             Id = g.id,
                                                             b_DocumentNo = g.B_DOCUMENTNO,
                                                             nb_ApplicationDate = g.CREATED_ON,
                                                             b_Dept = g.B_DEPT,
                                                             b_Employee = g.B_EMPLOYEE,
                                                             b_ProjectName = g.B_PROJECTNAME,
                                                             b_Preparer = g.B_PREPARER,
                                                             b_Destination = g.B_DESTINATION,
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
                if (para != null)
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
        /// 保存申请单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveBusinessTravel(BusinessTravelModel model)
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
                    if (UserBll.IsCEObyUserName(inn, model.b_Employee))
                    {
                        isCEO = true;
                        model.b_IsHangUp = true;
                        model.b_HangUpActivityName = "Administrative approval";
                    }
                }

                //查看人员信息是否完整
                if (!isCEO && string.IsNullOrEmpty(employeeInfo.B_CENTRE))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "TRCommon", "TRItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (!isCEO && string.IsNullOrEmpty(model.b_SeniorManager) && string.IsNullOrEmpty(model.b_Director) && string.IsNullOrEmpty(model.b_VP))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("申请人的基础信息不完整，无法提交申请,请联系系统管理员！", "TRCommon", "TRItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //验证高级经理
                if (!string.IsNullOrEmpty(model.b_SeniorManager))
                {
                    lineleader = IdentityDA.GetIdentityByKeyedName(inn, model.b_SeniorManager);
                    if (lineleader.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的高级经理！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }
                //部门总监
                if (!string.IsNullOrEmpty(model.b_Director))
                {
                    deptleader = IdentityDA.GetIdentityByKeyedName(inn, model.b_Director);
                    if (deptleader.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的部门总监！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //验证中心领导
                if (!string.IsNullOrEmpty(model.b_VP))
                {
                    divisionvp = IdentityDA.GetIdentityByKeyedName(inn, model.b_VP);
                    if (divisionvp.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的VP！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //验证输入的项目名称是否正确
                if (!string.IsNullOrEmpty(model.b_ProjectName))
                {
                    List<string> Project = model.b_ProjectName.Split(';').Where(x => x != "").Distinct().ToList();
                    foreach (var projectname in Project)
                    {
                        if (!ProjectManageDA.GetProjectManageNoBy(inn, projectname))
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("请输入正确的项目名称!", "ERCommon", "ERItemType", ViewBag.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                    model.b_ProjectName = "";
                    foreach (var item in Project)
                    {
                        model.b_ProjectName = model.b_ProjectName + item + ";";
                    }
                }

                string presentRemark = "";
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    //数据准备
                    Item itemRoot = inn.newItem();
                    if (string.IsNullOrEmpty(model.status))
                    {
                        //设置单据编号
                        string dataStr = DateTime.Now.ToString("yyyyMM");
                        model.b_DocumentNo = "BT" + "-" + dataStr + "-";
                        itemRoot = inn.newItem("b_BusinessTravel", "add");
                        itemRoot = OperationData(model, itemRoot);
                    }
                    else if (model.status == "Start")
                    {
                        //删除机票代订
                        BusinessTravelBll.DeleteFlightBookingItem(inn, model.Id);

                        //删除酒店代订
                        BusinessTravelBll.DeleteHotelBookingItem(inn, model.Id);

                        //修改编辑数据
                        itemRoot = inn.newItem("b_BusinessTravel", "edit");
                        itemRoot.setAttribute("id", model.Id);
                        itemRoot = OperationData(model, itemRoot);
                    }
                    //备注
                    presentRemark = model.b_Remark;
                    if (!string.IsNullOrEmpty(model.b_Remark))
                    {
                        if (!string.IsNullOrEmpty(model.Id))
                        {
                            string oldRemark = BusinessTravelBll.GetOldRemarkById(inn, model.Id);
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
                            model.b_DocumentNo = BusinessTravelBll.CreateRecordNo(inn, model.Id);
                        }
                    }
                    //没有错误，提交事务
                    ts.Complete();
                }


                //推动流程状态
                if (model.operation == "submit")
                {
                    if (model.b_Type == "Non Project")
                    {
                        //添加高级经理
                        List<string> lineLeaders = new List<string>() { "Dept.Manager" };
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_BusinessTravel", lineLeaders);
                        if (lineleader != null)
                        {
                            string lineLeaderId = lineleader.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.Id, lineLeaderId, lineLeaders, "innovator.b_BusinessTravel");
                        }

                        //添加部门总监
                        List<string> departmentLeaders = new List<string> { "Dept.Director" };
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_BusinessTravel", departmentLeaders);
                        if (deptleader != null)
                        {
                            string departmentLeaderId = deptleader.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.Id, departmentLeaderId, departmentLeaders, "innovator.b_BusinessTravel");
                        }
                        //添加中心VP
                        List<string> divisionVps = new List<string>() { "Division VP" };
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_BusinessTravel", divisionVps);
                        if (divisionvp != null && (model.b_TripType == "International" || deptleader == null))
                        {
                            string divisionVpId = divisionvp.getProperty("id");
                            ActivityBll.AddActivityAuth(inn, model.Id, divisionVpId, divisionVps, "innovator.b_BusinessTravel");
                        }
                    }
                    else
                    {
                        //当为项目类型时   添加项目审核人员
                        List<string> projectAudits = new List<string> { "Project Manager", "Project Director", "Project VP" };
                        //删除项目审核人员
                        ActivityBll.DeleteActivityAuthById(inn, model.Id, "innovator.b_BusinessTravel", projectAudits);
                        //添加项目审核人员
                        AddProjectAudit(inn, model);
                    }

                    bool isAdministrativeSupport = false;
                    if (model.b_FlightIsssue == "FlightIsssue" || model.b_FlightBooking == "FlightBooking" || model.b_HotelBooking == "HotelBooking" || model.b_Didi == "Didi" || model.b_Others == "Others")
                    {
                        isAdministrativeSupport = true;
                    }

                    //根据地区添加流程节点，角色审核
                    BusinessTravelBll.AddBusinessTraveAudit(inn, model.Id, model.b_Location, employeeInfo.B_CENTRE, isAdministrativeSupport, model.b_TripType);

                    //如果activityId为空时，任务ID
                    if (string.IsNullOrEmpty(model.activityId))
                    {
                        Item activityItem = ActivityDA.GetActivityByItemId(inn, model.Id, "innovator.b_BusinessTravel");
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
                            BusinessTravelBll.HangUpAutoAudit(inn, model.Id, model.b_HangUpActivityName, ref choicePath);
                        }

                        BusinessTravelBll bll = new BusinessTravelBll();
                        //判断下一步骤如果无人审核   直接过掉
                        bll.AutomaticCompletionTask(inn, model.Id, ref choicePath);

                    }

                    if (choicePath != null)
                    {
                        EmailEntity emailEntity = new EmailEntity();
                        emailEntity.ItemId = model.Id;
                        emailEntity.ApplicantDepartment = model.b_Dept;
                        emailEntity.ApplicantName = model.b_Employee;
                        emailEntity.RecordNo = model.b_DocumentNo;
                        BusinessTravelBll.SendEmailByOperation(inn, emailEntity, choicePath);
                    }
                }

            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message); ;
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 操作数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="itemRoot"></param>
        /// <returns></returns>
        private Item OperationData(BusinessTravelModel model, Item itemRoot)
        {
            itemRoot.setProperty("b_documentno", model.b_DocumentNo);
            itemRoot.setProperty("b_companycode", model.b_CompanyCode);
            itemRoot.setProperty("b_location", model.b_Location);
            itemRoot.setProperty("b_applicationdate", model.b_ApplicationDate);
            itemRoot.setProperty("b_triptype", model.b_TripType);
            itemRoot.setProperty("b_type", model.b_Type);
            itemRoot.setProperty("b_preparer", model.b_Preparer);
            itemRoot.setProperty("b_employee", model.b_Employee);
            itemRoot.setProperty("b_staffno", model.b_StaffNo);
            itemRoot.setProperty("b_position", model.b_Position);
            itemRoot.setProperty("b_dept", model.b_Dept);
            itemRoot.setProperty("b_costcenter", model.b_CostCenter);
            itemRoot.setProperty("b_mobile", model.b_Mobile);
            itemRoot.setProperty("b_projectname", model.b_ProjectName);
            itemRoot.setProperty("b_destination", model.b_Destination);
            itemRoot.setProperty("b_seniormanager", model.b_SeniorManager);
            itemRoot.setProperty("b_director", model.b_Director);
            itemRoot.setProperty("b_vp", model.b_VP);
            itemRoot.setProperty("b_traveldate", model.b_TravelDate);
            itemRoot.setProperty("b_estimatedreturndate", model.b_EstimatedReturnDate);
            itemRoot.setProperty("b_purpose", model.b_Purpose);
            itemRoot.setProperty("b_travelschedule", model.b_TravelSchedule);
            itemRoot.setProperty("b_flightisssue", model.b_FlightIsssue);
            itemRoot.setProperty("b_flightbooking", model.b_FlightBooking);
            itemRoot.setProperty("b_hotelbooking", model.b_HotelBooking);
            itemRoot.setProperty("b_didi", model.b_Didi);
            itemRoot.setProperty("b_others", model.b_Others);
            itemRoot.setProperty("b_trafficexpense", model.b_TrafficExpense.ToString());
            itemRoot.setProperty("b_hotelexpense", model.b_HotelExpense.ToString());
            itemRoot.setProperty("b_fixedsubsidy", model.b_FixedSubsidy.ToString());
            itemRoot.setProperty("b_otherexpenses", model.b_OtherExpenses.ToString());
            model.b_IsReimbursement = 0;
            itemRoot.setProperty("b_isreimbursement", model.b_IsReimbursement.ToString());
            itemRoot.setProperty("b_didimoney", model.b_DidiMoney.ToString());
            itemRoot.setProperty("b_didiaddmoney", model.b_DidiAddMoney.ToString());


            if (model.b_Others == "Others")
            {
                itemRoot.setProperty("b_othercontent", model.b_OtherContent);
            }
            else
            {
                itemRoot.setProperty("b_othercontent", "");
            }
            itemRoot.setProperty("b_travelbudget", model.b_TravelBudget.ToString());
            if (model.operation == "submit")
            {
                itemRoot.setProperty("b_ishangup", "0");
                itemRoot.setProperty("b_hangupactivityname", "");
            }


            if (model.b_FlightBooking == "FlightBooking" && model.FlightBookingItems != null)
            {
                for (int i = 0; i < model.FlightBookingItems.Count; i++)
                {
                    var item = model.FlightBookingItems[i];
                    var R_FlightBooking = inn.newItem("R_FlightBooking", "add");
                    var FlightBooking = inn.newItem("b_FlightBooking", "add");
                    FlightBooking.setProperty("b_firstname", item.b_FirstName);
                    FlightBooking.setProperty("b_lastname", item.b_LastName);
                    FlightBooking.setProperty("b_idtype", item.b_IDType);
                    FlightBooking.setProperty("b_idcardno", item.b_IDCardNo);
                    FlightBooking.setProperty("b_nationality", item.b_Nationality);
                    FlightBooking.setProperty("b_passportnumber", item.b_PassportNumber);
                    FlightBooking.setProperty("b_dateofexpiration", item.b_Dateofexpiration);
                    FlightBooking.setProperty("b_dateofbirth", item.b_Dateofbirth);
                    FlightBooking.setProperty("b_address", item.b_Address);
                    FlightBooking.setProperty("b_gooff", item.b_Gooff.ToString());
                    FlightBooking.setProperty("b_goplace", item.b_Goplace);
                    FlightBooking.setProperty("b_flightnumber", item.b_Flightnumber);
                    R_FlightBooking.setRelatedItem(FlightBooking);
                    itemRoot.addRelationship(R_FlightBooking);
                }
            }

            if (model.b_HotelBooking == "HotelBooking" && model.HotelBookingItems != null)
            {
                for (int i = 0; i < model.HotelBookingItems.Count; i++)
                {
                    var item = model.HotelBookingItems[i];
                    var R_HotelBooking = inn.newItem("R_HotelBooking", "add");
                    var HotelBooking = inn.newItem("b_HotelBooking", "add");
                    HotelBooking.setProperty("b_checkindate", item.b_Checkindate);
                    HotelBooking.setProperty("b_leavedate", item.b_Leavedate);
                    HotelBooking.setProperty("b_specificaddress", item.b_Specificaddress);
                    R_HotelBooking.setRelatedItem(HotelBooking);
                    itemRoot.addRelationship(R_HotelBooking);
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
                        var btFile = inn.newItem("R_File", "add");
                        var fileItem = inn.newItem("File", "get");
                        fileItem.setAttribute("id", fileId);
                        btFile.setRelatedItem(fileItem);
                        itemRoot.addRelationship(btFile);
                    }
                }
            }
            return itemRoot;
        }

        /// <summary>
        /// 审核出差单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult AuditBusinessTravel(BusinessTravelModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                EmailEntity emailEntity = new EmailEntity();
                emailEntity.ItemId = model.Id;
                emailEntity.ApplicantDepartment = model.b_Dept;
                emailEntity.ApplicantName = model.b_Employee;
                emailEntity.RecordNo = model.b_DocumentNo;

                WORKFLOW_PROCESS_PATH choicePath = AuditChoiceLine(model, inn);
                string presentRemark = model.b_Remark;

                //修改编辑数据
                var itemRoot = inn.newItem("b_BusinessTravel", "edit");
                itemRoot.setAttribute("id", model.Id);
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    string oldRemark = BusinessTravelBll.GetOldRemarkById(inn, model.Id);
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
                Item result;
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    if (model.status == "Administrative approval")
                    {
                        //删除机票代订
                        BusinessTravelBll.DeleteFlightBookingItem(inn, model.Id);

                        //删除酒店代订
                        BusinessTravelBll.DeleteHotelBookingItem(inn, model.Id);

                        itemRoot = OperationData(model, itemRoot);
                    }

                    result = itemRoot.apply();
                    //没有错误，提交事务
                    ts.Complete();
                }
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

                    BusinessTravelBll bll = new BusinessTravelBll();
                    //判断下一步骤如果无人审核   直接过掉
                    bll.AutomaticCompletionTask(inn, model.Id, ref choicePath);

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
                                if (model.b_Type == "Non Project" && UserBll.CeoBeforeIsAudit(inn, model.b_SeniorManager, model.b_Director, model.b_VP, model.b_Employee))
                                {
                                    choicePath = WorkFlowBll.AutoCompleteActivityByParam(model.Id, "innovator.b_BusinessTravel", "agree");
                                }
                            }
                        }
                    }

                    //判断活动是否结束，如果结束发送邮件到下一环节
                    if (ActivityBll.ActivityIsClosed(inn, choicePath.SOURCE_ID))
                    {
                        BusinessTravelBll.SendEmailByOperation(inn, emailEntity, choicePath, model.b_Location);
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
        ///  审核选择的路线
        /// </summary>
        /// <param name="model"></param>
        /// <param name="inn"></param>
        /// <returns></returns>
        public static WORKFLOW_PROCESS_PATH AuditChoiceLine(BusinessTravelModel model, Innovator inn)
        {
            //获取路线
            var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
            string lineName = BusinessTravelBll.GetLineNameByActivityName(inn, model.status);
            WORKFLOW_PROCESS_PATH choicePath = listActivity.Where(x => x.NAME == lineName).FirstOrDefault();
            return choicePath;
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <returns></returns>
        public JsonResult RefuseBusinessTravel(BusinessTravelModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    //修改编辑数据
                    var itemRoot = inn.newItem("b_BusinessTravel", "edit");
                    itemRoot.setAttribute("id", model.Id);
                    string oldRemark = BusinessTravelBll.GetOldRemarkById(inn, model.Id);
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
        public JsonResult HangUpBusinessTravel(BusinessTravelModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var itemRoot = inn.newItem("b_BusinessTravel", "edit");
                itemRoot.setAttribute("id", model.Id);
                itemRoot.setProperty("b_ishangup", "1");
                itemRoot.setProperty("b_hangupactivityname", model.status);
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    string oldRemark = BusinessTravelBll.GetOldRemarkById(inn, model.Id);
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

                    if (!result.isError())
                    {
                        ReturnToStart(model, presentRemark);
                    }
                    else
                    {
                        retModel.AddError("errorMessage", result.getErrorString());
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
        /// 日志
        /// </summary>
        /// <param name="para"></param>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        public JsonResult GetBusinessTravelHistoryList(DataTableParameter para, string item_Id)
        {
            int total = 0;
            List<HistoryModel> listDatas = GetBusinessTravelHistoryList(out total, para, item_Id);
            foreach (var model in listDatas)
            {
                model.Create_onStr = model.Create_onStr.GetValueOrDefault().AddHours(8);
                model.Created_on = model.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
            }
            return Json(new
            {
                sEcho = para.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = listDatas
            }, JsonRequestBehavior.AllowGet);
        }

        private static List<HistoryModel> GetBusinessTravelHistoryList(out int total, DataTableParameter para, string item_Id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_BUSINESSTRAVEL
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


        /// <summary>
        /// 根据ID获取出差申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetBusinessTravelById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result = BusinessTravelBll.GetBusinessTravelObjById(inn, id);
                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                BusinessTravelModel model = new BusinessTravelModel();
                model.Id = result.getProperty("id");
                model.b_DocumentNo = result.getProperty("b_documentno");
                model.b_CompanyCode = result.getProperty("b_companycode");
                model.b_Location = result.getProperty("b_location");
                model.b_ApplicationDate = DateTime.Parse(result.getProperty("b_applicationdate")).ToString("yyyy-MM-dd");
                model.b_TripType = result.getProperty("b_triptype");
                model.b_Type = result.getProperty("b_type");
                model.b_Preparer = result.getProperty("b_preparer");
                model.b_Employee = result.getProperty("b_employee");
                model.b_StaffNo = result.getProperty("b_staffno");
                model.b_Position = result.getProperty("b_position");
                model.b_Dept = result.getProperty("b_dept");
                model.b_CostCenter = result.getProperty("b_costcenter");
                model.b_Mobile = result.getProperty("b_mobile");
                model.b_ProjectName = result.getProperty("b_projectname");
                model.b_Destination = result.getProperty("b_destination");
                model.b_SeniorManager = result.getProperty("b_seniormanager");
                model.b_Director = result.getProperty("b_director");
                model.b_VP = result.getProperty("b_vp");
                model.b_TravelDate = DateTime.Parse(result.getProperty("b_traveldate")).ToString("yyyy-MM-dd");
                model.b_EstimatedReturnDate = DateTime.Parse(result.getProperty("b_estimatedreturndate")).ToString("yyyy-MM-dd");
                model.b_Purpose = result.getProperty("b_purpose");
                model.b_TravelSchedule = result.getProperty("b_travelschedule");
                model.b_TravelBudget = !string.IsNullOrEmpty(result.getProperty("b_travelbudget")) ? decimal.Parse(result.getProperty("b_travelbudget")) : 0;
                model.b_FlightIsssue = result.getProperty("b_flightisssue");
                model.b_FlightBooking = result.getProperty("b_flightbooking");
                model.b_HotelBooking = result.getProperty("b_hotelbooking");
                model.b_Didi = result.getProperty("b_didi");
                model.b_Others = result.getProperty("b_others");
                model.b_OtherContent = result.getProperty("b_othercontent");
                model.b_IsHangUp = result.getProperty("b_ishangup") == "1" ? true : false;
                model.b_HangUpActivityName = result.getProperty("b_hangupactivityname");
                model.OldRemark = result.getProperty("b_remark");
                if (model.OldRemark != null && model.OldRemark.Contains("\r\n"))
                {
                    model.OldRemark = model.OldRemark.Replace("\r\n", "<br/>");
                }
                model.b_TrafficExpense = !string.IsNullOrEmpty(result.getProperty("b_trafficexpense")) ? decimal.Parse(result.getProperty("b_trafficexpense")) : 0;
                model.b_HotelExpense = !string.IsNullOrEmpty(result.getProperty("b_hotelexpense")) ? decimal.Parse(result.getProperty("b_hotelexpense")) : 0;
                model.b_FixedSubsidy = !string.IsNullOrEmpty(result.getProperty("b_fixedsubsidy")) ? decimal.Parse(result.getProperty("b_fixedsubsidy")) : 0;
                model.b_OtherExpenses = !string.IsNullOrEmpty(result.getProperty("b_otherexpenses")) ? decimal.Parse(result.getProperty("b_otherexpenses")) : 0;
                model.b_DidiMoney = !string.IsNullOrEmpty(result.getProperty("b_didimoney")) ? decimal.Parse(result.getProperty("b_didimoney")) : 0;
                model.b_DidiAddMoney = !string.IsNullOrEmpty(result.getProperty("b_didiaddmoney")) ? decimal.Parse(result.getProperty("b_didiaddmoney")) : 0;

                //机票代订
                Item Relation = result.getRelationships("R_FlightBooking");
                if (Relation.getItemCount() > 0)
                {
                    model.FlightBookingItems = new List<FlightBooking>();
                    for (int i = 0; i < Relation.getItemCount(); i++)
                    {
                        Item ItemObJ = Relation.getItemByIndex(i).getRelatedItem();
                        FlightBooking itemModel = new FlightBooking();
                        itemModel.Id = ItemObJ.getProperty("id");
                        itemModel.b_FirstName = ItemObJ.getProperty("b_firstname");
                        itemModel.b_LastName = ItemObJ.getProperty("b_lastname");
                        itemModel.b_IDType = ItemObJ.getProperty("b_idtype");
                        itemModel.b_IDCardNo = ItemObJ.getProperty("b_idcardno");
                        itemModel.b_Nationality = ItemObJ.getProperty("b_nationality");
                        itemModel.b_PassportNumber = ItemObJ.getProperty("b_passportnumber");
                        itemModel.b_Dateofexpiration = !string.IsNullOrEmpty(ItemObJ.getProperty("b_dateofexpiration")) ? DateTime.Parse(ItemObJ.getProperty("b_dateofexpiration")).ToString("yyyy-MM-dd") : "";
                        itemModel.b_Dateofbirth = ItemObJ.getProperty("b_dateofbirth");
                        itemModel.b_Address = ItemObJ.getProperty("b_address");
                        itemModel.b_Gooff = ItemObJ.getProperty("b_gooff");
                        itemModel.b_Goplace = ItemObJ.getProperty("b_goplace");
                        itemModel.b_Flightnumber = ItemObJ.getProperty("b_flightnumber");
                        model.FlightBookingItems.Add(itemModel);
                    }
                }

                //酒店代订
                Item HotelRelation = result.getRelationships("R_HotelBooking");
                if (HotelRelation.getItemCount() > 0)
                {
                    model.HotelBookingItems = new List<HotelBooking>();
                    for (int i = 0; i < HotelRelation.getItemCount(); i++)
                    {
                        Item ItemObJ = HotelRelation.getItemByIndex(i).getRelatedItem();
                        HotelBooking itemHote = new HotelBooking();
                        itemHote.Id = ItemObJ.getProperty("id");
                        itemHote.b_Checkindate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_checkindate")) ? DateTime.Parse(ItemObJ.getProperty("b_checkindate")).ToString("yyyy-MM-dd") : "";
                        itemHote.b_Leavedate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_leavedate")) ? DateTime.Parse(ItemObJ.getProperty("b_leavedate")).ToString("yyyy-MM-dd") : "";
                        itemHote.b_Specificaddress = ItemObJ.getProperty("b_specificaddress");
                        model.HotelBookingItems.Add(itemHote);
                    }
                }

                //获取文件信息
                Item nFiles = result.getRelationships("R_File");
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
        /// 删除出差单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteBusinessTravel(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //删除文件
                BusinessTravelBll.DeleteFile(inn, id);
                //删除机票代订
                BusinessTravelBll.DeleteFlightBookingItem(inn, id);
                //删除酒店代订
                BusinessTravelBll.DeleteHotelBookingItem(inn, id);
                //删除主表基础信息
                BusinessTravelBll.DeleteBusinessTravel(inn, id);
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
        public JsonResult RecallBusinessTravel(string id, string activityId)
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
                    Item activityItem = ActivityDA.GetActivityByItemId(adminInn, id, "Administrators", "b_BusinessTravel");
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
        /// <param name="presentRemark"></param>
        private void ReturnToStart(BusinessTravelModel model, string presentRemark)
        {
            EmailEntity emailEntity = new EmailEntity();
            emailEntity.ItemId = model.Id;
            emailEntity.ApplicantDepartment = model.b_Dept;
            emailEntity.ApplicantName = model.b_Employee;
            emailEntity.RecordNo = model.b_DocumentNo;

            //获取路线
            var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
            WORKFLOW_PROCESS_PATH choicePath = null;
            if (!string.IsNullOrEmpty(model.status))
            {
                choicePath = listActivity.Where(x => x.NAME == "ReturnStart").FirstOrDefault();
            }
            string errorStr = "";
            if (model.status == "Project Manager" || model.status == "Project Director" || model.status == "Project VP")
            {
                //获取Admin的Aras 连接
                var adminInn = WorkFlowBll.GetAdminInnovator();
                if (adminInn != null)
                {
                    //获取Admin 对当前任务权限数据
                    Item activityItem = ActivityDA.GetActivityByItemId(adminInn, model.Id, "Administrators", "b_BusinessTravel");
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
                errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark, Userinfo);
            }
            if (string.IsNullOrEmpty(errorStr))
            {
                BusinessTravelBll.SendEmailByOperation(inn, emailEntity, choicePath);
            }
        }

        /// <summary>
        /// 获取查询Select流程状态数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkflowStatusList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<SelectModel> list = new List<SelectModel>();
                var result = WorkFlowBll.GetWorkflowStatusList(inn, "b_BusinessTravelWorkFlow");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (int i = 0; i < result.getItemCount(); i++)
                    {
                        SelectModel model = new SelectModel();
                        var item = result.getItemByIndex(i);
                        model.value = item.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(model.value))
                        {
                            model.text = Common.GetChineseValueByParam(model.value, "BTManageWorkFlow", "WorkFlow", Userinfo.language);
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


        private static void AddProjectAudit(Innovator inn, BusinessTravelModel model)
        {
            List<string> managerIds = new List<string>();
            List<string> directorIds = new List<string>();
            List<string> vpIds = new List<string>();
            List<string> projectManagers = new List<string> { "Project Manager" };
            List<string> projectDirectors = new List<string> { "Project Director" };
            List<string> projectVps = new List<string> { "Project VP" };

            List<string> projectNameList = model.b_ProjectName.Split(';').Where(x => x != "").ToList();
            foreach (var item in projectNameList)
            {
                Item projectItem = ProjectManageDA.GetProjectManageByName(inn, item);
                if (!projectItem.isError() && projectItem.getItemCount() > 0)
                {
                    //获取项目经理
                    string projectManagerName = projectItem.getItemByIndex(0).getProperty("b_projectmanager");
                    //获取项目总监
                    string projectDirectorName = projectItem.getItemByIndex(0).getProperty("b_projectdirector");
                    //获取项目VP
                    string projectVpName = projectItem.getItemByIndex(0).getProperty("b_projectvp");

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
                        if (directorId == vpId && model.b_TripType == "International")
                        {
                            directorIds.Remove(directorId);
                            index--;
                            break;
                        }
                        else if (directorId == vpId && model.b_TripType == "Domestic")
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
                        if (managerId == vpId && model.b_TripType == "International")
                        {
                            managerIds.Remove(managerId);
                            index--;
                            break;
                        }
                        else if (managerId == vpId && model.b_TripType == "Domestic")
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
                    ActivityBll.AddActivityAuth(inn, model.Id, id, projectManagers, "innovator.b_BusinessTravel", voting_weight);
                }
            }

            //将部门总监审核人  添加到流程审批节点
            if (directorIds.Count > 0)
            {
                int voting_weight = Common.CalculationWeight(directorIds.Count);
                foreach (var id in directorIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.Id, id, projectDirectors, "innovator.b_BusinessTravel", voting_weight);
                }
            }

            //将部门VP审核人   添加到流程审核节点
            if (vpIds.Count > 0 && (model.b_TripType == "International" || directorIds.Count == 0))
            {
                int voting_weight = Common.CalculationWeight(vpIds.Count);
                foreach (var id in vpIds)
                {
                    ActivityBll.AddActivityAuth(inn, model.Id, id, projectVps, "innovator.b_BusinessTravel", voting_weight);
                }
            }
        }

        /// <summary>
        ///  验证用户信息
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


        /// <summary>
        ///  获取机票、酒店待定配置
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult GetBookingStaffingByUserName(string userName)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var bookingStaffing = BookingStaffingDA.GetBookingStaffingByUserName(userName);
                retModel.data = bookingStaffing;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }



    }
}