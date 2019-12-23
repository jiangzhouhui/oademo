using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class AgentSetDA
    {
        /// <summary>
        /// 根据ID删除委托
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item DeleteAgentSetById(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_AgentSet' action='delete' id='" + id + "'></Item></AML>";
            Item result = inn.applyAML(strAml);
            return result;
        }

        /// <summary>
        /// 根据当前时间获取委托
        /// </summary>
        /// <returns></returns>
        public static List<B_AGENTSET> GetAgentSetByUserName(string userName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var list = db.B_AGENTSET.Where(x => x.B_AGENTNAME == userName && x.B_ISVALID=="1").ToList();
                return list;
            }
        }
    }
}
