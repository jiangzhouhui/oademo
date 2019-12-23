using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Filter;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers
{
    [PageAuthorizationFilter]
    public class WorkFlowController : BaseController
    {
        // GET: WorkFlow
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 根据数据ID获取工作流
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public JsonResult GetWorkFlowByDataId(string dataId)
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<ACTIVITY> list = WorkFlowDA.GetWorkFlowActivityByOrderId(dataId);
                List<ActivityModel> activityList = new List<ActivityModel>();
                foreach (var item in list)
                {
                    ActivityModel model = new ActivityModel();
                    model.Id = item.ID;
                    model.Keyed_Name = item.KEYED_NAME;
                    model.X = item.X;
                    model.Y = item.Y;
                    List<WORKFLOW_PROCESS_PATH> processPaths = WorkFlowDA.GetProcessPathBySourceId(item.ID);
                    List<ProcessPathModel> listPaths = new List<ProcessPathModel>();
                    foreach (var processPath in processPaths)
                    {
                        ProcessPathModel modelPath = new ProcessPathModel();
                        modelPath.Id = processPath.ID;
                        modelPath.Lable = processPath.LABEL;
                        modelPath.segments = processPath.SEGMENTS;
                        modelPath.X = processPath.X;
                        modelPath.Y = processPath.Y;
                        //获取到达节点的坐标
                        ACTIVITY relatedModel = list.Where(x => x.ID == processPath.RELATED_ID).FirstOrDefault();
                        if (relatedModel != null)
                        {
                            modelPath.RelatedX = relatedModel.X;
                            modelPath.RelatedY = relatedModel.Y;
                        }
                        listPaths.Add(modelPath);
                    }
                    model.ProcessPaths = listPaths;
                    activityList.Add(model);
                }
                retModel.data = activityList;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取流程图，根据流程图名称
        /// </summary>
        /// <param name="keyedName"></param>
        /// <returns></returns>
        public JsonResult GetWorkFlowMapByKeyedName(string keyedName)
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<ACTIVITY_TEMPLATE> list = WorkFlowDA.GetWorkFlowMapActivityByKeyedName(keyedName);
                List<ActivityModel> activityList = new List<ActivityModel>();
                foreach (var item in list)
                {
                    ActivityModel model = new ActivityModel();
                    model.Id = item.ID;
                    model.Keyed_Name = item.KEYED_NAME;
                    model.X = item.X;
                    model.Y = item.Y;
                    List<WORKFLOW_MAP_PATH> processPaths = WorkFlowDA.GetWorkMapPathBySourceId(item.ID);
                    List<ProcessPathModel> listPaths = new List<ProcessPathModel>();
                    foreach (var processPath in processPaths)
                    {
                        ProcessPathModel modelPath = new ProcessPathModel();
                        modelPath.Id = processPath.ID;
                        modelPath.Lable = processPath.NAME;
                        modelPath.segments = processPath.SEGMENTS;
                        modelPath.X = processPath.X;
                        modelPath.Y = processPath.Y;
                        //获取到达节点的坐标
                        ACTIVITY_TEMPLATE relatedModel = list.Where(x => x.ID == processPath.RELATED_ID).FirstOrDefault();
                        if (relatedModel != null)
                        {
                            modelPath.RelatedX = relatedModel.X;
                            modelPath.RelatedY = relatedModel.Y;
                        }
                        listPaths.Add(modelPath);
                    }
                    model.ProcessPaths = listPaths;
                    activityList.Add(model);
                }
                retModel.data = activityList;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据活动ID 获取路线
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkflowProcessPathByActivityId(string activityId)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(activityId);
                //listActivity = listActivity.Where(x => x.NAME == "").ToList();
                //if (listActivity != null)
                //{
                //    for (var i = 0; i < listActivity.Count(); i++)
                //    {
                //        WORKFLOW_PROCESS_PATH obj = listActivity[i];

                //        Item activityItem = ActivityDA.GetActivityById(inn, obj.RELATED_ID);
                //        if (!activityItem.isError())
                //        {
                //            string name = activityItem.getProperty("keyed_name");
                //            obj.NAME = "returnTo" + name;
                //        }
                //    }
                //}
                retModel.data = listActivity;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult CompleteActivity(CompleteActivityModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                WORKFLOW_PROCESS_PATH path = new WORKFLOW_PROCESS_PATH();
                Item itemPrManage = inn.newItem("B_PRMANAGE", "get");
                itemPrManage.setAttribute("id", model.itemId);
                itemPrManage = itemPrManage.apply();
                string prRecordNo = itemPrManage.getProperty("b_prrecordno");
                string b_BusinessDepartment = itemPrManage.getProperty("b_businessdepartment");
                string b_Applicant = itemPrManage.getProperty("b_applicant");

                if (string.IsNullOrEmpty(model.comments))
                {
                    retModel.AddError("errorMessage", @OABordrinCommon.Common.GetLanguageValueByParam("退回必须填写备注！", "ERCommon", "ERItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                //获取当前退回人员权重
                var ActivityAssignmentItem = ActivityAssignmentDA.GetActivityAssignmentById(inn, model.activityAssignmentId);
                if (!ActivityAssignmentItem.isError() && ActivityAssignmentItem.getItemCount() > 0)
                {
                    int voting_weight = int.Parse(ActivityAssignmentItem.getItemByIndex(0).getProperty("voting_weight"));
                    //修改权重
                    if (voting_weight < 100)
                    {
                        //获取Admin的Aras 连接
                        var adminInn = WorkFlowBll.GetAdminInnovator();
                        if (adminInn != null)
                        {
                            //获取Admin 对当前任务权限数据
                            Item activityItem = ActivityDA.GetActivityByItemId(adminInn, model.itemId, "Administrators", "B_PrManage");
                            if (!activityItem.isError())
                            {
                                string activityId = activityItem.getProperty("activityid");
                                string activityAssignmentId = activityItem.getProperty("activityassignmentid");
                                ActivityDA.CompleteActivity(adminInn, activityId, activityAssignmentId, model.pathId, "", "", Userinfo.UserName + "对单据进行了退回操作！ 备注：" + model.comments);
                            }
                        }
                    }
                    else
                    {
                        string result = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, model.pathId, "", "", model.comments);
                        if (!string.IsNullOrEmpty(result))
                        {
                            retModel.AddError("errorMessage", result);
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                path = WorkflowProcessPathDA.GetWorkflowProcessPathById(model.pathId);
                PrManageBll.SendEmailByOperation(inn, prRecordNo, b_Applicant, "", path, model.itemId);

                string b_remark = PrManageBll.AddRemark(inn, model.comments, model.itemId, Userinfo.UserName);
                var itemRoot = inn.newItem("B_PRMANAGE", "edit");
                itemRoot.setAttribute("id", model.itemId);
                itemRoot.setProperty("b_remark", b_remark);
                itemRoot = itemRoot.apply();

            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 保存转办
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveWorkflowTurnToDo(CompleteActivityModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var user = UserDA.GetUserByFirstName(model.delegateToName);
                if (user == null)
                {
                    retModel.AddError("errorMessage", "选择的转办不存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if (model.delegateToName == Userinfo.UserName)
                {
                    retModel.AddError("errorMessage", "转办人不能选择自己！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                IDENTITY dentity = IdentityDA.GetIdentityByActivityAssignmentId(model.activityAssignmentId);
                string identityName = dentity.KEYED_NAME;
                if (identityName.Trim() != Userinfo.UserName)
                {
                    retModel.AddError("errorMessage", "代理权限不能进行转办！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                string errorString = ActivityDA.CompleteActivity(inn, model.activityId, model.activityAssignmentId, "", "Delegate", model.delegateToName, Userinfo.UserName + "转办到：" + model.delegateToName, Userinfo);
                if (string.IsNullOrEmpty(errorString))
                {
                    WorkFlowBll.TurnToDoSendEmail(Userinfo.UserName, model.delegateToName, model.recordNo, model.linkStr, user.EMAIL);
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存加签
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveWorkflowActivitySign(CompleteActivityModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> authIds = new List<string>();
                List<string> oldAuthIds = new List<string>();
                List<string> addAuthIds = new List<string>();

                List<string> listEmail = new List<string>();

                //验证输入的加签用户是否存在！并且获取IdentityId;
                List<string> listName = model.PersonList.Split(';').Where(x => x != "" && x != null).Select(x => x.Trim()).Distinct().ToList();
                if (listName != null && listName.Count() > 0)
                {
                    for (int i = 0; i < listName.Count; i++)
                    {
                        string textValue = listName[i];
                        USER user = UserDA.GetUserByFirstName(textValue);
                        if (user == null)
                        {
                            retModel.AddError("errorMessage", "输入的人员在系统中不存在！");
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            listEmail.Add(user.EMAIL);

                            Item identity = IdentityDA.GetIdentityByKeyedName(inn, textValue);
                            if (!identity.isError() && identity.getItemCount() > 0)
                            {
                                string identityId = identity.getProperty("id");
                                authIds.Add(identityId);
                            }
                        }
                    }
                }
                //获取当前活动的名称
                Item activity = ActivityDA.GetActivityById(inn, model.activityId);
                string keyedName = activity.getProperty("keyed_name").Trim();
                List<string> activityNames = new List<string> { keyedName };


                //获取现有的审核权限
                Item activityAssignments = ActivityAssignmentDA.GetActivityAssignment(inn, model.activityId);
                if (!activityAssignments.isError() && activityAssignments.getItemCount() > 0)
                {
                    for (int i = 0; i < activityAssignments.getItemCount(); i++)
                    {
                        Item activityAssignmentItem = activityAssignments.getItemByIndex(i);
                        string id = activityAssignmentItem.getProperty("id");
                        string related_id = activityAssignmentItem.getProperty("related_id");
                        oldAuthIds.Add(related_id);
                        //删除现有审核权限
                        ActivityAssignmentDA.deleteActivityAssignment(inn, id);
                    }
                }

                //添加该活动的审核权限
                addAuthIds.AddRange(authIds);
                addAuthIds.AddRange(oldAuthIds);
                addAuthIds = addAuthIds.Distinct().ToList();
                if (addAuthIds.Count > 0)
                {
                    int voting_weight = Common.CalculationWeight(addAuthIds.Count);
                    foreach (var id in addAuthIds)
                    {
                        ActivityBll.AddActivityAuth(inn, model.itemId, id, activityNames, model.operateTable, voting_weight, "Active");
                    }
                }

                //加签成功发送邮件
                string nameStr = "";
                listEmail = listEmail.Distinct().ToList();
                if (listName != null && listName.Count > 0)
                {
                    for (int i = 0; i < listName.Count; i++)
                    {
                        if (i != listName.Count - 1)
                        {
                            nameStr += listName[i] + "、";
                        }
                        else
                        {
                            nameStr += listName[i];
                        }
                    }
                }
                WorkFlowBll.WorkflowActivitySignSendEmail(Userinfo.UserName, nameStr, model.recordNo, model.linkStr, listEmail);
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }




    }
}