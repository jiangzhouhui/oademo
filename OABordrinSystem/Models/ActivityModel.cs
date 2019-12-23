using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class ActivityModel
    {
        public string Id { get; set; }

        public string Keyed_Name { get; set; }

        public int? X { get; set; }

        public int? Y { get; set; }

        public List<ProcessPathModel> ProcessPaths { get; set; }

    }

    public class ProcessPathModel
    {
        public string Id { get; set; }

        public string Lable { get; set; }

        public string segments { get; set; }

        public int? X { get; set; }

        public int? Y { get; set; }

        public string Lable_zc { get; set; }

        public int? RelatedX { get; set; }

        public int? RelatedY { get; set; }
    }

    public class CompleteActivityModel
    {


        public string itemId { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public string activityId { get; set; }

        /// <summary>
        /// 授权ID
        /// </summary>
        public string activityAssignmentId { get; set; }

        /// <summary>
        ///  单据编号
        /// </summary>
        public string recordNo { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string linkStr { get; set; }

        /// <summary>
        /// 路线ID
        /// </summary>
        public string pathId { get; set; }

        /// <summary>
        /// 路线名称
        /// </summary>
        public string pathName { get; set; }


        /// <summary>
        /// 委托人 
        /// </summary>
        public string delegateToName { get; set; }


        /// <summary>
        /// 委托人Id
        /// </summary>
        public string delegateToId { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string comments { get; set; }


        /// <summary>
        /// 人员列表
        /// </summary>
        public string PersonList { get; set; }

        /// <summary>
        /// 操作的表格
        /// </summary>
        public string operateTable { get; set; }

    }


}