using Aras.IOM;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class WorkFlowDA
    {
        public static List<WORKFLOW_MAP> GetWorkFlowMap()
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var List = db.WORKFLOW_MAP.ToList();
                return List;
            }
        }

        /// <summary>
        /// 获取工作流根据数据ID
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        public static List<ACTIVITY> GetWorkFlowActivityByOrderId(string dataId)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var data = (from g in db.WORKFLOW
                            join t in db.WORKFLOW_PROCESS on g.RELATED_ID equals t.ID
                            join y in db.WORKFLOW_PROCESS_ACTIVITY on t.ID equals y.SOURCE_ID
                            join u in db.ACTIVITY on y.RELATED_ID equals u.ID
                            where g.SOURCE_ID == dataId
                            select u).ToList();
                return data;
            }
        }

        /// <summary>
        /// 获取工作流路线根据SourceId
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static List<WORKFLOW_PROCESS_PATH> GetProcessPathBySourceId(string sourceId)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var list = (from g in db.WORKFLOW_PROCESS_PATH
                            where g.SOURCE_ID == sourceId
                            select g).ToList();
                return list;
            }
        }


        /// <summary>
        /// 根据流程图名称获取流程图任务
        /// </summary>
        /// <param name="keyedName"></param>
        /// <returns></returns>
        public static List<ACTIVITY_TEMPLATE> GetWorkFlowMapActivityByKeyedName(string keyedName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var data = (from g in db.WORKFLOW_MAP
                            join t in db.WORKFLOW_MAP_ACTIVITY on g.ID equals t.SOURCE_ID
                            join u in db.ACTIVITY_TEMPLATE on t.RELATED_ID equals u.ID
                            where g.KEYED_NAME == keyedName
                            select u).ToList();
                return data;
            }
        }

        /// <summary>
        /// 获取流程图路径
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static List<WORKFLOW_MAP_PATH> GetWorkMapPathBySourceId(string sourceId)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var list = (from g in db.WORKFLOW_MAP_PATH where g.SOURCE_ID == sourceId select g).ToList();
                return list;
            }
        }


        /// <summary>
        /// 获取审核对象根据委托
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="activityAssignmentId"></param>
        public static ACTIVITY_ASSIGNMENT GetInnvatorByAgent(Innovator inn, UserInfo userInfo, string activityId, string activityAssignmentId, ref string delegateName)
        {
            ACTIVITY_ASSIGNMENT obj = null;
            AgentAuthEntity agentAuth = null;
            IDENTITY dentity = IdentityDA.GetIdentityByActivityAssignmentId(activityAssignmentId);
            if (userInfo.AgentAuth != null)
            {
                agentAuth = userInfo.AgentAuth.Where(x => x.agentRoles.Contains(dentity.ID)).FirstOrDefault();
                if (agentAuth != null)
                {
                    delegateName = agentAuth.delegateName;
                }
            }

            //当人员信息权限中没有当前Identity权限 并且  Identity在委托的的权限中存在时！则为委托审核
            if (!userInfo.Roles.Contains(dentity.ID) && agentAuth != null)
            {
                //获取当前人员的Id
                Item identity = IdentityDA.GetIdentityByKeyedName(inn, userInfo.UserName);
                string identityId = identity.getProperty("id");

                var ActivityAssignmentItem = ActivityAssignmentDA.GetActivityAssignmentById(inn, activityAssignmentId);
                int voting_weight = int.Parse(ActivityAssignmentItem.getItemByIndex(0).getProperty("voting_weight"));
                //修改当前审核权限为0
                ActivityAssignmentDA.UpdateActivityAssignmentVotingWeight(inn, activityAssignmentId, 0);
                //插入委托人员权限
                ActivityAssignmentDA.AddActivityAssignment(inn, activityId, identityId, voting_weight);
                //获取插入权限的ID
                obj = ActivityAssignmentDA.GetActivity_AssignmentByCondition(activityId, identityId);
            }
            return obj;
        }


    }
}
