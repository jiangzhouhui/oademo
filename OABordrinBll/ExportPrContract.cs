
using Aras.IOM;
using Microsoft.Office.Interop.Word;
using System.Configuration;

namespace OABordrinBll
{
    public static class ExportPrContract
    {
        /// <summary>
        /// 采购合同
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ExportPurchaseContract(string templatePath, Item model,string email)
        {
         
            object oMissing = System.Reflection.Missing.Value;
            //创建一个Word应用程序实例
            _Application oWord = new Application();
            //设置为不可见
            oWord.Visible = false;
            //模板文件地址
            object oTemplate = templatePath;

            //获取单据编号
            string b_prrecordNo = model.getProperty("b_prrecordno");

            //保存路径
            object filename = ConfigurationManager.AppSettings["DownloadContractPath"] + b_prrecordNo + ".doc";

            //以模板为基础生成文档
            _Document oDoc = oWord.Documents.Add(ref oTemplate, ref oMissing, ref oMissing, ref oMissing);
            //声明书签数组
            object[] oBookMark = new object[9];
            //赋值书签名
            oBookMark[0] = "b_PrRecordNo";
            oBookMark[1] = "b_Buyer";
            oBookMark[2] = "BuyerAddress";
            oBookMark[3] = "InvoiceAddress";
            oBookMark[4] = "BankUserName";
            oBookMark[5] = "BankAccount";
            oBookMark[6] = "OpenBank";
            oBookMark[7] = "DutyParagraph";
            oBookMark[8] = "BuyerEmail";
            //赋值任意数据到书签的位置
            oDoc.Bookmarks.get_Item(ref oBookMark[0]).Range.Text = model.getProperty("b_prrecordno");
            oDoc.Bookmarks.get_Item(ref oBookMark[1]).Range.Text = model.getProperty("b_buyer");
            oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "上海市闵行区江月路1599号2楼，4-6楼";
            oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = "上海市闵行区江月路1599号2楼，4-6楼 " + model.getProperty("b_buyer");
            oDoc.Bookmarks.get_Item(ref oBookMark[8]).Range.Text = email;

            //获取判断签约方
            string b_ContractParty = model.getProperty("b_contractparty");
            if (b_ContractParty.Contains("上海思致汽车工程技术有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "上海思致汽车工程技术有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "31001511920052503427";
                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = "建设银行上海长寿路支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = "91310115677893509Y";
            }
            else if(b_ContractParty.Contains("南京盛和新能源科技有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "南京盛和新能源科技有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "93190154740001692";
                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = "上海浦东发展银行股份有限公司南京浦口支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = "91320111MA1NR9UH1M";
            }
            else if(b_ContractParty.Contains("淮安骏盛新能源科技有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "淮安骏盛新能源科技有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "492370493139";
                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = "中国银行股份有限公司淮安淮阴支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = "91320804MA1P6Q435W";
            }
            else if(b_ContractParty.Contains("南京博郡新能源汽车有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "南京博郡新能源汽车有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "93190154740001502";
                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = "浦发银行南京浦口支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = "91320111MA1N3BWF2G";
            }
            else if(b_ContractParty.Contains("南京博郡汽车有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "南京博郡汽车有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "93190154740001721";
                oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = "上海浦东发展银行股份有限公司南京浦口支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = "91320111MA1NTGNJ3T";
            }


            //需求列表信息
            Item Relation = model.getRelationships("b_RequestInfo");
            //需求信息
            int count = Relation.getItemCount();

            //在表格中添加行
            var tableObj = oDoc.Content.Tables[1];
            //添加行所在的位置
            Row row = tableObj.Rows[12];
            for (int i = 0; i < count - 1; i++)
            {
                oDoc.Content.Tables[1].Rows.Add(row);
            }

            for (int index = 0; index < Relation.getItemCount(); index++)
            {
                int nindex = index + 12;
                Item ItemObJ = Relation.getItemByIndex(index).getRelatedItem();
                tableObj.Cell(nindex, 1).Range.Text = (index + 1).ToString();
                tableObj.Cell(nindex, 2).Range.Text = ItemObJ.getProperty("b_requestlist");
                tableObj.Cell(nindex, 3).Range.Text = ItemObJ.getProperty("b_specificationquantity");
                tableObj.Cell(nindex, 4).Range.Text = ItemObJ.getProperty("b_qty");
                tableObj.Cell(nindex, 5).Range.Text = ItemObJ.getProperty("b_unit");
            }

            oDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing,
                  ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                  ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing);
            oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
            //关闭word
            oWord.Quit(ref oMissing, ref oMissing, ref oMissing);


            return filename.ToString();
        }


        /// <summary>
        /// 内部合同
        /// </summary>
        /// <param name="templatePath"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ExportInternalContract(string templatePath, Item model, string email)
        {
            object oMissing = System.Reflection.Missing.Value;
            //创建一个Word应用程序实例
            _Application oWord = new Application();
            //设置为不可见
            oWord.Visible = false;
            //模板文件地址
            object oTemplate = templatePath;

            //获取单据编号
            string b_prrecordNo = model.getProperty("b_prrecordno");

            //保存路径
            object filename = ConfigurationManager.AppSettings["DownloadContractPath"] + b_prrecordNo + ".doc";

            //以模板为基础生成文档
            _Document oDoc = oWord.Documents.Add(ref oTemplate, ref oMissing, ref oMissing, ref oMissing);

            //声明书签数组
            object[] oBookMark = new object[8];
            oBookMark[0] = "b_Buyer";
            oBookMark[1] = "BuyerAddress";
            oBookMark[2] = "OpenBank";
            oBookMark[3] = "BankUserName";
            oBookMark[4] = "BankAccount";
            oBookMark[5] = "DutyParagraph";
            oBookMark[6] = "InvoiceAddress";
            oBookMark[7] = "BuyerEmail";

            //赋值任意数据到书签的位置
            oDoc.Bookmarks.get_Item(ref oBookMark[0]).Range.Text = model.getProperty("b_buyer");
            oDoc.Bookmarks.get_Item(ref oBookMark[1]).Range.Text = "上海市闵行区江月路1599号2楼，4-6楼";
            oDoc.Bookmarks.get_Item(ref oBookMark[6]).Range.Text = "上海市闵行区江月路1599号2楼，4-6楼 " + model.getProperty("b_buyer");
            oDoc.Bookmarks.get_Item(ref oBookMark[7]).Range.Text = email;

            //获取判断签约方
            string b_ContractParty = model.getProperty("b_contractparty");

            if (b_ContractParty.Contains("上海思致汽车工程技术有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "建设银行上海长寿路支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = "上海思致汽车工程技术有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "31001511920052503427";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "91310115677893509Y";
            }
            else if (b_ContractParty.Contains("南京盛和新能源科技有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "上海浦东发展银行股份有限公司南京浦口支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = "南京盛和新能源科技有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "93190154740001692";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "91320111MA1NR9UH1M";
            }
            else if (b_ContractParty.Contains("淮安骏盛新能源科技有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "中国银行股份有限公司淮安淮阴支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = "淮安骏盛新能源科技有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "492370493139";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "91320804MA1P6Q435W";
            }
            else if (b_ContractParty.Contains("南京博郡新能源汽车有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "浦发银行南京浦口支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = "南京博郡新能源汽车有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "93190154740001502";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "91320111MA1N3BWF2G";
            }
            else if(b_ContractParty.Contains("南京博郡汽车有限公司"))
            {
                oDoc.Bookmarks.get_Item(ref oBookMark[2]).Range.Text = "上海浦东发展银行股份有限公司南京浦口支行";
                oDoc.Bookmarks.get_Item(ref oBookMark[3]).Range.Text = "南京博郡汽车有限公司";
                oDoc.Bookmarks.get_Item(ref oBookMark[4]).Range.Text = "93190154740001721";
                oDoc.Bookmarks.get_Item(ref oBookMark[5]).Range.Text = "91320111MA1NTGNJ3T";
            }


            oDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
             ref oMissing, ref oMissing);
            oDoc.Close(ref oMissing, ref oMissing, ref oMissing);
            //关闭word
            oWord.Quit(ref oMissing, ref oMissing, ref oMissing);


            return filename.ToString();

        }


    }
}
