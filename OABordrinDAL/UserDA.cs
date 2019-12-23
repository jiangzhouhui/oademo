using Aras.IOM;
using OAEntitys;
using System.Collections.Generic;
using System.Linq;


namespace OABordrinDAL
{
    public class UserDA
    {
        /// <summary>
        /// 根据登陆名称获取User信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static USER GetUserByLoginName(string loginName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var userObj = (from g in db.USER
                               where g.LOGIN_NAME == loginName && g.LOGON_ENABLED=="1"
                               select g).FirstOrDefault();

                return userObj;
            }
        }

        /// <summary>
        /// 根据用户全称获取用户对象
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public static USER GetUserByFirstName(string firstName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var userObj = (from g in db.USER
                               where g.FIRST_NAME == firstName
                               select g).FirstOrDefault();
                return userObj;
            }
        }

        /// <summary>
        /// 根据KeyedName获取对象
        /// </summary>
        /// <param name="keyedName"></param>
        /// <returns></returns>
        public static IDENTITY GetUserByKeyedName(string keyedName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var identityObj = (from g in db.IDENTITY where g.KEYED_NAME == keyedName select g).FirstOrDefault();
                return identityObj;
            }
        }


        /// <summary>
        /// 验证用户是否存在
        /// </summary>
        /// <returns></returns>
        public static bool ValidUserIsExist(Innovator inn, string name)
        {
            string amlStr = "<AML><Item type='USER' action='get'><keyed_name>{0}</keyed_name></Item></AML>";
            amlStr = string.Format(amlStr, name);
            var result = inn.applyAML(amlStr);
            if (result.getItemCount() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证域登录帐号是否存在
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static bool ValidLoginNameIsExist(Innovator inn, string loginName)
        {
            string amlStr = "<AML><Item type='USER' action='get'><login_name>{0}</login_name></Item></AML>";
            amlStr = string.Format(amlStr, loginName);
            var result = inn.applyAML(amlStr);
            if (result.getItemCount() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据 Identity 获取User
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="IdentityId"></param>
        /// <returns></returns>
        public static Item GetUserByIdentityId(Innovator inn, string IdentityId)
        {
            string amlStr = "<AML><Item type='USER' action='get'><owned_by_id>{0}</owned_by_id></Item></AML>";
            amlStr = string.Format(amlStr, IdentityId);
            var result = inn.applyAML(amlStr);
            return result;
        }

        /// <summary>
        /// 根据Identity 获取Email
        /// </summary>
        /// <param name="identitys"></param>
        public static void GetEmailByIdentitys(Innovator inn, Item identitys, List<string> listEmail, List<string> names)
        {
            if (!identitys.isError() && identitys.getItemCount() > 0)
            {
                for (int i = 0; i < identitys.getItemCount(); i++)
                {
                    Item identityItem = identitys.getItemByIndex(i);
                    string isAlias = identityItem.getProperty("is_alias");
                    string id = identityItem.getProperty("id");
                    if (isAlias == "1")
                    {
                        //根据Id 获取邮箱账户
                        Item userItem = UserDA.GetUserByIdentityId(inn, id);
                        string email = userItem.getProperty("email");
                        string name = userItem.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(name) && names.IndexOf(name) < 0 && name != "Innovator Admin")
                        {
                            names.Add(name);
                        }
                        if (!string.IsNullOrEmpty(email) && listEmail.IndexOf(email) < 0)
                        {
                            listEmail.Add(email);
                        }
                    }
                    else
                    {

                        Item childIdentity = IdentityDA.GetChildIdentity(inn, id);
                        GetEmailByIdentitys(inn, childIdentity, listEmail, names);
                    }
                }
            }
        }


        /// <summary>
        /// 根据Identity 获取Email
        /// </summary>
        /// <param name="identitys"></param>
        public static void GetNamesByIdentitys(Innovator inn, Item identitys, ref string names)
        {
            if (!identitys.isError() && identitys.getItemCount() > 0)
            {
                for (int i = 0; i < identitys.getItemCount(); i++)
                {
                    Item identityItem = identitys.getItemByIndex(i);
                    string isAlias = identityItem.getProperty("is_alias");
                    string id = identityItem.getProperty("id");
                    if (isAlias == "1")
                    {
                        //根据Id 获取邮箱账户
                        Item userItem = UserDA.GetUserByIdentityId(inn, id);
                        string name = userItem.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(name) && names.IndexOf(name) < 0 && name != "Innovator Admin")
                        {
                            names += name + "、";
                        }
                    }
                    else
                    {

                        Item childIdentity = IdentityDA.GetChildIdentity(inn, id);
                        GetNamesByIdentitys(inn, childIdentity, ref names);
                    }
                }
            }
        }






    }
}
