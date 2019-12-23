using OABordrinCommon;
using OABordrinEntity;
using OABordrinDAL;
using System.Collections.Generic;
using OAEntitys;
using System.Runtime.Caching;
using System.Linq;
using Aras.IOM;

namespace OABordrinBll
{
    public static class UserBll
    {
        /// <summary>
        /// 保存用户信息
        /// </summary>
        public static void SaveUserInfoToCache(UserInfo user)
        {
            UserInfo accountEntities = MemoryCacheUtils.Get(user.LoginName) as UserInfo;
            if (accountEntities == null)
            {
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.Priority = CacheItemPriority.NotRemovable;
                MemoryCacheUtils.Set(user.LoginName, user, policy);
            }
        }

        /// <summary>
        /// 根据用户名称获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        public static UserInfo GetUserInfoByUserName(string userName)
        {
            UserInfo accountEntities = MemoryCacheUtils.Get(userName) as UserInfo;
            if (accountEntities == null)
            {
                USER userObJ = UserDA.GetUserByLoginName(userName);
                accountEntities = new UserInfo();
                if (userObJ != null)
                {
                    accountEntities.UserId = userObJ.ID;
                    accountEntities.UserName = userObJ.KEYED_NAME;
                    accountEntities.LoginName = userObJ.LOGIN_NAME;
                    accountEntities.Password = userObJ.PASSWORD;
                    accountEntities.Email = userObJ.EMAIL;
                    accountEntities.b_JobNumber = userObJ.B_JOBNUMBER;
                    accountEntities.b_AffiliatedCompany = userObJ.B_AFFILIATEDCOMPANY;
                    accountEntities.language = "Chinese";
                }
            }
            return accountEntities;
        }


        /// <summary>
        /// 获取OA所有的用户信息
        /// </summary>
        /// <param name="inn"></param>
        public static List<USER> GetAllUserInfo()
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                return db.USER.ToList();
            }

        }

        /// <summary>
        /// 根据用户名判断角色是否为CEO
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_Employee">用户名称</param>
        /// <returns></returns>
        public static bool IsCEObyUserName(Innovator inn, string b_Employee)
        {
            List<IDENTITY> identityList = new List<IDENTITY>();
            identityList.AddRange(IdentityDA.GetMemberByIdentityName("CEO"));
            int ncount = identityList.Where(x => x.KEYED_NAME.Trim() == b_Employee.Trim()).Count();
            if(ncount>0 || b_Employee== "Zhiwei Zhang (张志伟)")
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 判断CEO是否审核过
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_LineLeader">直属领导</param>
        /// <param name="b_DepartmentLeader">部门领导</param>
        /// <param name="b_DivisionVP">VP</param>
        /// <returns></returns>
        public static bool CeoBeforeIsAudit(Innovator inn, string b_LineLeader, string b_DepartmentLeader, string b_DivisionVP,string b_Employee)
        {
            USER employeeInfo = UserDA.GetUserByFirstName(b_Employee);
            List<IDENTITY> identityList = new List<IDENTITY>();
            if(employeeInfo!=null)
            {
                if(employeeInfo.B_CENTRE == "盛和")
                {
                    identityList = IdentityDA.GetMemberByIdentityName("GMSH");
                }
                else if(employeeInfo.B_CENTRE== "骏盛")
                {
                    identityList = IdentityDA.GetMemberByIdentityName("GMJS");
                }
                else
                {
                    identityList = IdentityDA.GetMemberByIdentityName("CEO");
                }
            }

            //判断CEO在之前是否审核过
            if (identityList != null && identityList.Count > 0 && (identityList.Where(x => x.KEYED_NAME.Trim() == b_LineLeader).Count() > 0 || identityList.Where(x => x.KEYED_NAME.Trim() == b_DepartmentLeader).Count() > 0 || identityList.Where(x => x.KEYED_NAME.Trim() == b_DivisionVP).Count() > 0))
            {
                return true;
            }
            return false;
        }








    }
}
