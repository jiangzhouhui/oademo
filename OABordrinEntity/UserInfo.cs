using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinEntity
{
    public class UserInfo
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string LoginName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string b_JobNumber { get; set; }

        public string HTTP_USER_AGENT { get; set; }

        public string UserIp { get; set; }

        public DateTime ExpireDate { get; set; }

        public string department { get; set; }

        
        /// <summary>
        /// 所属公司
        /// </summary>
        public string b_AffiliatedCompany { get; set; }

        public Innovator inn { get; set; }


        /// <summary>
        /// 语言
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 角色信息
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// 菜单权限
        /// </summary>
        public List<string> MemuAuth { get; set; }

        /// <summary>
        /// 委托权限信息
        /// </summary>
        public List<AgentAuthEntity> AgentAuth { get; set; }

        /// <summary>
        /// 委托创建时间
        /// </summary>
        public DateTime AgentCreateTime { get; set; }

    }
}
