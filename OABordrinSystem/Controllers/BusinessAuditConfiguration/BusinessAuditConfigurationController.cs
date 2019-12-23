using Aras.IOM;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.BusinessAuditConfiguration
{
    public class BusinessAuditConfigurationController : BaseController
    {
        // GET: BusinessAuditConfiguration
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取审核配置列表
        /// </summary>
        /// <param name="para"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public JsonResult GetAuditConfigurationList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            var dataList = GetAuditConfigurationList(out total, para, searchValue);

            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    item.b_Type = item.b_Type == "Project" ? "项目" : "非项目";
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' Id='" + item.Id + "'></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.Id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' id='" + item.Id + "' ></a>";
                    strHtml = string.Format(strHtml, item.b_RoleName, linkAList);
                    item.b_RoleName = strHtml;
                    item.b_CostCenters = "<div style='width:200px;word-wrap:break-word;'>" + item.b_CostCenters + "</div>";
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

        public static List<ExpenseAuditConfigurationModel> GetAuditConfigurationList(out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {

                IQueryable<ExpenseAuditConfigurationModel> datas = (from g in db.B_EXPENSEAUDITCONFIGURATION
                                                                    where g.B_ROLENAME.Contains(searchValue) || g.B_TYPE.Contains(searchValue) || g.B_COMPANYCODE.Contains(searchValue) || g.B_PROJECTNAME.Contains(searchValue) || g.B_COSTCENTERS.Contains(searchValue) || g.B_HANDLEPERSONS.Contains(searchValue)
                                                                    select new ExpenseAuditConfigurationModel()
                                                                    {
                                                                        Id = g.id,
                                                                        b_RoleName = g.B_ROLENAME,
                                                                        b_Type = g.B_TYPE,
                                                                        b_CompanyCode = g.B_COMPANYCODE,
                                                                        b_ProjectName = g.B_PROJECTNAME,
                                                                        b_CostCenters = g.B_COSTCENTERS,
                                                                        b_HandlePersons = g.B_HANDLEPERSONS

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
        /// 保存报销审核配置
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveBusinessAuditConfiguration(ExpenseAuditConfigurationModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<B_EXPENSEAUDITCONFIGURATION> datalist = ExpenseAuditConfigurationBll.GetAllExpenseAuditConfiguration();

                if (model.b_Type == "Non Project")
                {
                    if (string.IsNullOrEmpty(model.b_CostCenters))
                    {
                        retModel.AddError("errorMessage", "当为非项目时，必须选择成本中心！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }



                    //验证输入的成本中心代码是否正确
                    List<string> list = model.b_CostCenters.Split(',').ToList();
                    if (list != null && list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            var OrganizationalStructure = OrganizationalStructureBll.GetOrganizationalStructureByCostCenter(item);
                            if (OrganizationalStructure == null)
                            {
                                retModel.AddError("errorMessage", "选择的成本中心代码不存在！");
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }

                            var obj = datalist.Where(x => x.B_ROLENAME == model.b_RoleName && x.B_TYPE == model.b_Type && x.B_COMPANYCODE == model.b_CompanyCode && x.B_COSTCENTERS.Split(',').Contains(item) && x.id != model.Id).FirstOrDefault();
                            if (obj != null)
                            {
                                string errorStr = "角色名称：" + model.b_RoleName + "、项目类型：" + (model.b_Type == "Project" ? "项目" : "非项目") + "、公司代码：" + model.b_CompanyCode + "、成本中心代码：" + item + ",已经存在!";
                                retModel.AddError("errorMessage", errorStr);
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }

                }
                else
                {
                    //验证输入的项目名称是否正确
                    if (string.IsNullOrEmpty(model.b_ProjectName))
                    {
                        retModel.AddError("errorMessage", "当为项目时必须选择项目名称！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    var projectItem = ProjectManageDA.GetProjectManageByName(inn, model.b_ProjectName);
                    if (projectItem.isError() || projectItem.getItemCount() == 0)
                    {
                        retModel.AddError("errorMessage", "输入的项目名称不存在！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    var obj = datalist.Where(x => x.B_ROLENAME == model.b_RoleName && x.B_TYPE == model.b_Type && x.B_COMPANYCODE == model.b_CompanyCode && x.B_PROJECTNAME == model.b_ProjectName && x.id != model.Id).FirstOrDefault();
                    if (obj != null)
                    {
                        string errorStr = "角色名称：" + model.b_RoleName + "、项目类型：" + (model.b_Type == "Project" ? "项目" : "非项目") + "、公司代码：" + model.b_CompanyCode + "、项目名称：" + model.b_ProjectName + ",已经存在!";
                        retModel.AddError("errorMessage", errorStr);
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //判断输入的处理人是否存在
                if (!string.IsNullOrEmpty(model.b_HandlePersons))
                {
                    List<string> users = model.b_HandlePersons.Split(';').Where(x => x != "").Distinct().ToList();
                    foreach (var user in users)
                    {
                        if (!UserDA.ValidUserIsExist(inn, user))
                        {
                            retModel.AddError("errorMessage", "输入的人员在系统中不存在！");
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                    model.b_HandlePersons = "";
                    foreach (var item in users)
                    {
                        model.b_HandlePersons = model.b_HandlePersons + item + ";";
                    }

                }
                Item ExpenseAuditConfiguration;
                if (string.IsNullOrEmpty(model.Id))
                {
                    ExpenseAuditConfiguration = inn.newItem("b_ExpenseAuditConfiguration", "add");
                }
                else
                {
                    ExpenseAuditConfiguration = inn.newItem("b_ExpenseAuditConfiguration", "edit");
                    ExpenseAuditConfiguration.setAttribute("id", model.Id);
                }
                ExpenseAuditConfiguration.setProperty("b_rolename", model.b_RoleName);
                ExpenseAuditConfiguration.setProperty("b_type", model.b_Type);
                ExpenseAuditConfiguration.setProperty("b_companycode", model.b_CompanyCode);
                if (model.b_Type == "Non Project")
                {
                    ExpenseAuditConfiguration.setProperty("b_projectname", "");
                    ExpenseAuditConfiguration.setProperty("b_costcenters", model.b_CostCenters);
                }
                else
                {
                    ExpenseAuditConfiguration.setProperty("b_projectname", model.b_ProjectName);
                    ExpenseAuditConfiguration.setProperty("b_costcenters", "");
                }

                ExpenseAuditConfiguration.setProperty("b_handlepersons", model.b_HandlePersons);
                var result = ExpenseAuditConfiguration.apply();
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
        /// 根据Id 获取数据对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetExpenseAuditConfigurationById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var expenseAuditConfiguration = ExpenseAuditConfigurationBll.GetExpenseAuditConfigurationById(inn, id);
                ExpenseAuditConfigurationModel model = new ExpenseAuditConfigurationModel();
                model.Id = expenseAuditConfiguration.id;
                model.b_RoleName = expenseAuditConfiguration.B_ROLENAME;
                model.b_Type = expenseAuditConfiguration.B_TYPE;
                model.b_CompanyCode = expenseAuditConfiguration.B_COMPANYCODE;
                model.b_ProjectName = expenseAuditConfiguration.B_PROJECTNAME;
                model.b_CostCenters = expenseAuditConfiguration.B_COSTCENTERS;
                model.b_HandlePersons = expenseAuditConfiguration.B_HANDLEPERSONS;
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除费用审核配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteBusinessAuditConfiguration(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                ExpenseAuditConfigurationBll.DeleteExpenseAuditConfiguration(inn, id);
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        //上传审核人信息
        public JsonResult UploadExpenseAuditFile()
        {
            var retModel = new JsonReturnModel();
            try
            {
                if (Request.Files == null || Request.Files.Count == 0)
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("请选择您要上传的附件！", "PRCommon", "PRItemType", Userinfo.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                HttpPostedFileBase prfile = Request.Files[0];
                string fileName = prfile.FileName.Substring(prfile.FileName.LastIndexOf("\\") + 1, prfile.FileName.Length - (prfile.FileName.LastIndexOf("\\")) - 1);

                if (!fileName.ToLower().Contains(".xls") && !fileName.ToLower().Contains(".xlsx"))
                {
                    retModel.AddError("errorMessage", "只能上传Excel文件！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                string filePath = ConfigurationManager.AppSettings["UploadPath"] + fileName;
                prfile.SaveAs(filePath);


                //获取数据库审核人信息
                List<USER> allUser = UserBll.GetAllUserInfo();
                List<B_EXPENSEAUDITCONFIGURATION> list = new List<B_EXPENSEAUDITCONFIGURATION>();

                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    IWorkbook workbook = null;

                    if (fileName.ToLower().Contains(".xlsx"))
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    else
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    ISheet sheet = workbook.GetSheetAt(0);
                    int rowNum = sheet.PhysicalNumberOfRows;

                    for (int i = 0; i < rowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (i != 0)
                        {
                            B_EXPENSEAUDITCONFIGURATION model = new B_EXPENSEAUDITCONFIGURATION();
                            model.B_ROLENAME = row.GetCell(0) != null ? row.GetCell(0).ToString().Trim() : "";
                            model.B_TYPE = row.GetCell(1) != null ? row.GetCell(1).ToString().Trim() : "";
                            model.B_COMPANYCODE = row.GetCell(2) != null ? row.GetCell(2).ToString().Trim() : "";
                            model.B_PROJECTNAME = row.GetCell(3) != null ? row.GetCell(3).ToString().Trim() : "";
                            model.B_COSTCENTERS = row.GetCell(4) != null ? row.GetCell(4).ToString().Trim() : "";
                            model.B_HANDLEPERSONS = row.GetCell(5) != null ? row.GetCell(5).ToString().Trim() : "";

                            if (model.B_TYPE == "非项目")
                            {
                                model.B_TYPE = "Non Project";
                            }
                            else if (model.B_TYPE == "项目")
                            {
                                model.B_TYPE = "Project";
                            }
                            else
                            {
                                retModel.AddError("errorMessage", i + 1 + "行2列输入的项目类型错误！");
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }

                            //判断输入的处理人是否存在
                            if (!string.IsNullOrEmpty(model.B_HANDLEPERSONS))
                            {
                                var name = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_HANDLEPERSONS.ToUpper()).FirstOrDefault();
                                model.B_HANDLEPERSONS.Split(';').Where(x => x != "").Distinct().ToList();
                                if (name == null)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行6列上传的人员在系统中不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    model.B_HANDLEPERSONS = name.FIRST_NAME;
                                }
                            }

                            if (model.B_TYPE == "Non Project")
                            {
                                if (string.IsNullOrEmpty(model.B_COSTCENTERS))
                                {
                                    retModel.AddError("errorMessage", i + 1 + "当为非项目时，必须选择成本中心！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }

                                //验证输入的成本中心代码是否正确
                                List<string> listcon = model.B_COSTCENTERS.Split(',').Where(x => x != "").ToList();
                                if (listcon != null && listcon.Count > 0)
                                {
                                    foreach (var item in listcon)
                                    {
                                        var OrganizationalStructure = OrganizationalStructureBll.GetOrganizationalStructureByCostCenter(item);
                                        if (OrganizationalStructure == null)
                                        {
                                            retModel.AddError("errorMessage", i + 1 + "行选择的成本中心" + item + "代码不存在！");
                                            return Json(retModel, JsonRequestBehavior.AllowGet);
                                        }

                                        var obj = list.Where(x => x.B_ROLENAME == model.B_ROLENAME && x.B_TYPE == model.B_TYPE && x.B_COMPANYCODE == model.B_COMPANYCODE && x.B_COSTCENTERS.Split(',').Contains(item)).FirstOrDefault();
                                        if (obj != null)
                                        {
                                            string errorStr = i + 1 + "行角色名称：" + model.B_ROLENAME + "、项目类型：" + (model.B_TYPE == "Project" ? "项目" : "非项目") + "、公司代码：" + model.B_COMPANYCODE + "、成本中心代码：" + item + ",导入时重复!";
                                            retModel.AddError("errorMessage", errorStr);
                                            return Json(retModel, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //验证输入的项目名称是否正确
                                if (string.IsNullOrEmpty(model.B_PROJECTNAME))
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行当为项目时必须选择项目名称！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }

                                var projectItem = ProjectManageDA.GetProjectManageByName(inn, model.B_PROJECTNAME);
                                if (projectItem.isError() || projectItem.getItemCount() == 0)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行4列输入的项目名称不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }

                                var obj = list.Where(x => x.B_ROLENAME == model.B_ROLENAME && x.B_TYPE == model.B_TYPE && x.B_COMPANYCODE == model.B_COMPANYCODE && x.B_PROJECTNAME == model.B_PROJECTNAME).FirstOrDefault();
                                if (obj != null)
                                {
                                    string errorStr = i + 1 + "行角色名称：" + model.B_ROLENAME + "、项目类型：" + (model.B_TYPE == "Project" ? "项目" : "非项目") + "、公司代码：" + model.B_COMPANYCODE + "、项目名称：" + model.B_PROJECTNAME + ",导入时重复!";
                                    retModel.AddError("errorMessage", errorStr);
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                            }
                            list.Add(model);
                        }
                    }
                    list = list.Distinct().ToList();

                    //把数据添加到数据库中
                    if (list != null && list.Count > 0)
                    {
                        //删除审核人配置表数据
                        var result = ExpenseAuditConfigurationBll.DeleteExpenseAuditConfiguration(inn);
                        if (result.isError())
                        {
                            retModel.AddError("errorMessage", "上传发生错误！");
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }

                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            var ExpAudit = inn.newItem("B_EXPENSEAUDITCONFIGURATION", "add");
                            ExpAudit.setProperty("b_rolename", item.B_ROLENAME);
                            ExpAudit.setProperty("b_type", item.B_TYPE);
                            if (item.B_TYPE == "Non Project")
                            {
                                ExpAudit.setProperty("b_projectname", "");
                                ExpAudit.setProperty("b_costcenters", item.B_COSTCENTERS);
                            }
                            else
                            {
                                ExpAudit.setProperty("b_projectname", item.B_PROJECTNAME);
                                ExpAudit.setProperty("b_costcenters", "");
                            }
                            ExpAudit.setProperty("b_companycode", item.B_COMPANYCODE);
                            string companyName = CompanyInfoBll.GetCompanyNameByCode(inn, item.B_COMPANYCODE);
                            string value = item.B_COMPANYCODE + " (" + companyName + ")";
                            ExpAudit.setProperty("b_companycode", item.B_COMPANYCODE = value);
                            ExpAudit.setProperty("b_handlepersons", item.B_HANDLEPERSONS);
                            var resultAdd = ExpAudit.apply();
                        }
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