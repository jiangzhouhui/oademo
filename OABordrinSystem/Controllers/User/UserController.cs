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

namespace OABordrinSystem.Controllers.User
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("人员信息", "CommonName", "Common", ViewBag.language);
            return View();
        }

        /// <summary>
        /// 获取人员信息列表
        /// </summary>
        /// <param name="para"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public JsonResult GetUserList(DataTableParameter para, string searchValue)
        {
            int total = 0;
            var dataList = GetUserList(out total, para, searchValue);
            if (dataList != null)
            {
                foreach (var item in dataList)
                {
                    string strHtml = "<div class='row'><div class='col-md-8'>{0}</div><div class='col-md-4' style='text-align:right'>{1}</div></div>";
                    string linkAList = "<a class='glyphicon glyphicon-pencil edit' title='修改' Id='" + item.Id + "'></a>";
                    linkAList += "&nbsp;&nbsp;&nbsp;" + "<a class='glyphicon glyphicon-eye-open detail' title='详情' id='" + item.Id + "' ></a>";
                    strHtml = string.Format(strHtml, item.first_Name, linkAList);
                    item.first_Name = strHtml;
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


        public static List<UserModel> GetUserList(out int total, DataTableParameter para, string searchValue)
        {
            using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
            {
                IQueryable<UserModel> datas = (from g in db.USER
                                               where g.LOGON_ENABLED=="1" && (g.FIRST_NAME.Contains(searchValue) || g.B_JOBNUMBER.Contains(searchValue) || g.EMAIL.Contains(searchValue) || g.B_CENTRE.Contains(searchValue) || g.B_DEPARTMENT.Contains(searchValue) || g.B_SENIORMANAGER.Contains(searchValue) || g.B_DIRECTOR.Contains(searchValue) || g.B_VP.Contains(searchValue))
                                               select new UserModel()
                                               {
                                                   Id = g.ID,
                                                   first_Name = g.FIRST_NAME,
                                                   b_JobNumber = g.B_JOBNUMBER,
                                                   Email = g.EMAIL,
                                                   b_Centre = g.B_CENTRE,
                                                   b_Department = g.B_DEPARTMENT,
                                                   b_SeniorManager = g.B_SENIORMANAGER,
                                                   b_Director = g.B_DIRECTOR,
                                                   b_VP = g.B_VP,
                                                   b_AffiliatedCompany=g.B_AFFILIATEDCOMPANY
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
        /// 保存用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveUser(UserModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {

                //判断中心部门是否存在
                if (!string.IsNullOrEmpty(model.b_Centre))
                {
                    var centreItem = OrganizationalStructureDA.GetOrganizationalStructureByParam(inn, model.b_Centre, 2);
                    if (!centreItem.isError() && centreItem.getItemCount() <= 0)
                    {
                        retModel.AddError("errorMessage", "填写的中心不存在！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }

                    if (!string.IsNullOrEmpty(model.b_Department))
                    {
                        //获取中心部门节点代号
                        string centreNodeCode = centreItem.getProperty("b_nodecode");

                        //判断部门是否在中心部门下存在
                        List<B_ORGANIZATIONALSTRUCTURE> organizationalStructureList = new List<B_ORGANIZATIONALSTRUCTURE>();
                        //获取组织架构
                        List<B_ORGANIZATIONALSTRUCTURE> dataList = OrganizationalStructureBll.GetOrganizationalStructureList();

                        OrganizationalStructureBll.GetChildByParent(inn, centreNodeCode, organizationalStructureList, dataList);

                        int count = organizationalStructureList.Where(x => x.B_NODENAME == model.b_Department).Count();
                        if (count <= 0)
                        {
                            retModel.AddError("errorMessage", "填写的部门在中心下不存在！");
                            return Json(retModel, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                //判断输入的高级经理是否存在
                if (!string.IsNullOrEmpty(model.b_SeniorManager))
                {
                    var itemSeniorManager = IdentityDA.GetIdentityByKeyedName(inn, model.b_SeniorManager);
                    if (itemSeniorManager.isError())
                    {
                        retModel.AddError("errorMessage", "输入的高级经理不存在！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //判断输入的总监是否存在
                if (!string.IsNullOrEmpty(model.b_Director))
                {
                    var itemDirector = IdentityDA.GetIdentityByKeyedName(inn, model.b_Director);
                    if (itemDirector.isError())
                    {
                        retModel.AddError("errorMessage", "输入的总监不存在！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                //判断输入的VP是否存在
                if (!string.IsNullOrEmpty(model.b_VP))
                {
                    var itemVP = IdentityDA.GetIdentityByKeyedName(inn, model.b_VP);
                    if (itemVP.isError())
                    {
                        retModel.AddError("errorMessage", "输入的VP不存在！");
                        return Json(retModel, JsonRequestBehavior.AllowGet);
                    }
                }

                Innovator adminInn = WorkFlowBll.GetAdminInnovator();
                if (adminInn != null)
                {
                    var item = adminInn.newItem("User", "edit");
                    item.setAttribute("id", model.Id);
                    item.setProperty("b_jobnumber", model.b_JobNumber);
                    item.setProperty("b_chinesename", model.b_ChineseName);
                    item.setProperty("b_englishname", model.b_EnglishName);
                    item.setProperty("email", model.Email);
                    item.setProperty("telephone", model.Telephone);
                    item.setProperty("b_centre", model.b_Centre);
                    item.setProperty("b_department", model.b_Department);
                    item.setProperty("b_idnumber", model.b_IdNumber);
                    item.setProperty("b_seniormanager", model.b_SeniorManager);
                    item.setProperty("b_director", model.b_Director);
                    item.setProperty("b_vp", model.b_VP);
                    item.setProperty("b_affiliatedcompany", model.b_AffiliatedCompany);
                    var result = item.apply();
                    if (result.isError())
                    {
                        retModel.AddError("errorMessage", result.getErrorString());
                    }
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
        public JsonResult GetUserById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var bootItem = inn.newItem("User", "get");
                bootItem.setAttribute("id", id);
                Item result = bootItem.apply();
                if (result.isError())
                {
                    retModel.AddError("errorMessage", result.getErrorString());
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }
                UserModel model = new UserModel();

                model.Id = result.getProperty("id");
                model.first_Name = result.getProperty("first_name");
                model.Email = result.getProperty("email");
                model.Telephone = result.getProperty("telephone");
                model.b_JobNumber = result.getProperty("b_jobnumber");
                model.b_ChineseName = result.getProperty("b_chinesename");
                model.b_EnglishName = result.getProperty("b_englishname");
                model.b_Centre = result.getProperty("b_centre");
                model.b_Department = result.getProperty("b_department");
                model.b_IdNumber = result.getProperty("b_idnumber");
                model.b_SeniorManager = result.getProperty("b_seniormanager");
                model.b_Director = result.getProperty("b_director");
                model.b_VP = result.getProperty("b_vp");
                model.b_AffiliatedCompany = result.getProperty("b_affiliatedcompany");
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        //上传人员信息
        public JsonResult UploadUserFile()
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
                //获取数据库  所有的用户信息
                List<USER> allUser = UserBll.GetAllUserInfo();
                List<USER> list = new List<USER>();
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

                    //获取整个组织架构
                    List<B_ORGANIZATIONALSTRUCTURE> dataList = OrganizationalStructureBll.GetOrganizationalStructureList();

                    for (int i = 0; i < rowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (i != 0)
                        {
                            USER model = new USER();
                            if (row.GetCell(0) != null)
                            {
                                model.B_JOBNUMBER = row.GetCell(0) != null ?  row.GetCell(0).ToString().Trim() : "";
                                model.B_CHINESENAME = row.GetCell(1) != null ? row.GetCell(1).ToString().Trim() : "";
                                model.B_ENGLISHNAME = row.GetCell(2) != null ? row.GetCell(2).ToString().Trim() : "";
                                model.B_CENTRE = row.GetCell(3) != null ? row.GetCell(3).ToString().Trim() : "";
                                model.B_DEPARTMENT = row.GetCell(4) != null ? row.GetCell(4).ToString().Trim() : "";
                                model.B_SENIORMANAGER = row.GetCell(5) != null ? row.GetCell(5).ToString().Trim() : "";
                                model.B_DIRECTOR = row.GetCell(6) != null ? row.GetCell(6).ToString().Trim() : "";
                                model.B_VP = row.GetCell(7) != null ? row.GetCell(7).ToString().Trim() : "";
                                model.B_AFFILIATEDCOMPANY = row.GetCell(8) != null ? row.GetCell(8).ToString().Trim() : "";

                                //根据用户名称判断用户是否存在
                                int count = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_ENGLISHNAME.ToUpper()).Count();
                                if (count == 0)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行上传的用户不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }


                                //判断中心是否存在
                                B_ORGANIZATIONALSTRUCTURE centreObj = dataList.Where(x => x.B_NODENAME == model.B_CENTRE && x.B_NODELEVEL == 2).FirstOrDefault();
                                if (centreObj == null)
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行上传的中心不存在！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }

                                //判断部门是否存在
                                List<B_ORGANIZATIONALSTRUCTURE> organizationalStructureList = new List<B_ORGANIZATIONALSTRUCTURE>();
                                if (!string.IsNullOrEmpty(model.B_DEPARTMENT))
                                {
                                    OrganizationalStructureBll.GetChildByParent(inn, centreObj.B_NODECODE, organizationalStructureList, dataList);
                                    int countDepartment = organizationalStructureList.Where(x => x.B_NODENAME == model.B_DEPARTMENT).Count();
                                    if (countDepartment == 0)
                                    {
                                        retModel.AddError("errorMessage", i + 1 + "行上传的部门不存在！");
                                        return Json(retModel, JsonRequestBehavior.AllowGet);
                                    }
                                }

                                //判断上传的高级经理是否存在
                                if (!string.IsNullOrEmpty(model.B_SENIORMANAGER))
                                {
                                    var itemSeniorManager = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_SENIORMANAGER.ToUpper()).FirstOrDefault();
                                    if (itemSeniorManager == null)
                                    {
                                        retModel.AddError("errorMessage", i + 1 + "行上传的高级经理不存在！");
                                        return Json(retModel, JsonRequestBehavior.AllowGet);
                                    }
                                    model.B_SENIORMANAGER = itemSeniorManager.FIRST_NAME;
                                }

                                //判断上传的总监是否存在
                                if (!string.IsNullOrEmpty(model.B_DIRECTOR))
                                {
                                    var itemDirector = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_DIRECTOR.ToUpper()).FirstOrDefault();
                                    if (itemDirector == null)
                                    {
                                        retModel.AddError("errorMessage", i + 1 + "行上传的总监不存在！");
                                        return Json(retModel, JsonRequestBehavior.AllowGet);
                                    }
                                    model.B_DIRECTOR = itemDirector.FIRST_NAME;
                                }

                                //判断上传的VP是否存在
                                if (!string.IsNullOrEmpty(model.B_VP))
                                {
                                    var itemVP = allUser.Where(x => x.LOGIN_NAME.ToUpper() == model.B_VP.ToUpper()).FirstOrDefault();
                                    if (itemVP == null)
                                    {
                                        retModel.AddError("errorMessage", i + 1 + "行上传的VP不存在！");
                                        return Json(retModel, JsonRequestBehavior.AllowGet);
                                    }
                                    model.B_VP = itemVP.FIRST_NAME;
                                }

                                //判断上传所属公司是否正确
                                if(!string.IsNullOrEmpty(model.B_AFFILIATEDCOMPANY))
                                {
                                    List<string> arrList = model.B_AFFILIATEDCOMPANY.Split(';').Where(x => x != "").ToList();
                                    foreach(var item in arrList)
                                    {
                                        if(item!= "博郡" && item!="思致")
                                        {
                                            retModel.AddError("errorMessage", i + 1 + "行上传的所属公司不正确！");
                                            return Json(retModel, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                                list.Add(model);
                            }
                        }
                    }

                    Innovator adminInn = WorkFlowBll.GetAdminInnovator();

                    //修改数据库中的数据
                    if (list != null && list.Count > 0 && adminInn != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            //string userName = item.B_ENGLISHNAME.ToUpper() + " " + "(" + item.B_CHINESENAME + ")";
                            USER userObj = allUser.Where(x => x.LOGIN_NAME.ToUpper() == item.B_ENGLISHNAME.ToUpper()).First();
                            var user = adminInn.newItem("User", "edit");
                            user.setAttribute("id", userObj.ID);
                            user.setProperty("b_jobnumber", item.B_JOBNUMBER);
                            user.setProperty("b_chinesename", item.B_CHINESENAME);
                            user.setProperty("b_englishname", item.B_ENGLISHNAME);
                            user.setProperty("b_centre", item.B_CENTRE);
                            user.setProperty("b_department", item.B_DEPARTMENT);
                            user.setProperty("b_seniormanager", item.B_SENIORMANAGER);
                            user.setProperty("b_director", item.B_DIRECTOR);
                            user.setProperty("b_vp", item.B_VP);
                            user.setProperty("b_affiliatedcompany", item.B_AFFILIATEDCOMPANY);
                            var result = user.apply();
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