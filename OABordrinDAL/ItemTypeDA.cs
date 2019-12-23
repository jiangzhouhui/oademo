using Aras.IOM;
using OABordrinCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class ItemTypeDA
    {
        /// <summary>
        /// 根据名称获取ItemType
        /// </summary>
        /// <returns></returns>
        public static Item GetItemTypeByName(Innovator innovator, string name)
        {
            string amlStr = "<AML><Item type='ITEMTYPE' action='get'><name>" + name + "</name></Item></AML>";
            return innovator.applyAML(amlStr);
        }


        /// <summary>
        ///获取拥有的菜单权限
        /// </summary>
        /// <param name="innovator"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetMenuAuthByIdentity(Innovator innovator, string id)
        {
            string str = "";
            //获取权限字符串
            var enumDescriptions = EnumDescription.GetFieldTexts<SystemMenuList>(true);

            if (enumDescriptions != null && enumDescriptions.Count > 0)
            {
                for (int i = 0; i < enumDescriptions.Count; i++)
                {
                    if(i== (enumDescriptions.Count-1))
                    {
                        str = str + "'" + enumDescriptions[i].FieldName + "'";
                    }
                    else
                    {
                        str = str + "'" + enumDescriptions[i].FieldName + "',";
                    }
                }
            }
            string amlStr = "<AML><Item type='ITEMTYPE' action='get'><keyed_name condition ='in'>"+ str + "</keyed_name><Relationships><Item type='TOC Access' action='get'><related_id condition='in'>" + id + "</related_id></Item></Relationships></Item></AML>";
            //获取是否有角色管理访问权限
            //string strSql = "select g.KEYED_NAME from innovator.ITEMTYPE g inner join  innovator.TOC_ACCESS t on g.ID = t.SOURCE_ID inner join innovator.[IDENTITY] y on t.RELATED_ID = y.ID where y.ID = '" + id + "'";
            Item result = innovator.applyAML(amlStr);
            return result;
        }
    }
}
