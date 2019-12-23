using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class OrganizationalStructureDA
    {
        /// <summary>
        /// 判断二级中心是否存在
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static bool IsExistCentre(Innovator inn, string nodeName)
        {
            string strSql = "select * from b_OrganizationalStructure where b_NodeName=N'" + nodeName + "' and  b_NodeLevel=2";

            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");

            if (!result.isError() && result.getItemCount() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 根据父节点代码  获取子节点
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="ParentCode"></param>
        /// <returns></returns>
        public static List<B_ORGANIZATIONALSTRUCTURE> GetChildByParentNodeCode(Innovator inn, string ParentCode)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_ORGANIZATIONALSTRUCTURE.Where(x => x.B_PARENTNODECODE == ParentCode).ToList();
            }
        }


        /// <summary>
        /// 根据参数获取节点信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="nodeName"></param>
        /// <param name="b_NodeLevel"></param>
        /// <returns></returns>
        public static Item GetOrganizationalStructureByParam(Innovator inn, string nodeName, int b_NodeLevel)
        {
            string strSql = "select * from b_OrganizationalStructure where b_NodeName=N'" + nodeName + "' and b_NodeLevel='" + b_NodeLevel + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return result;
        }

        /// <summary>
        /// 查询根据领导
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_NodePersonName"></param>
        /// <param name="b_NodeLevel"></param>
        /// <returns></returns>
        public static Item GetOrganizationalStructureByLeader(Innovator inn, string b_NodePersonName, int b_NodeLevel)
        {
            string strSql = "select * from b_OrganizationalStructure where b_NodePersonName=N'" + b_NodePersonName + "' and b_NodeLevel='" + b_NodeLevel + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return result;
        }


        /// <summary>
        /// 根据名称判断是否存在
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static Item GetOrganizationalStructureByParam(Innovator inn, string nodeName)
        {
            string strSql = "select * from b_OrganizationalStructure where b_NodeName=N'" + nodeName + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return result;
        }

        /// <summary>
        /// 根据节点代码  获取节点
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_NodeCode"></param>
        /// <returns></returns>
        public static Item GetOrganizationalStructureByNodeCode(Innovator inn,string b_NodeCode)
        {
            string strSql = "select * from b_OrganizationalStructure where b_NodeCode='" + b_NodeCode + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return result;
        }
    }
}
