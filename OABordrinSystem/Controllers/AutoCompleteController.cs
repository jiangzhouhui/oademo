using OABordrinCommon;
using OABordrinDAL;
using OABordrinSystem.Models;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers
{
    public class AutoCompleteController : BaseController
    {
        public JsonResult GetDelegateToNames()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> delegateToNames = new List<string>();
                //获取Identitys
                string amlStr = "<AML><Item type='user' action='get'><logon_enabled>1</logon_enabled></Item></AML>";
                var result = inn.applyAML(amlStr);
                var count = result.getItemCount();
                for (int i = 0; i < count; i++)
                {
                    var identityObj = result.getItemByIndex(i);
                    string keyed_name = identityObj.getProperty("keyed_name");
                    delegateToNames.Add(keyed_name);
                }
                retModel.data = delegateToNames;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 采取采购人集合
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBuyerList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                var identityItem = inn.newItem("IDENTITY", "get");
                identityItem.setProperty("keyed_name", "采购员");
                var memberItems = inn.newItem("MEMBER", "get");
                identityItem.addRelationship(memberItems);
                var result = identityItem.apply();
                if (!result.isError())
                {
                    var items = result.getRelationships();
                    for (int i = 0; i < items.getItemCount(); i++)
                    {
                        var item = items.getItemByIndex(i).getRelatedItem();
                        string value = item.getProperty("keyed_name");
                        list.Add(value);
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
        /// 获取项目列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProjectList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = "";
                string applicableCompanyStr = "";
                List<string> arrList = null;
                if (!string.IsNullOrEmpty(Userinfo.b_AffiliatedCompany))
                {
                    arrList = Userinfo.b_AffiliatedCompany.Split(';').Where(x => x != "").ToList();
                    if (arrList != null && arrList.Count > 0)
                    {
                        applicableCompanyStr = " and b_ApplicableCompany in (";
                        for (var i = 0; i < arrList.Count; i++)
                        {
                            string value = arrList[i];
                            if (i == arrList.Count - 1)
                            {
                                applicableCompanyStr = applicableCompanyStr + "N'" + value + "')";
                            }
                            else
                            {
                                applicableCompanyStr = applicableCompanyStr + "N'" + value + "',";
                            }
                        }
                    }

                    strSql = "select * from innovator.B_PROJECTMANAGE where b_IsInUse='1' " + applicableCompanyStr + " order by B_SORT desc";
                    var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                    if (!result.isError() && result.getItemCount() > 0)
                    {
                        for (var i = 0; i < result.getItemCount(); i++)
                        {
                            var item = result.getItemByIndex(i);
                            string value = item.getProperty("b_projectname");
                            list.Add(value);
                        }
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
        /// 获取地区列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRegionList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = "select * from innovator.b_region";
                var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (var i = 0; i < result.getItemCount(); i++)
                    {
                        var item = result.getItemByIndex(i);
                        string value = item.getProperty("b_regionname");
                        list.Add(value);
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
        /// 获取费用类别
        /// </summary>
        /// <returns></returns>
        public JsonResult GetExpenseCategoryList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = "select * from innovator.b_ExpenseCategory";
                var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (var i = 0; i < result.getItemCount(); i++)
                    {
                        var item = result.getItemByIndex(i);
                        string value = item.getProperty("b_costname");
                        list.Add(value);
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
        /// 交通类别
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTrafficCategoryList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = "select * from innovator.b_TrafficCategory";
                var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (var i = 0; i < result.getItemCount(); i++)
                    {
                        var item = result.getItemByIndex(i);
                        string value = item.getProperty("b_trafficname");
                        list.Add(value);
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
        /// 获取货币种类
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCurrencyTypeList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = "select * from innovator.b_CurrencyType order by [CREATED_ON]";
                var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (var i = 0; i < result.getItemCount(); i++)
                    {
                        var item = result.getItemByIndex(i);
                        string value = item.getProperty("b_currencyname");
                        list.Add(value);
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
        /// 获取PR单编号集合
        /// </summary>
        /// <returns></returns>
        public JsonResult GetContractNoList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = @"select y.* from B_PRMANAGE g inner join b_ChoiceSuppliers t on t.SOURCE_ID=g.id inner join B_PrChoiceSuppliers y on t.RELATED_ID=y.id 
                              inner join WORKFLOW u on u.SOURCE_ID=g.id inner join WORKFLOW_PROCESS i on u.RELATED_ID=i.id where i.STATE='Closed'";
                var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (var i = 0; i < result.getItemCount(); i++)
                    {
                        var item = result.getItemByIndex(i);
                        string value = item.getProperty("b_pono");
                        list.Add(value);
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
        /// 获取组织结构公司代码
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStructureCompanyCode()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                string strSql = "select b_CompanyName,b_CompanyCode from innovator.b_CompanyInfo";
                var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
                if (!result.isError() && result.getItemCount() > 0)
                {
                    for (var i = 0; i < result.getItemCount(); i++)
                    {
                        var item = result.getItemByIndex(i);
                        string b_CompanyName = item.getProperty("b_companyname");
                        string b_CompanyCode = item.getProperty("b_companycode");
                        string value = b_CompanyCode + " (" + b_CompanyName + ")";
                        list.Add(value);
                    }
                }
                list = list.Distinct().ToList();
                retModel.data = list;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取身份列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
                {
                    var datas = (from g in db.USER
                                 select g);
                    list = datas.Select(x => x.KEYED_NAME).ToList();
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
        /// 获取成本中心列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCostCenterList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<string> list = new List<string>();
                using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
                {
                    var datas = (from g in db.B_ORGANIZATIONALSTRUCTURE select g);
                    list = datas.Select(x => x.B_COSTCENTER).Distinct().ToList();
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
        /// 获取税码列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTaxCodeList()
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<SelectListItem> list = new List<SelectListItem>();
                using (InnovatorSolutionsEntities db = new InnovatorSolutionsEntities())
                {
                    var datas = (from g in db.B_TAXCODECONFIGURE select g);
                    datas = Common.OrderBy(datas, "CREATED_ON", true);
                    if (datas != null && datas.Count() > 0)
                    {
                        foreach (var item in datas)
                        {
                            SelectListItem itemObj = new SelectListItem();
                            itemObj.Text = item.B_TAXCODE;
                            TaxCodeTypeList? codeType = EnumDescription.GetEnumByText<TaxCodeTypeList>(item.B_TAXCODE);
                            itemObj.Value = ((int)codeType * 0.01).ToString();
                            list.Add(itemObj);
                        }
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
        /// 获取出差单
        /// </summary>
        /// <param name="b_Employee"></param>
        /// <returns></returns>
        public JsonResult GetTravelRecordNo(string b_Employee = "")
        {
            var retModel = new JsonReturnModel();
            try
            {
                List<BusinessTravelModel> returnList = new List<BusinessTravelModel>();
                List<B_BUSINESSTRAVEL> list = BusinessTravelDA.GetBusinessTravelByEmployee(b_Employee);
                foreach (var item in list)
                {
                    BusinessTravelModel model = new BusinessTravelModel();
                    model.b_DocumentNo = item.B_DOCUMENTNO;
                    model.b_ApplicationDate = item.B_APPLICATIONDATE.GetValueOrDefault().ToString("yyyy-MM-dd");
                    model.b_Type = item.B_TYPE;
                    model.b_ProjectName = string.IsNullOrEmpty(item.B_PROJECTNAME) ? "" : item.B_PROJECTNAME;
                    model.b_Destination = item.B_DESTINATION;
                    returnList.Add(model);
                }
                retModel.data = returnList;
            }
            catch (Exception ex)
            {
                retModel.AddError("errorMessage", ex.Message);
            }
            return Json(retModel, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据中文字符获取对应的英文字符
        /// </summary>
        /// <param name="chineseValue"></param>
        /// <param name="languageCategory"></param>
        /// <returns></returns>
        public string GetLanguageValueByParam(string chineseValue, string languageCategory, string fileName)
        {
            string languageType = Userinfo.language;
            //string languageType = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            if (languageType.IndexOf("zh") > 0)
            {
                return chineseValue;
            }
            else
            {
                string value = MemoryCacheUtils.Get(chineseValue) as string;
                if (string.IsNullOrEmpty(value))
                {
                    //获取语言数据
                    string filePath = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\" + fileName + ".json";
                    String jsonStr = Common.GetFileJson(filePath);
                    value = Common.GetKeyValue(jsonStr, languageCategory, chineseValue);
                }
                return value;
            }
        }










    }
}