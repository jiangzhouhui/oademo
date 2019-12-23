using Aras.IOM;
using OABordrinCommon;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Caching;
using System.Text;


namespace OABordrinBll
{
    public static class ProjectManageBll
    {

        /// <summary>
        /// 获取OA所有的用户信息
        /// </summary>
        /// <param name="inn"></param>
        public static List<B_PROJECTMANAGE> GetAllProjectInfo()
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_PROJECTMANAGE.ToList();
            }

        }
    }
}
