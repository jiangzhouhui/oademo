using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class MenuAuthManageModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Is_Alias
        /// </summary>
        public string Is_Alias { get; set; }

        /// <summary>
        /// 身份
        /// </summary>
        public string KEYED_NAME { get; set; }

        /// <summary>
        /// 权限数据
        /// </summary>
        public string AuthStr { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<string> AuthList { get; set; }

    }
}