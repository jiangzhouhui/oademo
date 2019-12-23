using Aras.IOM;
using OABordrinDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class ActivityBll
    {
        /// <summary>
        /// 添加流程节点权限
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="identityId"></param>
        public static string AddActivityAuthById(Innovator inn, string ItemId, string identityId, List<string> names, string tableName)
        {
            string strError = "";
            //根据任务名称获取任务
            Item activitys = ActivityDA.GetActivityByNames(inn, names, ItemId, tableName);
            //获取当前任务已经存在的处理人
            if (!activitys.isError())
            {
                //获取任务Id
                string strAml = "<AML>";
                for (var i = 0; i < activitys.getItemCount(); i++)
                {
                    var item = activitys.getItemByIndex(i);
                    string source_id = item.getProperty("id");
                    Item activityAssignments = ActivityAssignmentDA.GetActivityAssignment(inn, source_id);
                    string ids = "";
                    if (!activityAssignments.isError() && activityAssignments.getItemCount() > 0)
                    {
                        for (int index = 0; index < activityAssignments.getItemCount(); index++)
                        {
                            var activityAssignment = activityAssignments.getItemByIndex(index);
                            string id = activityAssignment.getProperty("id");

                            if (index != activityAssignments.getItemCount() - 1)
                            {
                                ids += id + ",";
                            }
                            else
                            {
                                ids += id;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(ids))
                    {
                        ActivityAssignmentDA.deleteActivityAssignment(inn, ids);
                    }

                    strAml += "<Item type = 'ACTIVITY ASSIGNMENT' action = 'add'>";
                    strAml += "<source_id>" + source_id + "</source_id><related_id>";
                    strAml += "<Item type='IDENTITY' action='get' id='" + identityId + "'>";
                    strAml += "</Item></related_id></Item>";
                }
                strAml += "</AML>";
                var result = inn.applyAML(strAml);
                if (result.isError())
                {
                    strError = result.getErrorString();
                }
            }
            else
            {
                strError = activitys.getErrorString();
                return strError;
            }
            return strError;
        }


        /// <summary>
        /// 删除Activity的审核权限不包含admin
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="ItemId"></param>
        public static void DeleteActivityAuthById(Innovator inn, string ItemId, string tableName, List<string> names)
        {
            //根据任务名称获取任务
            Item activitys = ActivityDA.GetActivityByNames(inn, names, ItemId, tableName);
            //获取当前任务已经存在的处理人
            if (!activitys.isError())
            {
                for (var i = 0; i < activitys.getItemCount(); i++)
                {
                    var item = activitys.getItemByIndex(i);
                    string source_id = item.getProperty("id");
                    Item activityAssignments = ActivityAssignmentDA.GetActivityAssignment(inn, source_id);
                    string ids = "";
                    if (!activityAssignments.isError() && activityAssignments.getItemCount() > 0)
                    {
                        for (int index = 0; index < activityAssignments.getItemCount(); index++)
                        {
                            var activityAssignment = activityAssignments.getItemByIndex(index);
                            string id = activityAssignment.getProperty("id");

                            if (index != activityAssignments.getItemCount() - 1)
                            {
                                ids += id + ",";
                            }
                            else
                            {
                                ids += id;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(ids))
                    {
                        ActivityAssignmentDA.deleteActivityAssignment(inn, ids);
                    }
                }
            }
        }

        /// <summary>
        /// 添加活动审核
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="ItemId">数据ID</param>
        /// <param name="identityId">权限ID</param>
        /// <param name="names">节点名称列表</param>
        /// <param name="tableName">主表名称</param>
        /// <param name="voting_weight">权重</param>
        public static void AddActivityAuth(Innovator inn, string ItemId, string identityId, List<string> names, string tableName, int voting_weight = 100,string activityStatus= "Pending")
        {
            //根据任务名称获取任务
            Item activitys = ActivityDA.GetActivityByNames(inn, names, ItemId, tableName, activityStatus);
            if (!activitys.isError())
            {
                string strAml = "<AML>";
                for (var i = 0; i < activitys.getItemCount(); i++)
                {
                    var item = activitys.getItemByIndex(i);
                    string source_id = item.getProperty("id");
                    Item activityAssignments = ActivityAssignmentDA.GetActivityAssignment(inn, source_id);
                    strAml += "<Item type = 'ACTIVITY ASSIGNMENT' action = 'add'>";
                    strAml += "<voting_weight>" + voting_weight + "</voting_weight>";
                    strAml += "<source_id>" + source_id + "</source_id><related_id>";
                    strAml += "<Item type='IDENTITY' action='get' id='" + identityId + "'>";
                    strAml += "</Item></related_id></Item>";
                }
                strAml += "</AML>";
                var result = inn.applyAML(strAml);
            }
        }


        /// <summary>
        /// 判断活动是否关闭
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool ActivityIsClosed(Innovator inn, string id)
        {
            Item activityItem = ActivityDA.GetActivityById(inn, id);
            bool isClosed = false;
            if (!activityItem.isError() && activityItem.getItemCount() > 0)
            {
                string state = activityItem.getProperty("state");
                if(state== "Closed")
                {
                    isClosed= true;
                }
                else
                {
                    isClosed= false;
                }
            }
            return isClosed;
        }

        /// <summary>
        /// 添加节点审核
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="activityName"></param>
        /// <param name="tableName"></param>
        /// <param name="AuditStr"></param>
        public static void AddActivityAudit(Innovator inn, string id, string activityName, string tableName, List<string> AuditStr)
        {
            List<string> activityNames = new List<string> { activityName };
            List<string> ids = new List<string>();
            if (AuditStr != null && AuditStr.Count > 0)
            {
                foreach (var item in AuditStr)
                {
                    Item identity = IdentityDA.GetIdentityByKeyedName(inn, item);
                    if (!identity.isError() && identity.getItemCount() > 0)
                    {
                        string identityId = identity.getProperty("id");
                        ids.Add(identityId);
                    }
                }
            }
            ids = ids.Distinct().ToList();
            if (ids.Count > 0)
            {
                foreach (var identityId in ids)
                {
                    ActivityBll.AddActivityAuth(inn, id, identityId, activityNames, tableName);
                }
            }

        }
    }
}
