using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public static class WorkFlowBll
    {
        /// <summary>
        /// 转办发送邮件
        /// </summary>
        /// <param name="trunUserName"></param>
        /// <param name="todoUserName"></param>
        /// <param name="recordNo"></param>
        public static void TurnToDoSendEmail(string trunUserName, string todoUserName, string recordNo, string linkStr, string emailAddress)
        {
            string subject = "The form  No.< " + recordNo + " > has been transferred to you by <" + trunUserName + "> for your approval.";
            string body = "Dear " + todoUserName + ",<br/><br/>";
            body += "The form  No. < " + recordNo + " > has been transferred to you by <" + trunUserName + "> for your approval.<br/>";
            body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + linkStr;
            List<string> listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");
            listEmail.Add("xiangxiang.zhou@bordrin.com");
            //listEmail.Add("Jingjian.Gong@Bordrin.com");
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }

        /// <summary>
        /// 加签发送邮件
        /// </summary>
        /// <param name="addSignUserName"></param>
        /// <param name="todoUserName"></param>
        /// <param name="recordNo"></param>
        /// <param name="linkStr"></param>
        /// <param name="listEmail"></param>
        public static void WorkflowActivitySignSendEmail(string addSignUserName, string todoUserName, string recordNo, string linkStr, List<string> listEmail)
        {

            string subject = addSignUserName + " has added a request[" + recordNo + "] for your approval.";
            string body = "Dear " + todoUserName + ",<br/><br/>";
            body += addSignUserName + " has added a request[" + recordNo + "] for your approval.<br/>";
            body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + linkStr;
            listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");
            listEmail.Add("xiangxiang.zhou@bordrin.com");
            //listEmail.Add("Jingjian.Gong@Bordrin.com");
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }


        /// <summary>
        /// 委托代理发送邮件
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_DelegateName">委托人</param>
        /// <param name="b_AgentName">代理人</param>
        /// <param name="StartDate">生效时间</param>
        /// <param name="EndDate">终止时间</param>
        /// <param name="b_AgentContent"></param>
        public static void SendAgentEmail(Innovator inn, string b_DelegateName, string b_AgentName, DateTime StartDate, DateTime EndDate, string b_AgentContent)
        {
            List<string> listEmail = new List<string>();
            USER user = UserDA.GetUserByFirstName(b_AgentName);
            string emailAddress = "";
            if (user != null)
            {
                emailAddress = user.EMAIL;
                listEmail.Add(emailAddress);
            }
            if (!string.IsNullOrEmpty(emailAddress))
            {
                string subject = "OA代理设置通知：" + b_DelegateName + "设置您为" + StartDate.ToString("yyyy-MM-dd hh:mm:ss") + "至" + EndDate.ToString("yyyy-MM-dd hh:mm:ss") + "这段期间内的职务代理人。【OA system notification】" + b_DelegateName + " sets up the OA approval agent for the period from " + StartDate.ToString("yyyy-MM-dd hh:mm:ss") + " to " + EndDate.ToString("yyyy-MM-dd hh:mm:ss");
                string body = "Dear " + b_AgentName + ",<br/><br/>";
                body += "" + b_DelegateName + "设置您为" + StartDate.ToString("yyyy-MM-dd hh:mm:ss") + "至" + EndDate.ToString("yyyy-MM-dd hh:mm:ss") + "这段期间内的职务代理人。<br/>";
                body += "代理模块: " + b_AgentContent + "<br/>";
                body += "" + b_DelegateName + " sets up the OA approval agent for the period from " + StartDate.ToString("yyyy-MM-dd hh:mm:ss") + " to " + EndDate.ToString("yyyy-MM-dd hh:mm:ss") + "<br/>";
                listEmail = new List<string>();
                listEmail.Add("zhouhui.jiang@bordrin.com");
                listEmail.Add("ping.chen@bordrin.com");
                listEmail.Add("xiangxiang.zhou@bordrin.com");
                new Task(() =>
                {
                    MailOperator.SendMail(listEmail, subject, body);
                }).Start();
            }
        }


        /// <summary>
        /// 获取流程状态Select列表
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="workFlowName"></param>
        /// <returns></returns>
        public static Item GetWorkflowStatusList(Innovator inn, string workFlowName)
        {
            string sqlStr = "select y.KEYED_NAME from WORKFLOW_MAP g inner join WORKFLOW_MAP_ACTIVITY t on g.id=t.SOURCE_ID inner join ACTIVITY_TEMPLATE y on t.related_id=y.id where g.KEYED_NAME='" + workFlowName + "' and g.IS_CURRENT=1 order by y.CREATED_ON asc";
            Item result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
            return result;
        }


        /// <summary>
        /// 自动完成
        /// </summary>
        public static WORKFLOW_PROCESS_PATH AutoCompleteActivityByParam(string id, string tableName, string lineName = "agree")
        {
            string url = ConfigurationManager.AppSettings["ArasUrl"];
            string dbName = ConfigurationManager.AppSettings["ArasDB"];
            string accountName = ConfigurationManager.AppSettings["Administrator"];
            string password = ConfigurationManager.AppSettings["ArasPassword"];

            //使用管理员帐号登陆
            HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, dbName, accountName, password);
            Item login_result = conn.Login();
            if (!login_result.isError())
            {
                var inn = login_result.getInnovator();
                Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "Administrators", tableName);
                if (!activityItem.isError())
                {
                    string activityId = activityItem.getProperty("activityid");
                    string activityAssignmentId = activityItem.getProperty("activityassignmentid");
                    //任务路线
                    var listActivity = WorkflowProcessPathDA.GetWorkflowProcessPathByActivityId(activityId);
                    WORKFLOW_PROCESS_PATH choicePath = listActivity.Where(x => x.NAME == lineName).FirstOrDefault();
                    //替换符
                    ReplaceChars(choicePath);
                    string errorStr = ActivityDA.CompleteActivity(inn, activityId, activityAssignmentId, choicePath.ID, choicePath.NAME, "", "AutoComplete");
                    if (string.IsNullOrEmpty(errorStr))
                    {
                        return choicePath;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 替换字符
        /// </summary>
        /// <param name="choicePath"></param>
        public static void ReplaceChars(WORKFLOW_PROCESS_PATH choicePath)
        {
            choicePath.NAME = choicePath.NAME.Replace(">=", "&gt;=");
            choicePath.NAME = choicePath.NAME.Replace("<", "&lt;");
        }

        /// <summary>
        /// 获取Admin 登录连接
        /// </summary>
        /// <returns></returns>
        public static Innovator GetAdminInnovator()
        {
            string url = ConfigurationManager.AppSettings["ArasUrl"];
            string dbName = ConfigurationManager.AppSettings["ArasDB"];
            string accountName = ConfigurationManager.AppSettings["Administrator"];
            string password = ConfigurationManager.AppSettings["ArasPassword"];

            //使用管理员帐号登陆
            HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, dbName, accountName, password);
            Item login_result = conn.Login();

            if (!login_result.isError())
            {
                var inn = login_result.getInnovator();
                return inn;
            }
            return null;
        }



        /// <summary>
        /// 添加工作流节点的角色审核权限
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="roleName"></param>
        /// <param name="region"></param>
        /// <param name="activityName"></param>
        /// <param name="tableName"></param>
        public static void AddWorkFlowRoleAuditByRegion(Innovator inn, string id, string roleName, string region, string activityName, string tableName)
        {
            List<string> activityNames = new List<string> { activityName };
            if (!string.IsNullOrEmpty(region))
            {
                region = "OASystem;" + region;
            }
            //获取财务分析员
            Item identity = IdentityDA.GetIdentityByParam(inn, roleName, region);
            if (!identity.isError() && identity.getItemCount() > 0)
            {
                string identityid = identity.getProperty("id");
                ActivityBll.AddActivityAuth(inn, id, identityid, activityNames, tableName);

            }
        }


        //public static bool IsBeforeAuditByCondition(Innovator inn,string itemId, string tableName, List<IDENTITY> identitys)
        //{
        //    string sqlStr = "select count(p.ID) as count from " + tableName + " g inner join WORKFLOW t on g.id=t.SOURCE_ID inner join WORKFLOW_PROCESS y on t.RELATED_ID=y.ID" +
        //                    " inner join WORKFLOW_PROCESS_ACTIVITY u on y.ID=u.SOURCE_ID inner join ACTIVITY i on u.RELATED_ID=i.ID inner join ACTIVITY_ASSIGNMENT o on i.ID=o.SOURCE_ID" +
        //                    " inner join USER p on o.CLOSED_BY=p.ID where g.id='" + itemId + "' and o.CLOSED_BY is not null and o.COMMENTS!='AutoComplete' and p.FIRST_NAME in (";

        //    for (var i = 0; i < identitys.Count; i++)
        //    {
        //        if (i != (identitys.Count - 1))
        //        {
        //            sqlStr += "'" + identitys[i].KEYED_NAME + "'" + ",";
        //        }
        //        else
        //        {
        //            sqlStr += "'" + identitys[i].KEYED_NAME + "')";
        //        }
        //    }
        //    Item result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
        //    int count = 0;
        //    if(!result.isError())

        //}






    }
}
