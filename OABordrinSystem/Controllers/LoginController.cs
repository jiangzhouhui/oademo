
using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinSystem.Utils;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OABordrinSystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index(string errorStr,bool isAdLogin=true)
        {
            //获取请求使用的语言
            string language = Request.Headers["Accept-Language"].ToString();
            language = language.Split(',')[0].ToString();
            if (language.IndexOf("en") >= 0)
            {
                ViewBag.language = "English";
            }
            else
            {
                ViewBag.language = "Chinese";
            }
            ViewBag.language = language;
            ViewBag.errorStr = errorStr;

            //判断是否存在对应的cookie
            HttpCookie cookie = HttpContext.Request.Cookies["Passport.Token"];
            if (cookie != null)
            {
                return Redirect("/Home/Index");
            }

            //判断是否为AD域登陆
            if(isAdLogin)
            {
                string userName = CommonMethod.GetUserLoginName(HttpContext);
                if (!string.IsNullOrEmpty(userName))
                {
                    // 设置用户 cookie
                    HttpCookie createCookie = new HttpCookie("Passport.Token");
                    createCookie.Value = userName;
                    createCookie.Expires = DateTime.Now.AddHours(8);
                    createCookie.Secure = FormsAuthentication.RequireSSL;
                    Response.Cookies.Add(createCookie);
                    return Redirect("/Home/Index");
                }
            }
            return View();
        }

        /// <summary>
        /// 判断用户是否已经登录
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginAuthentication()
        {
            var retModel = new JsonReturnModel();
            try
            {

                HttpCookie cookie = HttpContext.Request.Cookies["Passport.Token"];
                if (cookie != null)
                {
                    retModel.data = "login success";
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("ErrorMessge", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 登入(Login)
        /// </summary>
        /// <param name="pUrl">URL</param>
        /// <param name="pDBName">DBName</param>
        /// <param name="pUserName">UserName</param>
        /// <param name="pPassword">Password</param>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            //判断当前系统语言
            string language = Request.Headers["Accept-Language"].ToString();
            language = language.Split(',')[0].ToString();
            if (language.IndexOf("en") >= 0)
            {
                language = "English";
            }
            else
            {
                language = "Chinese";
            }

            string url = ConfigurationManager.AppSettings["ArasUrl"];
            string dbName = ConfigurationManager.AppSettings["ArasDB"];
            string username = Request.Form["Username"];
            string password = Request.Form["Password"];
            string ChoicePath = Request.Form["ChoicePath"];
            string str = "";
            try
            {
                //string DomainKey = "admin";
                //string ForceSha = "0";
                //string passwordStr = CommonMethod.md5string16(DomainKey, ForceSha == "1" ? true : false) + CommonMethod.md5string16(username.ToLower(), ForceSha == "1" ? true : false) + username.ToLower() + DomainKey;
                //HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, dbName, username, password);
                //Item login_result = conn.Login();
                //if (login_result.isError())
                //{
                //}
                //strPassword = md5string16(DomainKey, IIf(ForceSha = "1", True, False)) + md5string16(strUserName.ToLower(), IIf(ForceSha = "1", True, False)) + strUserName.ToLower() + DomainKey 参考代码
                UserInfo user = new UserInfo();
                string errorMsg = "";
                //if (username == "admin")
                //{
                //    HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, dbName, username, password);
                //    Item login_result = conn.Login();
                //    if (login_result.isError())
                //    {
                //        if (conn != null) { conn.Logout(); }
                //        str = login_result.getErrorString();
                //        int startIndex = (str.IndexOf(":") + 1);
                //        if (startIndex > 0) { str = str.Substring(startIndex); }
                //        if (str.Contains("Authentication")) { str = "Invalid user or password"; }
                //    }
                //    else
                //    {
                //        errorMsg = "AD Login OK";
                //    }
                //}
                //else
                //{
                //    errorMsg = LoginAD(username, password);
                //}
                errorMsg = "AD Login OK";
                if (errorMsg == "AD Login OK")
                {
                    //获取用户信息
                    USER userObJ = UserDA.GetUserByLoginName(username);
                    // 创建登录凭证      

                    user.UserId = userObJ.ID;
                    user.UserName = userObJ.KEYED_NAME;
                    user.LoginName = userObJ.LOGIN_NAME;
                    user.Password = userObJ.PASSWORD;
                    user.HTTP_USER_AGENT = Request.UserAgent;
                    user.UserIp = Request.UserHostAddress;
                    user.b_JobNumber = userObJ.B_JOBNUMBER;
                    user.Email = userObJ.EMAIL;
                    user.language = language;
                    user.b_AffiliatedCompany = userObJ.B_AFFILIATEDCOMPANY;

                    //获取AD域中的信息
                    CommonMethod.GetAdInfoByUser(user, "bordrin.com");

                    user.ExpireDate = DateTime.Now.AddDays(1);
                    //Innovator.ScalcMD5(user.Password)
                    HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, dbName, user.LoginName, user.Password);
                    Item login_result = conn.Login();
                    if (login_result.isError())
                    {
                        if (conn != null) { conn.Logout(); }
                        str = login_result.getErrorString();
                        int startIndex = (str.IndexOf(":") + 1);
                        if (startIndex > 0) { str = str.Substring(startIndex); }
                        if (str.Contains("Authentication")) { str = "Invalid user or password"; }
                    }
                    else
                    {

                        var inn = login_result.getInnovator();
                        //string token = Guid.NewGuid().ToString("N").ToUpper();
                        //获取当前角色身份
                        List<string> listRoles = IdentityDA.getIdentityListByUserID(inn, user.UserId);
                        user.Roles = listRoles;
                        //获取当前权限信息
                        if (user.MemuAuth == null)
                        {
                            user.MemuAuth = new List<string>();
                            for (int j = 0; j < listRoles.Count; j++)
                            {
                                string id = listRoles[j];
                                //根据Id获取权限列表
                                Item ItemTypes = ItemTypeDA.GetMenuAuthByIdentity(inn, id);
                                if (ItemTypes.getItemCount() > 0)
                                {
                                    for (int i = 0; i < ItemTypes.getItemCount(); i++)
                                    {
                                        Item itemobj = ItemTypes.getItemByIndex(i);
                                        string itemName = itemobj.getProperty("name");
                                        if (user.MemuAuth.IndexOf(itemName) < 0)
                                        {
                                            user.MemuAuth.Add(itemName);
                                        }

                                    }
                                }
                            }
                        }
                        user.inn = inn;

                        //获取委托的权限数据
                        DateTime currentTime = DateTime.Now.AddMinutes(-10);
                        if ((user.AgentAuth == null && user.AgentCreateTime == null) || (user.AgentCreateTime != null && currentTime > user.AgentCreateTime))
                        {
                            List<AgentSetEntity> AgentSetList = AgentSetBll.GetAgentSetByUserName(user.UserName);
                            if (AgentSetList.Count > 0)
                            {
                                AgentSetBll.GetAgentRoles(inn, user, AgentSetList);
                            }
                        }


                        UserBll.SaveUserInfoToCache(user);
                        // 设置用户 cookie
                        HttpCookie cookie = new HttpCookie("Passport.Token");
                        cookie.Value = user.LoginName;
                        cookie.Expires = DateTime.Now.AddHours(8);
                        cookie.Secure = FormsAuthentication.RequireSSL;
                        Response.Cookies.Add(cookie);
                        if (ChoicePath == "0")
                        {
                            return Redirect("/Portal/Index");
                        }
                        else
                        {
                            return Redirect("/Home/Index");
                        }
                    }
                }
                else
                {
                    str = "Invalid user or password";
                }
            }
            catch (Exception ex)
            {
                str = "Invalid user or password";
            }
            return RedirectToAction("Index", "Login", new { errorStr = str, isAdLogin=false });
        }


        /// <summary>
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            HttpCookie cookie = HttpContext.Request.Cookies["Passport.Token"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                MemoryCacheUtils.Clear(cookie.Value);
                cookie.Expires = DateTime.Today.AddDays(-2);
                Response.Cookies.Add(cookie);
                Request.Cookies.Remove("Passport.Token");
            }
            return RedirectToAction("Index", "Login", new { errorStr = "", isAdLogin = false });
        }


        //AD域登陆
        private string LoginAD(string username, string password)
        {
            string DomainName = "bordrin.com";
            string ldapPath = "LDAP://" + DomainName;
            string errorMsg = "";
            //string userName = txtUser.Text;
            //string pwd = txtADPwd.Text;

            if (!TryAuthenticate(DomainName, username, password))
            {
                errorMsg = "AD Login Error";
            }
            else
            {
                errorMsg = "AD Login OK";
                USER userObJ= UserDA.GetUserByLoginName(username);
                if(userObJ==null)
                {
                    errorMsg = "AD Login Error";
                }
            }
            return errorMsg;
        }

        public static bool TryAuthenticate(string domain, string username, string password)
        {
            bool isLogin = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}", domain), username, password);
                entry.RefreshCache();
                //DirectoryEntry obj = GetUserEntryByAccount(entry, "Zhouhui.Jiang");
                //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                //if (obj.Parent != null && obj.Parent.Properties["ou"].Value != null && !string.IsNullOrEmpty(obj.Parent.Properties["ou"].Value.ToString()))
                //{
                //    user.department = obj.Parent.Properties["ou"].Value.ToString();
                //}

                //string hostName = ipGlobalProperties.HostName;
                //string domainName = ipGlobalProperties.DomainName;
                //PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName);
                //UserPrincipal userPrincipal = new UserPrincipal(principalContext);
                //PrincipalSearcher principalSearcher = new PrincipalSearcher(userPrincipal);
                ////principalSearcher.QueryFilter;
                //StringBuilder sb = new StringBuilder();
                //foreach (UserPrincipal userPrincipalSearchResult in principalSearcher.FindAll())
                //{
                //    sb.AppendLine(string.Format("UPN:{0}", userPrincipalSearchResult.UserPrincipalName));
                //    sb.AppendLine(string.Format("姓氏Last Name:{0}", userPrincipalSearchResult.Surname));
                //    sb.AppendLine(string.Format("中间名:{0}", userPrincipalSearchResult.MiddleName));
                //    sb.AppendLine(string.Format("Given Name/First Name名:{0}", userPrincipalSearchResult.GivenName));
                //    sb.AppendLine(string.Format("名称:{0}", userPrincipalSearchResult.Name));
                //    sb.AppendLine(string.Format("上次登录时间:{0}", userPrincipalSearchResult.LastLogon));
                //}
                isLogin = true;
            }
            catch (Exception ex)
            {
                isLogin = false;
            }
            return isLogin;
        }
    }
}


