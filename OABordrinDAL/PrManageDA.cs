using Aras.IOM;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinDAL
{
    public class PrManageDA
    {
        /// <summary>
        /// 修改编号
        /// </summary>
        public static void UpdatePrManageRecordNo(string id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                //db.B_PRMANAGE.Where(x=>x.)
            }

        }

        /// <summary>
        /// 获取合同单号 获取数据
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="contractNo"></param>
        /// <returns></returns>
        public static Item GetPrManageByContractNo(Innovator inn, string contractNo)
        {
            string strSql = @"select g.*,y.b_Buyer,y.b_PrRecordNo from [innovator].B_PrChoiceSuppliers g inner join [innovator].[B_CHOICESUPPLIERS] t on g.id=t.RELATED_ID 
                            inner join [innovator].B_PRMANAGE y on t.SOURCE_ID=y.id where g.b_PoNo='"+ contractNo + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            return result;
        }


        /// <summary>
        /// 删除询价信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void DeleteQuotationItem(Innovator inn, string id)
        {
            //询价信息
            var QuotationItem = inn.newItem("b_QuotationItem", "get");
            QuotationItem.setAttribute("where", "source_id='" + id + "'");
            QuotationItem.setAttribute("select", "related_id");
            var oldItems = QuotationItem.apply();
            string whereItem = "";
            string requestIds = "";
            //string where
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }

                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='b_QuotationItem' action='purge' idlist='" + requestIds + "'></Item><Item type='b_PrQuotationItem' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }


        /// <summary>
        /// 删除重复采购信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static void DeleteRepeateItem(Innovator inn, string id)
        {
            var repeatedPurchasing = inn.newItem("b_RepeatedPurchasing", "get");
            repeatedPurchasing.setAttribute("where", "source_id='" + id + "'");
            repeatedPurchasing.setAttribute("select", "related_id");
            var oldItems = repeatedPurchasing.apply();
            string whereItem = "";
            string requestIds = "";
            //string where
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='b_RepeatedPurchasing' action='purge' idlist='" + requestIds + "'></Item><Item type='b_PrQuotationItem' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);

            }
        }

        /// <summary>
        /// 删除选择供应商
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteChoiceSupplier(Innovator inn, string id)
        {
            var choiceSupplier = inn.newItem("b_ChoiceSuppliers", "get");
            choiceSupplier.setAttribute("where", "source_id='" + id + "'");
            choiceSupplier.setAttribute("select", "related_id");
            var oldItems = choiceSupplier.apply();

            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='b_ChoiceSuppliers' action='purge' idlist='" + requestIds + "'></Item><Item type='b_PrChoiceSuppliers' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }

        /// <summary>
        /// 删除对应的附件信息
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        public static void DeleteFile(Innovator inn, string id)
        {
            var b_PrManageFiles = inn.newItem("b_PrManageFiles", "get");
            b_PrManageFiles.setAttribute("where", "source_id='" + id + "'");
            b_PrManageFiles.setAttribute("select", "related_id");
            var oldItems = b_PrManageFiles.apply();
            string whereItem = "";
            string requestIds = "";
            if (oldItems.getItemCount() > 0)
            {
                for (int i = 0; i < oldItems.getItemCount(); i++)
                {
                    var item = oldItems.getItemByIndex(i);
                    whereItem += item.getProperty("related_id") + ",";
                    requestIds += item.getProperty("id") + ",";
                }
                whereItem = whereItem.Substring(0, whereItem.LastIndexOf(','));
                requestIds = requestIds.Substring(0, requestIds.LastIndexOf(','));
                string amlStr = "<AML><Item type='b_PrManageFiles' action='purge' idlist='" + requestIds + "'></Item><Item type='File' action='purge'  idlist='" + whereItem + "'></Item></AML>";
                var result = inn.applyAML(amlStr);
            }
        }


    }
}
