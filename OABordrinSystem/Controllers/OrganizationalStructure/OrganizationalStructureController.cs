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
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers.OrganizationalStructure
{
    public class OrganizationalStructureController : BaseController
    {
        // GET: OrganizationalStructure
        public ActionResult Index()
        {
            ViewBag.CurrentName = Common.GetLanguageValueByParam("组织架构", "CommonName", "Common", ViewBag.language);
            return View();
        }

        /// <summary>
        /// 获取组织架构列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOrganizationalStructureList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<OrganizationalStructureModel> list = new List<OrganizationalStructureModel>();
                //获取组织架构
                List<B_ORGANIZATIONALSTRUCTURE> dataList = OrganizationalStructureBll.GetOrganizationalStructureList();

                //获取树形结构的数据
                List<B_ORGANIZATIONALSTRUCTURE> data = new List<B_ORGANIZATIONALSTRUCTURE>();
                OrganizationalStructureBll.GetChildByParent(inn, "", data, dataList);
                if (data != null && data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        OrganizationalStructureModel model = new OrganizationalStructureModel();
                        var item = data[i];
                        model.id = item.id;
                        model.b_NodeName = item.B_NODENAME;
                        model.b_NodeCode = item.B_NODECODE;
                        model.b_ParentNodeCode = item.B_PARENTNODECODE;
                        model.b_NodePersonName = item.B_NODEPERSONNAME;
                        model.b_NodeLevel = item.B_NODELEVEL;
                        model.b_CostCenter = string.IsNullOrEmpty(item.B_COSTCENTER) ? "" : item.B_COSTCENTER;
                        model.b_CompanyCode = string.IsNullOrEmpty(item.B_COMPANYCODE) ? "" : item.B_COMPANYCODE;
                        model.Number = i + 1;
                        if (!string.IsNullOrEmpty(model.b_ParentNodeCode))
                        {
                            var parentNode = list.Where(x => x.b_NodeCode == model.b_ParentNodeCode).FirstOrDefault();
                            model.ParentNumber = parentNode.Number;
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

        /// <summary>
        /// 根据Id 获取结构信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetOrganizationalStructureById(string id)
        {
            var retModel = new JsonReturnModel();
            try
            {
                OrganizationalStructureModel model = new OrganizationalStructureModel();
                B_ORGANIZATIONALSTRUCTURE obj = OrganizationalStructureBll.GetOrganizationalStructureById(id);
                if (obj != null)
                {
                    model.id = obj.id;
                    model.b_NodeName = obj.B_NODENAME;
                    model.b_NodeCode = obj.B_NODECODE;
                    model.b_ParentNodeCode = obj.B_PARENTNODECODE;
                    model.b_NodePersonName = obj.B_NODEPERSONNAME;
                    model.b_NodeLevel = obj.B_NODELEVEL;
                    model.b_CostCenter = obj.B_COSTCENTER;
                    model.b_CompanyCode = obj.B_COMPANYCODE;
                }
                retModel.data = model;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadOrganizationalStructureFile()
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
                List<B_ORGANIZATIONALSTRUCTURE> list = new List<B_ORGANIZATIONALSTRUCTURE>();
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
                            B_ORGANIZATIONALSTRUCTURE entity = new B_ORGANIZATIONALSTRUCTURE();
                            if (row.GetCell(0) != null)
                            {
                                entity.B_NODENAME = row.GetCell(0) != null ? row.GetCell(0).ToString().Trim() : "";
                                entity.B_NODECODE = row.GetCell(1) != null ? row.GetCell(1).ToString().Trim() : "";
                                entity.B_PARENTNODECODE = row.GetCell(2) != null ? row.GetCell(2).ToString().Trim() : "";
                                entity.B_NODEPERSONNAME = row.GetCell(3) != null ? row.GetCell(3).ToString().Trim() : "";
                                string nodeLevelStr = row.GetCell(4) != null ? row.GetCell(4).ToString().Trim() : "";
                                entity.B_COMPANYCODE = row.GetCell(5) != null ? row.GetCell(5).ToString().Trim() : "";
                                entity.B_COSTCENTER = row.GetCell(6) != null ? row.GetCell(6).ToString().Trim() : "";

                                int nodeLevel;
                                if (!int.TryParse(nodeLevelStr, out nodeLevel))
                                {
                                    retModel.AddError("errorMessage", i + 1 + "行5列上传的数据类型不正确！");
                                    return Json(retModel, JsonRequestBehavior.AllowGet);
                                }
                                entity.B_NODELEVEL = nodeLevel;

                                //判断节点负责人在系统中是否存在
                                if(!string.IsNullOrEmpty(entity.B_NODEPERSONNAME))
                                {
                                    if (!UserDA.ValidLoginNameIsExist(inn, entity.B_NODEPERSONNAME))
                                    {
                                        retModel.AddError("errorMessage", i + 1 + "行4列上传的负责人不正确！");
                                        return Json(retModel, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                list.Add(entity);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                //string strSQL = "";
                //将数据插入数据库
                if (list != null && list.Count > 0)
                {
                    var result = OrganizationalStructureBll.DeleteOrganizationalStructure(inn);
                    if (!result.isError())
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            var OrganizationalStructure = inn.newItem("b_OrganizationalStructure", "add");
                            OrganizationalStructure.setProperty("b_nodename", item.B_NODENAME);
                            OrganizationalStructure.setProperty("b_nodecode", item.B_NODECODE);
                            OrganizationalStructure.setProperty("b_parentnodecode", item.B_PARENTNODECODE);
                            OrganizationalStructure.setProperty("b_nodepersonname", item.B_NODEPERSONNAME);
                            OrganizationalStructure.setProperty("b_nodelevel", item.B_NODELEVEL.ToString());
                            OrganizationalStructure.setProperty("b_costcenter", item.B_COSTCENTER);
                            OrganizationalStructure.setProperty("b_companycode", item.B_COMPANYCODE);
                            OrganizationalStructure.apply();
                        }
                    }
                    else
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
        /// 保存部门信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult SaveOrganizationalStructure(OrganizationalStructureModel model)
        {
            var retModel = new JsonReturnModel();
            try
            {
                var item = inn.newItem("b_OrganizationalStructure", "edit");
                item.setAttribute("id", model.id);
                item.setProperty("b_costcenter", model.b_CostCenter);
                item.setProperty("b_companycode", model.b_CompanyCode);
                var resut = item.apply();
                if (resut.isError())
                {
                    retModel.AddError("errorMessage", resut.getErrorString());
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