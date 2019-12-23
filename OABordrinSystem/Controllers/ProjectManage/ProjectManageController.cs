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

namespace OABordrinSystem.Controllers.ProjectManage
{
    public class ProjectManageController : BaseController
    {
        // GET: ProjectManage
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("项目管理", "CommonName", "Common", Userinfo.language);
            return View();
        }

        public JsonResult GetProjectManageList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            var dataList = GetProjectManageList(Userinfo.Roles, out total, para, searchValue);

            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-6'>{0}</div><div class='col-md-6' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' Id='" + item.id + "'></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.id + "' ></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-remove delete' title='删除' id='" + item.id + "' ></a>";
                    strHtml = string.Format(strHtml, item.b_ProjectRecordNo, linkAList);
                    item.b_ProjectRecordNo = strHtml;
                    item.b_IsInUse = item.b_IsInUse == "1" ? "是" : "否";
                    if(item.b_PrIsUse!=null)
                    {
                        item.b_PrIsUse = item.b_PrIsUse == "1" ? "是" : "否";
                    }
                    item.b_ProjectName = "<div style='width:150px;word-wrap:break-word;'>" + item.b_ProjectName + "</div>";
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


        public static List<ProjectManageModel> GetProjectManageList(List<string> roles, out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<ProjectManageModel> datas = (from g in db.B_PROJECTMANAGE
                                                        where g.B_PROJECTNAME.Contains(searchValue) || g.B_PROJECTRECORDNO.Contains(searchValue) || g.B_PMTORPATLEADER.Contains(searchValue) || g.B_PROJECTMANAGER.Contains(searchValue) || g.B_PROJECTDIRECTOR.Contains(searchValue) || g.B_PROJECTVP.Contains(searchValue)
                                                        select new ProjectManageModel()
                                                        {
                                                            id = g.id,
                                                            b_ProjectRecordNo = g.B_PROJECTRECORDNO,
                                                            b_ProjectName = g.B_PROJECTNAME,
                                                            b_PmtOrPatLeaderId = g.B_PMTORPATLEADERID,
                                                            b_PmtOrPatLeader = g.B_PMTORPATLEADER,
                                                            b_ProjectDirectorId = g.B_PROJECTDIRECTORID,
                                                            b_ProjectDirector = g.B_PROJECTDIRECTOR,
                                                            b_ProjectManagerId = g.B_PROJECTMANAGERID,
                                                            b_ProjectManager = g.B_PROJECTMANAGER,
                                                            b_IsInUse = g.B_ISINUSE,
                                                            b_ProjectVP = g.B_PROJECTVP,
                                                            b_ApplicableCompany=g.B_APPLICABLECOMPANY
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
        public JsonResult SaveProjectManage(ProjectManageModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                //判断输入的项目编号是否存在
                if (ProjectManageDA.isExistProjectRecordNo(inn, model.b_ProjectRecordNo, model.id))
                {
                    retModel.AddError("errorMessage", "输入的项目编号已经存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }


                //验证输入的项目名称是否已经存在
                if (ProjectManageDA.GetProjectManageByName(inn, model.b_ProjectName, model.id))
                {
                    retModel.AddError("errorMessage", "输入的项目名称已经存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }


                //验证输入的PMT/PAT Leader 是否存在
                var itemLeader = IdentityDA.GetIdentityByKeyedName(inn, model.b_PmtOrPatLeader);
                if (itemLeader.isError())
                {
                    retModel.AddError("errorMessage", "输入的PMT/PAT Leader不存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                model.b_PmtOrPatLeaderId = itemLeader.getProperty("id");


                //验证输入的项目经理是否存在
                var projectManager = IdentityDA.GetIdentityByKeyedName(inn, model.b_ProjectManager);
                if (projectManager.isError())
                {
                    retModel.AddError("errorMessage", "输入的项目经理不存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                model.b_ProjectManagerId = projectManager.getProperty("id");



                //验证输入的项目总监 是否存在
                var itemDirector = IdentityDA.GetIdentityByKeyedName(inn, model.b_ProjectDirector);
                if (itemDirector.isError())
                {
                    retModel.AddError("errorMessage", "输入的项目总监不存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                model.b_ProjectDirectorId = itemDirector.getProperty("id");

                //验证输入的项目VP 是否存在
                var itemProjectVp = IdentityDA.GetIdentityByKeyedName(inn, model.b_ProjectVP);
                if (itemProjectVp.isError())
                {
                    retModel.AddError("errorMessage", "输入的项目VP不存在！");
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                Item item = null;
                if (string.IsNullOrEmpty(model.id))
                {
                    item = inn.newItem("B_PROJECTMANAGE", "add");
                }
                else
                {
                    item = inn.newItem("B_PROJECTMANAGE", "edit");
                    item.setAttribute("id", model.id);

                }
                item.setProperty("b_projectrecordno", model.b_ProjectRecordNo);
                item.setProperty("b_projectname", model.b_ProjectName);
                item.setProperty("b_pmtorpatleader", model.b_PmtOrPatLeader);
                item.setProperty("b_projectdirector", model.b_ProjectDirector);
                item.setProperty("b_projectdirectorid", model.b_ProjectDirectorId);
                item.setProperty("b_pmtorpatleaderid", model.b_PmtOrPatLeaderId);
                item.setProperty("b_projectmanagerid", model.b_ProjectManagerId);
                item.setProperty("b_projectmanager", model.b_ProjectManager);
                item.setProperty("b_isinuse", model.b_IsInUse);
                item.setProperty("b_projectvp", model.b_ProjectVP);
                item.setProperty("b_sort", model.b_Sort.ToString());
                item.setProperty("b_applicablecompany", model.b_ApplicableCompany);
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
        /// 根据Id获取项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetProjectManageById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var bootItem = inn.newItem("B_PROJECTMANAGE", "get");
                bootItem.setAttribute("id", id);
                Item result = bootItem.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                ProjectManageModel model = new ProjectManageModel();
                model.id = result.getProperty("id");
                model.b_ProjectName = result.getProperty("b_projectname");
                model.b_PmtOrPatLeaderId = result.getProperty("b_pmtorpatleaderid");
                model.b_PmtOrPatLeader = result.getProperty("b_pmtorpatleader");
                model.b_ProjectDirectorId = result.getProperty("b_projectdirectorid");
                model.b_ProjectDirector = result.getProperty("b_projectdirector");
                model.b_ProjectManagerId = result.getProperty("b_projectmanagerId");
                model.b_ProjectManager = result.getProperty("b_projectmanager");
                model.b_ProjectRecordNo = result.getProperty("b_projectrecordno");
                model.b_IsInUse = result.getProperty("b_isinuse");
                model.b_ProjectVP = result.getProperty("b_projectvp");
                model.b_ApplicableCompany = result.getProperty("b_applicablecompany");
                string b_sort = result.getProperty("b_sort");
                b_sort = string.IsNullOrEmpty(b_sort)  ? "0" : b_sort;
                model.b_Sort =int.Parse(b_sort);
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除数据根据Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteProjectManageById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var result = ProjectManageDA.DeleteProjectManageById(inn, id);
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
        /// 上传项目信息
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadProjectManageFile()
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
                //保存上传文件到指定目录
                string filePath = ConfigurationManager.AppSettings["UploadPath"] + fileName;
                prfile.SaveAs(filePath);

                //获取数据库 所有用户信息
                List<USER> allUser = UserBll.GetAllUserInfo();
                List<B_PROJECTMANAGE> allProject = ProjectManageBll.GetAllProjectInfo();
                List<B_PROJECTMANAGE> list = new List<B_PROJECTMANAGE>();

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

                    //获取整个项目信息
                    List<B_ORGANIZATIONALSTRUCTURE> dataList = OrganizationalStructureBll.GetOrganizationalStructureList();

                    for (int i = 0; i < rowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (i != 0)
                        {
                            B_PROJECTMANAGE model = new B_PROJECTMANAGE();
                            model.B_PROJECTRECORDNO = row.GetCell(0) != null ? row.GetCell(0).ToString().Trim() : "";
                            model.B_PROJECTNAME = row.GetCell(1) != null ? row.GetCell(1).ToString().Trim() : "";
                            model.B_PMTORPATLEADER = row.GetCell(2) != null ? row.GetCell(2).ToString().Trim() : "";
                            model.B_PROJECTMANAGER = row.GetCell(3) != null ? row.GetCell(3).ToString().Trim() : "";
                            model.B_PROJECTDIRECTOR = row.GetCell(4) != null ? row.GetCell(4).ToString().Trim() : "";
                            model.B_PROJECTVP = row.GetCell(5) != null ? row.GetCell(5).ToString().Trim() : "";
                            model.B_ISINUSE = row.GetCell(6) != null ? row.GetCell(6).ToString().Trim() : "";

                            //判断输入的项目编号是否存在
                            if (ProjectManageDA.isExistProjectRecordNo(inn, model.B_PROJECTRECORDNO, model.id))
                            {
                                retModel.AddError("errorMessage", i + 1 + "行1列输入的项目编号已经存在！");
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }

                            //验证输入的项目名称是否已经存在
                            if (ProjectManageDA.GetProjectManageByName(inn, model.B_PROJECTNAME, model.id))
                            {
                                retModel.AddError("errorMessage", i + 1 + "行2列输入的项目名称已经存在！");
                                return Json(retModel, JsonRequestBehavior.AllowGet);
                            }

                            //判断的PMT/PAT Leader是否存在
                            if (!string.IsNullOrEmpty(model.B_PMTORPATLEADER))
                            {                                
                                var Leader = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_PMTORPATLEADER.ToUpper()).FirstOrDefault();
                                if (Leader == null)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行3列上传的PMT/PAT经理不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    model.B_PMTORPATLEADER = Leader.FIRST_NAME;
                                    var itemLeader = IdentityDA.GetIdentityByKeyedName(inn, model.B_PMTORPATLEADER);
                                    model.B_PMTORPATLEADERID = itemLeader.getProperty("id");
                                }
                            }

                            //判断项目经理是否存在
                            if (!string.IsNullOrEmpty(model.B_PROJECTMANAGER))
                            {
                                var Leader = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_PROJECTMANAGER.ToUpper()).FirstOrDefault();
                                if (Leader == null)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行4列上传的项目经理不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    model.B_PROJECTMANAGER = Leader.FIRST_NAME;
                                    var itemManager = IdentityDA.GetIdentityByKeyedName(inn,model.B_PROJECTMANAGER);
                                    model.B_PROJECTMANAGERID = itemManager.getProperty("id");
                                }
                            }

                            //判断上传的总监是否存在
                            if (!string.IsNullOrEmpty(model.B_PROJECTDIRECTOR))
                            {
                                var Leader = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_PROJECTDIRECTOR.ToUpper()).FirstOrDefault();
                                if (Leader == null)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行5列上传的总监不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    model.B_PROJECTDIRECTOR = Leader.FIRST_NAME;
                                    var itemDirector = IdentityDA.GetIdentityByKeyedName(inn,model.B_PROJECTDIRECTOR);
                                    model.B_PROJECTDIRECTORID = itemDirector.getProperty("id");
                                }
                            }

                            //判断上传的VP是否存在
                            if (!string.IsNullOrEmpty(model.B_PROJECTVP))
                            {
                                var Leader = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_PROJECTVP.ToUpper()).FirstOrDefault();
                                if (Leader == null)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行6列上传的VP不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    model.B_PROJECTVP = Leader.FIRST_NAME;
                                }
                            }
                            list.Add(model);
                        }
                    }
   
                    list = list.Distinct().ToList();

                    //把数据添加到数据库中
                    if (list != null && list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            B_PROJECTMANAGE ProManagerObj = allProject.Where(x => item.B_PROJECTRECORDNO.ToUpper() == item.B_PROJECTNAME.ToUpper()).First();
                            var promanager = inn.newItem("b_ProjectManage", "add");
                            promanager.setProperty("b_projectrecordno", item.B_PROJECTRECORDNO);
                            promanager.setProperty("b_projectname", item.B_PROJECTNAME);                           
                            promanager.setProperty("b_pmtorpatleaderid", item.B_PMTORPATLEADERID);                            
                            promanager.setProperty("b_pmtorpatleader", item.B_PMTORPATLEADER);
                            promanager.setProperty("b_projectmanagerid", item.B_PROJECTMANAGERID);                           
                            promanager.setProperty("b_projectmanager", item.B_PROJECTMANAGER);
                            promanager.setProperty("b_projectdirectorid", item.B_PROJECTDIRECTORID);
                            promanager.setProperty("b_projectdirector", item.B_PROJECTDIRECTOR);
                            promanager.setProperty("b_projectvp", item.B_PROJECTVP);                   
                            promanager.setProperty("b_isinuse", item.B_ISINUSE);                           
                            var result = promanager.apply();
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