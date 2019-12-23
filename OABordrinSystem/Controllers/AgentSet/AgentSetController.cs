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

namespace OABordrinSystem.Controllers.AgentSet
{
    public class AgentSetController : BaseController
    {
        // GET: AgentSet
        public ActionResult Index()
        {
            ViewBag.UserName = Userinfo.UserName;
            ViewBag.CurrentName = "代理设置";
            return View();
        }

        /// <summary>
        /// 获取代理设置列表
        /// </summary>
        /// <param name="para"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public JsonResult GetAgentSetList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            var dataList = GetAgentSetList(Userinfo,out total, para, searchValue);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-8'>{0}</div><div class='col-md-4' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' Id='" + item.id + "'></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.id + "' ></a>";
                    strHtml = string.Format(strHtml, item.b_DelegateName, linkAList);
                    item.b_DelegateName = strHtml;
                    if (!string.IsNullOrEmpty(item.b_StartDateMinute))
                    {
                        item.b_StartDate = item.b_StartDate + " " + item.b_StartDateMinute;
                    }

                    if (!string.IsNullOrEmpty(item.b_EndDateMinute))
                    {
                        item.b_EndDate = item.b_EndDate + " " + item.b_EndDateMinute;
                    }
                    item.AgentModuleList = new List<string>(item.b_AgentContent.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                    if (item.AgentModuleList.Count > 0)
                    {
                        string agentContext = "";
                        foreach (var agentModule in item.AgentModuleList)
                        {
                            string value = EnumDescription.GetFieldText(EnumDescription.GetEnumByTextUseDefault<AgentModule>(agentModule));
                            agentContext = agentContext + value + ";";
                        }
                        item.b_AgentContent = agentContext;
                    }
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

        public static List<AgentSetModel> GetAgentSetList(UserInfo userInfo, out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<AgentSetModel> datas = (from g in db.B_AGENTSET
                                                   where g.B_DELEGATENAME== userInfo.UserName && g.B_ISVALID=="1" &&  (g.B_DELEGATENAME.Contains(searchValue) || g.B_AGENTNAME.Contains(searchValue) || g.B_AGENTCONTENT.Contains(searchValue))
                                                   select new AgentSetModel()
                                                   {
                                                       id = g.id,
                                                       b_DelegateName = g.B_DELEGATENAME,
                                                       b_AgentName = g.B_AGENTNAME,
                                                       b_StartDate =g.B_STARTDATE ,
                                                       b_StartDateMinute = g.B_STARTDATEMINUTE,
                                                       b_EndDate = g.B_ENDDATE,
                                                       b_EndDateMinute = g.B_ENDDATEMINUTE,
                                                       b_AgentContent = g.B_AGENTCONTENT,
                                                       b_AgentReason = g.B_AGENTREASON
                                                   });
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
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveAgentSet(AgentSetModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //限制输入的代理人不能为自己
                if(model.b_AgentName==Userinfo.UserName)
                {
                    retModel.AddError("errorMessage", "代理人选择不能为自己！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                var b_AgentName = IdentityDA.GetIdentityByKeyedName(inn, model.b_AgentName);
                if (b_AgentName.isError())
                {
                    retModel.AddError("errorMessage", "找不到对应的代理人！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                if(model.AgentModuleList==null || model.AgentModuleList.Count==0)
                {
                    retModel.AddError("errorMessage", "请您选择授权模块！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                string value = string.Join(";", model.AgentModuleList.ToArray()) + ";" ;
                model.b_AgentContent = value;

                //验证输入的时间是否正确
                string b_StartDate = !string.IsNullOrEmpty(model.b_StartDateMinute) ? model.b_StartDate + " " + model.b_StartDateMinute : model.b_StartDate;
                string b_EndDate = !string.IsNullOrEmpty(model.b_EndDateMinute) ? model.b_EndDate + " " + model.b_EndDateMinute : model.b_EndDate;

                DateTime StartDate;
                if (!DateTime.TryParse(b_StartDate, out StartDate))
                {
                    retModel.AddError("errorMessage", "输入的生效时间错误！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                DateTime EndDate;
                if (!DateTime.TryParse(b_EndDate, out EndDate))
                {
                    retModel.AddError("errorMessage", "输入的终止时间错误！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                Item item = null;
                if (string.IsNullOrEmpty(model.id))
                {
                    item = inn.newItem("b_AgentSet", "add");
                }
                else
                {
                    item = inn.newItem("b_AgentSet", "edit");
                    item.setAttribute("id", model.id);
                }
                item.setProperty("b_delegatename", model.b_DelegateName);
                item.setProperty("b_agentname", model.b_AgentName);
                item.setProperty("b_startdate", model.b_StartDate);
                item.setProperty("b_startdateminute", model.b_StartDateMinute);
                item.setProperty("b_enddate", model.b_EndDate);
                item.setProperty("b_enddateminute", model.b_EndDateMinute);
                item.setProperty("b_agentcontent", model.b_AgentContent);
                item.setProperty("b_isvalid", model.b_IsValid);
                item.setProperty("b_agentreason", model.b_AgentReason);
                var result = item.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                }
                else
                {
                    if(model.b_IsValid=="1")
                    {
                        string agentContext = "";
                        if (model.AgentModuleList.Count > 0)
                        {
                          
                            foreach (var agentModule in model.AgentModuleList)
                            {
                                string moduleStr = EnumDescription.GetFieldText(EnumDescription.GetEnumByTextUseDefault<AgentModule>(agentModule));
                                agentContext = agentContext + moduleStr + ";";
                            }
                        }
                        WorkFlowBll.SendAgentEmail(inn, model.b_DelegateName, model.b_AgentName, StartDate, EndDate, agentContext);
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
        /// 根据ID获取代理设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetAgentSetById(string id,string type)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var bootItem = inn.newItem("b_AgentSet", "get");
                bootItem.setAttribute("id", id);
                Item result = bootItem.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                AgentSetModel model = new AgentSetModel();
                model.id = result.getProperty("id");
                model.b_DelegateName = result.getProperty("b_delegatename");
                model.b_AgentName = result.getProperty("b_agentname");
                model.b_EndDateMinute = result.getProperty("b_enddateminute");
                model.b_StartDateMinute = result.getProperty("b_startdateminute");
                model.b_StartDate = result.getProperty("b_startdate");
                model.b_EndDate = result.getProperty("b_enddate");
                if (type== "detail")
                {
                    string startDate = model.b_StartDate + " " + model.b_StartDateMinute;
                    string endDate = model.b_EndDate + " " + model.b_EndDateMinute;
                    model.b_StartDate = DateTime.Parse(startDate).ToString("yyyy-MM-dd hh:mm:ss");
                    model.b_EndDate = DateTime.Parse(endDate).ToString("yyyy-MM-dd hh:mm:ss");
                }

                model.b_AgentContent = result.getProperty("b_agentcontent");
                model.b_AgentReason = result.getProperty("b_agentreason");
                model.b_IsValid = result.getProperty("b_isvalid");
                model.AgentModuleList = new List<string>(model.b_AgentContent.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取授权模块列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAgentModuleList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                var enumDescriptions = EnumDescription.GetFieldTexts<AgentModule>(true);
                var selectList = new List<SelectListItem>();

                selectList.AddRange(
                      enumDescriptions.ToArray().Select(
                          m => new SelectListItem() { Text = m.EnumDisplayText, Value = m.FieldName }));
                retModel.data = selectList;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除数据根据Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteAgentSetById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var result = AgentSetDA.DeleteAgentSetById(inn, id);
                if (result.isError())
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






    }
}