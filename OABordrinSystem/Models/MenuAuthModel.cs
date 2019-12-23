using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class MenuAuthModel
    {

        /// <summary>
        /// 页面标识
        /// </summary>
        public string MenuIdentity { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string linkStr { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string ParentNode { get; set; }


        /// <summary>
        /// 节点样式
        /// </summary>
        public string ParentClass { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int sortNumber { get; set; }
    }
}