using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class TaxCodeConfigureBll
    {
        /// <summary>
        /// 根据税码获取科目
        /// </summary>
        /// <param name="b_TaxCode"></param>
        /// <returns></returns>
        public static string GeTaxCodeConfigureByText(string b_TaxCode)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var obj = db.B_TAXCODECONFIGURE.Where(x => x.B_TAXCODE.Equals(b_TaxCode)).FirstOrDefault();
                return obj.B_LEDGERACCOUNT;
            }
        }

    }
}
