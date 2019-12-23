using Aras.IOM;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.PrManage
{
    public class SearchPrManageController : BaseController
    {
        // GET: SearchPrManage
        public ActionResult Index()
        {
            List<string> listRoleName = IdentityDA.GetIdentityByUserName(Userinfo.UserName);
            //if (listRoleName.Contains("采购员") || listRoleName.Contains("PRReader") || listRoleName.Contains("采购部接收PR") || Userinfo.LoginName == "admin")
            //{
            //    ViewBag.CanExport = true;
            //}
            //else
            //{
            //    ViewBag.CanExport = false;
            //}

            if(listRoleName.Contains("Purchaser Export"))
            {
                ViewBag.CanExport = true;
            }
            else
            {
                ViewBag.CanExport = false;
            }
            ViewBag.CurrentName = Common.GetLanguageValueByParam("查询", "CommonName", "Common", ViewBag.language);
            return View("~/Views/PrManage/SearchPrManage.cshtml");
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="para"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public JsonResult GetSearchPrManageList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetSearchPrManageList(inn, Userinfo.Roles, out total, para, searchValue, startTime, endTime, status);
            //获取当前人员角色信息
            List<string> listRoleName = IdentityDA.GetIdentityByUserName(Userinfo.UserName);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    item.status = Common.GetItemStatus(item.id);
                    item.b_RaisedDate = item.nb_RaisedDate.ToString("yyyy-MM-dd");
                    if (item.status != "End")
                    {
                        var result = ActivityDA.GetActivityByItemId(inn, item.id, "innovator.B_PRMANAGE");
                        if (!result.isError())
                        {
                            item.activityId = result.getItemByIndex(0).getProperty("activityid");
                            item.AuditorStr = ActivityDA.GetActivityOperator(inn, item.activityId);
                            item.AuditorStr = "<div style='width:180px;word-wrap:break-word;'>" + item.AuditorStr + "</div>";
                        }
                    }
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-list-alt history' title='日志' id='" + item.id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-asterisk workflow' title='流程' id='" + item.id + "' ItemStatus='" + item.status + "' b_VersionNo='" + item.b_VersionNo + "'></a>";
                    if (item.b_Buyer == Userinfo.UserName || listRoleName.Contains("采购员") || listRoleName.Contains("PRReader") || listRoleName.Contains("采购部接收PR") || listRoleName.Contains("财务分析员") || listRoleName.Contains("财务经理") || listRoleName.Contains("财务总监") || listRoleName.Contains("CFO") || Userinfo.LoginName == "admin" || item.b_Applicant == Userinfo.UserName)
                    {
                        linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-print Print' title='打印' id='" + item.id + "'></a>";
                    }
                    strHtml = string.Format(strHtml, item.b_PrRecordNo, linkAList);
                    item.b_PrRecordNo = strHtml;
                    item.status = Common.GetChineseValueByParam(item.status, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
                }
            }
            return Json(new
            {
                sEcho = para.sEcho,
                iTotalRecords = total,
                iTotalDisplayRecords = total,
                aaData = dataList
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        private static List<PrManageModel> GetSearchPrManageList(Innovator inn, List<string> roles, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {

            //获取的权限PRReader
            Item identityItem = IdentityDA.GetIdentityByKeyedName(inn, "PRReader");
            string PRReaderId = "";
            if (identityItem != null && identityItem.getItemCount() > 0)
            {
                PRReaderId = identityItem.getProperty("id");
            }

            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<PrManageModel> datas = (from g in db.B_PRMANAGE
                                                   join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                   join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                   join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                   join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                   join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                   join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                   where (roles.Contains(p.ID) || roles.Contains(PRReaderId)) && (g.B_PRRECORDNO.Contains(searchValue) || g.B_PROJECTNAME.Contains(searchValue) || g.B_BUSINESSDEPARTMENT.Contains(searchValue) || g.B_APPLICANT.Contains(searchValue) || g.B_BUYER.Contains(searchValue) || g.B_PURCHASECONTENT.Contains(searchValue))
                                                   select new PrManageModel
                                                   {
                                                       id = g.id,
                                                       b_PrRecordNo = g.B_PRRECORDNO,
                                                       b_ProjectName = g.B_PROJECTNAME,
                                                       b_PurchaseContent = g.B_PURCHASECONTENT,
                                                       b_BusinessDepartment = g.B_BUSINESSDEPARTMENT,
                                                       nb_RaisedDate = g.CREATED_ON,
                                                       b_Applicant = g.B_APPLICANT,
                                                       b_Buyer = g.B_BUYER,
                                                       b_Budget = g.B_BUDGET,
                                                       b_BudgetCode = g.B_BUDGETCODE,
                                                       b_VersionNo=g.B_VERSIONNO,
                                                       status = (from z in db.B_PRMANAGE
                                                                 join x in db.WORKFLOW on z.id equals x.SOURCE_ID
                                                                 join c in db.WORKFLOW_PROCESS on x.RELATED_ID equals c.ID
                                                                 join v in db.WORKFLOW_PROCESS_ACTIVITY on c.ID equals v.SOURCE_ID
                                                                 join b in db.ACTIVITY on v.RELATED_ID equals b.ID
                                                                 join n in db.ACTIVITY_ASSIGNMENT on b.ID equals n.SOURCE_ID
                                                                 join m in db.IDENTITY on n.RELATED_ID equals m.ID
                                                                 where b.STATE == "active" && z.id == g.id
                                                                 select b).FirstOrDefault().KEYED_NAME
                                                   }).Distinct();

                if (!string.IsNullOrEmpty(status))
                {
                    if (status != "End")
                    {
                        datas = datas.Where(x => x.status == status);
                    }
                    else
                    {
                        datas = datas.Where(x => x.status == null);
                    }
                }

                // 时间查询
                if (startTime != null)
                {
                    datas = datas.Where(x => x.nb_RaisedDate >= startTime);
                }

                if (endTime != null)
                {
                    datas = datas.Where(x => x.nb_RaisedDate <= endTime);
                }

                total = datas.Count();
                if (para != null)
                {
                    //排序
                    if (para.sSortType == "asc")
                    {
                        datas = Common.OrderBy(datas, para.iSortTitle, false);
                    }
                    else
                    {
                        datas = Common.OrderBy(datas, para.iSortTitle, true);
                    }
                    //分页
                    datas = datas.Skip(para.iDisplayStart).Take(para.iDisplayLength);
                }
                return datas.ToList();
            }
        }


        public FileResult ExportPRByCondition(string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetSearchPrManageList(inn, Userinfo.Roles, out total, null, searchValue, startTime, endTime, status);
            Stream ms = ExportPRByCondition(dataList, Userinfo.language);
            return File(ms, "application/vnd.ms-excel", "Pr单信息" + ".xls");
        }



        private static Stream ExportPRByCondition(List<PrManageModel> dataList, string language)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            // 设置字体    
            IFont headfont = workbook.CreateFont();
            headfont.FontName = "微软雅黑";

            // 建表头
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).CellStyle.SetFont(headfont);
            row.CreateCell(0).SetCellValue("编号");
            row.CreateCell(1).SetCellValue("采购内容");
            row.CreateCell(2).SetCellValue("项目名称");
            row.CreateCell(3).SetCellValue("申请时间");
            row.CreateCell(4).SetCellValue("申请人");
            row.CreateCell(5).SetCellValue("需求部门");
            row.CreateCell(6).SetCellValue("预算号");
            row.CreateCell(7).SetCellValue("预算");
            row.CreateCell(8).SetCellValue("采购员");
            row.CreateCell(9).SetCellValue("流程状态");
            //row.CreateCell(7).SetCellValue("供应商");
            //row.CreateCell(8).SetCellValue("合同价格");

            dataList = dataList.OrderByDescending(x => x.b_PrRecordNo).ToList();
            for (var i = 0; i < dataList.Count; i++)
            {
                PrManageModel model = dataList[i];
                IRow rowItem = sheet.CreateRow(i + 1);
                rowItem.CreateCell(0).SetCellValue(model.b_PrRecordNo);
                rowItem.CreateCell(1).SetCellValue(model.b_PurchaseContent);
                rowItem.CreateCell(2).SetCellValue(model.b_ProjectName);
                rowItem.CreateCell(3).SetCellValue(model.nb_RaisedDate.ToString("yyyy-MM-dd"));
                rowItem.CreateCell(4).SetCellValue(model.b_Applicant);
                rowItem.CreateCell(5).SetCellValue(model.b_BusinessDepartment);
                rowItem.CreateCell(6).SetCellValue(model.b_BudgetCode);
                rowItem.CreateCell(7).SetCellValue(model.b_Budget.ToString());
                rowItem.CreateCell(8).SetCellValue(model.b_Buyer);
                model.status = Common.GetItemStatus(model.id);
                model.status = Common.GetChineseValueByParam(model.status, "PrManageWorkFlow", "WorkFlow", language);
                rowItem.CreateCell(9).SetCellValue(model.status);

                //rowItem.CreateCell(7).SetCellValue(model.b_SourcedSupplier);
                //rowItem.CreateCell(8).SetCellValue(model.b_ContractPriceStr.ToString());
            }

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// 采购导出
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public FileResult ExportPurchaseByCondition(string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetSearchPrManageList(inn, Userinfo.Roles, out total, null, searchValue, startTime, endTime, status);
            List<string> ids = dataList.Select(x => x.id).ToList();
            //获取采购合同数据
            List<PrExportContractModel> list = GetPurchaseContractData(ids);
            Stream ms = ExportPurchaseByCondition(list, Userinfo.language);
            return File(ms, "application/vnd.ms-excel", "PR合同信息" + ".xls");
        }


        private static Stream ExportPurchaseByCondition(List<PrExportContractModel> dataList, string language)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            // 设置字体    
            IFont headfont = workbook.CreateFont();
            headfont.FontName = "微软雅黑";

            // 建表头
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).CellStyle.SetFont(headfont);
            row.CreateCell(0).SetCellValue(Common.GetLanguageValueByParam("合同编码", "PRExportContract", "PRItemType", language));
            row.CreateCell(1).SetCellValue(Common.GetLanguageValueByParam("采购员", "B_PrManage", "PRItemType", language));
            row.CreateCell(2).SetCellValue(Common.GetLanguageValueByParam("采购内容", "B_PrManage", "PRItemType", language));
            row.CreateCell(3).SetCellValue(Common.GetLanguageValueByParam("需求部门", "B_PrManage", "PRItemType", language));
            row.CreateCell(4).SetCellValue(Common.GetLanguageValueByParam("需求人", "PRExportContract", "PRItemType", language));
            row.CreateCell(5).SetCellValue(Common.GetLanguageValueByParam("预算号", "B_PrManage", "PRItemType", language));
            row.CreateCell(6).SetCellValue(Common.GetLanguageValueByParam("预算金额（元，含税）", "PRExportContract", "PRItemType", language));
            row.CreateCell(7).SetCellValue(Common.GetLanguageValueByParam("中标供应商名称", "PRExportContract", "PRItemType", language));
            row.CreateCell(8).SetCellValue(Common.GetLanguageValueByParam("是否单一供应商或紧急采购", "PRExportContract", "PRItemType", language));
            row.CreateCell(9).SetCellValue(Common.GetLanguageValueByParam("合同金额", "PRExportContract", "PRItemType", language));
            row.CreateCell(10).SetCellValue(Common.GetLanguageValueByParam("比预算节约金额", "PRExportContract", "PRItemType", language));
            row.CreateCell(11).SetCellValue(Common.GetLanguageValueByParam("所属公司", "PRExportContract", "PRItemType", language));
            for (var i = 0; i < dataList.Count; i++)
            {
                PrExportContractModel model = dataList[i];
                IRow rowItem = sheet.CreateRow(i + 1);
                rowItem.CreateCell(0).SetCellValue(model.b_PoNo);
                rowItem.CreateCell(1).SetCellValue(model.b_Buyer);
                rowItem.CreateCell(2).SetCellValue(model.b_PurchaseContent);
                rowItem.CreateCell(3).SetCellValue(model.b_BusinessDepartment);
                rowItem.CreateCell(4).SetCellValue(model.b_Applicant);
                rowItem.CreateCell(5).SetCellValue(model.b_BudgetCode);
                rowItem.CreateCell(6).SetCellValue(model.b_Budget.ToString());
                rowItem.CreateCell(7).SetCellValue(model.b_Supplier);
                if(model.b_IsSingleSupplier=="1" || model.b_UrgentPurchase=="1")
                {
                    model.SingleSource = "Y";
                }
                else
                {
                    model.SingleSource = "N";
                }
                rowItem.CreateCell(8).SetCellValue(model.SingleSource);
                rowItem.CreateCell(9).SetCellValue(model.b_ContractPrice.ToString());
                int count = dataList.Where(x => x.source_id == model.source_id).Count();
                if(count==1)
                {
                    model.SaveAmount = (model.b_Budget + model.b_AdditionalBudget) - model.b_ContractPrice;
                }
                else
                {
                    model.SaveAmount = 0;
                }
                model.SaveAmount = Math.Round(model.SaveAmount, 2);
                rowItem.CreateCell(10).SetCellValue(model.SaveAmount.ToString());
                rowItem.CreateCell(11).SetCellValue(model.b_ContractParty);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;

        }


        /// <summary>
        /// 获取Pr合同数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<PrExportContractModel> GetPurchaseContractData(List<string> ids)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<PrExportContractModel> data = (from g in db.B_PRCHOICESUPPLIERS
                                                          join t in db.B_CHOICESUPPLIERS on g.id equals t.RELATED_ID
                                                          join y in db.B_PRMANAGE on t.SOURCE_ID equals y.id
                                                          where ids.Contains(y.id)
                                                          select new PrExportContractModel
                                                          {
                                                              source_id = y.id,
                                                              b_PoNo = g.B_PONO,
                                                              b_Buyer = y.B_BUYER,
                                                              b_PurchaseContent = y.B_PURCHASECONTENT,
                                                              b_BusinessDepartment = y.B_BUSINESSDEPARTMENT,
                                                              b_Applicant = y.B_APPLICANT,
                                                              b_BudgetCode = y.B_BUDGETCODE,
                                                              b_Budget = y.B_BUDGET,
                                                              b_Supplier = g.B_SUPPLIER,
                                                              b_IsSingleSupplier = y.B_ISSINGLESUPPLIER,
                                                              b_UrgentPurchase = y.B_URGENTPURCHASE,
                                                              b_ContractPrice = g.B_CONTRACTPRICE,
                                                              b_ContractParty = y.B_CONTRACTPARTY
                                                          });
                return data.ToList();
            }
        }




        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Print(string id)
        {
            Item result = PrManageBll.GetPrManageObjById(inn, id);
            //拆解数据
            PrManageModel model = new PrManageModel();
            model.id = result.getProperty("id");
            model.b_PrRecordNo = result.getProperty("b_prrecordno");
            model.b_PrType = result.getProperty("b_prtype");
            model.b_BusinessDepartment = result.getProperty("b_businessdepartment");
            model.b_Budget = decimal.Parse(result.getProperty("b_budget"));
            model.b_ApplicantId = result.getProperty("b_applicantid");
            model.b_Applicant = result.getProperty("b_applicant");
            model.b_RaisedDate = DateTime.Parse(result.getProperty("b_raiseddate")).ToString("yyyy-MM-dd");
            model.b_EmailAddress = result.getProperty("b_emailaddress");
            model.b_PhoneNo = result.getProperty("b_phoneno");
            model.b_ProjectName = result.getProperty("b_projectname");
            model.b_BudgetCode = result.getProperty("b_budgetcode");
            model.b_ProjectLeader = result.getProperty("b_projectleader");
            model.b_ProjectManager = result.getProperty("b_projectmanager");
            model.b_ProjectDirector = result.getProperty("b_projectdirector");
            model.b_BudgetStatus = result.getProperty("b_budgetstatus");
            model.b_BuyerId = result.getProperty("b_buyerid");
            model.b_Buyer = result.getProperty("b_buyer");
            model.b_UrgentPurchase = result.getProperty("b_urgentpurchase") == "0" ? false : true;
            model.b_RepetitivePurchase = result.getProperty("b_repetitivepurchase") == "0" ? false : true;
            model.b_AuthorizedPurchase = result.getProperty("b_authorizedpurchase") == "0" ? false : true;
            //model.b_SourcedSupplier = result.getProperty("b_sourcedsupplier");
            //model.b_ContractPrice = result.getProperty("b_contractprice");
            //model.b_PoNo = result.getProperty("b_pono");
            //model.b_ContractProperty = result.getProperty("b_contractproperty");
            model.b_PurchaseContent = result.getProperty("b_purchasecontent");
            model.b_ContractParty = result.getProperty("b_contractparty");
            model.b_ApplicantAddress = result.getProperty("b_applicantaddress");
            model.b_IsSingleSupplier = result.getProperty("b_issinglesupplier") == "0" ? false : true;
            model.b_ContractType = result.getProperty("b_contracttype");
            model.b_DeptManager = result.getProperty("b_deptmanager");
            model.b_DeptDirector = result.getProperty("b_deptdirector");
            model.b_CostCenter = result.getProperty("b_costcenter");
            model.b_PurchasingReason = result.getProperty("b_purchasingreason");
            model.b_AdditionalBudget = result.getProperty("b_additionalbudget");
            model.b_DeptVP = result.getProperty("b_deptvp");
            model.UserName = Userinfo.UserName;
            model.status = Common.GetItemStatus(id);
            Item Relation = result.getRelationships("b_RequestInfo");
            if (Relation.getItemCount() > 0)
            {
                model.PrManageItems = new List<PrManageItem>();
                for (int i = 0; i < Relation.getItemCount(); i++)
                {
                    Item ItemObJ = Relation.getItemByIndex(i).getRelatedItem();
                    PrManageItem itemModel = new PrManageItem();
                    itemModel.id = ItemObJ.getProperty("id");
                    itemModel.b_No = ItemObJ.getProperty("b_no");
                    itemModel.b_RequestList = ItemObJ.getProperty("b_requestlist");
                    itemModel.b_SpecificationQuantity = ItemObJ.getProperty("b_specificationquantity");
                    itemModel.b_ProjectNo = ItemObJ.getProperty("b_projectno");
                    itemModel.b_TaskNo = ItemObJ.getProperty("b_taskno");
                    itemModel.b_Qty = int.Parse(ItemObJ.getProperty("b_qty"));
                    itemModel.b_Unit = ItemObJ.getProperty("b_unit");
                    model.PrManageItems.Add(itemModel);
                }
            }
            List<string> listRoleName = IdentityDA.GetIdentityByUserName(Userinfo.UserName);
            model.IsPurchasingAuth = false;

            if (model.UserName == model.b_Buyer || listRoleName.Contains("采购员") || listRoleName.Contains("采购部接收PR") || listRoleName.Contains("PRReader") || Userinfo.LoginName == "admin")
            {
                model.IsPurchasingAuth = true;
                //询价信息
                Item quotationRelation = result.getRelationships("b_QuotationItem");
                if (quotationRelation.getItemCount() > 0)
                {
                    model.PrQuotationItems = new List<PrQuotationItem>();
                    for (int i = 0; i < quotationRelation.getItemCount(); i++)
                    {
                        Item itemObj = quotationRelation.getItemByIndex(i).getRelatedItem();
                        PrQuotationItem itemModel = new PrQuotationItem();
                        itemModel.id = itemObj.getProperty("id");
                        itemModel.b_Supplier = itemObj.getProperty("b_supplier");
                        itemModel.b_Quotation = itemObj.getProperty("b_quotation");
                        itemModel.b_Remarks = itemObj.getProperty("b_remarks");
                        model.PrQuotationItems.Add(itemModel);
                    }
                }

                //获取重复信息
                Item repeatedPurchasing = result.getRelationships("b_RepeatedPurchasing");
                if (repeatedPurchasing.getItemCount() > 0)
                {
                    model.PrRepeateItems = new List<PrRepeateItem>();
                    for (int i = 0; i < repeatedPurchasing.getItemCount(); i++)
                    {
                        Item itemObj = repeatedPurchasing.getItemByIndex(i).getRelatedItem();
                        PrRepeateItem itemModel = new PrRepeateItem();
                        itemModel.id = itemObj.getProperty("id");
                        itemModel.b_PrRecordNo = itemObj.getProperty("b_prrecordno");
                        itemModel.b_PreviousSupplier = itemObj.getProperty("b_previoussupplier");
                        itemModel.b_ContractNo = itemObj.getProperty("b_contractno");
                        //itemModel.b_ContractPrice = itemObj.getProperty("b_contractprice");
                        itemModel.b_PreviousBuyer = itemObj.getProperty("b_previousbuyer");
                        model.PrRepeateItems.Add(itemModel);
                    }
                }

                //获取挑选的供应商
                Item b_ChoiceSuppliers = result.getRelationships("b_ChoiceSuppliers");
                if (b_ChoiceSuppliers.getItemCount() > 0)
                {
                    model.PrChoiceSupplierItems = new List<PrChoiceSupplierItem>();
                    for (int i = 0; i < b_ChoiceSuppliers.getItemCount(); i++)
                    {
                        Item itemObj = b_ChoiceSuppliers.getItemByIndex(i).getRelatedItem();
                        PrChoiceSupplierItem itemModel = new PrChoiceSupplierItem();
                        itemModel.id = itemObj.getProperty("id");
                        itemModel.b_Supplier = itemObj.getProperty("b_supplier");
                        itemModel.b_ContractPrice = itemObj.getProperty("b_contractprice");
                        itemModel.b_PoNo = itemObj.getProperty("b_pono");
                        itemModel.b_ContractProperty = itemObj.getProperty("b_contractproperty");
                        itemModel.b_PaymentClause = itemObj.getProperty("b_paymentclause");
                        model.PrChoiceSupplierItems.Add(itemModel);
                    }
                }
            }
            model.HistoryList = GetPrManageHistoryList(model.id);
            foreach (var item in model.HistoryList)
            {
                item.Created_on = item.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                item.ItemStatus = Common.GetChineseValueByParam(item.ItemStatus, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
                item.OperateStr = Common.GetChineseValueByParam(item.OperateStr, "PrManageWorkFlow", "WorkFlow", Userinfo.language);
            }
            return View("~/Views/PrManage/PrintPrManage.cshtml", model);
        }


        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="total"></param>
        /// <param name="para"></param>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        private static List<HistoryModel> GetPrManageHistoryList(string item_Id)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_PRMANAGE
                                                  join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                  join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                  join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                  join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                  join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                  join p in db.USER on o.CLOSED_BY equals p.ID
                                                  where g.id == item_Id && i.STATE == "Closed" && p.KEYED_NAME != "Innovator Admin"
                                                  select new HistoryModel
                                                  {
                                                      OperateStr = o.PATH,
                                                      Comments = o.COMMENTS,
                                                      Create_onStr = o.CLOSED_ON,
                                                      CreateName = p.KEYED_NAME,
                                                      ItemStatus = i.KEYED_NAME
                                                  });
                datas = Common.OrderBy(datas, "Create_onStr", true);

                return datas.ToList();


            }
        }


    }
}