using OABordrinEntity;
using OABordrinService.ADDomain;
using OABordrinService.Entity;
using OABordrinService.MD5;
using OABordrinSystem.Controllers.ExpenseReimbursement;
using OABordrinSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace OABordrinService
{
    /// <summary>
    /// OAService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class OAService : System.Web.Services.WebService
    {
        [WebMethod(Description = "获取用户加密令牌")]
        public string MD5Encode(string userName, string password)
        {
            string encode = "";
            string errorMsg = ADLogin.LoginAD(userName, password);
            if (errorMsg == "AD Login OK")
            {
                userName = userName + ";" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                encode = SecurityBll.Encode(userName);
            }
            return encode;
        }

        [WebMethod(Description = "获取OA代办事项")]
        public List<TodoEntity> GetToDoList(string loginName, string token, string ErrorStr = "")
        {
            string str = SecurityBll.Decode(token);
            string[] strList = str.Split(';');
            string name = strList[0];
            DateTime dateNow = DateTime.Parse(strList[1]);
            DateTime CurrentTime = DateTime.Now.AddHours(-8);
            if (CurrentTime > dateNow)
            {
                ErrorStr = "令牌超时";
            }
            List<TodoEntity> list = new List<TodoEntity>();
            if (loginName == name && ErrorStr == "")
            {
                UserInfo user = new UserInfo();
                list = ToDoServiceBll.ToDoServiceBll.GetTodoList(loginName, user);
            }
            return list;
        }


        [WebMethod(Description = "获取OA代办事项")]
        public List<TodoEntity> GetToDoListSimple(string loginName)
        {
            List<TodoEntity> list = new List<TodoEntity>();
            if (loginName != "")
            {
                UserInfo user = new UserInfo();
                list = ToDoServiceBll.ToDoServiceBll.GetTodoList(loginName, user);
            }
            return list;
        }



    }
}
