using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class RegionDA
    {
        /// <summary>
        /// 是否存在该地区
        /// </summary>
        public static bool isExistRegionByName(Innovator inn,string regionName)
        {
            string strSql = "select * from innovator.b_region where b_RegionName=N'"+ regionName + "'";
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
    }
}
