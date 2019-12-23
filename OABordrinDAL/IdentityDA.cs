using Aras.IOM;
using OABordrinCommon;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class IdentityDA
    {
        /// <summary>
        /// 获取当前登陆人的角色权限
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static List<string> GetIdentityByUserName(string userName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                List<string> list = (from g in db.IDENTITY
                                     join t in db.MEMBER on g.ID equals t.SOURCE_ID
                                     join y in db.IDENTITY on t.RELATED_ID equals y.ID
                                     where g.IS_ALIAS == "0" && y.IS_ALIAS == "1" && y.NAME == userName
                                     select g).Select(x => x.NAME).ToList();
                return list;
            }
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <param name="innovator"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static List<string> getIdentityListByUserID(Innovator innovator, string userid)
        {
            //'=========================================================================================== 
            Innovator inn = innovator;
            int i;
            string user_id = userid;
            StringCollection identity_id_list = new StringCollection();
            //'================= ==========================================================================
            //'1.取出使用者的 User Identity ID
            #region 取出使用者的 User Identity ID
            Item a = inn.newItem("Alias", "get");
            a.setAttribute("select", "id, related_id(id, name)");
            a.setProperty("source_id", user_id);
            Item r_a = a.apply();
            if (r_a.isError())
            {
                return null;
            }
            else
            {
                if (r_a.isCollection())
                {
                    for (i = 0; i < r_a.getItemCount() - 1; i++)
                    {
                        if (!identity_id_list.Contains(r_a.getItemByIndex(i).getRelatedItem().getProperty("id")))
                        {
                            identity_id_list.Add(r_a.getItemByIndex(i).getRelatedItem().getProperty("id"));
                        }
                    }
                }
                else
                {
                    identity_id_list.Add(r_a.getRelatedItem().getProperty("id"));
                }
            }
            #endregion
            //'===========================================================================================
            //'2.取得 World Identity ID
            #region 取得 World Identity ID
            Item ident = inn.newItem("Identity", "get");
            ident.setAttribute("select", "id, name");
            ident.setProperty("name", "World");
            Item r_ident = ident.apply();
            if (r_ident.isError())
            {
                return null;
            }
            else
            {
                identity_id_list.Add(r_ident.getID());
            }
            ident = null;
            r_ident = null;
            #endregion
            //'===========================================================================================
            //'3.取得 所屬的所有 Group Identity
            #region 取得 所屬的所有 Group Identity
            Item member;
            Item result;
            //'3-1.將 identity_id_list 從 StringCollection 轉成 String
            StringBuilder builder = new StringBuilder();
            StringEnumerator enumerator = identity_id_list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                if (current != null)
                {
                    builder.Append("'");
                    builder.Append(current.Replace("'", "''"));
                    builder.Append("',");
                }
            }
            StringBuilder new_builder = new StringBuilder(builder.ToString(0, (builder.Length - 1)));
            bool flag = true;

            #endregion
            //'3-2.取出所有 Identity 的父階
            #region 取出所有 Identity 的父階
            do
            {
                member = inn.newItem("Member", "get");
                member.setAttribute("select", "id, source_id(id, name)");
                member.setAttribute("where", "(related_id in (" + new_builder.ToString() + ")) and (source_id not in (" + new_builder.ToString() + "))");
                result = member.apply();
                if (result.isError())
                {
                    flag = false;
                }
                else
                {
                    if (result.isCollection())
                    {
                        for (i = 0; i < result.getItemCount() - 1; i++)
                        {
                            if (!identity_id_list.Contains(result.getItemByIndex(i).getPropertyItem("source_id").getProperty("id")))
                            {
                                identity_id_list.Add(result.getItemByIndex(i).getPropertyItem("source_id").getProperty("id"));
                            }
                        }
                    }
                    else
                    {
                        if (!identity_id_list.Contains(result.getPropertyItem("source_id").getID()))
                        {
                            identity_id_list.Add(result.getPropertyItem("source_id").getID());
                        }
                    }
                }
                builder = null;
                new_builder = null;
                builder = new System.Text.StringBuilder();
                enumerator = identity_id_list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (current != null)
                    {
                        builder.Append("'");
                        builder.Append(current.Replace("'", "''"));
                        builder.Append("',");
                    }
                }
                new_builder = new StringBuilder(builder.ToString(0, (builder.Length - 1)));
                member = null;
                result = null;
            } while (flag);
            #endregion

            //'3-3.將 identity_id_list 從 StringCollection 轉成 String
            #region 將 identity_id_list 從 StringCollection 轉成 String
            builder = null;
            new_builder = null;
            builder = new StringBuilder();
            enumerator = identity_id_list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string current = enumerator.Current;
                if (current != null)
                {
                    builder.Append("'");
                    builder.Append(current.Replace("'", "''"));
                    builder.Append("',");
                }
            }
            #endregion
            new_builder = new StringBuilder(builder.ToString(0, (builder.Length - 1)));

            List<string> roleList = new List<string>();
            if (!string.IsNullOrEmpty(new_builder.ToString()))
            {
                new_builder = new_builder.Replace("'", "");
                roleList = new_builder.ToString().Split(',').ToList();
            }
            return roleList;
        }


        /// <summary>
        /// 获取角色管理列表
        /// </summary>
        /// <returns></returns>
        public static Item GetRoleManageList(Innovator innovator)
        {
            string amlStr = "<AML><Item type='IDENTITY' action='get'><is_alias>0</is_alias><Relationships><Item type='MEMBER' action='get'></Item></Relationships></Item></AML>";
            var result = innovator.applyAML(amlStr);
            return result;
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public static List<IDENTITY> GetRoleManageList(out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var datas = (from g in db.IDENTITY
                             where g.IS_ALIAS == "0" && (g.NAME.Contains(searchValue) || g.DESCRIPTION.Contains(searchValue)) && g.DESCRIPTION.Contains("OASystem")
                             select g);

                //排序
                if (para.sSortType == "asc")
                {
                    datas = Common.OrderBy(datas, para.iSortTitle, false);
                }
                else
                {
                    datas = Common.OrderBy(datas, para.iSortTitle, true);
                }

                total = datas.Count();
                //分页
                datas = datas.Skip(para.iDisplayStart).Take(para.iDisplayLength);
                return datas.ToList();
            }
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <returns></returns>
        public static List<IDENTITY> GetIdentityList(out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var datas = (from g in db.IDENTITY
                             where g.NAME.Contains(searchValue)
                             select g);
                //排序
                if (para.sSortType == "asc")
                {
                    datas = Common.OrderBy(datas, para.iSortTitle, false);
                }
                else
                {
                    datas = Common.OrderBy(datas, para.iSortTitle, true);
                }

                total = datas.Count();
                //分页
                datas = datas.Skip(para.iDisplayStart).Take(para.iDisplayLength);
                return datas.ToList();
            }
        }



        /// <summary>
        /// 获取角色对象
        /// </summary>
        /// <param name="innovator"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetRoleManageById(Innovator innovator, string id)
        {
            string amlStr = "<AML><Item type='IDENTITY' action='get' id='" + id + "'><is_alias>0</is_alias><Relationships><Item type='MEMBER' action='get'></Item></Relationships></Item></AML>";
            return innovator.applyAML(amlStr);
        }

        /// <summary>
        /// 获取角色对象
        /// </summary>
        /// <param name="innovator"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetIdentityById(Innovator innovator, string id)
        {
            string amlStr = "<AML><Item type='IDENTITY' action='get' id='" + id + "'></Item></AML>";
            return innovator.applyAML(amlStr);
        }



        /// <summary>
        /// 添加角色
        /// </summary>
        /// <returns></returns>
        public static Item InsertRoleManage(Innovator innovator, string amlStr)
        {
            var result = innovator.applyAML(amlStr);
            return result;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="innovator"></param>
        /// <param name="amlStr"></param>
        /// <returns></returns>
        public static Item DeleteRoleManage(Innovator innovator, string id)
        {
            string amlStr = "<AML><Item type='IDENTITY' action='delete' id='" + id + "'></Item></AML>";
            var result = innovator.applyAML(amlStr);
            return result;
        }


        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static List<IDENTITY> GetMemberById(string sourceId)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var datas = (from g in db.MEMBER
                             join t in db.IDENTITY on g.RELATED_ID equals t.ID
                             where g.SOURCE_ID == sourceId
                             select t).ToList();
                return datas;
            }
        }


        /// <summary>
        /// 获取成员名称
        /// </summary>
        /// <param name="IdentityName"></param>
        /// <returns></returns>
        public static List<IDENTITY> GetMemberByIdentityName(string IdentityName)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                List<IDENTITY> list = (from g in db.IDENTITY
                                       join t in db.MEMBER on g.ID equals t.SOURCE_ID
                                       join y in db.IDENTITY on t.RELATED_ID equals y.ID
                                       where g.NAME == IdentityName
                                       select y).ToList();
                return list;
            }
        }



        /// <summary>
        /// 判断是否已经存在
        /// </summary>
        /// <returns></returns>
        public static bool ValidIsExistByKeyed_Name(string keyed_name, string id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var obj = (from g in db.IDENTITY
                           where g.KEYED_NAME == keyed_name && g.ID != id
                           select g).FirstOrDefault();
                return obj != null ? true : false;
            }
        }

        /// <summary>
        /// 根据Keyed_Name获取列表
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="keyed_name"></param>
        /// <returns></returns>
        public static Item GetIdentityByKeyedName(Innovator inn, string keyed_name)
        {
            string amlStr = "<AML><Item type='IDENTITY' action='get'><keyed_name>" + keyed_name + "</keyed_name></Item></AML>";
            return inn.applyAML(amlStr);
        }


        /// <summary>
        /// 根据条件获取身份
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="keyed_name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Item GetIdentityByCondition(Innovator inn, string id, string keyed_name)
        {
            string strSql = "select count(*) as ncount from innovator.[IDENTITY] where keyed_name='" + keyed_name + "' and id!='" + id + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }


        /// <summary>
        /// 获取当前任务的操作权限
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static Item GetIdentityByActivityId(Innovator inn, string activityId)
        {
            string strSql = "select t.* from ACTIVITY_ASSIGNMENT g inner join [innovator].[IDENTITY] t on g.RELATED_ID=t.id  where g.SOURCE_ID='" + activityId + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }

        /// <summary>
        /// 获取当前任务的操作权限
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public static Item GetIdentityByActivityId(Innovator inn, string activityId, bool isClose)
        {
            string strSql = "select t.* from ACTIVITY_ASSIGNMENT g inner join [innovator].[IDENTITY] t on g.RELATED_ID=t.id  where g.SOURCE_ID='" + activityId + "' and g.CLOSED_BY is null and g.VOTING_WEIGHT>0";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }


        /// <summary>
        /// 获取权限子节点
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="identityId"></param>
        /// <returns></returns>
        public static Item GetChildIdentity(Innovator inn, string identityId)
        {
            string strSql = "select y.* from [innovator].[IDENTITY] g inner join [innovator].[MEMBER] t on g.id=t.source_id inner join [innovator].[IDENTITY] y on t.related_id=y.id where g.id='" + identityId + "'";
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }

        /// <summary>
        /// 根据名称地区，获取角色权限
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="keyed_name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Item GetIdentityByParam(Innovator inn, string keyed_name, string description)
        {
            string strSql = "select * from innovator.[IDENTITY] where keyed_name=N'" + keyed_name + "'";
            if (!string.IsNullOrEmpty(description))
            {
                strSql += "and innovator.[IDENTITY].[DESCRIPTION]=N'" + description + "'";
            }
            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return returnItem;
        }


        /// <summary>
        /// 根据审核的ID  获取人员IdentityId
        /// </summary>
        /// <param name="ActivityAssignmentId"></param>
        /// <returns></returns>
        public static IDENTITY GetIdentityByActivityAssignmentId(string ActivityAssignmentId)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                var identity = (from g in db.ACTIVITY_ASSIGNMENT
                                join p in db.IDENTITY on g.RELATED_ID equals p.ID
                                where g.ID == ActivityAssignmentId
                                select p).FirstOrDefault();
                return identity;
            }
        }

    }
}
