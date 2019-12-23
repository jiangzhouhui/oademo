using Aras.IOM;
using OABordrinDAL;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class OrganizationalStructureBll
    {
        /// <summary>
        /// 获取子节点集合
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_ParentNodeCode"></param>
        /// <param name="list"></param>
        public static void GetChildByParent(Innovator inn, string b_ParentNodeCode, List<B_ORGANIZATIONALSTRUCTURE> list, List<B_ORGANIZATIONALSTRUCTURE> dataList)
        {
            List<B_ORGANIZATIONALSTRUCTURE> newList = dataList.Where(x => x.B_PARENTNODECODE == b_ParentNodeCode).ToList();
            if (newList != null && newList.Count > 0)
            {
                for (var i = 0; i < newList.Count; i++)
                {
                    var item = newList[i];
                    list.Add(item);
                    GetChildByParent(inn, item.B_NODECODE, list, dataList);
                }
            }
        }

        /// <summary>
        /// 删除组织结构
        /// </summary>
        public static Item DeleteOrganizationalStructure(Innovator inn)
        {
            string strSql = "delete innovator.b_OrganizationalStructure";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }

        /// <summary>
        /// 获取结构数据
        /// </summary>
        /// <returns></returns>
        public static List<B_ORGANIZATIONALSTRUCTURE> GetOrganizationalStructureList()
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_ORGANIZATIONALSTRUCTURE.ToList();
            }
        }

        /// <summary>
        /// 根据ID 获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static B_ORGANIZATIONALSTRUCTURE GetOrganizationalStructureById(string id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_ORGANIZATIONALSTRUCTURE.Where(x => x.id == id).FirstOrDefault();
            }
        }


        /// <summary>
        /// 根据成本中心获取组织结构
        /// </summary>
        /// <param name="CostCenter"></param>
        /// <returns></returns>
        public static B_ORGANIZATIONALSTRUCTURE GetOrganizationalStructureByCostCenter(string CostCenter)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_ORGANIZATIONALSTRUCTURE.Where(x => x.B_COSTCENTER == CostCenter).FirstOrDefault();
            }
        }


    }
}
