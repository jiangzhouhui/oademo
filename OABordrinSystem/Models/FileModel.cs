using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinSystem.Models
{
    public class FileModel
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public string source_id { get; set; }

        /// <summary>
        /// 关系类Id
        /// </summary>
        public string relationId { get; set; }

        /// <summary>
        /// 文件编码类型
        /// </summary>
        public string mimeType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string comments { get; set; }

    }
}