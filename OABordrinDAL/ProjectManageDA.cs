using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class ProjectManageDA
    {
        /// <summary>
        /// 获取项目管理根据名称
        /// </summary>
        public static bool GetProjectManageByName(Innovator inn, string name, string id)
        {

            string strSql = "select * from innovator.B_PROJECTMANAGE where b_projectname='" + name + "' and id!='" + id + "'";
            Item result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            //string strAml = "<AML><Item type='B_PROJECTMANAGE' action='get' where='id!=" + id + "'><b_projectname>" + name + "</b_projectname></Item></AML>";
            //var result = inn.applyAML(strAml);
            return result.getItemCount() > 0 ? true : false;
        }

        /// <summary>
        /// 是否存在项目编号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_ProjectRecordNo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool isExistProjectRecordNo(Innovator inn, string b_ProjectRecordNo, string id)
        {
            string strSql = "select * from innovator.B_PROJECTMANAGE where b_ProjectRecordNo='" + b_ProjectRecordNo + "' and id!='" + id + "'";
            Item result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return result.getItemCount() > 0 ? true : false;
        }


        /// <summary>
        /// 删除项目根据Id
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item DeleteProjectManageById(Innovator inn, string id)
        {
            string strAml = "<AML><Item type='B_PROJECTMANAGE' action='delete' id='" + id + "'></Item></AML>";
            Item result = inn.applyAML(strAml);
            return result;
        }


        /// <summary>
        /// 根据项目名称获取项目
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Item GetProjectManageByName(Innovator inn, string name)
        {
            string strAml = "<AML><Item type='B_PROJECTMANAGE' action='get'><b_projectname>" + name + "</b_projectname></Item></AML>";
            Item result = inn.applyAML(strAml);
            return result;
        }

        /// <summary>
        /// 根据项目名称获取多个项目
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetProjectManageNoBy(Innovator inn, string name)
        {
            string strAml = "<AML><Item type='B_PROJECTMANAGE' action='get'><b_projectname>" + name + "</b_projectname></Item></AML>";
            strAml = string.Format(strAml, name);
            var result = inn.applyAML(strAml);
            if (result.getItemCount() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
