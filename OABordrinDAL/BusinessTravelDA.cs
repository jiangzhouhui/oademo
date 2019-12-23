using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class BusinessTravelDA
    {
        /// <summary>
        /// 获取出差单根据申请人，并且为未报销的数据
        /// </summary>
        /// <param name="b_Employee"></param>
        /// <returns></returns>
        public static List<B_BUSINESSTRAVEL> GetBusinessTravelByEmployee(string b_Employee)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var list = (from g in db.B_BUSINESSTRAVEL
                            join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                            join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                            where y.STATE == "Closed" && g.B_EMPLOYEE == b_Employee && g.B_ISREIMBURSEMENT == 0
                            select g).ToList();
                //db.B_BUSINESSTRAVEL.Where(x => x.B_EMPLOYEE == b_Employee && x.B_ISREIMBURSEMENT==0).ToList();
                return list;
            }
        }

        /// <summary>
        /// 根据出差单编号获取数据集合
        /// </summary>
        /// <param name="recordNo"></param>
        /// <returns></returns>
        public static B_BUSINESSTRAVEL GetBusinessTravelByParam(string recordNo,string b_Employee)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var obj = (from g in db.B_BUSINESSTRAVEL
                           join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                           join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                           where y.STATE == "Closed" && g.B_EMPLOYEE == b_Employee && g.B_ISREIMBURSEMENT == 0 && g.B_DOCUMENTNO == recordNo
                           select g).FirstOrDefault();


                return obj;
            }
        }

        /// <summary>
        /// 修改报销状态
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_IsReimbursement"></param>
        /// <param name="id"></param>
        public static void UpdateBusinessTravelIsReimbursement(Innovator inn,string b_IsReimbursement,string b_BTRecordNo)
        {
            string sqlStr = "update b_BusinessTravel set b_IsReimbursement='"+ b_IsReimbursement + "' where b_documentno='" + b_BTRecordNo + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + sqlStr + "</sqlCommend>");
        }
    }
}
