using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class WorkflowProcessPathDA
    {
        public static List<WORKFLOW_PROCESS_PATH> GetWorkflowProcessPathByActivityId(string activityId)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var listPath = (from g in db.WORKFLOW_PROCESS_PATH
                                join t in db.ACTIVITY on g.RELATED_ID equals t.ID
                                where g.SOURCE_ID == activityId && t.CLONED_AS == null
                                select g).ToList();
                return listPath;
            }
        }


        public static WORKFLOW_PROCESS_PATH GetWorkflowProcessPathById(string id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var path = (from g in db.WORKFLOW_PROCESS_PATH where g.ID == id select g).First();
                return path;
            }
        }


    }
}
