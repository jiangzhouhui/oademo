using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class BookingStaffingDA
    {
        public static void DeleteBookingStaffing(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='b_BookingStaffing' action='delete' id='" + id + "'></Item></AML>";
            var result = inn.applyAML(strAml);
        }

        /// <summary>
        /// 根据用户名称获取行政代订
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static B_BOOKINGSTAFFING GetBookingStaffingByUserName(string username)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
               return db.B_BOOKINGSTAFFING.Where(x => x.B_USERNAME == username).FirstOrDefault();
            }
        }


    }
}
