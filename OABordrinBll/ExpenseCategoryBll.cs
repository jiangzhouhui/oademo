using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class ExpenseCategoryBll
    {
        /// <summary>
        /// 判断费用类别是否存在
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_CostName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsExistExpenseCategory(Innovator inn, string b_CostName, string id)
        {
            string strSql = "select * from innovator.b_ExpenseCategory where b_CostName=N'" + b_CostName + "' and id!='" + id + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            if (!returnItem.isError() && returnItem.getItemCount() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除费用类别
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item DeleteExpenseCategoryById(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_ExpenseCategory' action='delete' id='" + id + "'></Item></AML>";
            Item result = inn.applyAML(strAml);
            return result;
        }

        /// <summary>
        /// 获取费用类别根据名称
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Item GetExpenseCategoryByName(Innovator inn,string b_CostName)
        {
            string strSql = "select * from innovator.b_ExpenseCategory where b_CostName=N'" + b_CostName + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }


    }
}
