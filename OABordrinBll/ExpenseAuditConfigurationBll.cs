using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class ExpenseAuditConfigurationBll
    {
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public static List<B_EXPENSEAUDITCONFIGURATION> GetAllExpenseAuditConfiguration()
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_EXPENSEAUDITCONFIGURATION.ToList();
            }
        }


        /// <summary>
        /// 根据ID获取数据
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static B_EXPENSEAUDITCONFIGURATION GetExpenseAuditConfigurationById(Innovator inn, string id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_EXPENSEAUDITCONFIGURATION.Where(x => x.id == id).FirstOrDefault();
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteExpenseAuditConfiguration(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_ExpenseAuditConfiguration' action='delete' id='" + id + "'></Item></AML>";
            var result = inn.applyAML(strAml);
        }


        /// <summary>
        /// 根据角色名称、公司代码、项目名称获取审核配置
        /// </summary>
        /// <param name="b_roleName"></param>
        /// <param name="b_CompanyCode"></param>
        /// <param name="b_ProjectName"></param>
        public static B_EXPENSEAUDITCONFIGURATION GetExpenseAuditConfigurationByCondition(string b_roleName, string b_CompanyCode, string b_ProjectName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.B_EXPENSEAUDITCONFIGURATION.Where(x => x.B_ROLENAME == b_roleName && x.B_COMPANYCODE == b_CompanyCode && x.B_PROJECTNAME == b_ProjectName).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据角色名称、公司代码、成本中心
        /// </summary>
        /// <param name="b_roleName"></param>
        /// <param name="b_CompanyCode"></param>
        /// <param name="b_CostCenter"></param>
        /// <returns></returns>
        public static B_EXPENSEAUDITCONFIGURATION GetExpenseAuditConfigurationByNonProject(string b_roleName, string b_CompanyCode, string b_CostCenter, List<B_EXPENSEAUDITCONFIGURATION> datalist)
        {
            string CompanyCodeStr = b_CompanyCode.Substring(0, b_CompanyCode.IndexOf('(')).Trim();
            b_CostCenter = b_CostCenter.Substring(CompanyCodeStr.Length, b_CostCenter.Length - CompanyCodeStr.Length);
            return datalist.Where(x => x.B_ROLENAME == b_roleName && x.B_COMPANYCODE == b_CompanyCode && x.B_COSTCENTERS.Split(',').Contains(b_CostCenter)).FirstOrDefault();
        }

        /// <summary>
        /// 获取项目费用审核配置
        /// </summary>
        /// <param name="b_roleName"></param>
        /// <param name="b_CompanyCode"></param>
        /// <param name="b_ProjectNames"></param>
        /// <param name="datalist"></param>
        /// <returns></returns>
        public static List<B_EXPENSEAUDITCONFIGURATION> GetExpenseAuditConfigurationByProject(string b_roleName, string b_CompanyCode, List<string> b_ProjectNames, List<B_EXPENSEAUDITCONFIGURATION> datalist)
        {
            return datalist.Where(x => x.B_ROLENAME == b_roleName && x.B_COMPANYCODE == b_CompanyCode && b_ProjectNames.Contains(x.B_PROJECTNAME)).ToList();
        }


        /// <summary>
        /// 删除审核配置表
        /// </summary>
        /// <param name="inn"></param>
        /// <returns></returns>
        public static Item DeleteExpenseAuditConfiguration(Innovator inn)
        {
            string strSql = "delete innovator.b_ExpenseAuditConfiguration";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }
    }
}
