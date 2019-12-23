using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class WorkFlowVersionManagementDA
    {
        /// <summary>
        /// 获取流程版本 根据版本号
        /// </summary>
        /// <param name="versionNo"></param>
        /// <returns></returns>
        public static B_WORKFLOWVERSIONMANAGEMENT GetWorkFlowVersionByVersionNo(string versionNo)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var obj = db.B_WORKFLOWVERSIONMANAGEMENT.Where(x => x.B_VERSIONNO == versionNo).FirstOrDefault();
                return obj;
            }
        }
    }
}
