using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class AgentSetBll
    {
        /// <summary>
        /// 根据用户名称获取相关的委托
        /// </summary>
        /// <param name="UserName">用户名称</param>
        /// <param name="moduleName">模块名称</param>
        /// <returns></returns>
        public static List<AgentSetEntity> GetAgentSetByUserName(string UserName, string moduleName = "")
        {
            //根据用户名获取 当前时间有效的委托
            var list = AgentSetDA.GetAgentSetByUserName(UserName);
            if (!string.IsNullOrEmpty(moduleName) && list != null && list.Count > 0)
            {
                list = list.Where(x => x.B_AGENTCONTENT.Split(';').Contains(moduleName)).ToList();
            }

            List<AgentSetEntity> nlist = new List<AgentSetEntity>();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    AgentSetEntity model = new AgentSetEntity();
                    model.b_DelegateName = item.B_DELEGATENAME;
                    model.b_AgentName = item.B_AGENTNAME;
                    string startDate = !string.IsNullOrEmpty(item.B_STARTDATEMINUTE) ? item.B_STARTDATE + " " + item.B_STARTDATEMINUTE : item.B_STARTDATE;
                    string endDate = !string.IsNullOrEmpty(item.B_ENDDATEMINUTE) ? item.B_ENDDATE + " " + item.B_ENDDATEMINUTE : item.B_ENDDATE;
                    model.b_StartDate = DateTime.Parse(startDate);
                    model.b_EndDate = DateTime.Parse(endDate);
                    model.b_AgentContent = item.B_AGENTCONTENT;
                    nlist.Add(model);
                }

                nlist = nlist.Where(x => x.b_StartDate <= DateTime.Now && x.b_EndDate >= DateTime.Now).ToList();
            }
            return nlist;
        }


        /// <summary>
        /// 获取委托的权限信息
        /// </summary>
        /// <param name="UserName">用户名称</param>
        /// <param name="moduleName">模块名称</param>
        public static void GetAgentRoles(Innovator inn, UserInfo Userinfo, List<AgentSetEntity> list, string moduleName = "")
        {
            //判断当前该模块的委托信息是否已经获取
            //List<string> agentRoles = new List<string>();
            List<AgentAuthEntity> AgentAuthList = new List<AgentAuthEntity>();
            //var list = GetAgentSetByUserName(Userinfo.UserName, moduleName);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    AgentAuthEntity agentAuth = new AgentAuthEntity();
                    //根据名称查询用户
                    USER user = UserDA.GetUserByFirstName(item.b_DelegateName);
                    List<string> listRoles = IdentityDA.getIdentityListByUserID(inn, user.ID);
                    //agentRoles.AddRange(listRoles);
                    agentAuth.delegateName = item.b_DelegateName;
                    agentAuth.agentRoles = listRoles;
                    if (!string.IsNullOrEmpty(item.b_AgentContent))
                    {
                        agentAuth.moduleNames = item.b_AgentContent.Split(';').Where(x => x != "").ToList();
                    }
                    if (AgentAuthList.Where(x => x.delegateName == agentAuth.delegateName).ToList().Count == 0)
                    {
                        AgentAuthList.Add(agentAuth);
                    }
                }
                //将数据插入缓存
                Userinfo.AgentAuth = AgentAuthList;
                Userinfo.AgentCreateTime = DateTime.Now;
                //MemoryCacheUtils.Clear(Userinfo.LoginName);
                //CacheItemPolicy policy = new CacheItemPolicy();
                //policy.Priority = CacheItemPriority.NotRemovable;
                //MemoryCacheUtils.Set(Userinfo.LoginName, Userinfo, policy);
            }
            //return agentRoles;
        }

        /// <summary>
        /// 获取授权权限
        /// </summary>
        /// <param name="Userinfo"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static List<string> GetAgentRoles(UserInfo Userinfo, string moduleName)
        {
            List<string> roles = new List<string>();
            if (Userinfo.AgentAuth != null)
            {
                List<AgentAuthEntity> list = Userinfo.AgentAuth.Where(x => x.moduleNames.Contains(moduleName)).ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        roles.AddRange(item.agentRoles);
                    }
                }
            }
            return roles.Distinct().ToList();
        }
    }
}
