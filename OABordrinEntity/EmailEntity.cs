using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinEntity
{
    public class EmailEntity
    {
        /// <summary>
        /// ItemId
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ApplicantId { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>
        /// 申请部门
        /// </summary>
        public string ApplicantDepartment { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string RecordNo { get; set; }

        /// <summary>
        /// 出差单号
        /// </summary>
        public string BTRecordNo { get; set; }


    }
}
