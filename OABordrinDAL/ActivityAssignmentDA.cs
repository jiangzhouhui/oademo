using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class ActivityAssignmentDA
    {
        public static void AddActivityAssignment(Innovator inn, string source_id, string related_id, int voting_weight)
        {
            string amlStr = "<AML><Item type='ACTIVITY ASSIGNMENT' action='add'><source_id>" + source_id + "</source_id><related_id>" + related_id + "</related_id><voting_weight>" + voting_weight + "</voting_weight></Item></AML>";
            var errorStr = inn.applyAML(amlStr).getErrorString();
        }


        public static Item GetActivityAssignment(Innovator inn, string source_id)
        {
            string strSql = "select g.* from ACTIVITY_ASSIGNMENT g inner join [innovator].[IDENTITY] t on g.RELATED_ID=t.id  where g.SOURCE_ID='" + source_id + "' and t.name!='Administrators'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }

        public static Item GetActivityAssignmentById(Innovator inn, string id)
        {
            string strSql = "select * from innovator.ACTIVITY_ASSIGNMENT where id='" + id + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }


        public static void deleteActivityAssignment(Innovator inn, string ids)
        {
            string amlStr = "<AML><Item type='ACTIVITY ASSIGNMENT' action='purge' idlist='" + ids + "'></Item></AML>";
            var result = inn.applyAML(amlStr);
        }


        public static void UpdateActivityAssignmentVotingWeight(Innovator inn, string id, int votingWeight)
        {
            string strSql = "update innovator.ACTIVITY_ASSIGNMENT set Voting_Weight='" + votingWeight + "' where id='" + id + "'";
            inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
        }

        /// <summary>
        /// 获取审核权限
        /// </summary>
        /// <param name="source_id">活动Id</param>
        /// <param name="related_id">人员权限ID</param>
        /// <returns></returns>
        public static ACTIVITY_ASSIGNMENT GetActivity_AssignmentByCondition(string source_id, string related_id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var obj = db.ACTIVITY_ASSIGNMENT.Where(x => x.SOURCE_ID == source_id && x.RELATED_ID == related_id).FirstOrDefault();
                return obj;
            }
        }


    }
}
