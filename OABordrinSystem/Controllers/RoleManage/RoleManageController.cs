using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.RoleManage
{
    public class RoleManageController : BaseController
    {
        // GET: RoleManage
        public ActionResult Index()
        {
            if (Userinfo.LoginName == "admin")
            {
                ViewBag.CanAdd = true;
            }
            else
            {
                ViewBag.CanAdd = false;
            }
            ViewBag.LoginName = Userinfo.LoginName;
            ViewBag.CurrentName = Common.GetLanguageValueByParam("角色管理", "CommonName", "Common", Userinfo.language);
            return View();
        }

        /// <summary>
        /// 获取角色列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleManageList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            List<RoleManageModel> list = new List<RoleManageModel>();
            var dataList = IdentityDA.GetRoleManageList(out total, para, searchValue);
            if (dataList.Count() > 0)
            {
                for (int index = 0; index < dataList.Count(); index++)
                {
                    RoleManageModel model = new RoleManageModel();
                    IDENTITY item = dataList[index];
                    string strHtml = "<div class='row'><div class='col-md-8'>{0}</div><div class='col-md-4' style='text-align:right'>{1}</div></div>";

                    string linkAList = "<a class='glyphicon glyphicon-cog configRole' title='配置'  RoleId='" + item.ID + "'></a>";
                    if (Userinfo.LoginName == "admin")
                    {
                        linkAList += "&nbsp;&nbsp;&nbsp;&nbsp;<a class='glyphicon glyphicon-trash deleteRole' title='删除' RoleId='" + item.ID + "'></a>";
                    }
                    strHtml = string.Format(strHtml, item.KEYED_NAME, linkAList);
                    model.KEYED_NAME = strHtml;
                    model.Id = item.ID;
                    if (item.DESCRIPTION.Contains(";"))
                    {
                        List<string> listStr = item.DESCRIPTION.Split(';').ToList();
                        if (listStr.Count >= 2)
                        {
                            model.Region = listStr[1];
                        }
                    }
                    //获取成员列表
                    List<IDENTITY> memberList = IdentityDA.GetMemberById(model.Id);
                    foreach (var member in memberList)
                    {
                        model.PersonList = model.PersonList + member.NAME + ";";
                    }
                    list.Add(model);
                }
            }
            return Json(new
            {
                sEcho = para.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = list
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存角色管理
        /// </summary>
        public JsonResult SaveRoleManage(string id, string KEYED_NAME, string personList, string region)
        {
            var retModel = new JsonReturnModel();
            try
            {
                string description = "OASystem;";
                if (!string.IsNullOrEmpty(region))
                {
                    //判断地区是否在数据中存在
                    if (!RegionDA.isExistRegionByName(inn, region))
                    {
                        retModel.AddError("errorMessage", "选择的地区不存在！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                    //根据角色名称和地区判断是否已经存在该角色
                    description = description + region;
                }
       
                if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(region))
                {
                    KEYED_NAME = KEYED_NAME + "（" + region + "）";
                }

                //////判断Keyed_Name是否存在
                //if (IdentityDA.ValidIsExistByKeyed_Name(KEYED_NAME,id))
                //{
                //    retModel.AddError("errorMessage", "输入的角色已经存在！");
                //    return Json(retModel, JsonRequestBehavior.AllowGet);
                //}

                Item identity = IdentityDA.GetIdentityByCondition(inn, id, KEYED_NAME);
                int count = int.Parse(identity.getProperty("ncount"));
                if (count > 0)
                {
                    retModel.AddError("errorMessage", "输入的角色和地区已经存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                Item result;
                if (string.IsNullOrEmpty(id))
                {
                    //拼写Insert语句
                    string amlStr = "<AML><Item type='IDENTITY' action='add'><name>" + KEYED_NAME + "</name><description>" + description + "</description>";
                    List<string> list = personList.Split(';').Where(x => x != "" && x != null).Select(x => x.Trim()).Distinct().ToList();
                    if (list.Count() > 0)
                    {
                        amlStr += "<Relationships>";
                        for (int i = 0; i < list.Count; i++)
                        {
                            amlStr += "<Item type='MEMBER' action='add'>";
                            amlStr += "<related_id> ";
                            string textValue = list[i];
                            //验证用户在数据库中是否存在
                            if (UserDA.ValidUserIsExist(inn, textValue))
                            {
                                amlStr += "<Item type='IDENTITY' action='get'><name>" + list[i] + "</name></Item>";
                            }
                            else
                            {
                                retModel.AddError("errorMessage", "输入的人员在系统中不存在！");
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }
                            amlStr += "</related_id></Item>";
                        }
                        amlStr += "</Relationships>";
                    }
                    amlStr += "</Item></AML>";
                    result = IdentityDA.InsertRoleManage(inn, amlStr);
                }
                else
                {
                    string amlStr = "<AML><Item type='IDENTITY' action='edit' id='" + id + "'>";
                    amlStr += "<name>" + KEYED_NAME + "</name>";
                    amlStr += "<description>" + description + "</description>";
                    Item item = IdentityDA.GetRoleManageById(inn, id);
                    if (!string.IsNullOrEmpty(item.getErrorString()))
                    {
                        retModel.AddError("errorMessage", item.getErrorString());
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                    List<string> list = personList.Split(';').Where(x => x != "" && x != null).Select(x => x.Trim()).Distinct().ToList();
                    Item Relation = item.getRelationships();
                    List<string> oldList = new List<string>();
                    if (Relation.getItemCount() > 0 || list.Count() > 0)
                    {
                        amlStr += "<Relationships>";
                    }
                    if (Relation.getItemCount() > 0)
                    {
                        //删除不存在的
                        for (int i = 0; i < Relation.getItemCount(); i++)
                        {
                            Item relationObJ = Relation.getItemByIndex(i);
                            string name = relationObJ.getRelatedItem().getProperty("name");
                            oldList.Add(name);
                            string strValue = list.Where(x => x == name).FirstOrDefault();
                            if (string.IsNullOrEmpty(strValue))
                            {
                                string memberId = relationObJ.getProperty("id");
                                amlStr += "<Item type='MEMBER' action='delete' id='" + memberId + "'></Item>";
                            }
                        }
                    }
                    if (list.Count() > 0)
                    {
                        //添加新的
                        for (int index = 0; index < list.Count(); index++)
                        {
                            string value = list[index];
                            var obj = oldList.Where(x => x == value).FirstOrDefault();
                            //验证用户在数据库中是否存在
                            if (UserDA.ValidUserIsExist(inn, value))
                            {
                                if (string.IsNullOrEmpty(obj))
                                {
                                    amlStr += "<Item type='MEMBER' action='add'>";
                                    amlStr += "<related_id> ";
                                    amlStr += "<Item type='IDENTITY' action='get'><name>" + value + "</name></Item>";
                                    amlStr += "</related_id></Item>";
                                }
                            }
                            else
                            {
                                retModel.AddError("errorMessage", "输入的人员在系统中不存在！");
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    if (Relation.getItemCount() > 0 || list.Count() > 0)
                    {
                        amlStr += "</Relationships>";
                    }
                    amlStr += "</Item></AML>";
                    result = inn.applyAML(amlStr);
                }
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
        /// 刪除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteRoleManage(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result = IdentityDA.DeleteRoleManage(inn, id);
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
        /// 获取角色根据ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetRoleManageById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                RoleManageModel model = new RoleManageModel();
                Item result = IdentityDA.GetRoleManageById(inn, id);
                if (string.IsNullOrEmpty(result.getErrorString()))
                {
                    model.KEYED_NAME = result.getProperty("name");
                    model.Id = result.getProperty("id");
                    string description = result.getProperty("description");
                    if (description.Contains(";"))
                    {
                        List<string> listStr = description.Split(';').ToList();
                        if (listStr.Count >= 2)
                        {
                            model.Region = listStr[1];
                        }
                    }

                    Item Relation = result.getRelationships();
                    if (Relation.getItemCount() > 0)
                    {
                        for (int i = 0; i < Relation.getItemCount(); i++)
                        {
                            Item relationObJ = Relation.getItemByIndex(i);
                            model.PersonList = model.PersonList + relationObJ.getRelatedItem().getProperty("name") + ";";
                        }
                    }
                    retModel.data = model;
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


    }
}