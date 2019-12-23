using Aras.IOM;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class ActivityDA
    {
        ///// <summary>
        ///// 完成任务
        ///// </summary>
        //public static string CompleteActivity(Innovator inn, string strAML)
        //{
        //    Item resItem = inn.applyAML(strAML);
        //    if (resItem.isError()) return resItem.getErrorString();
        //    return "";
        //}

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="activityId">任务ID</param>
        /// <param name="activityAssignmentId">权限ID</param>
        /// <param name="path">选择路线</param>
        /// <param name="delegateTo">委托到</param>
        /// <returns></returns>
        public static string CompleteActivity(Innovator inn, string activityId, string activityAssignmentId, string pathId, string pathName, string delegateTo, string comments, UserInfo userInfo=null)
        {
            if(userInfo!=null && string.IsNullOrEmpty(delegateTo))
            {
                string delegateName="";
                ACTIVITY_ASSIGNMENT activity_assignment = WorkFlowDA.GetInnvatorByAgent(inn, userInfo, activityId, activityAssignmentId,ref delegateName);
                if (activity_assignment != null)
                {
                    activityAssignmentId = activity_assignment.ID;
                    comments ="使用 " + delegateName + " 代理权限审核完成。备注：" + comments;
                }
            }
    
            //获取委托的ID
            string delegateToId = "";
            if (!string.IsNullOrEmpty(delegateTo))
            {
                delegateToId = UserDA.GetUserByKeyedName(delegateTo).ID;
            }
            string MD5Auth = "";
            string AuthMode = "";
            var strAML = "<AML><Item type='Activity' action='EvaluateActivity'>";
            strAML += "<Activity>" + activityId + "</Activity>";
            strAML += "<ActivityAssignment>" + activityAssignmentId + "</ActivityAssignment>";
            strAML += "<Paths>";
            strAML += "<Path id='" + pathId + "'>" + pathName + "</Path>";
            strAML += "</Paths>";
            strAML += "<DelegateTo>" + delegateToId + "</DelegateTo>";
            strAML += "<Tasks>";
            strAML += "</Tasks>";
            strAML += "<Variables></Variables>";
            strAML += "<Authentication mode='" + AuthMode + "'>" + MD5Auth + "</Authentication>";
            strAML += "<Comments>" + comments + "</Comments>";
            strAML += "<Complete>1</Complete>";
            strAML += "</Item></AML>";
            Item resItem = inn.applyAML(strAML);
            if (resItem.isError()) return resItem.getErrorString();
            return "";
        }

        /// <summary>
        /// 获取当前活动
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetActivityByItemId(Innovator inn, string id, string tableName)
        {
            string sqlStr = @"select i.ID as activityId,o.ID as activityAssignmentId,i.KEYED_NAME from {0} g inner join 
                              innovator.WORKFLOW t on g.id=t.SOURCE_ID inner join 
                              innovator.WORKFLOW_PROCESS y on t.RELATED_ID=y.ID inner join 
                              innovator.WORKFLOW_PROCESS_ACTIVITY u on y.ID=u.SOURCE_ID inner join
                              innovator.ACTIVITY i on u.RELATED_ID=i.ID inner join 
                              innovator.ACTIVITY_ASSIGNMENT o on i.ID=o.SOURCE_ID where i.STATE = 'active' and g.id='" + id + "'";

            sqlStr = string.Format(sqlStr, tableName);
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
            return returnItem;
        }



        /// <summary>
        /// 节点审核，根据登录角色权限
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="tableName"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static Item GetActivityAuditByLoginInfo(Innovator inn, string id, string tableName, List<string> roles, List<string> agentRoles = null)
        {
            List<string> newRoles = new List<string>();
            if (roles != null && roles.Count > 0)
            {
                newRoles.AddRange(roles);
            }
            if (agentRoles != null && agentRoles.Count > 0)
            {
                newRoles.AddRange(agentRoles);
            }


            string sqlStr = @"select i.ID as activityId,o.ID as activityAssignmentId,i.KEYED_NAME from {0} g inner join 
                              innovator.WORKFLOW t on g.id=t.SOURCE_ID inner join 
                              innovator.WORKFLOW_PROCESS y on t.RELATED_ID=y.ID inner join 
                              innovator.WORKFLOW_PROCESS_ACTIVITY u on y.ID=u.SOURCE_ID inner join
                              innovator.ACTIVITY i on u.RELATED_ID=i.ID inner join 
                              innovator.ACTIVITY_ASSIGNMENT o on i.ID=o.SOURCE_ID inner join 
                              innovator.[IDENTITY] p on o.RELATED_ID=p.ID
                              where i.STATE = 'active' and o.CLOSED_BY is null and g.id='" + id + "'";

            if (newRoles != null && newRoles.Count > 0)
            {
                sqlStr += " and p.id in (";
                for (int i = 0; i < newRoles.Count; i++)
                {
                    if (i == newRoles.Count - 1)
                    {
                        sqlStr += "'" + newRoles[i] + "'" + ")";
                    }
                    else
                    {
                        sqlStr += "'" + newRoles[i] + "',";
                    }
                }
            }
            sqlStr = string.Format(sqlStr, tableName);
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
            return returnItem;
        }




        /// <summary>
        /// 根据条件获取活动
        /// </summary>
        /// <returns></returns>
        public static Item GetActivityByItemId(Innovator inn, string id, string assignmentName, string tableName)
        {
            string sqlStr = @"select i.ID as activityId,o.ID as activityAssignmentId from {0} g inner join 
                              innovator.WORKFLOW t on g.id=t.SOURCE_ID inner join 
                              innovator.WORKFLOW_PROCESS y on t.RELATED_ID=y.ID inner join 
                              innovator.WORKFLOW_PROCESS_ACTIVITY u on y.ID=u.SOURCE_ID inner join
                              innovator.ACTIVITY i on u.RELATED_ID=i.ID inner join 
                              innovator.ACTIVITY_ASSIGNMENT o on i.ID=o.SOURCE_ID inner join 
                              innovator.[IDENTITY] p on o.related_id=p.id
                              where i.STATE = 'active' and g.id='" + id + "' and p.name='" + assignmentName + "'";

            sqlStr = string.Format(sqlStr, tableName);

            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");

            return returnItem;

        }








        /// <summary>
        /// 获取活动根据Id
        /// </summary>
        /// <returns></returns>
        public static Item GetActivityById(Innovator inn, string id)
        {
            string strAML = "<AML><Item type='ACTIVITY' action='get' id='" + id + "'></Item></AML>";
            var returnItem = inn.applyAML(strAML);
            return returnItem;
        }


        /// <summary>
        /// 获取活动根据活动名称
        /// </summary>
        /// <returns></returns>
        public static Item GetActivityByNames(Innovator inn, List<string> names, string ItemId, string tableName, string activityStatus = "Pending")
        {
            if (names == null || names.Count() == 0)
            {
                return null;
            }
            string whereStr = " where i.KEYED_NAME in (";
            for (int i = 0; i < names.Count(); i++)
            {
                if (i == (names.Count() - 1))
                {
                    whereStr += "'" + names[i] + "'" + ")";
                }
                else
                {
                    whereStr += "'" + names[i] + "'" + ",";
                }
            }
            whereStr += " and g.id='" + ItemId + "' and i.state='" + activityStatus + "'";
            string sqlStr = @"select i.*  from {0} g inner join 
                              innovator.WORKFLOW t on g.id=t.SOURCE_ID inner join 
                              innovator.WORKFLOW_PROCESS y on t.RELATED_ID=y.ID inner join 
                              innovator.WORKFLOW_PROCESS_ACTIVITY u on y.ID=u.SOURCE_ID inner join
                              innovator.ACTIVITY i on u.RELATED_ID=i.ID";
            sqlStr = sqlStr + whereStr;

            sqlStr = string.Format(sqlStr, tableName);

            var activitys = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
            return activitys;
        }


        /// <summary>
        /// 获取当前任务操作人
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static string GetActivityOperator(Innovator inn, string activityId)
        {
            string names = "";
            //获取当前任务操作权限
            Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId);

            UserDA.GetNamesByIdentitys(inn, identitys, ref names);

            if (!string.IsNullOrEmpty(names))
            {
                names = names.Substring(0, names.Length - 1);
            }
            return names;
        }

        /// <summary>
        /// 获取当前任务操作人
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static string GetActivityOperator(Innovator inn, string activityId, bool isClose)
        {
            string names = "";
            //获取当前任务操作权限
            Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId, isClose);

            UserDA.GetNamesByIdentitys(inn, identitys, ref names);

            if (!string.IsNullOrEmpty(names))
            {
                names = names.Substring(0, names.Length - 1);
            }
            return names;
        }


        /// <summary>
        /// 获取当前任务，登录人的审核权限列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetActivityIdentitys(Innovator inn, string activityId, List<string> Roles)
        {
            List<string> identitys = new List<string>();
            //获取当前任务操作权限
            Item identityItems = IdentityDA.GetIdentityByActivityId(inn, activityId);
            if (!identityItems.isError() && identityItems.getItemCount() > 0)
            {
                for (int i = 0; i < identityItems.getItemCount(); i++)
                {
                    var item = identityItems.getItemByIndex(i);
                    string identityId = item.getProperty("id");
                    if (Roles.Contains(identityId))
                    {
                        identitys.Add(identityId);
                    }
                }
            }
            return identitys;
        }
    }
}
