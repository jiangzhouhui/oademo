using System;

namespace OABordrinSystem.Models
{
    public class HistoryModel
    {


        /// <summary>
        /// 单据状态
        /// </summary>
        public string ItemStatus { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string OperateStr { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Create_onStr { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string Created_on { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
    }
}