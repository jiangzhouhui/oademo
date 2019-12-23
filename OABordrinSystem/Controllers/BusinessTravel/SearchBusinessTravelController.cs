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

namespace OABordrinSystem.Controllers.BusinessTravel
{
    public class SearchBusinessTravelController : BaseController
    {
        // GET: SearchBusinessTravel
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("查询出差", "b_BusinessTravel", "BTItemType", ViewBag.language);
            return View("~/Views/BusinessTravel/SearchBusinessTravel.cshtml");
        }

        public JsonResult GetSearchBusinessTravelList(DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetSearchBusinessTravelList(Userinfo.inn, Userinfo.Roles, out total, para, searchValue, startTime, endTime, status, Userinfo.UserName);

            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    if (!string.IsNullOrEmpty(item.status) && item.status != null && item.status != "End")
                    {
                        var result = ActivityDA.GetActivityByItemId(inn, item.Id, "innovator.B_BUSINESSTRAVEL");
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
                    if (item.status == "Administrative approval" || item.status == "End")
                    {
                        linkAList += "&nbsp;&nbsp;<a class='glyphicon glyphicon-print Print' title='打印' id='" + item.Id + "'></a>";
                    }

                    strHtml = string.Format(strHtml, item.b_DocumentNo, linkAList);
                    item.b_ProjectName = "<div style='width:100px;word-wrap:break-word;'>" + item.b_ProjectName + "</div>";
                    item.b_DocumentNo = strHtml;
                    item.b_ApplicationDate = item.nb_ApplicationDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    item.status = Common.GetChineseValueByParam(item.status, "BTManageWorkFlow", "WorkFlow", Userinfo.language);
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

        public static List<BusinessTravelModel> GetSearchBusinessTravelList(Innovator inn, List<string> roles, out int total, DataTableParameter para, string searchValue, DateTime? startTime, DateTime? endTime, string status, string employee)
        {
            //获取的权限PRReader
            Item identityItem = IdentityDA.GetIdentityByKeyedName(inn, "BTReader");
            string BTReaderId = "";
            if (identityItem != null && identityItem.getItemCount() > 0)
            {
                BTReaderId = identityItem.getProperty("id");
            }
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<BusinessTravelModel> datas = (from g in db.B_BUSINESSTRAVEL
                                                               join t in db.WORKFLOW on g.id equals t.SOURCE_ID
                                                               join y in db.WORKFLOW_PROCESS on t.RELATED_ID equals y.ID
                                                               join u in db.WORKFLOW_PROCESS_ACTIVITY on y.ID equals u.SOURCE_ID
                                                               join i in db.ACTIVITY on u.RELATED_ID equals i.ID
                                                               join o in db.ACTIVITY_ASSIGNMENT on i.ID equals o.SOURCE_ID
                                                               join p in db.IDENTITY on o.RELATED_ID equals p.ID
                                                               where (roles.Contains(p.ID) || g.B_EMPLOYEE == employee || roles.Contains(BTReaderId)) && (g.B_DOCUMENTNO.Contains(searchValue) || g.B_DEPT.Contains(searchValue) || g.B_EMPLOYEE.Contains(searchValue))
                                                               select new BusinessTravelModel
                                                               {
                                                                   Id = g.id,
                                                                   b_DocumentNo = g.B_DOCUMENTNO,
                                                                   nb_ApplicationDate = g.CREATED_ON,
                                                                   b_Dept = g.B_DEPT,
                                                                   b_Employee = g.B_EMPLOYEE,
                                                                   b_ProjectName = g.B_PROJECTNAME,
                                                                   b_Destination = g.B_DESTINATION,
                                                                   status =(from z in db.B_BUSINESSTRAVEL
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
            Item result = BusinessTravelBll.GetBusinessTravelObjById(inn, id);
            BusinessTravelModel model = new BusinessTravelModel();
            model.Id = result.getProperty("id");
            model.b_DocumentNo = result.getProperty("b_documentno");
            model.b_CompanyCode = result.getProperty("b_companycode");
            model.b_Location = result.getProperty("b_location");
            model.b_ApplicationDate = DateTime.Parse(result.getProperty("b_applicationdate")).ToString("yyyy-MM-dd");
            model.b_TripType = result.getProperty("b_triptype");
            model.b_Type = result.getProperty("b_type");
            model.b_Preparer = result.getProperty("b_preparer");
            model.b_Employee = result.getProperty("b_employee");
            model.b_StaffNo = result.getProperty("b_staffno");
            model.b_Position = result.getProperty("b_position");
            model.b_Dept = result.getProperty("b_dept");
            model.b_CostCenter = result.getProperty("b_costcenter");
            model.b_Mobile = result.getProperty("b_mobile");
            model.b_ProjectName = result.getProperty("b_projectname");
            model.b_Destination = result.getProperty("b_destination");
            model.b_SeniorManager = result.getProperty("b_seniormanager");
            model.b_Director = result.getProperty("b_director");
            model.b_VP = result.getProperty("b_vp");
            model.b_TravelDate = DateTime.Parse(result.getProperty("b_traveldate")).ToString("yyyy-MM-dd");
            model.b_EstimatedReturnDate = DateTime.Parse(result.getProperty("b_estimatedreturndate")).ToString("yyyy-MM-dd");
            model.b_Purpose = result.getProperty("b_purpose");
            model.b_TravelSchedule = result.getProperty("b_travelschedule");
            model.b_TravelBudget = !string.IsNullOrEmpty(result.getProperty("b_travelbudget")) ? decimal.Parse(result.getProperty("b_travelbudget")) : 0;
            model.b_FlightIsssue = result.getProperty("b_flightisssue");
            model.b_FlightBooking = result.getProperty("b_flightbooking");
            model.b_HotelBooking = result.getProperty("b_hotelbooking");
            model.b_Didi = result.getProperty("b_didi");
            model.b_Others = result.getProperty("b_others");
            model.b_OtherContent = result.getProperty("b_othercontent");
            model.b_IsHangUp = result.getProperty("b_ishangup") == "1" ? true : false;
            model.b_HangUpActivityName = result.getProperty("b_hangupactivityname");
            model.OldRemark = result.getProperty("b_remark");
            model.b_TrafficExpense = !string.IsNullOrEmpty(result.getProperty("b_trafficexpense")) ? decimal.Parse(result.getProperty("b_trafficexpense")) : 0;
            model.b_HotelExpense = !string.IsNullOrEmpty(result.getProperty("b_hotelexpense")) ? decimal.Parse(result.getProperty("b_hotelexpense")) : 0;
            model.b_FixedSubsidy = !string.IsNullOrEmpty(result.getProperty("b_fixedsubsidy")) ? decimal.Parse(result.getProperty("b_fixedsubsidy")) : 0;
            model.b_OtherExpenses = !string.IsNullOrEmpty(result.getProperty("b_otherexpenses")) ? decimal.Parse(result.getProperty("b_otherexpenses")) : 0;
            model.b_DidiMoney = !string.IsNullOrEmpty(result.getProperty("b_didimoney")) ? decimal.Parse(result.getProperty("b_didimoney")) : 0;
            model.b_DidiAddMoney = !string.IsNullOrEmpty(result.getProperty("b_didiaddmoney")) ? decimal.Parse(result.getProperty("b_didiaddmoney")) : 0;

            //机票代订
            Item Relation = result.getRelationships("R_FlightBooking");
            if (Relation.getItemCount() > 0)
            {
                model.FlightBookingItems = new List<FlightBooking>();
                for (int i = 0; i < Relation.getItemCount(); i++)
                {
                    Item ItemObJ = Relation.getItemByIndex(i).getRelatedItem();
                    FlightBooking itemModel = new FlightBooking();
                    itemModel.Id = ItemObJ.getProperty("id");
                    itemModel.b_FirstName= ItemObJ.getProperty("b_firstname");
                    itemModel.b_LastName = ItemObJ.getProperty("b_lastname");
                    itemModel.b_IDType = ItemObJ.getProperty("b_idtype");
                    itemModel.b_IDCardNo = ItemObJ.getProperty("b_idcardno");
                    itemModel.b_Nationality = ItemObJ.getProperty("b_nationality");
                    itemModel.b_PassportNumber = ItemObJ.getProperty("b_passportnumber");
                    itemModel.b_Dateofexpiration = !string.IsNullOrEmpty(ItemObJ.getProperty("b_dateofexpiration")) ? DateTime.Parse(ItemObJ.getProperty("b_dateofexpiration")).ToString("yyyy-MM-dd") : "";
                    itemModel.b_Dateofbirth = ItemObJ.getProperty("b_dateofbirth");
                    itemModel.b_Address = ItemObJ.getProperty("b_address");
                    itemModel.b_Gooff = ItemObJ.getProperty("b_gooff");
                    itemModel.b_Goplace = ItemObJ.getProperty("b_goplace");
                    itemModel.b_Flightnumber = ItemObJ.getProperty("b_flightnumber");
                    model.FlightBookingItems.Add(itemModel);
                }
            }

            //酒店代订
            Item HotelRelation = result.getRelationships("R_HotelBooking");
            if (HotelRelation.getItemCount() > 0)
            {
                model.HotelBookingItems = new List<HotelBooking>();
                for (int i = 0; i < HotelRelation.getItemCount(); i++)
                {
                    Item ItemObJ = HotelRelation.getItemByIndex(i).getRelatedItem();
                    HotelBooking itemHote = new HotelBooking();
                    itemHote.Id = ItemObJ.getProperty("id");
                    itemHote.b_Checkindate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_checkindate")) ? DateTime.Parse(ItemObJ.getProperty("b_checkindate")).ToString("yyyy-MM-dd") : "";
                    itemHote.b_Leavedate = !string.IsNullOrEmpty(ItemObJ.getProperty("b_leavedate")) ? DateTime.Parse(ItemObJ.getProperty("b_leavedate")).ToString("yyyy-MM-dd") : "";
                    itemHote.b_Specificaddress = ItemObJ.getProperty("b_specificaddress");
                    model.HotelBookingItems.Add(itemHote);
                }
            }

            //日志信息
            model.HistoryList = GetBusinessTravelHistoryList(id);
            foreach (var item in model.HistoryList)
            {
                item.Created_on = item.Create_onStr.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss");
            }
            return View("~/Views/BusinessTravel/PrintBusinessTravel.cshtml", model);

        }


        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        private static List<HistoryModel> GetBusinessTravelHistoryList(string item_Id)
        {
            List<HistoryModel> listModel = new List<HistoryModel>();
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<HistoryModel> datas = (from g in db.B_BUSINESSTRAVEL
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
        public FileResult ExportBusinessTravelByCondition(string searchValue, DateTime? startTime, DateTime? endTime, string status)
        {
            int total = 0;
            var dataList = GetSearchBusinessTravelList(inn, Userinfo.Roles, out total, null, searchValue , endTime, startTime, status, Userinfo.UserName);
            Stream ms = ExportBusinessTravelByData(dataList);
            return File(ms, "application/vnd.ms-excel", "凭证清单" + ".xls");
        }

        /// <summary>
        /// 导出凭证清单
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Stream ExportBusinessTravelByData(List<BusinessTravelModel> dataList)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet();
            // 设置字体    
            IFont headfont = workbook.CreateFont();
            headfont.FontName = "微软雅黑";

            // 建表头
            IRow row = sheet.CreateRow(0);
            row.CreateCell(0).CellStyle.SetFont(headfont);
            row.CreateCell(0).SetCellValue("单号");
            row.CreateCell(1).SetCellValue("申请日期");
            row.CreateCell(2).SetCellValue("申请部门");
            row.CreateCell(3).SetCellValue("申请人");
            row.CreateCell(4).SetCellValue("项目名称");
            row.CreateCell(5).SetCellValue("目的地");
            row.CreateCell(6).SetCellValue("流程状态");

            dataList = dataList.OrderByDescending(x => x.b_DocumentNo).ToList();
            for (var i = 0; i < dataList.Count; i++)
            {
                BusinessTravelModel model = dataList[i];
                IRow rowItem = sheet.CreateRow(i + 1);
                rowItem.CreateCell(0).SetCellValue(model.b_DocumentNo);
                rowItem.CreateCell(1).SetCellValue(model.nb_ApplicationDate.ToString());
                rowItem.CreateCell(2).SetCellValue(model.b_Dept);
                rowItem.CreateCell(3).SetCellValue(model.b_Employee);
                rowItem.CreateCell(4).SetCellValue(model.b_ProjectName);
                rowItem.CreateCell(5).SetCellValue(model.b_Destination);
                model.status = Common.GetStatus(model.Id);
                model.status = Common.GetChineseValueByParam(model.status, "BTManageWorkFlow", "WorkFlow");
                rowItem.CreateCell(6).SetCellValue(model.status);

            }
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// 获取查询Select流程状态数据
        /// </summary>
        /// <param name="para"></param>
        /// <param name="item_Id"></param>
        /// <returns></returns>
        public JsonResult GetWorkflowStatusList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<SelectModel> list = new List<SelectModel>();
                var result = WorkFlowBll.GetWorkflowStatusList(inn, "b_BusinessTravelWorkFlow");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (int i = 0; i < result.getItemCount(); i++)
                    {
                        SelectModel model = new SelectModel();
                        var item = result.getItemByIndex(i);
                        model.value = item.getProperty("keyed_name");
                        if (!string.IsNullOrEmpty(model.value))
                        {
                            model.text = Common.GetChineseValueByParam(model.value, "BTManageWorkFlow", "WorkFlow", Userinfo.language);
                        }
                        list.Add(model);
                    }
                }
                retModel.data = list;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


    }
}