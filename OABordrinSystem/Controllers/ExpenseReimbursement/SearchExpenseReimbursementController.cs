using Aras.IOM;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.ExpenseReimbursement
{
    public class SearchExpenseReimbursementController : BaseController
    {
        // GET: SearchExpenseReimbursement
        public ActionResult Index()
        {
            List<string> listRoleName = IdentityDA.GetIdentityByUserName(Userinfo.UserName);
            if (listRoleName.Contains("SAP导出"))
            {
                ViewBag.CanExport = true;
            }
            else
            {
                ViewBag.CanExport = false;
            }

            ViewBag.CurrentName = Common.GetLanguageValueByParam("查询费用报销", "ERCommon", "ERItemType", ViewBag.language);
            return View("~/Views/ExpenseReimbursement/SearchExpenseReimbursement.cshtml");
        }

        public JsonResult GetSearchExpenseReimbursementList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetSearchExpenseReimbursementList(Userinfo.inn, Userinfo.Roles, out total, para, searchValue, startTime, endTime, status, Userinfo.UserName);

            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    if (!string.IsNullOrEmpty(item.status) && item.status != null && item.status != "End")
                    {
                        var result = ActivityDA.GetActivityByItemId(inn, item.Id, "innovator.B_EXPENSEREIMBURSEMENT");
                        if (!result.isError())
                        {
                            item.activityId = result.getItemByIndex(0).getProperty("activityid");
                            item.AuditorStr = ActivityDA.GetActivityOperator(inn, item.activityId, false);
                            item.AuditorStr = "<div style='width:150px;word-wrap:break-word;'>" + item.AuditorStr + "</div>";
                        }
                    }
                    else
                    {
                        item.status = "End";
                    }
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.Id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-list-alt history' title='日志' id='" + item.Id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-asterisk workflow' title='流程' id='" + item.Id + "' ItemStatus='" + item.status + "' ></a>";
                    if (item.status == "Expense Accountant Check" || item.status == "Financial Director" || item.status == "Expense Accountant Creation" || item.status == "End")
                    {
                        linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-print Print' title='打印' id='" + item.Id + "'></a>";
                    }
                    if (Userinfo.UserName == "Innovator Admin" && item.status == "Expense Accountant Check")
                    {
                        linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-envelope SendEmail' title='重发提醒报销凭证' Id='" + item.Id + "' ></a>";
                    }
                    strHtml = string.Format(strHtml, item.b_RecordNo, linkAList);
                    item.b_RecordNo = strHtml;
                    item.b_ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    item.status = Common.GetChineseValueByParam(item.status, "ERManageWorkFlow", "WorkFlow", Userinfo.language);
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

        private static List<ExpenseReimbursementModel> GetSearchExpenseReimbursementList(Innovator inn, List<string> roles, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status, string employee)
        {

            //获取的权限PRReader
            Item identityItem = IdentityDA.GetIdentityByKeyedName(inn, "ERReader");
            string ERReaderId = "";
            if (identityItem != null && identityItem.getItemCount() > 0)
            {
                ERReaderId = identityItem.getProperty("id");
            }

            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<ExpenseReimbursementModel> datas = (from g in db.B_EXPENSEREIMBURSEMENT
                                                               join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                               join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                               join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                               join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                               join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                               join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                               where (roles.Contains(p.ID) || g.B_EMPLOYEE == employee || roles.Contains(ERReaderId)) && (g.B_RECORDNO.Contains(searchValue) || g.B_DEPT.Contains(searchValue) || g.B_EMPLOYEE.Contains(searchValue))
                                                               select new ExpenseReimbursementModel
                                                               {
                                                                   Id = g.id,
                                                                   b_RecordNo = g.B_RECORDNO,
                                                                   nb_ApplicationDate = g.CREATED_ON,
                                                                   b_Dept = g.B_DEPT,
                                                                   b_Employee = g.B_EMPLOYEE,
                                                                   b_DueCompany = g.B_DUECOMPANY,
                                                                   status = (from z in db.B_EXPENSEREIMBURSEMENT
                                                                             join x in db.WORKFLOW on z.id equals x.SOURCE_ID
                                                                             join c in db.WORKFLOW_PROCESS on x.RELATED_ID equals c.ID
                                                                             join v in db.WORKFLOW_PROCESS_ACTIVITY on c.ID equals v.SOURCE_ID
                                                                             join b in db.ACTIVITY on v.RELATED_ID equals b.ID
                                                                             join n in db.ACTIVITY_ASSIGNMENT on b.ID equals n.SOURCE_ID
                                                                             join m in db.IDENTITY on n.RELATED_ID equals m.ID
                                                                             where b.STATE == "active" && z.id == g.id
                                                                             select b).FirstOrDefault().KEYED_NAME
                                                               }).Distinct();



                // 时间查询
                if (startTime != null)
                {
                    datas = datas.Where(x => x.nb_ApplicationDate >= startTime);
                }

                if (endTime != null)
                {
                    datas = datas.Where(x => x.nb_ApplicationDate <= endTime);
                }

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

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Print(string id)
        {
            Item result = ExpenseReimbursementBll.GetExpenseReimbursementObjById(inn, id);
            //拆解数据
            ExpenseReimbursementModel model = new ExpenseReimbursementModel();
            model.Id = result.getProperty("id");
            model.b_RecordNo = result.getProperty("b_recordno");
            model.b_CompanyCode = result.getProperty("b_companycode");
            model.b_ReimbursementPlace = result.getProperty("b_reimbursementplace");
            model.b_IsBudgetary = result.getProperty("b_isbudgetary") == "1" ? true : false;
            model.b_Preparer = result.getProperty("b_preparer");
            model.b_PreparerNo = result.getProperty("b_preparerno");
            model.b_ApplicationDate = DateTime.Parse(result.getProperty("b_applicationdate")).ToString("yyyy-MM-dd");
            model.b_Employee = result.getProperty("b_employee");
            model.b_StaffNo = result.getProperty("b_staffno");
            model.b_Position = result.getProperty("b_position");
            model.b_Dept = result.getProperty("b_dept");
            model.b_CostCenter = result.getProperty("b_costcenter");
            model.b_Tel = result.getProperty("b_tel");
            model.b_AdvancedAmount = !string.IsNullOrEmpty(result.getProperty("b_advancedamount")) ? decimal.Parse(result.getProperty("b_advancedamount")) : 0;
            model.b_TotalExpense = !string.IsNullOrEmpty(result.getProperty("b_totalexpense")) ? decimal.Parse(result.getProperty("b_totalexpense")) : 0;
            model.b_DueCompany = result.getProperty("b_duecompany");
            model.b_IsHangUp = result.getProperty("b_ishangup") == "1" ? true : false;
            model.b_AmountInWords = result.getProperty("b_amountinwords");
            model.b_TotalAmount = !string.IsNullOrEmpty(result.getProperty("b_totalamount")) ? decimal.Parse(result.getProperty("b_totalamount")) : 0;
            model.b_Type = result.getProperty("b_type");
            model.b_AttachmentsQuantity = !string.IsNullOrEmpty(result.getProperty("b_attachmentsquantity")) ? int.Parse(result.getProperty("b_attachmentsquantity")) : 0;
            model.b_LineLeader = result.getProperty("b_lineleader");
            model.b_DepartmentLeader = result.getProperty("b_departmentleader");
            model.b_DivisionVP = result.getProperty("b_divisionvp");
            model.OldRemark = result.getProperty("b_remark");

            //报销明细
            Item Relation = result.getRelationships("R_ReimbursementItem");
            if (Relation.getItemCount() > 0)
            {
                model.ReimbursementItems = new List<ReimbursementItem>();
                for (int i = 0; i < Relation.getItemCount(); i++)
                {
                    Item ItemObJ = Relation.getItemByIndex(i).getRelatedItem();
                    ReimbursementItem itemModel = new ReimbursementItem();
                    itemModel.Id = ItemObJ.getProperty("id");
                    itemModel.b_Date = !string.IsNullOrEmpty(ItemObJ.getProperty("b_date")) ? DateTime.Parse(ItemObJ.getProperty("b_date")).ToString("yyyy-MM-dd") : "";
                    itemModel.b_CategoryNumber = ItemObJ.getProperty("b_categorynumber");
                    itemModel.b_ProjectName = ItemObJ.getProperty("b_projectname");
                    itemModel.b_BudgetNumber = ItemObJ.getProperty("b_budgetnumber");
                    itemModel.b_Currency = ItemObJ.getProperty("b_currency");
                    itemModel.b_Rate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_rate")) ? float.Parse(ItemObJ.getProperty("b_rate")) : 0;
                    itemModel.b_OriginalCurrency = !string.IsNullOrEmpty(ItemObJ.getProperty("b_originalcurrency")) ? decimal.Parse(ItemObJ.getProperty("b_originalcurrency")) : 0;
                    itemModel.b_Count = !string.IsNullOrEmpty(ItemObJ.getProperty("b_count")) ? int.Parse(ItemObJ.getProperty("b_count")) : 0;
                    itemModel.b_TaxRate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxrate")) ? float.Parse(ItemObJ.getProperty("b_taxrate")) : 0;
                    itemModel.b_Tax = !string.IsNullOrEmpty(ItemObJ.getProperty("b_tax")) ? decimal.Parse(ItemObJ.getProperty("b_tax")) : 0;
                    itemModel.b_TaxFreeAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_taxfreeamount")) ? decimal.Parse(ItemObJ.getProperty("b_taxfreeamount")) : 0;
                    itemModel.b_CNYSubtotal = !string.IsNullOrEmpty(ItemObJ.getProperty("b_cnysubtotal")) ? decimal.Parse(ItemObJ.getProperty("b_cnysubtotal")) : 0;
                    model.ReimbursementItems.Add(itemModel);
                }
            }

            //借款明细
            Item loanItemRelation = result.getRelationships("R_LoanItem");
            if (loanItemRelation.getItemCount() > 0)
            {
                model.LoanItems = new List<LoanItem>();
                for (int i = 0; i < loanItemRelation.getItemCount(); i++)
                {
                    Item ItemObJ = loanItemRelation.getItemByIndex(i).getRelatedItem();
                    LoanItem item = new LoanItem();
                    item.id = ItemObJ.getProperty("id");
                    item.b_LoanOrderNo = ItemObJ.getProperty("b_loanorderno");
                    item.b_Date = !string.IsNullOrEmpty(ItemObJ.getProperty("b_date")) ? DateTime.Parse(ItemObJ.getProperty("b_date")).ToString("yyyy-MM-dd") : "";
                    item.b_Borrower = ItemObJ.getProperty("b_borrower");
                    item.b_LoanAmount = !string.IsNullOrEmpty(ItemObJ.getProperty("b_loanamount")) ? decimal.Parse(ItemObJ.getProperty("b_loanamount")) : 0;
                    item.b_LoanReason = ItemObJ.getProperty("b_loanreason");
                    model.LoanItems.Add(item);
                }
            }

            //日志信息
            model.HistoryList = GetExpenseReimbursementHistoryList(id);
            foreach (var item in model.HistoryList)
            {
                item.Created_on = item.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
            }
            return View("~/Views/ExpenseReimbursement/PrintExpenseReimbursement.cshtml", model);
        }




        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        private static List<HistoryModel> GetExpenseReimbursementHistoryList(string item_Id)
        {
            List<HistoryModel> listModel = new List<HistoryModel>();
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_EXPENSEREIMBURSEMENT
                                                  join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                  join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                  join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                  join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                  join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                  join p in db.USER on o.CLOSED_BY equals p.ID
                                                  where g.id == item_Id && o.CLOSED_BY != null && o.COMMENTS != "AutoComplete"
                                                  select new HistoryModel
                                                  {
                                                      OperateStr = o.PATH,
                                                      Comments = o.COMMENTS,
                                                      Create_onStr = o.CLOSED_ON,
                                                      CreateName = p.KEYED_NAME,
                                                      ItemStatus = i.KEYED_NAME
                                                  });

                datas = Common.OrderBy(datas, "Create_onStr", true);
                listModel = datas.ToList();
            }

            if (listModel != null && listModel.Count > 0)
            {
                foreach (var model in listModel)
                {
                    model.Create_onStr = model.Create_onStr.GetValueOrDefault().AddHours(8);
                    model.Created_on = model.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return listModel;
        }

        /// <summary>
        /// 导出凭证清单
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public FileResult ExportExpenseReimbursementByCondition(string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            List<ApiExpenseReimbursementEntity> list = new List<ApiExpenseReimbursementEntity>();
            int total = 0;
            var dataList = GetSearchExpenseReimbursementList(inn, Userinfo.Roles, out total, null, searchValue, startTime, endTime, status, Userinfo.UserName);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    if (string.IsNullOrEmpty(item.status))
                    {
                        item.status = "End";
                    }

                    if (item.status == "Expense Accountant Creation" || item.status == "End")
                    {
                        List<ApiExpenseReimbursementEntity> datas = ExpenseReimbursementBll.SendExpenseAccountantCreation(inn, item.Id);
                        if (datas != null)
                        {
                            List<ApiExpenseReimbursementEntity> newApiEntity = new List<ApiExpenseReimbursementEntity>();
                            foreach (var data in datas)
                            {
                                if (data.b_TaxRate > 0)
                                {
                                    int rateInt = int.Parse((data.b_TaxRate.Value * double.Parse("100")).ToString());
                                    TaxCodeTypeList? obj = EnumDescription.GetEnumByValue<TaxCodeTypeList>(rateInt);
                                    string textDescription = EnumDescription.GetFieldText(obj);
                                    //根据描述获取对应的科目
                                    string subject = TaxCodeConfigureBll.GeTaxCodeConfigureByText(textDescription);

                                    data.DMBTR = data.b_TaxFreeAmount;
                                    ApiExpenseReimbursementEntity entity = new ApiExpenseReimbursementEntity();
                                    entity.BUKRS = data.BUKRS;
                                    entity.XBLNR = data.XBLNR;
                                    entity.BLDAT = data.BLDAT;
                                    entity.BUDAT = data.BUDAT;
                                    entity.BKTXT = data.BKTXT;
                                    entity.NUMPG = data.NUMPG;
                                    entity.PROTYP = data.PROTYP;
                                    entity.POSID = data.POSID;
                                    entity.HKONT = subject;
                                    entity.DMBTR = data.b_Tax;
                                    entity.KOSTL = "";
                                    entity.SGTXT = data.SGTXT;
                                    entity.AUFNR = data.AUFNR;
                                    newApiEntity.Add(data);
                                    newApiEntity.Add(entity);
                                }
                                else
                                {
                                    newApiEntity.Add(data);
                                }
                            }
                            //计算合计金额
                            ApiExpenseReimbursementEntity totalEntity = new ApiExpenseReimbursementEntity();
                            totalEntity.BUKRS = newApiEntity.First().BUKRS;
                            totalEntity.XBLNR = newApiEntity.First().XBLNR;
                            totalEntity.BLDAT = newApiEntity.First().BLDAT;
                            totalEntity.BUDAT = newApiEntity.First().BUDAT;
                            totalEntity.BKTXT = newApiEntity.First().b_StaffNo;
                            totalEntity.HKONT = "2241999999";
                            totalEntity.DMBTR = newApiEntity.Select(x => x.DMBTR).Sum();
                            newApiEntity.Add(totalEntity);
                            list.AddRange(newApiEntity);
                        }
                    }
                }
            }
            Stream ms = ExportExpenseReimbursementByData(list);
            return File(ms, "application/vnd.ms-excel", "凭证清单" + ".xls");
        }

        /// <summary>
        /// 导出凭证清单
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        private static Stream ExportExpenseReimbursementByData(List<ApiExpenseReimbursementEntity> list)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            // 设置字体    
            IFont headfont = workbook.CreateFont();
            headfont.FontName = "微软雅黑";

            // 建表头
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).CellStyle.SetFont(headfont);
            row.CreateCell(0).SetCellValue("公司代码");
            row.CreateCell(1).SetCellValue("报销单编号");
            row.CreateCell(2).SetCellValue("凭证日期");
            row.CreateCell(3).SetCellValue("过账日期");
            row.CreateCell(4).SetCellValue("凭证抬头文本");
            row.CreateCell(5).SetCellValue("页数");
            row.CreateCell(6).SetCellValue("项目类型");
            row.CreateCell(7).SetCellValue("WBS编号");
            row.CreateCell(8).SetCellValue("科目");
            row.CreateCell(9).SetCellValue("金额");
            row.CreateCell(10).SetCellValue("成本中心");
            row.CreateCell(11).SetCellValue("行项目文本");
            row.CreateCell(12).SetCellValue("内部订单编号");

            for (var i = 0; i < list.Count; i++)
            {
                ApiExpenseReimbursementEntity model = list[i];


                IRow rowItem = sheet.CreateRow(i + 1);
                rowItem.CreateCell(0).SetCellValue(model.BUKRS);
                rowItem.CreateCell(1).SetCellValue(model.XBLNR);
                rowItem.CreateCell(2).SetCellValue(model.BLDAT);
                rowItem.CreateCell(3).SetCellValue(model.BUDAT);
                rowItem.CreateCell(4).SetCellValue(model.BKTXT);
                rowItem.CreateCell(5).SetCellValue(model.NUMPG.ToString());
                rowItem.CreateCell(6).SetCellValue(model.PROTYP);
                rowItem.CreateCell(7).SetCellValue(model.POSID);
                rowItem.CreateCell(8).SetCellValue(model.HKONT);
                rowItem.CreateCell(9).SetCellValue(model.DMBTR.ToString());
                rowItem.CreateCell(10).SetCellValue(model.KOSTL);
                rowItem.CreateCell(11).SetCellValue(model.SGTXT);
                rowItem.CreateCell(12).SetCellValue(model.AUFNR);
            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// 管理员重新发送凭证邮件
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult ExpenseAccountantCheckSendEmail(string Id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var expenseReimbursement = inn.newItem("b_ExpenseReimbursement", "get");
                expenseReimbursement.setAttribute("id", Id);
                var result = expenseReimbursement.apply();
                if (!result.isError())
                {
                    string b_RecordNo = result.getProperty("b_recordno");
                    string b_Employee = result.getProperty("b_employee");
                    if (!string.IsNullOrEmpty(b_RecordNo) && !string.IsNullOrEmpty(b_Employee))
                    {
                        ExpenseReimbursementBll.ExpenseAccountantCheckSendEmail(inn, b_Employee, b_RecordNo);
                    }
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

    }
}