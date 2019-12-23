using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OABordrinSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.MenuAuthManage
{
    public class MenuAuthManageController : BaseController
    {
        // GET: MenuAuthManage
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("菜单权限管理", "CommonName", "Common",Userinfo.language);
            return View();
        }

        /// <summary>
        /// 获取列表信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetIdentityList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            List<MenuAuthManageModel> list = new List<MenuAuthManageModel>();
            var dataList = IdentityDA.GetIdentityList(out total, para, searchValue);
            if (dataList != null)
            {
                for (int index = 0; index < dataList.Count(); index++)
                {
                    MenuAuthManageModel model = new MenuAuthManageModel();
                    var item = dataList[index];
                    string strHtml = "<div class='row'><div class='col-md-8'>{0}</div><div class='col-md-4' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-cog configMemuAuth' title='配置'  Id='" + item.ID + "' ></a>";
                    strHtml = string.Format(strHtml, item.KEYED_NAME, linkAList);
                    model.Id = item.ID;
                    model.KEYED_NAME = strHtml;
                    model.AuthStr = "";
                    Item Items= ItemTypeDA.GetMenuAuthByIdentity(inn, item.ID);
                    if(Items.getItemCount()>0)
                    {
                        for(int i=0;i< Items.getItemCount();i++)
                        {
                            Item obj = Items.getItemByIndex(i);
                            string lable = obj.getProperty("name");
                            model.AuthStr += lable + ";";
                        }
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
        /// 获取菜单权限根据帐号
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMenuAuthByIdentity(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();

                MenuAuthManageModel model = new MenuAuthManageModel();
                Item Identity = IdentityDA.GetIdentityById(inn, id);
                if (Identity.getItemCount() > 0)
                {
                    model.Id = Identity.getProperty("id");
                    model.Is_Alias = Identity.getProperty("is_alias");
                    model.KEYED_NAME = Identity.getProperty("keyed_name");
                    Item result = ItemTypeDA.GetMenuAuthByIdentity(inn, id);
                    if (result.getItemCount() > 0)
                    {
                        for (int index = 0; index < result.getItemCount(); index++)
                        {
                            Item item = result.getItemByIndex(index);
                            string keyed_name = item.getProperty("keyed_name");
                            list.Add(keyed_name);

                        }
                    }
                    model.AuthList = list;
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
        /// 保存菜单权限
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveMenuAuth(MenuAuthManageModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item result;
                string amlStr = "";
                Item item = ItemTypeDA.GetMenuAuthByIdentity(inn, model.Id);
                //删除已经有的
                if (item.getItemCount() > 0)
                {
                    for (int i = 0; i < item.getItemCount(); i++)
                    {
                        var itemObj = item.getItemByIndex(i);
                        string keyedName = itemObj.getProperty("keyed_name");
                        string value = "";
                        if (model.AuthList != null)
                        {
                            value = model.AuthList.Where(x => x == keyedName.Trim()).FirstOrDefault();
                        }
                        if (string.IsNullOrEmpty(value))
                        {
                            string typeId = itemObj.getProperty("id");
                            //获取关系类TOC_ACCESS对象
                            Item Relation=  itemObj.getRelationships();
                            string tocId = "";
                            for(int k=0;k< Relation.getItemCount();k++)
                            {
                                Item relationObJ = Relation.getItemByIndex(k);
                                string name = relationObJ.getRelatedItem().getProperty("name");
                                if(name== model.KEYED_NAME)
                                {
                                    tocId= relationObJ.getProperty("id");
                                }
                            }
                            if (!string.IsNullOrEmpty(typeId) && !string.IsNullOrEmpty(tocId))
                            {
                                amlStr += "<Item type='ITEMTYPE' action='edit' id='" + typeId + "'><Relationships>";
                                amlStr += "<Item type='TOC Access' action='delete' id='"+ tocId + "'>";
                                amlStr += "</Item>";
                                amlStr += "</Relationships></Item>";
                            }
                        }
                    }
                }

                if (model.AuthList != null && model.AuthList.Count > 0)
                {
                    for (int i = 0; i < model.AuthList.Count; i++)
                    {
                        string value = model.AuthList[i];
                        bool isExist = false;
                        for (int j = 0; j < item.getItemCount(); j++)
                        {
                            var itemObj = item.getItemByIndex(j);
                            string keyedName = itemObj.getProperty("keyed_name");
                            if (keyedName.Trim() == value)
                            {
                                isExist = true;
                            }
                        }
                        //如果不存在新增
                        if (!isExist)
                        {
                            result = ItemTypeDA.GetItemTypeByName(inn, value);
                            if (!string.IsNullOrEmpty(result.getErrorString()))
                            {
                                retModel.AddError("errorMessage", result.getErrorString());
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }
                            string typeId = result.getProperty("id");
                            if (!string.IsNullOrEmpty(typeId))
                            {
                                amlStr += "<Item type='ITEMTYPE' action='edit' id='" + typeId + "'><Relationships>";
                                amlStr += "<Item type='TOC Access' action='add'>";
                                amlStr += "<related_id> ";
                                amlStr += "<Item type='IDENTITY' action='get'><name>" + model.KEYED_NAME + "</name></Item>";
                                amlStr += "</related_id></Item>";
                                amlStr += "</Relationships></Item>";
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(amlStr))
                {
                    amlStr = "<AML>" + amlStr + "</AML>";
                    result = inn.applyAML(amlStr);
                    if (!string.IsNullOrEmpty(result.getErrorString()))
                    {
                        retModel.AddError("errorMessage", result.getErrorString());
                    }
                }
                //if(item.getItemCount()>0)
                //{
                //    //不存在的新增
                //    for(int index=0;index< item.getItemCount();index++)
                //    {
                //        var itemObj = item.getItemByIndex(index);
                //        string keyedName = item.getProperty("keyed_name");
                //        string value= model.AuthList.Where(x => x == keyedName.Trim()).FirstOrDefault();
                //        if(string.IsNullOrEmpty(value))
                //        {
                //            amlStr += "<Item type='TOC Access' action='add'>";
                //            amlStr += "<related_id> ";
                //            amlStr += "<Item type='IDENTITY' action='get'><name>" + value + "</name></Item>";
                //            amlStr += "</related_id></Item>";
                //        }
                //    }

                //}
                //if (model)
                //List<Item> itemTypes = new List<Item>();
                //if(itemTypes)



            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取系统菜单列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSystemMenuList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                var enumDescriptions = EnumDescription.GetFieldTexts<SystemMenuList>(true);
                var selectList = new List<SelectListItem>();
        
                selectList.AddRange(
                      enumDescriptions.ToArray().Select(
                          m => new SelectListItem() { Text = m.EnumDisplayText, Value = m.FieldName }));
                retModel.data = selectList;
            }
            catch(Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }




    }
}