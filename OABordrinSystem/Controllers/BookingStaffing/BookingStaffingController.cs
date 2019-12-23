using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.BookingStaffing
{
    public class BookingStaffingController : BaseController
    {
        // GET: BookingStaffing
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("行政代订", "BTCommon", "BTItemType", ViewBag.language);
            return View();
        }

        public JsonResult GetBookingStaffingList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            List<BookingStaffingModel> dataList = new List<BookingStaffingModel>();
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<BookingStaffingModel> datas = (from g in db.B_BOOKINGSTAFFING
                                                          where g.B_USERNAME.Contains(searchValue)
                                                          select new BookingStaffingModel
                                                          {
                                                              id=g.id,
                                                              b_UserName = g.B_USERNAME
                                                          });

                datas = datas.Distinct();
                total = datas.Count();

                //排序
                if (para != null)
                {
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
                dataList = datas.ToList();

                if(dataList!=null)
                {
                    foreach(var item in dataList)
                    {
                        string strHtml = "<div class='row'><div class='col-md-3'>{0}</div><div class='col-md-9' style='text-align:left'>{1}</div></div>";
                        string linkAList = "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.id + "' ></a>";
                        linkAList += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' id='" + item.id + "' ></a>";
                        strHtml = string.Format(strHtml, item.b_UserName, linkAList);
                        item.b_UserName = strHtml;
                    }
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
        /// 保存行政代订
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveBookingStaffing(BookingStaffingModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //验证输入的员工名称是否存在
                //验证高级经理
                if (!string.IsNullOrEmpty(model.b_UserName))
                {
                    Item UserNameObj = IdentityDA.GetIdentityByKeyedName(inn, model.b_UserName);
                    if (UserNameObj.isError())
                    {
                        retModel.AddError("errorMessage", "找不到对应的员工名称！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                var BookingStaffing = inn.newItem("b_BookingStaffing", "add");
                BookingStaffing.setProperty("b_username", model.b_UserName);
                var result = BookingStaffing.apply();
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
        /// 删除行政代订
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult deleteBookingStaffing(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                BookingStaffingDA.DeleteBookingStaffing(inn,id);
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetBookingStaffingById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
                {
                    var item= db.B_BOOKINGSTAFFING.Where(x => x.id == id).FirstOrDefault();
                    if(item!=null)
                    {
                        BookingStaffingModel model = new BookingStaffingModel();
                        model.id = item.id;
                        model.b_UserName = item.B_USERNAME;
                        retModel.data = model;
                    }
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