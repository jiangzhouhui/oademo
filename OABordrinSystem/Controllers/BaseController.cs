using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinSystem.Utils;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers
{
    public class BaseController : Controller
    {
        protected UserInfo Userinfo;

        protected Innovator inn;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookie cookie = HttpContext.Request.Cookies["Passport.Token"];
            if (cookie == null) // 站内凭证不存在
            {
                Response.Redirect("/Login/Index");
            }
            else
            {
                if (Userinfo == null || Userinfo.LoginName != cookie.Value)
                {
                    //string language = Request.Headers["Accept-Language"].ToString();
                    //language = language.Split(',')[0].ToString();
                    Userinfo = UserBll.GetUserInfoByUserName(cookie.Value);

                    if (Userinfo == null)
                    {
                        Response.Redirect("/Login/Index");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Userinfo.department))
                        {
                            CommonMethod.GetAdInfoByUser(Userinfo, "bordrin.com");
                        }
                        ViewBag.userName = Userinfo.UserName;
                        ViewBag.language = Userinfo.language;
                    }

                    bool isaddCache = false;

                    inn = Userinfo.inn;
                    if (inn == null || inn.getUserID() != Userinfo.UserId)
                    {
                        using (ArasInnovator arasConn = new ArasInnovator())
                        {
                            inn = arasConn.ArasConnection(Userinfo.LoginName, Userinfo.Password);
                            Userinfo.inn = inn;
                            isaddCache = true;
                        }
                    }


                    //如果角色为空，获取角色权限
                    if (Userinfo.Roles == null)
                    {
                        Userinfo.Roles = IdentityDA.getIdentityListByUserID(inn, Userinfo.UserId);
                        isaddCache = true;
                    }

                    //如果菜单权限为空，获取菜单权限
                    if (Userinfo.MemuAuth == null)
                    {
                        Userinfo.MemuAuth = new List<string>();
                        for (int j = 0; j < Userinfo.Roles.Count; j++)
                        {
                            string id = Userinfo.Roles[j];
                            //根据Id获取权限列表
                            Item ItemTypes = ItemTypeDA.GetMenuAuthByIdentity(inn, id);
                            if (ItemTypes.getItemCount() > 0)
                            {
                                for (int i = 0; i < ItemTypes.getItemCount(); i++)
                                {
                                    Item itemobj = ItemTypes.getItemByIndex(i);
                                    string itemName = itemobj.getProperty("name");
                                    if (Userinfo.MemuAuth.IndexOf(itemName) < 0)
                                    {
                                        Userinfo.MemuAuth.Add(itemName);
                                    }

                                }
                            }
                        }
                        isaddCache = true;
                    }

                    //获取委托的权限数据
                    DateTime currentTime = DateTime.Now.AddMinutes(-10);
                    if ((Userinfo.AgentAuth == null && Userinfo.AgentCreateTime == null) || (Userinfo.AgentCreateTime != null && currentTime > Userinfo.AgentCreateTime))
                    {
                        List<AgentSetEntity> AgentSetList = AgentSetBll.GetAgentSetByUserName(Userinfo.UserName);
                        if (AgentSetList.Count > 0)
                        {
                            AgentSetBll.GetAgentRoles(inn, Userinfo, AgentSetList);
                            isaddCache = true;
                        }
                    }

                    //重新添加缓存
                    if (isaddCache)
                    {
                        CacheItemPolicy policy = new CacheItemPolicy();
                        policy.Priority = CacheItemPriority.NotRemovable;
                        MemoryCacheUtils.Set(Userinfo.LoginName, Userinfo, policy);
                    }
                }
                else
                {
                    ViewBag.userName = Userinfo.UserName;
                    ViewBag.language = Userinfo.language;
                }

                ViewData["MemuAuth"] = Userinfo.MemuAuth;
                string strController = filterContext.RouteData.Values["controller"].ToString();
                switch (strController)
                {
                    case "MenuAuthManage":
                        string menuAuthManage = Userinfo.MemuAuth.Where(x => x == "b_MenuAuthManage").FirstOrDefault();
                        if (string.IsNullOrEmpty(menuAuthManage))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    case "RoleManage":
                        string roleManage = Userinfo.MemuAuth.Where(x => x == "b_RoleManage").FirstOrDefault();
                        if (string.IsNullOrEmpty(roleManage))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;

                    case "ProjectManage":
                        string projectManage = Userinfo.MemuAuth.Where(x => x == "b_ProjectManage").FirstOrDefault();
                        if (string.IsNullOrEmpty(projectManage))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    case "OrganizationalStructure":
                        string organizationalStructure = Userinfo.MemuAuth.Where(x => x == "b_OrganizationalStructure").FirstOrDefault();
                        if(string.IsNullOrEmpty(organizationalStructure))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    case "User":
                        string user = Userinfo.MemuAuth.Where(x => x == "b_User").FirstOrDefault();
                        if(string.IsNullOrEmpty(user))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    case "ExpenseCategory":
                        string expenseCategory = Userinfo.MemuAuth.Where(x => x == "b_ExpenseCategory").FirstOrDefault();
                        if(string.IsNullOrEmpty(expenseCategory))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    case "ExpenseAuditConfiguration":
                        string expenseAuditConfiguration= Userinfo.MemuAuth.Where(x => x == "b_ExpenseAuditConfiguration").FirstOrDefault();
                        if(string.IsNullOrEmpty(expenseAuditConfiguration))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    case "AgentSet":
                        string agentSet = Userinfo.MemuAuth.Where(x => x == "b_AgentSet").FirstOrDefault();
                        if (string.IsNullOrEmpty(agentSet))
                        {
                            Response.Redirect("/Home/AuthWarn");
                        }
                        break;
                    default:
                        break;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}