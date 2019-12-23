using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class CompanyInfoBll
    {
        /// <summary>
        /// 根据代码 获取公司名称
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetCompanyNameByCode(Innovator inn, string code)
        {
            string companyName="";
            string strSql = "select b_CompanyName from innovator.b_CompanyInfo where b_CompanyCode=" + code;
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            if (!result.isError() && result.getItemCount() > 0)
            {
                 companyName = result.getItemByIndex(0).getProperty("b_companyname");
            }
            return companyName;
        }



    }
}
