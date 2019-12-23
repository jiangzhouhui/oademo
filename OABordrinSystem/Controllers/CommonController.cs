using OABordrinBll;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers
{
    public class CommonController : BaseController
    {
        /// <summary>
        /// 根据名称获取用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult GetUserByName(string name)
        {
            var retModel = new JsonReturnModel();
            try
            {
                using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
                {
                    USER item = db.USER.Where(x => x.FIRST_NAME == name).FirstOrDefault();

                    if (item != null)
                    {
                        UserModel model = new UserModel();
                        model.Id = item.ID;
                        model.first_Name = item.FIRST_NAME;
                        model.Email = item.EMAIL;
                        model.Telephone = item.TELEPHONE;
                        model.b_JobNumber = item.B_JOBNUMBER;
                        model.b_ChineseName = item.B_CHINESENAME;
                        model.b_EnglishName = item.B_ENGLISHNAME;
                        model.b_Centre = item.B_CENTRE;
                        model.b_Department = item.B_DEPARTMENT;
                        model.b_SeniorManager = item.B_SENIORMANAGER;
                        model.b_Director = item.B_DIRECTOR;
                        model.b_VP = item.B_VP;
                        if (!string.IsNullOrEmpty(model.b_Centre))
                        {
                            //根据中心名称获取中心领导
                            var structure = OrganizationalStructureDA.GetOrganizationalStructureByParam(inn, model.b_Centre, 2);
                            if (!structure.isError() && structure.getItemCount() > 0)
                            {
                                string b_nodepersonname = structure.getItemByIndex(0).getProperty("b_nodepersonname");
                                string b_NodeCode = structure.getItemByIndex(0).getProperty("b_nodecode");
                                string b_ParentNodeCode = structure.getItemByIndex(0).getProperty("b_parentnodecode");
                                string b_CompanyCode = structure.getItemByIndex(0).getProperty("b_companycode");
                                string b_CostCenter = structure.getItemByIndex(0).getProperty("b_costcenter");

                                USER centreUser = UserDA.GetUserByLoginName(b_nodepersonname);
                                if (centreUser != null && centreUser.FIRST_NAME != name)
                                {
                                    model.b_CentreLeader = centreUser.FIRST_NAME;
                                }
                                else if (centreUser != null && centreUser.FIRST_NAME == name)
                                {
                                    //获取上级节点
                                    var parentNode = OrganizationalStructureDA.GetOrganizationalStructureByNodeCode(inn, b_ParentNodeCode);
                                    if (!parentNode.isError() && parentNode.getItemCount() > 0)
                                    {
                                        string lineLeader = parentNode.getItemByIndex(0).getProperty("b_nodepersonname");
                                        USER lineLeaderUser = UserDA.GetUserByLoginName(lineLeader);
                                        if (lineLeaderUser != null && lineLeaderUser.FIRST_NAME != name)
                                        {
                                            model.b_LineLeader = lineLeaderUser.FIRST_NAME;
                                        }
                                    }
                                }


                                if (!string.IsNullOrEmpty(model.b_Department))
                                {
                                    //获取结构数据
                                    List<B_ORGANIZATIONALSTRUCTURE> list = OrganizationalStructureBll.GetOrganizationalStructureList();
                                    //获取中心下的组织结构
                                    List<B_ORGANIZATIONALSTRUCTURE> childNodeList = new List<B_ORGANIZATIONALSTRUCTURE>();
                                    OrganizationalStructureBll.GetChildByParent(inn, b_NodeCode, childNodeList, list);
                                    if (childNodeList != null && childNodeList.Count > 0)
                                    {
                                        B_ORGANIZATIONALSTRUCTURE departObj = childNodeList.Where(x => x.B_NODENAME == model.b_Department).FirstOrDefault();
                                        if (departObj != null)
                                        {
                                            b_CompanyCode = departObj.B_COMPANYCODE;
                                            b_CostCenter = departObj.B_COSTCENTER;
                                            USER departUser = UserDA.GetUserByLoginName(departObj.B_NODEPERSONNAME);
                                            if (departUser != null && departUser.FIRST_NAME != name)
                                            {
                                                model.b_DepartmentLeader = departUser.FIRST_NAME;
                                            }
                                        }
                                    }
                                }
                                model.b_CompanyCode = b_CompanyCode;
                                model.b_CostCenter = b_CostCenter;

                                if (!string.IsNullOrEmpty(model.b_CompanyCode))
                                {
                                    //根据公司代码  获取公司信息
                                    string companyName = CompanyInfoBll.GetCompanyNameByCode(inn, model.b_CompanyCode);
                                    if(!string.IsNullOrEmpty(companyName))
                                    {
                                        model.b_CompanyCode= model.b_CompanyCode + " (" + companyName + ")";
                                    }
                                }
                            }
                        }
                        retModel.data = model;
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
        /// 中文翻译成英文
        /// </summary>
        /// <param name="chineseValue"></param>
        /// <param name="languageCategory"></param>
        /// <param name="fileName"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public JsonResult GetLanguageValueByParam(string chineseValue, string languageCategory, string fileName)
        {
            var retModel = new JsonReturnModel();
            try
            {
                string value = Common.GetLanguageValueByParam(chineseValue, languageCategory, fileName, Userinfo.language);
                retModel.data = value;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 根据版本编号 获取流程版本信息
        /// </summary>
        /// <param name="versionNo"></param>
        /// <returns></returns>
        public JsonResult GetWorkFlowVersionByVersionNo(string versionNo)
        {
            var retModel = new JsonReturnModel();
            try
            {
                B_WORKFLOWVERSIONMANAGEMENT workFlowVersion =WorkFlowVersionManagementDA.GetWorkFlowVersionByVersionNo(versionNo);
                retModel.data = workFlowVersion;
            }
            catch(Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }





    }
}