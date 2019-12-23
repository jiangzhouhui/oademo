using Aras.IOM;
using OABordrinBll;
using OABordrinCommon;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.ExpenseCategory
{
    public class ExpenseCategoryController : BaseController
    {
        // GET: ExpenseCategory
        public ActionResult Index()
        {
            ViewBag.CurrentName = @OABordrinCommon.Common.GetLanguageValueByParam("费用类别", "ERCommon", "ERItemType", ViewBag.language);
            return View();
        }

        public JsonResult GetExpenseCategoryList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            var dataList = GetExpenseCategoryList(out total, para, searchValue);

            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' Id='" + item.Id + "'></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.Id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' id='" + item.Id + "' ></a>";
                    strHtml = string.Format(strHtml, item.b_CostName, linkAList);
                    item.b_CostName = strHtml;
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


        public static List<ExpenseCategoryModel> GetExpenseCategoryList(out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<ExpenseCategoryModel> datas = (from g in db.B_EXPENSECATEGORY
                                                          where g.B_COSTNAME.Contains(searchValue) || g.B_CORRESPONSUBJECT.Contains(searchValue)
                                                          select new ExpenseCategoryModel()
                                                          {
                                                              Id = g.id,
                                                              b_CostName = g.B_COSTNAME,
                                                              b_CorresponSubject = g.B_CORRESPONSUBJECT

                                                          });

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
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveExpenseCategory(ExpenseCategoryModel model)
        {
            var retModel = new JsonReturnModel();

            try
            {
                //判断费用类别是否已经存在
                if (ExpenseCategoryBll.IsExistExpenseCategory(inn, model.b_CostName, model.Id))
                {
                    retModel.AddError("errorMessage", @OABordrinCommon.Common.GetLanguageValueByParam("输入的费用类别已经存在！", "ERCommon", "ERItemType", ViewBag.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                Item item = null;
                if (string.IsNullOrEmpty(model.Id))
                {
                    item = inn.newItem("b_ExpenseCategory", "add");
                }
                else
                {
                    item = inn.newItem("b_ExpenseCategory", "edit");
                    item.setAttribute("id", model.Id);
                }
                item.setProperty("b_costname", model.b_CostName);
                item.setProperty("b_corresponsubject", model.b_CorresponSubject);
                var result = item.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                }
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetExpenseCategoryById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var bootItem = inn.newItem("b_ExpenseCategory", "get");
                bootItem.setAttribute("id", id);
                Item result = bootItem.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                ExpenseCategoryModel model = new ExpenseCategoryModel();

                model.Id = result.getProperty("id");
                model.b_CostName = result.getProperty("b_costname");
                model.b_CorresponSubject = result.getProperty("b_corresponsubject");
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除费用类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteExpenseCategoryById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var result = ExpenseCategoryBll.DeleteExpenseCategoryById(inn, id);
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                }
            }
            catch(Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }
    }
}