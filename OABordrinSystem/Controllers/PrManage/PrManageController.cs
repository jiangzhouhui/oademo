using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.PrManage
{
    public class PrManageController : BaseController
    {
        // GET: PrManage
        public ActionResult Index()
        {
            ViewBag.UserName = Userinfo.UserName;
            ViewBag.UserEmail = Userinfo.Email;
            ViewBag.department = Userinfo.department;
            ViewBag.CurrentName = Common.GetLanguageValueByParam("待办", "CommonName", "Common", ViewBag.language);
            return View();
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPrManageList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetPrManageList(Userinfo.Roles, out total, para, searchValue, startTime, endTime, status);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "";
                    if (item.status == "Start")
                    {
                        linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' activityId='" + item.activityId + "' activityAssignmentId='" + item.activityAssignmentId + "' Id='" + item.id + "' ItemStatus='" + item.status.Trim() + "'></a>";
                    }
                    else
                    {
                        linkAList += "<a class='glyphicon glyphicon-user audit' title='审核' activityId='" + item.activityId + "' activityAssignmentId='" + item.activityAssignmentId + "' id='" + item.id + "' ItemStatus='" + item.status.Trim() + "'></a>";
                    }
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-list-alt history' title='日志' id='" + item.id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-asterisk workflow' title='流程' ItemStatus='" + item.status + "' id='" + item.id + "' b_VersionNo='" + item.b_VersionNo + "' ></a>";
                    linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.id + "' ></a>";
                    if (item.status == "Start")
                    {
                        linkAList += "&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' id='" + item.id + "' ></a>";
                    }
                    strHtml = string.Format(strHtml, item.b_PrRecordNo, linkAList);
                    item.b_PrRecordNo = strHtml;
                    item.b_RaisedDate = item.nb_RaisedDate.ToString("yyyy-MM-dd");
                    item.AuditorStr = ActivityDA.GetActivityOperator(inn, item.activityId);
                    item.AuditorStr = "<div style='width:180px;word-wrap:break-word;'>" + item.AuditorStr + "</div>";
                    item.status = Common.GetChineseValueByParam(item.status, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
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

        public static List<PrManageModel> GetPrManageList(List<string> roles, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<PrManageModel> datas = (from g in db.B_PRMANAGE
                                                   join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                   join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                   join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                   join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                   join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                   join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                   where (i.STATE == "active" && roles.Contains(p.ID)) && (g.B_PRRECORDNO.Contains(searchValue) || g.B_PROJECTNAME.Contains(searchValue) || g.B_BUSINESSDEPARTMENT.Contains(searchValue) || g.B_APPLICANT.Contains(searchValue) || g.B_BUYER.Contains(searchValue) || g.B_PURCHASECONTENT.Contains(searchValue)) && o.CLOSED_BY == null
                                                   select new PrManageModel
                                                   {
                                                       id = g.id,
                                                       b_PrRecordNo = g.B_PRRECORDNO,
                                                       b_PurchaseContent = g.B_PURCHASECONTENT,
                                                       b_BusinessDepartment = g.B_BUSINESSDEPARTMENT,
                                                       nb_RaisedDate = g.CREATED_ON,
                                                       b_Applicant = g.B_APPLICANT,
                                                       b_Buyer = g.B_BUYER,
                                                       b_Budget = g.B_BUDGET,
                                                       b_VersionNo = g.B_VERSIONNO,
                                                       status = i.KEYED_NAME,
                                                       activityId = i.ID,
                                                       activityAssignmentId = o.ID
                                                   });
                // 时间查询
                if (startTime != null)
                {
                    datas = datas.Where(x => x.nb_RaisedDate >= startTime);
                }

                if (endTime != null)
                {
                    datas = datas.Where(x => x.nb_RaisedDate <= endTime);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    datas = datas.Where(x => x.status == status);
                }
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
        /// 保存采购需求单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SavePrManage(PrManageModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                if(model.operation == "submit" && model.b_VersionNo== "PR_001")
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("由于公司PR流程审批权限变更，2月9日前已创建未提交的申请单需要重新填写。请重新填写一张新PR单。", "PRCommon", "PRItemType", Userinfo.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(model.b_PrType))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("必须选择采购类型！", "PRCommon", "PRItemType", Userinfo.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //时间类型转换
                DateTime raisedDate;
                if (!DateTime.TryParse(model.b_RaisedDate, out raisedDate))
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("输入的时间格式有误！", "PRCommon", "PRItemType", Userinfo.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //查看当前人是否为CEO
                bool isCEO = false;
                USER employeeInfo = UserDA.GetUserByFirstName(Userinfo.UserName);
                if (UserBll.IsCEObyUserName(inn, Userinfo.UserName))
                {
                    isCEO = true;
                }

                Item departmentManager = null;
                Item departmentDirector = null;
                Item departmentVP = null;
                if (model.b_PrType == "Non Project")
                {
                    if (!string.IsNullOrEmpty(model.b_DeptManager))
                    {
                        departmentManager = IdentityDA.GetIdentityByKeyedName(inn, model.b_DeptManager);
                        if (departmentManager.isError())
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("找不到对应的部门经理！", "PRCommon", "PRItemType", Userinfo.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                        model.b_DeptManagerId = departmentManager.getProperty("id");
                    }

                    if (!string.IsNullOrEmpty(model.b_DeptDirector))
                    {
                        departmentDirector = IdentityDA.GetIdentityByKeyedName(inn, model.b_DeptDirector);
                        if (departmentDirector.isError())
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("找不到对应的部门总监！", "PRCommon", "PRItemType", Userinfo.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                        model.b_DeptDirectorId = departmentDirector.getProperty("id");
                    }

                    //if (!isCEO && string.IsNullOrEmpty(model.b_DeptManager) && string.IsNullOrEmpty(model.b_DeptDirector))
                    //{
                    //    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("部门经理和部门总监必填一项！", "PRCommon", "PRItemType", Userinfo.language));
                    //    return Json(retModel, JsonRequestBehavior.AllowGet);
                    //}


                    if (!string.IsNullOrEmpty(model.b_DeptManager) && !string.IsNullOrEmpty(model.b_DeptDirector) && model.b_DeptManager == model.b_DeptDirector)
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("部门经理和部门总监不可为同一人！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    if (!string.IsNullOrEmpty(model.b_DeptVP))
                    {
                        departmentVP = IdentityDA.GetIdentityByKeyedName(inn, model.b_DeptVP);
                        if (departmentVP.isError())
                        {
                            retModel.AddError("errorMessage", "找不到对应的部门VP！");
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                        model.b_DeptVPId = departmentVP.getProperty("id");
                    }
                }

                if (model.b_PrType == "project")
                {
                    if (string.IsNullOrEmpty(model.b_ProjectName))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("项目名称不可为空！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    //验证项目经理不可为空
                    if (string.IsNullOrEmpty(model.b_ProjectManager))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("项目经理不可为空！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(model.b_ProjectLeader))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("PMT/PAT leader不可为空！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    if (string.IsNullOrEmpty(model.b_ProjectDirector))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("项目总监 不可为空！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }


                    //1.获取IdentityId
                    Item projectLeader = IdentityDA.GetIdentityByKeyedName(inn, model.b_ProjectLeader);
                    if (projectLeader.isError())
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("项目中的PMT/PAT Leader无效！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }



                    Item projectManager = IdentityDA.GetIdentityByKeyedName(inn, model.b_ProjectManager);
                    if (projectManager.isError())
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("输入的项目经理无效！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }


                    Item projectDirector = IdentityDA.GetIdentityByKeyedName(inn, model.b_ProjectDirector);
                    if (projectDirector.isError())
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("项目中的Project Director无效！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                    model.b_ProjectLeaderId = projectLeader.getProperty("id");
                    model.b_ProjectDirectorId = projectDirector.getProperty("id");
                    model.b_ProjectManagerId = projectManager.getProperty("id");
                }

                Item itemRoot = inn.newItem();
                if (string.IsNullOrEmpty(model.status))
                {
                    itemRoot = AddOperation(model, raisedDate);
                }
                else if (model.status == "Start")
                {
                    itemRoot = UpdateOperation(model, raisedDate);
                }

                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    if (!string.IsNullOrEmpty(model.id))
                    {
                        string oldRemark = PrManageBll.GetOldRemarkById(inn, model.id);
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
                string id = model.id;
                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.status))
                    {
                        id = result.getProperty("id");
                        model.id = id;
                        if (!string.IsNullOrEmpty(id))
                        {
                            string sqlStr = "Declare @IndexNum int Select @IndexNum = isnull(Max(B_INDEXNUM), 0) + 1 from innovator.B_PRMANAGE where CREATED_ON &gt;= convert(varchar(10),DATEADD(yy, DATEDIFF(yy,0,getdate()), 0),120) and CREATED_ON &lt; convert(varchar(10),dateadd(ms,-3,DATEADD(yy,DATEDIFF(yy,0,getdate())+1,0)),120) update innovator.B_PRMANAGE set B_PRRECORDNO = B_PRRECORDNO + RIGHT('00000' + CAST(@IndexNum as varchar), 5),b_indexnum=@IndexNum where id = '" + id + "'";
                            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
                            model.b_PrRecordNo = PrManageBll.GetPrRecordNoById(inn, id);
                        }
                    }
                }

                //推动流程状态
                if (model.operation == "submit")
                {
                    //添加项目和部门的审核人
                    AddAuditInfoByVersion(inn, model, employeeInfo);

                    //如果activityId为空时，任务ID
                    if (string.IsNullOrEmpty(model.activityId))
                    {
                        Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.B_PRMANAGE");
                        if (!activityItem.isError())
                        {
                            model.activityId = activityItem.getProperty("activityid");
                            model.status = activityItem.getProperty("keyed_name");
                            model.activityAssignmentId = activityItem.getProperty("activityassignmentid");
                        }
                    }

                    //获取路线
                    var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
                    string lineName = PrManageBll.GetLineNameByActivityName(inn, model.id, model.activityId, model.status, model.b_PrType, model.b_VersionNo);
                    WORKFLOW_PROCESS_PATH choicePath = listActivity.Where(x => x.NAME == lineName).FirstOrDefault();
                    if (choicePath != null)
                    {
                        string errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark);
                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            retModel.AddError("errorMessage", errorStr);
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }

                        PrManageBll bll = new PrManageBll();
                        bll.AutomaticCompletionTask(inn, model.id, model.b_PrType, model.b_VersionNo, ref choicePath);
                        if (choicePath != null)
                        {
                            PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, choicePath);
                        }

                        //if (!string.IsNullOrEmpty(model.b_DeptManager))
                        //{
                        //    PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, choicePath);
                        //}

                        //if (string.IsNullOrEmpty(model.b_DeptManager))
                        //{
                        //    WORKFLOW_PROCESS_PATH DeptManagerChoicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.B_PRMANAGE");
                        //    if (DeptManagerChoicePath != null)
                        //    {
                        //        PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, DeptManagerChoicePath);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        private static void AddAuditInfoByVersion(Innovator inn, PrManageModel model, USER employeeInfo)
        {
            List<string> names = new List<string>() { "PMT/PAT Leader" };
            List<string> nnames = new List<string>() { "Project Director" };
            List<string> projectManagers = new List<string>() { "Project Manager" };
            List<string> deptManagers = new List<string>() { "Dept.Manager" };
            List<string> DeptDirectors = new List<string>() { "Dept.Director" };
            List<string> DeptVPs = new List<string>() { "Dept.VP" };
            if (model.b_VersionNo == "PR_001")
            {
                //如何是项目，添加PMT/PAT Leader、Project Director的权限
                if (model.b_PrType == "project")
                {
                    PrManageBll.AddPrManageAuthById(inn, model.id, model.b_ProjectLeaderId, names);
                    PrManageBll.AddPrManageAuthById(inn, model.id, model.b_ProjectDirectorId, nnames);
                    PrManageBll.AddPrManageAuthById(inn, model.id, model.b_ProjectManagerId, projectManagers);
                }
                else
                {
                    //获取ID域中的领导，设置Dept.Manager、Dept.Director

                    if (!string.IsNullOrEmpty(model.b_DeptManagerId))
                    {
                        PrManageBll.AddPrManageAuthById(inn, model.id, model.b_DeptManagerId, deptManagers);
                    }
                    else
                    {
                        PrManageBll.DeletePrManage(inn, model.id, deptManagers);
                    }


                    if (!string.IsNullOrEmpty(model.b_DeptDirectorId))
                    {
                        PrManageBll.AddPrManageAuthById(inn, model.id, model.b_DeptDirectorId, DeptDirectors);
                    }
                    else
                    {
                        PrManageBll.DeletePrManage(inn, model.id, DeptDirectors);
                    }
                }
            }

            if (model.b_VersionNo == "PR_002")
            {
                if (model.b_PrType == "project")
                {
                    PrManageBll.AddPrManageAuthById(inn, model.id, model.b_ProjectLeaderId, names);
                    PrManageBll.AddPrManageAuthById(inn, model.id, model.b_ProjectManagerId, projectManagers);
                    PrManageBll.AddPrManageAuthById(inn, model.id, model.b_ProjectDirectorId, nnames);

                    //当项目总监为空 或者  预算大于100万
                    if (string.IsNullOrEmpty(model.b_ProjectDirectorId) || model.b_Budget > (100 * 10000))
                    {
                        //添加CEO审核
                        if (employeeInfo.B_CENTRE == "盛和")
                        {
                            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, model.id, "GMSH", "", "GM", "innovator.B_PRMANAGE");
                        }
                        else if (employeeInfo.B_CENTRE == "骏盛")
                        {
                            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, model.id, "GMJS", "", "GM", "innovator.B_PRMANAGE");
                        }
                        else
                        {
                            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, model.id, "CEO", "", "GM", "innovator.B_PRMANAGE");
                        }
                    }
                }
                else
                {
                    //获取ID域中的领导，设置Dept.Manager、Dept.Director
                    if (!string.IsNullOrEmpty(model.b_DeptManagerId))
                    {
                        PrManageBll.AddPrManageAuthById(inn, model.id, model.b_DeptManagerId, deptManagers);
                    }
                    else
                    {
                        PrManageBll.DeletePrManage(inn, model.id, deptManagers);
                    }

                    if (!string.IsNullOrEmpty(model.b_DeptDirectorId))
                    {
                        PrManageBll.AddPrManageAuthById(inn, model.id, model.b_DeptDirectorId, DeptDirectors);
                    }
                    else
                    {
                        PrManageBll.DeletePrManage(inn, model.id, DeptDirectors);
                    }

                    //当总监为空  或者    预算大于50万   并且 VP不为空
                    if ((string.IsNullOrEmpty(model.b_DeptDirectorId) || model.b_Budget > (50 * 10000)) && !string.IsNullOrEmpty(model.b_DeptVPId))
                    {
                        PrManageBll.AddPrManageAuthById(inn, model.id, model.b_DeptVPId, DeptVPs);
                    }
                    else
                    {
                        PrManageBll.DeletePrManage(inn, model.id, DeptVPs);
                    }

                    //（当总监为空 并且 预算大于50万） 或者预算大于100万
                    if ((string.IsNullOrEmpty(model.b_DeptVPId) && model.b_Budget > (50 * 10000)) || model.b_Budget > (100 * 10000))
                    {
                        //添加CEO审核
                        if (employeeInfo.B_CENTRE == "盛和")
                        {
                            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, model.id, "GMSH", "", "GM", "innovator.B_PRMANAGE");
                        }
                        else if (employeeInfo.B_CENTRE == "骏盛")
                        {
                            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, model.id, "GMJS", "", "GM", "innovator.B_PRMANAGE");
                        }
                        else
                        {
                            WorkFlowBll.AddWorkFlowRoleAuditByRegion(inn, model.id, "CEO", "", "GM", "innovator.B_PRMANAGE");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pr单操作
        /// </summary>
        private Item AddOperation(PrManageModel model, DateTime raisedDate)
        {
            //获取PrManage的版本号
            string version = ConfigurationManager.AppSettings["PrManageVersion"];
            model.b_VersionNo = version;

            //设置单据编号
            model.b_PrRecordNo = "PR" + DateTime.Now.Year + "-";
            model.b_Budget = Math.Round(model.b_Budget, 2);
            var itemRoot = inn.newItem("B_PRMANAGE", "add");
            itemRoot.setProperty("b_prrecordno", model.b_PrRecordNo);
            itemRoot.setProperty("b_prtype", model.b_PrType);
            itemRoot.setProperty("b_businessdepartment", model.b_BusinessDepartment);
            itemRoot.setProperty("b_budget", model.b_Budget.ToString());
            itemRoot.setProperty("b_applicantid", Userinfo.UserId);
            itemRoot.setProperty("b_applicant", Userinfo.UserName);
            itemRoot.setProperty("b_raiseddate", raisedDate.ToString("yyyy-MM-dd"));
            itemRoot.setProperty("b_emailaddress", model.b_EmailAddress);
            itemRoot.setProperty("b_phoneno", model.b_PhoneNo);
            itemRoot.setProperty("b_projectname", model.b_ProjectName);
            itemRoot.setProperty("b_budgetcode", model.b_BudgetCode);
            itemRoot.setProperty("b_projectleader", model.b_ProjectLeader);
            itemRoot.setProperty("b_projectmanager", model.b_ProjectManager);
            itemRoot.setProperty("b_projectdirector", model.b_ProjectDirector);
            itemRoot.setProperty("b_indexnum", "0");
            itemRoot.setProperty("b_purchasecontent", model.b_PurchaseContent);
            itemRoot.setProperty("b_contractparty", model.b_ContractParty);
            itemRoot.setProperty("b_applicantaddress", model.b_ApplicantAddress);
            itemRoot.setProperty("b_deptmanager", model.b_DeptManager);
            itemRoot.setProperty("b_deptdirector", model.b_DeptDirector);
            itemRoot.setProperty("b_costcenter", model.b_CostCenter);
            itemRoot.setProperty("b_purchasingreason", model.b_PurchasingReason);
            itemRoot.setProperty("b_versionno", model.b_VersionNo);
            itemRoot.setProperty("b_deptvp", model.b_DeptVP);

            if (model.PrManageItems != null)
            {
                for (int i = 0; i < model.PrManageItems.Count; i++)
                {
                    var item = model.PrManageItems[i];
                    var requestInfo = inn.newItem("B_REQUESTINFO", "add");
                    var PrManageItem = inn.newItem("b_PrManageItem", "add");
                    PrManageItem.setProperty("b_no", (i + 1).ToString());
                    PrManageItem.setProperty("b_requestlist", item.b_RequestList);
                    PrManageItem.setProperty("b_specificationquantity", item.b_SpecificationQuantity);
                    PrManageItem.setProperty("b_projectno", item.b_ProjectNo);
                    PrManageItem.setProperty("b_taskno", item.b_TaskNo);
                    PrManageItem.setProperty("b_qty", item.b_Qty.ToString());
                    PrManageItem.setProperty("b_unit", item.b_Unit);
                    requestInfo.setRelatedItem(PrManageItem);
                    itemRoot.addRelationship(requestInfo);
                }
            }

            //文件保存
            if (model.fileIds != null)
            {
                for (int index = 0; index < model.fileIds.Count; index++)
                {
                    var fileId = model.fileIds[index];
                    var prManageFile = inn.newItem("b_PrManageFiles", "add");
                    var fileItem = inn.newItem("File", "get");
                    fileItem.setAttribute("id", fileId);
                    prManageFile.setRelatedItem(fileItem);
                    itemRoot.addRelationship(prManageFile);
                }
            }
            return itemRoot;
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="raisedDate"></param>
        /// <returns></returns>
        private Item UpdateOperation(PrManageModel model, DateTime raisedDate)
        {
            //先获取数据
            var GetrequestInfo = inn.newItem("B_REQUESTINFO", "get");
            GetrequestInfo.setAttribute("where", "source_id='" + model.id + "'");
            GetrequestInfo.setAttribute("select", "related_id");
            var oldItems = GetrequestInfo.apply();
            //string where
            if (oldItems.getItemCount() > 0)
            {
                string whereItem = "";
                string requestIds = "";
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                //删除需求清单
                string amlStr = "<AML><Item type='B_REQUESTINFO' action='purge' idlist='" + requestIds + "'></Item><Item type='B_PRMANAGEITEM' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }

            model.b_Budget = Math.Round(model.b_Budget, 2);
            var itemRoot = inn.newItem("B_PRMANAGE", "edit");
            itemRoot.setAttribute("id", model.id);
            itemRoot.setProperty("b_prtype", model.b_PrType);
            itemRoot.setProperty("b_businessdepartment", model.b_BusinessDepartment);
            itemRoot.setProperty("b_budget", model.b_Budget.ToString());
            itemRoot.setProperty("b_applicantid", Userinfo.UserId);
            itemRoot.setProperty("b_applicant", Userinfo.UserName);
            itemRoot.setProperty("b_raiseddate", raisedDate.ToString("yyyy-MM-dd"));
            itemRoot.setProperty("b_emailaddress", model.b_EmailAddress);
            itemRoot.setProperty("b_phoneno", model.b_PhoneNo);
            itemRoot.setProperty("b_projectname", model.b_ProjectName);
            itemRoot.setProperty("b_budgetcode", model.b_BudgetCode);
            itemRoot.setProperty("b_projectleader", model.b_ProjectLeader);
            itemRoot.setProperty("b_projectmanager", model.b_ProjectManager);
            itemRoot.setProperty("b_projectdirector", model.b_ProjectDirector);
            itemRoot.setProperty("b_purchasecontent", model.b_PurchaseContent);
            itemRoot.setProperty("b_contractparty", model.b_ContractParty);
            itemRoot.setProperty("b_applicantaddress", model.b_ApplicantAddress);
            itemRoot.setProperty("b_deptmanager", model.b_DeptManager);
            itemRoot.setProperty("b_deptdirector", model.b_DeptDirector);
            itemRoot.setProperty("b_costcenter", model.b_CostCenter);
            itemRoot.setProperty("b_purchasingreason", model.b_PurchasingReason);
            itemRoot.setProperty("b_deptvp", model.b_DeptVP);
            if (model.PrManageItems != null)
            {
                for (int i = 0; i < model.PrManageItems.Count; i++)
                {
                    var item = model.PrManageItems[i];
                    var requestInfo = inn.newItem("B_REQUESTINFO", "add");
                    var PrManageItem = inn.newItem("b_PrManageItem", "add");
                    PrManageItem.setProperty("b_no", (i + 1).ToString());
                    PrManageItem.setProperty("b_requestlist", item.b_RequestList);
                    PrManageItem.setProperty("b_specificationquantity", item.b_SpecificationQuantity);
                    PrManageItem.setProperty("b_projectno", item.b_ProjectNo);
                    PrManageItem.setProperty("b_taskno", item.b_TaskNo);
                    PrManageItem.setProperty("b_qty", item.b_Qty.ToString());
                    PrManageItem.setProperty("b_unit", item.b_Unit);
                    requestInfo.setRelatedItem(PrManageItem);
                    itemRoot.addRelationship(requestInfo);
                }
            }
            return itemRoot;
        }

        /// <summary>
        /// 审核PR单
        /// </summary>
        /// <returns></returns>
        public JsonResult AuditPrManage(PrManageModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //获取路线
                var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(model.activityId);
                WORKFLOW_PROCESS_PATH choicePath = null;
                string lineName = PrManageBll.GetLineNameByActivityName(inn, model.id, model.activityId, model.status, model.b_PrType, model.b_VersionNo);
                if (!string.IsNullOrEmpty(model.status))
                {
                    choicePath = listActivity.Where(x => x.NAME == lineName).FirstOrDefault();
                }
                //替换特殊字符
                WorkFlowBll.ReplaceChars(choicePath);
                var itemRoot = inn.newItem("B_PRMANAGE", "edit");
                itemRoot.setAttribute("id", model.id);

                if (model.status == "Dept.Manager" || model.status == "Dept.Director" || model.status == "PMT/PAT Leader" || model.status == "Project Manager" || model.status == "Project Director")
                {
                    itemRoot.setProperty("b_budgetcode", model.b_BudgetCode);
                }

                if (model.status == "Financial Analyst" || model.status == "Financial Manager" || model.status == "Financial Director")
                {
                    itemRoot.setProperty("b_budgetstatus", model.b_BudgetStatus);
                    itemRoot.setProperty("b_budgetcode", model.b_BudgetCode);
                    itemRoot.setProperty("b_costcenter", model.b_CostCenter);
                }

                if (model.status == "Receive PR")
                {
                    if (string.IsNullOrEmpty(model.b_Buyer))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("必须选择采购员！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    //验证挑选的采购员是否正确
                    var buyer = IdentityDA.GetIdentityByKeyedName(inn, model.b_Buyer);
                    if (!buyer.isError())
                    {
                        model.b_BuyerId = buyer.getProperty("id");
                        List<string> names = new List<string>() { "Buyer Inquiry", "Contract Registration", "Contract Management" };
                        //添加Buyer Inquiry、Contract Registration、Contract Management权限
                        PrManageBll.AddPrManageAuthById(inn, model.id, model.b_BuyerId, names);
                        itemRoot.setProperty("b_buyerid", model.b_BuyerId);
                        itemRoot.setProperty("b_buyer", model.b_Buyer);
                        itemRoot.setProperty("b_authorizedpurchase", model.b_AuthorizedPurchase ? "1" : "0");
                    }
                    else
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("输入的采购员不存在！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                if (model.status == "Buyer Inquiry")
                {
                    //验证数据项的数量不能小于等于0
                    if (model.PrQuotationItems == null || model.PrQuotationItems.Count() <= 0)
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("询价信息必须填写！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                    model.PrQuotationItems = model.PrQuotationItems.Distinct().ToList();
                    if (model.b_RepetitivePurchase)
                    {
                        if (model.PrRepeateItems == null || model.PrRepeateItems.Count() <= 0)
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("当选择重复采购时，重复采购不可为空！", "PRCommon", "PRItemType", Userinfo.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }

                        //验证原采购员
                        foreach (var itemRepeateItem in model.PrRepeateItems)
                        {
                            if (!string.IsNullOrEmpty(itemRepeateItem.b_PreviousBuyer))
                            {
                                var PreviousBuyer = IdentityDA.GetIdentityByKeyedName(inn, itemRepeateItem.b_PreviousBuyer);
                                if (PreviousBuyer.isError() || PreviousBuyer.getItemCount() <= 0)
                                {
                                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("重复采购输入的原采购员不存在！", "PRCommon", "PRItemType", Userinfo.language));
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }

                    //删除询价信息，和重复采购
                    PrManageDA.DeleteQuotationItem(inn, model.id);
                    PrManageDA.DeleteRepeateItem(inn, model.id);
                    itemRoot.setProperty("b_urgentpurchase", model.b_UrgentPurchase ? "1" : "0");
                    itemRoot.setProperty("b_repetitivepurchase", model.b_RepetitivePurchase ? "1" : "0");
                    itemRoot.setProperty("b_issinglesupplier", model.b_IsSingleSupplier ? "1" : "0");

                    for (int i = 0; i < model.PrQuotationItems.Count; i++)
                    {
                        var item = model.PrQuotationItems[i];
                        var quotationItem = inn.newItem("B_QUOTATIONITEM", "add");
                        var prQuotationItem = inn.newItem("B_PRQUOTATIONITEM", "add");
                        prQuotationItem.setProperty("b_no", (i + 1).ToString());
                        prQuotationItem.setProperty("b_supplier", item.b_Supplier);
                        prQuotationItem.setProperty("b_quotation", item.b_Quotation);
                        prQuotationItem.setProperty("b_remarks", item.b_Remarks);
                        quotationItem.setRelatedItem(prQuotationItem);
                        itemRoot.addRelationship(quotationItem);
                    }

                    if (model.PrRepeateItems != null)
                    {
                        for (int i = 0; i < model.PrRepeateItems.Count; i++)
                        {
                            var item = model.PrRepeateItems[i];
                            var repeatedPurchasing = inn.newItem("B_REPEATEDPURCHASING", "add");
                            var prRepeateItem = inn.newItem("B_PRREPEATEITEM", "add");
                            prRepeateItem.setProperty("b_prrecordno", item.b_PrRecordNo);
                            prRepeateItem.setProperty("b_previoussupplier", item.b_PreviousSupplier);
                            prRepeateItem.setProperty("b_contractno", item.b_ContractNo);
                            prRepeateItem.setProperty("b_previousbuyer", item.b_PreviousBuyer);
                            repeatedPurchasing.setRelatedItem(prRepeateItem);
                            itemRoot.addRelationship(repeatedPurchasing);
                        }
                    }
                }

                if (model.status == "Contract Registration")
                {

                    if (model.PrChoiceSupplierItems == null || model.PrChoiceSupplierItems.Count <= 0)
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("必须填写选点供应商信息！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    decimal ContractPrice = 0;
                    for (var i = 0; i < model.PrChoiceSupplierItems.Count; i++)
                    {
                        var item = model.PrChoiceSupplierItems[i];
                        ContractPrice = ContractPrice + decimal.Parse(item.b_ContractPrice);

                        //查询合同号在系统中是否已经存在
                        if (!PrManageBll.CheckedContractNoIsExist(inn, item.b_PoNo, item.id))
                        {
                            retModel.AddError("errorMessage", item.b_PoNo + ":" + Common.GetLanguageValueByParam("在系统中已经存在！", "PRCommon", "PRItemType", Userinfo.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                    ContractPrice = Math.Round(ContractPrice, 2);
                    decimal additionalBudget = 0;
                    if (ContractPrice > model.b_Budget && string.IsNullOrEmpty(model.b_AdditionalBudget))
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("必须填写追加预算金额！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    if (!string.IsNullOrEmpty(model.b_AdditionalBudget))
                    {
                        if (!decimal.TryParse(model.b_AdditionalBudget, out additionalBudget))
                        {
                            retModel.AddError("errorMessage", Common.GetLanguageValueByParam("输入的追加预算有误！", "PRCommon", "PRItemType", Userinfo.language));
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if ((additionalBudget + model.b_Budget) < ContractPrice)
                    {
                        retModel.AddError("errorMessage", Common.GetLanguageValueByParam("总预算不能小于合同价格！", "PRCommon", "PRItemType", Userinfo.language));
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    itemRoot.setProperty("b_additionalbudget", model.b_AdditionalBudget);
                    //删除选择供应商
                    PrManageDA.DeleteChoiceSupplier(inn, model.id);

                    for (var i = 0; i < model.PrChoiceSupplierItems.Count; i++)
                    {
                        var item = model.PrChoiceSupplierItems[i];
                        var choiceSuppliers = inn.newItem("b_ChoiceSuppliers", "add");
                        var prChoiceSuppliers = inn.newItem("b_PrChoiceSuppliers", "add");
                        prChoiceSuppliers.setProperty("b_supplier", item.b_Supplier);
                        prChoiceSuppliers.setProperty("b_contractprice", item.b_ContractPrice);
                        prChoiceSuppliers.setProperty("b_pono", item.b_PoNo);
                        prChoiceSuppliers.setProperty("b_contractproperty", item.b_ContractProperty);
                        prChoiceSuppliers.setProperty("b_paymentclause", item.b_PaymentClause);
                        choiceSuppliers.setRelatedItem(prChoiceSuppliers);
                        itemRoot.addRelationship(choiceSuppliers);
                    }
                }

                if (model.status == "Contract Management")
                {
                    itemRoot.setProperty("b_contracttype", model.b_ContractType);
                }
                string presentRemark = model.b_Remark;
                if (!string.IsNullOrEmpty(model.b_Remark))
                {
                    model.b_Remark = PrManageBll.AddRemark(inn, model.b_Remark, model.id, Userinfo.UserName);
                    itemRoot.setProperty("b_remark", model.b_Remark);
                }
                var result = itemRoot.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }


                //特殊邮件处理的代码   关闭CFO审核时添加，开启时这段代码需隐藏或删除
                Item activitys = null;
                if (model.status == "Financial Director")
                {
                    List<string> names = new List<string>() { "CFO" };
                    activitys = ActivityDA.GetActivityByNames(inn, names, model.id, "innovator.B_PRMANAGE");
                }

                if (choicePath != null)
                {
                    string errorStr = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, choicePath.ID, choicePath.NAME, "", presentRemark);
                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        retModel.AddError("errorMessage", errorStr);
                    }

                    PrManageBll bll = new PrManageBll();
                    bll.AutomaticCompletionTask(inn, model.id, model.b_PrType, model.b_VersionNo, ref choicePath);
                    //判断当前环节是否关闭
                    if (ActivityBll.ActivityIsClosed(inn, choicePath.SOURCE_ID))
                    {
                        //当后续节点任务都是自己的时候
                        if (model.status == "Dept.Manager" || model.status == "Dept.Director" || model.status== "Dept.VP" || model.status=="GM" || model.status == "PMT/PAT Leader" || model.status == "Project Manager")
                        {
                            PrManageBll.AutomaticCompletionTask(inn, model.id, Userinfo, ref choicePath);
                            bll.AutomaticCompletionTask(inn, model.id, model.b_PrType, model.b_VersionNo, ref choicePath);
                        }
                       //PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, choicePath);
                    }
                    if (choicePath != null && model.status != "Financial Director")
                    {
                        PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, choicePath);
                    }

                }

                //当自动完成时  特殊的邮件处理
                //if (model.status == "Dept.Manager" && string.IsNullOrEmpty(model.b_DeptDirector))
                //{
                //    string lineName = model.b_PrType == "project" ? "ProjectPR" : "Dept.PR";
                //    WORKFLOW_PROCESS_PATH DeptManagerChoicePath = WorkFlowBll.AutoCompleteActivityByParam(model.id, "innovator.B_PRMANAGE", lineName);
                //    if (DeptManagerChoicePath != null)
                //    {
                //        PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, DeptManagerChoicePath);
                //    }
                //}
                //特殊邮件处理的代码   关闭CFO审核时添加，开启时这段代码需隐藏或删除
                if (model.status == "Financial Director")
                {
                    if (!activitys.isError())
                    {
                        string id = activitys.getItemByIndex(0).getProperty("id");
                        //获取路线
                        var listPaths = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(id);
                        var cfoChoicePath = listPaths.Where(x => x.NAME == "agree").FirstOrDefault();
                        PrManageBll.SendEmailByOperation(inn, model.b_PrRecordNo, model.b_Applicant, model.b_Buyer, cfoChoicePath);
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
        /// 删除Pr单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeletePrManage(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //删除附件信息
                PrManageDA.DeleteFile(inn, id);

                //删除询价信息
                PrManageDA.DeleteQuotationItem(inn, id);
                //删除重复采购信息
                PrManageDA.DeleteRepeateItem(inn, id);
                //删除选择的供应商信息
                PrManageDA.DeleteChoiceSupplier(inn, id);

                //先获取数据
                var GetrequestInfo = inn.newItem("B_REQUESTINFO", "get");
                GetrequestInfo.setAttribute("where", "source_id='" + id + "'");
                GetrequestInfo.setAttribute("select", "related_id");
                var itemCount = GetrequestInfo.apply();

                string whereItem = "";
                if (itemCount.getItemCount() > 0)
                {
                    for (int i = 0; i < itemCount.getItemCount(); i++)
                    {
                        var item = itemCount.getItemByIndex(i);
                        whereItem += item.getProperty("related_id") + ",";
                    }
                    whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                }
                //删除需求清单
                string amlStr;
                if (whereItem != "")
                {
                    amlStr = "<AML><Item type='B_PRMANAGE' action='delete' id='" + id + "'></Item><Item type='B_PRMANAGEITEM' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                }
                else
                {
                    amlStr = "<AML><Item type='B_PRMANAGE' action='delete' id='" + id + "'></Item></AML>";
                }

                var result = inn.applyAML(amlStr);
                if (!string.IsNullOrEmpty(result.getErrorString()))
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
        /// 获取采购申请单 根据ID
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPrManageById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result = PrManageBll.GetPrManageObjById(inn, id);

                if (!string.IsNullOrEmpty(result.getErrorString()))
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                //拆解数据
                PrManageModel model = new PrManageModel();
                model.id = result.getProperty("id");
                model.b_PrRecordNo = result.getProperty("b_prrecordno");
                model.b_PrType = result.getProperty("b_prtype");
                model.b_BusinessDepartment = result.getProperty("b_businessdepartment");
                model.b_Budget = decimal.Parse(result.getProperty("b_budget"));
                model.b_ApplicantId = result.getProperty("b_applicantid");
                model.b_Applicant = result.getProperty("b_applicant");
                model.b_RaisedDate = DateTime.Parse(result.getProperty("b_raiseddate")).ToString("yyyy-MM-dd");
                model.b_EmailAddress = result.getProperty("b_emailaddress");
                model.b_PhoneNo = result.getProperty("b_phoneno");
                model.b_ProjectName = result.getProperty("b_projectname");
                model.b_BudgetCode = result.getProperty("b_budgetcode");
                model.b_ProjectLeader = result.getProperty("b_projectleader");
                model.b_ProjectManager = result.getProperty("b_projectmanager");
                model.b_ProjectDirector = result.getProperty("b_projectdirector");
                model.b_BudgetStatus = result.getProperty("b_budgetstatus");
                model.b_BuyerId = result.getProperty("b_buyerid");
                model.b_Buyer = result.getProperty("b_buyer");
                model.b_UrgentPurchase = result.getProperty("b_urgentpurchase") == "0" ? false : true;
                model.b_RepetitivePurchase = result.getProperty("b_repetitivepurchase") == "0" ? false : true;
                model.b_AuthorizedPurchase = result.getProperty("b_authorizedpurchase") == "0" ? false : true;
                model.b_PurchaseContent = result.getProperty("b_purchasecontent");
                model.b_ContractParty = result.getProperty("b_contractparty");
                model.b_ApplicantAddress = result.getProperty("b_applicantaddress");
                model.b_IsSingleSupplier = result.getProperty("b_issinglesupplier") == "0" ? false : true;
                model.b_ContractType = result.getProperty("b_contracttype");
                model.b_DeptManager = result.getProperty("b_deptmanager");
                model.b_DeptDirector = result.getProperty("b_deptdirector");
                model.b_CostCenter = result.getProperty("b_costcenter");
                model.b_PurchasingReason = result.getProperty("b_purchasingreason");
                model.b_AdditionalBudget = result.getProperty("b_additionalbudget");
                model.b_VersionNo = result.getProperty("b_versionno");
                model.b_DeptVP = result.getProperty("b_deptvp");
                model.UserName = Userinfo.UserName;
                model.status = Common.GetItemStatus(id);
                model.Remark = GetRemarkByPrHistory(id) + result.getProperty("b_remark");


                Item Relation = result.getRelationships("b_RequestInfo");
                if (Relation.getItemCount() > 0)
                {
                    model.PrManageItems = new List<PrManageItem>();
                    for (int i = 0; i < Relation.getItemCount(); i++)
                    {
                        Item ItemObJ = Relation.getItemByIndex(i).getRelatedItem();
                        PrManageItem itemModel = new PrManageItem();
                        itemModel.id = ItemObJ.getProperty("id");
                        itemModel.b_No = ItemObJ.getProperty("b_no");
                        itemModel.b_RequestList = ItemObJ.getProperty("b_requestlist");
                        itemModel.b_SpecificationQuantity = ItemObJ.getProperty("b_specificationquantity");
                        itemModel.b_ProjectNo = ItemObJ.getProperty("b_projectno");
                        itemModel.b_TaskNo = ItemObJ.getProperty("b_taskno");
                        itemModel.b_Qty = int.Parse(ItemObJ.getProperty("b_qty"));
                        itemModel.b_Unit = ItemObJ.getProperty("b_unit");
                        model.PrManageItems.Add(itemModel);
                    }
                }
                List<string> listRoleName = IdentityDA.GetIdentityByUserName(Userinfo.UserName);
                model.IsPurchasingAuth = false;
                if (model.UserName == model.b_Buyer || listRoleName.Contains("采购员") || listRoleName.Contains("采购部接收PR") || listRoleName.Contains("PRReader") || Userinfo.LoginName == "admin")
                {
                    model.IsPurchasingAuth = true;
                    //询价信息
                    Item quotationRelation = result.getRelationships("b_QuotationItem");
                    if (quotationRelation.getItemCount() > 0)
                    {
                        model.PrQuotationItems = new List<PrQuotationItem>();
                        for (int i = 0; i < quotationRelation.getItemCount(); i++)
                        {
                            Item itemObj = quotationRelation.getItemByIndex(i).getRelatedItem();
                            PrQuotationItem itemModel = new PrQuotationItem();
                            itemModel.id = itemObj.getProperty("id");
                            itemModel.b_Supplier = itemObj.getProperty("b_supplier");
                            itemModel.b_Quotation = itemObj.getProperty("b_quotation");
                            itemModel.b_Remarks = itemObj.getProperty("b_remarks");
                            model.PrQuotationItems.Add(itemModel);
                        }
                    }

                    //获取重复信息
                    Item repeatedPurchasing = result.getRelationships("b_RepeatedPurchasing");
                    if (repeatedPurchasing.getItemCount() > 0)
                    {
                        model.PrRepeateItems = new List<PrRepeateItem>();
                        for (int i = 0; i < repeatedPurchasing.getItemCount(); i++)
                        {
                            Item itemObj = repeatedPurchasing.getItemByIndex(i).getRelatedItem();
                            PrRepeateItem itemModel = new PrRepeateItem();
                            itemModel.id = itemObj.getProperty("id");
                            itemModel.b_PrRecordNo = itemObj.getProperty("b_prrecordno");
                            itemModel.b_PreviousSupplier = itemObj.getProperty("b_previoussupplier");
                            itemModel.b_ContractNo = itemObj.getProperty("b_contractno");
                            //itemModel.b_ContractPrice = itemObj.getProperty("b_contractprice");
                            itemModel.b_PreviousBuyer = itemObj.getProperty("b_previousbuyer");
                            model.PrRepeateItems.Add(itemModel);
                        }
                    }

                    //获取挑选的供应商
                    Item b_ChoiceSuppliers = result.getRelationships("b_ChoiceSuppliers");
                    if (b_ChoiceSuppliers.getItemCount() > 0)
                    {
                        model.PrChoiceSupplierItems = new List<PrChoiceSupplierItem>();
                        for (int i = 0; i < b_ChoiceSuppliers.getItemCount(); i++)
                        {
                            Item itemObj = b_ChoiceSuppliers.getItemByIndex(i).getRelatedItem();
                            PrChoiceSupplierItem itemModel = new PrChoiceSupplierItem();
                            itemModel.id = itemObj.getProperty("id");
                            itemModel.b_Supplier = itemObj.getProperty("b_supplier");
                            itemModel.b_ContractPrice = itemObj.getProperty("b_contractprice");
                            itemModel.b_PoNo = itemObj.getProperty("b_pono");
                            itemModel.b_ContractProperty = itemObj.getProperty("b_contractproperty");
                            itemModel.b_PaymentClause = itemObj.getProperty("b_paymentclause");
                            model.PrChoiceSupplierItems.Add(itemModel);
                        }
                    }
                }

                //获取文件信息
                Item nPrManageFiles = result.getRelationships("b_PrManageFiles");
                if (nPrManageFiles.getItemCount() > 0)
                {
                    model.Files = new List<FileModel>();
                    for (int i = 0; i < nPrManageFiles.getItemCount(); i++)
                    {
                        Item itemObj = nPrManageFiles.getItemByIndex(i).getRelatedItem();
                        FileModel itemModel = new FileModel();
                        itemModel.id = itemObj.getProperty("id");
                        itemModel.fileName = itemObj.getProperty("filename");
                        itemModel.source_id = nPrManageFiles.getItemByIndex(i).getProperty("source_id");
                        itemModel.relationId = nPrManageFiles.getItemByIndex(i).getProperty("id");
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
        /// 获取日志信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPrManageHistoryList(DataTableParameter para, string item_Id)
        {
            int total = 0;
            List<HistoryModel> listDatas = GetPrManageHistoryList(out total, para, item_Id);
            foreach (var model in listDatas)
            {

                model.Create_onStr = model.Create_onStr.GetValueOrDefault().AddHours(8);
                model.Created_on = model.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                model.ItemStatus = Common.GetChineseValueByParam(model.ItemStatus, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
                model.OperateStr = Common.GetChineseValueByParam(model.OperateStr, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
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

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="total"></param>
        /// <param name="para"></param>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        private static List<HistoryModel> GetPrManageHistoryList(out int total, DataTableParameter para, string item_Id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_PRMANAGE
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
        /// 获取备注信息
        /// </summary>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        public static string GetRemarkByPrHistory(string item_Id)
        {
            string remark = "";
            List<HistoryModel> list = new List<HistoryModel>();
            DateTime endTime = DateTime.Parse("2017-11-02 18:35:00");
            endTime = endTime.AddHours(-8);
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_PRMANAGE
                                                  join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                  join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                  join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                  join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                  join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                  join p in db.USER on o.CLOSED_BY equals p.ID
                                                  where g.id == item_Id && i.STATE == "Closed" && o.COMMENTS != "" && o.CLOSED_ON < endTime
                                                  select new HistoryModel
                                                  {
                                                      OperateStr = o.PATH,
                                                      Comments = o.COMMENTS,
                                                      Create_onStr = o.CLOSED_ON,
                                                      CreateName = p.KEYED_NAME,
                                                      ItemStatus = i.KEYED_NAME
                                                  });
                datas = Common.OrderBy(datas, "Create_onStr", false);
                list = datas.ToList();
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        HistoryModel model = list[i];
                        remark += model.CreateName + ":" + model.Comments + "<br/>";
                    }
                }
                return remark;
            }
        }




        /// <summary>
        /// 根据项目名称获取项目
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult GetProjectManageByName(string name)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var result = ProjectManageDA.GetProjectManageByName(inn, name);
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                ProjectManageModel model = new ProjectManageModel();
                model.id = result.getProperty("id");
                model.b_ProjectName = result.getProperty("b_projectname");
                model.b_PmtOrPatLeaderId = result.getProperty("b_pmtorpatleaderid");
                model.b_PmtOrPatLeader = result.getProperty("b_pmtorpatleader");
                model.b_ProjectDirectorId = result.getProperty("b_projectdirectorid");
                model.b_ProjectDirector = result.getProperty("b_projectdirector");
                model.b_ProjectManagerId = result.getProperty("b_projectmanagerid");
                model.b_ProjectManager = result.getProperty("b_projectmanager");
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据编号查询PR单
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPrManageByContractNo(string contractNo)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result = PrManageDA.GetPrManageByContractNo(inn, contractNo.Trim());
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                PrRepeateItem model = new PrRepeateItem();
                model.id = result.getProperty("id");
                model.b_PrRecordNo = result.getProperty("b_prrecordno");
                model.b_PreviousSupplier = result.getProperty("b_supplier");
                model.b_PreviousBuyer = result.getProperty("b_buyer");
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载合同
        /// </summary>
        /// <param name="contractType"></param>
        /// <returns></returns>
        public FilePathResult DownloadContract(string contractType, string id)
        {
            string filePath;
            string path;
            Item model = PrManageBll.GetPrManageObjById(inn, id);
            string b_BuyerId = model.getProperty("b_buyerid");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string buyerEmail = "";

            //获取采购员邮箱
            var item = UserDA.GetUserByIdentityId(inn, b_BuyerId);
            if (!item.isError())
            {
                buyerEmail = item.getProperty("email");
            }

            if (contractType == "Purchase Order")
            {
                filePath = baseDirectory + @"Template\采购订单模板.doc";
                path = ExportPrContract.ExportPurchaseContract(filePath, model, buyerEmail);
            }
            else
            {
                filePath = baseDirectory + @"Template\内部合同模板.doc";
                path = ExportPrContract.ExportInternalContract(filePath, model, buyerEmail);
            }
            var file = new FileInfo(path);
            if (file.Exists)
            {
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=" + HttpUtility.UrlEncode(file.Name));
                return File(file.FullName, "application/ms-word");
            }
            return null;
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UploadPrManageFile(string id, string status)
        {
            var retModel = new JsonReturnModel();
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    status = "Start";
                }

                FileModel file = new FileModel();
                if (Request.Files == null || Request.Files.Count == 0)
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("请选择您要上传的附件！", "PRCommon", "PRItemType", Userinfo.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                string fileName;
                HttpPostedFileBase prfile = Request.Files[0];
                fileName = prfile.FileName.Substring(prfile.FileName.LastIndexOf("\\") + 1, prfile.FileName.Length - (prfile.FileName.LastIndexOf("\\")) - 1);
                string filePath = ConfigurationManager.AppSettings["UploadPath"] + fileName;
                prfile.SaveAs(filePath);
                Item fileItem = inn.newItem("File", "add");
                fileItem.attachPhysicalFile(filePath);
                fileItem.setProperty("filename", fileName);
                fileItem.setProperty("comments", status);

                if (!string.IsNullOrEmpty(id))
                {
                    var prManageFile = inn.newItem("b_PrManageFiles", "add");
                    prManageFile.setProperty("source_id", id);
                    prManageFile.setRelatedItem(fileItem);
                    prManageFile = prManageFile.apply();
                    if (!prManageFile.isError())
                    {
                        Item item = prManageFile.getRelatedItem();
                        file.id = item.getProperty("id");
                        file.relationId = prManageFile.getProperty("id");
                        file.fileName = fileItem.getProperty("filename");
                        file.mimeType = item.getProperty("mimetype");
                        retModel.data = file;
                    }
                    else
                    {
                        retModel.AddError("errorMessage", prManageFile.getErrorString());
                    }

                }
                else
                {
                    fileItem = fileItem.apply();
                    if (!fileItem.isError())
                    {
                        file.id = fileItem.getProperty("id");
                        file.fileName = fileItem.getProperty("filename");
                        file.mimeType = fileItem.getProperty("mimetype");
                        retModel.data = file;
                    }
                    else
                    {
                        retModel.AddError("errorMessage", fileItem.getErrorString());
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
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeletePrManageFile(string id, string relationId)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item fileItem = inn.newItem("File", "delete");
                fileItem.setAttribute("id", id);
                if (!string.IsNullOrEmpty(relationId) && relationId != "null")
                {
                    Item prManageFileItem = inn.newItem("b_PrManageFiles", "delete");
                    prManageFileItem.setAttribute("id", relationId);
                    prManageFileItem.setRelatedItem(fileItem);
                    prManageFileItem.apply();
                    if (prManageFileItem.isError())
                    {
                        retModel.AddError("errorMessage", prManageFileItem.getErrorString());
                    }
                }
                else
                {
                    fileItem = fileItem.apply();
                    if (fileItem.isError())
                    {
                        retModel.AddError("errorMessage", fileItem.getErrorString());
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
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        public FilePathResult DownloadAttachment(string id)
        {
            Item item = inn.newItem("File", "get");
            item.setAttribute("id", id);
            Item file = item.apply();
            string checkout_path = "";
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];
            if (!file.isError())
            {
                Item result = file.checkout(downloadPath);
                checkout_path = result.getProperty("checkedout_path");
            }
            if (!string.IsNullOrEmpty(checkout_path))
            {
                string contentType = MimeMapping.GetMimeMapping(checkout_path);
                var fileOBJ = new FileInfo(checkout_path);
                if (fileOBJ.Exists)
                {
                    Response.AddHeader("Content-Disposition",
                                  "attachment; filename=" + HttpUtility.UrlEncode(fileOBJ.Name));
                    return File(fileOBJ.FullName, contentType);
                }
            }
            return null;
        }


        public JsonResult GetWorkflowProcessPathByActivityId(string activityId, string b_PrType, string b_DeptManager, string b_DeptDirector)
        {

            var retModel = new JsonReturnModel();
            try
            {
                List<WORKFLOW_PROCESS_PATH> list = new List<WORKFLOW_PROCESS_PATH>();
                var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(activityId);
                if (listActivity != null && listActivity.Count > 0)
                {
                    for (var i = 0; i < listActivity.Count; i++)
                    {
                        var item = listActivity[i];
                        string ActivityId = item.RELATED_ID;
                        if (b_PrType == "Non Project")
                        {
                            if (item.NAME.Contains("Return") && item.NAME != "ReturnProjectDirector" && item.NAME != "ReturnPMT/PATLeader" && item.NAME != "ReturnProjectManager" && item.NAME != "ReturnCFO")
                            {
                                if ((item.NAME == "ReturnDept.Manager" && string.IsNullOrEmpty(b_DeptManager)) || (item.NAME == "ReturnDept.Director" && string.IsNullOrEmpty(b_DeptDirector)))
                                {
                                    continue;
                                }

                                //判断当前环节点是否存在审核人
                                var result= ActivityAssignmentDA.GetActivityAssignment(inn, ActivityId);
                                if(!result.isError() && result.getItemCount()>0)
                                {
                                    item.NAME = Common.GetChineseValueByParam(item.NAME, "PrManageWorkFlow", "WorkFlow", ViewBag.language);
                                    list.Add(item);
                                }
                            }
                        }
                        else
                        {
                            if (item.NAME.Contains("Return") && item.NAME != "ReturnCFO" && item.NAME!= "ReturnDept.Manager" && item.NAME!= "ReturnDept.Director" && item.NAME!= "ReturnDept.VP")
                            {
                                if ((item.NAME == "ReturnDept.Manager" && string.IsNullOrEmpty(b_DeptManager)) || (item.NAME == "ReturnDept.Director" && string.IsNullOrEmpty(b_DeptDirector)))
                                {
                                    continue;
                                }
                                //判断当前环节点是否存在审核人
                                var result = ActivityAssignmentDA.GetActivityAssignment(inn, ActivityId);
                                if (!result.isError() && result.getItemCount() > 0)
                                {
                                    item.NAME = Common.GetChineseValueByParam(item.NAME, "PrManageWorkFlow", "WorkFlow", ViewBag.language);
                                    list.Add(item);
                                }
                            }
                        }
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


        //获取查询Select流程状态数据
        public JsonResult GetWorkflowStatusList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<SelectModel> list = new List<SelectModel>();
                var result = WorkFlowBll.GetWorkflowStatusList(inn, "PrManageWorkFlowVersion");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (int i = 0; i < result.getItemCount(); i++)
                    {
                        SelectModel model = new SelectModel();
                        var item = result.getItemByIndex(i);
                        model.value = item.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(model.value))
                        {
                            model.text = Common.GetChineseValueByParam(model.value, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
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
        /// 获取合同编号
        /// </summary>
        /// <param name="b_Buyer"></param>
        /// <returns></returns>
        public JsonResult GetContractPoNo(string b_Buyer)
        {
            var retModel = new JsonReturnModel();
            try
            {
                string contractNo = PrManageBll.CreatePoNo(inn, b_Buyer);

                retModel.data = contractNo;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取流程的审批人数
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public JsonResult GetAuditActivityCount(string activityId)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var item = ActivityAssignmentDA.GetActivityAssignment(inn, activityId);
                if (!item.isError())
                {
                    retModel.data = item.getItemCount();
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