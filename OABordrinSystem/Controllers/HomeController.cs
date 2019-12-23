using Aras.IOM;
using OABordrinCommon;
using OABordrinSystem.Filter;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers
{
    [PageAuthorizationFilter]
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("主页", "Common", "ItemType", Userinfo.language);


            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult AuthWarn()
        {
            return View();
        }


        /// <summary>
        /// 切换系统语言
        /// </summary>
        /// <returns></returns>
        public JsonResult SwitchLanguage(string currentLanguage)
        {
            var retModel = new JsonReturnModel();
            try
            {
                if (currentLanguage == "English")
                {
                    Userinfo.language = "Chinese";
                }
                else
                {
                    Userinfo.language = "English";
                }
                MemoryCacheUtils.Clear(Userinfo.LoginName);
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.Priority = CacheItemPriority.NotRemovable;
                MemoryCacheUtils.Set(Userinfo.LoginName, Userinfo, policy);
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取待办列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTodoList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                //获取PR单数据
                var list = GetPrManageList(Userinfo.Roles);

                list = list.OrderBy(x => x.create_on).ToList();

                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        item.CreateTimeStr = item.create_on.ToString("yyyy-MM-dd");
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
        /// 获取PR单数据
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        private static List<TodoModel> GetPrManageList(List<string> roles)
        {
            List<TodoModel> list = new List<TodoModel>();
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<TodoModel> datas = (from g in db.B_PRMANAGE
                                               join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                               join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                               join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                               join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                               join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                               join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                               where (i.STATE == "active" && roles.Contains(p.ID)) && o.CLOSED_BY == null && i.KEYED_NAME != "Start"
                                               select new TodoModel
                                               {
                                                   Id = g.id,
                                                   RecordNo = g.B_PRRECORDNO,
                                                   create_on = g.CREATED_ON,
                                                   CreateName = g.B_APPLICANT,
                                                   status = i.KEYED_NAME,
                                                   activityId = i.ID,
                                                   activityAssignmentId = o.ID
                                               });
                list = datas.ToList();
            }
            return list;
        }

        public JsonResult GetMenuAuth()
        {
            var retModel = new JsonReturnModel();
            try
            {
                //List<MenuAuthModel> menuAuths = new List<MenuAuthModel>();
                //if (Userinfo.MemuAuth != null && Userinfo.MemuAuth.Count > 0)
                //{
                //    foreach (var item in Userinfo.MemuAuth)
                //    {
                //        MenuAuthModel model = new MenuAuthModel();
                //        switch (item)
                //        {
                //            case "ProjectManagement":
                //                model.MenuIdentity = "b_ProjectManage";
                //                model.MenuName = Common.GetLanguageValueByParam("项目管理", "Common", "ItemType");
                //                model.linkStr = "/ProjectManage/Index";
                //                model.ParentNode =Common.GetLanguageValueByParam("项目管理", "Common", "ItemType");
                //                model.ParentClass = "fa fa-th";
                //                model.sortNumber = 1;
                //                menuAuths.Add(model);
                //                break;
                //            case "b_PrManage":
                //                model.MenuIdentity = "b_PrManage";
                //                model.MenuName = Common.GetLanguageValueByParam("采购PR单管理", "PRCommon", "ItemType");
                //                model.linkStr = "/PrManage/Index";
                //                model.ParentNode = Common.GetLanguageValueByParam("采购管理", "PRCommon", "ItemType");
                //                model.ParentClass = "fa fa-shopping-cart";
                //                model.sortNumber = 2;
                //                menuAuths.Add(model);
                //                break;
                //            case "b_SearchPrManage":
                //                model.MenuIdentity = "b_SearchPrManage";
                //                model.MenuName = Common.GetLanguageValueByParam("查询PR单", "Common", "ItemType");
                //                model.linkStr = "/SearchPrManage/Index";
                //                model.ParentNode = Common.GetLanguageValueByParam("采购管理", "PRCommon", "ItemType");
                //                model.ParentClass = "fa fa-shopping-cart";
                //                model.sortNumber = 3;
                //                menuAuths.Add(model);
                //                break;

                //            case "b_RoleManage":
                //                model.MenuIdentity = "b_RoleManage";
                //                model.MenuName = Common.GetLanguageValueByParam("角色管理", "Common", "ItemType");
                //                model.linkStr = "/RoleManage/Index";
                //                model.ParentNode = Common.GetLanguageValueByParam("权限配置", "Common", "ItemType");
                //                model.ParentClass = "fa fa-gear";
                //                model.sortNumber = 4;
                //                menuAuths.Add(model);
                //                break;
                //            case "b_MenuAuthManage":
                //                model.MenuIdentity = "b_MenuAuthManage";
                //                model.MenuName = Common.GetLanguageValueByParam("菜单权限管理", "Common", "ItemType");
                //                model.linkStr = "/MenuAuthManage/Index";
                //                model.ParentNode= Common.GetLanguageValueByParam("权限配置", "Common", "ItemType");
                //                model.ParentClass = "fa fa-gear";
                //                model.sortNumber = 5;
                //                menuAuths.Add(model);
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //}
                //retModel.data = menuAuths.OrderBy(x=>x.sortNumber);

                retModel.data = Userinfo.MemuAuth;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);

        }
    }
}