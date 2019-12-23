using Aras.IOM;
using OABordrinCommon;
using OABordrinDAL;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinBll
{
    public class PrManageBll
    {
        /// <summary>
        /// 添加管理权限
        /// </summary>
        /// <param name="ItemId"></param>
        /// <param name="identityId"></param>
        public static string AddPrManageAuthById(Innovator inn, string ItemId, string identityId, List<string> names)
        {
            string strError = "";
            //根据任务名称获取任务
            Item activitys = ActivityDA.GetActivityByNames(inn, names, ItemId, "innovator.B_PRMANAGE");
            //获取当前任务已经存在的处理人
            if (!activitys.isError())
            {
                //获取任务Id
                string strAml = "<AML>";
                for (var i = 0; i < activitys.getItemCount(); i++)
                {
                    var item = activitys.getItemByIndex(i);
                    string source_id = item.getProperty("id");
                    Item activityAssignments = ActivityAssignmentDA.GetActivityAssignment(inn, source_id);
                    string ids = "";
                    if (!activityAssignments.isError() && activityAssignments.getItemCount() > 0)
                    {
                        for (int index = 0; index < activityAssignments.getItemCount(); index++)
                        {
                            var activityAssignment = activityAssignments.getItemByIndex(index);
                            string id = activityAssignment.getProperty("id");

                            if (index != activityAssignments.getItemCount() - 1)
                            {
                                ids += id + ",";
                            }
                            else
                            {
                                ids += id;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(ids))
                    {
                        ActivityAssignmentDA.deleteActivityAssignment(inn, ids);
                    }

                    strAml += "<Item type = 'ACTIVITY ASSIGNMENT' action = 'add'>";
                    strAml += "<source_id>" + source_id + "</source_id><related_id>";
                    strAml += "<Item type='IDENTITY' action='get' id='" + identityId + "'>";
                    strAml += "</Item></related_id></Item>";
                }
                strAml += "</AML>";
                var result = inn.applyAML(strAml);
                if (result.isError())
                {
                    strError = result.getErrorString();
                }
            }
            else
            {
                strError = activitys.getErrorString();
                return strError;
            }
            return strError;
        }


        /// <summary>
        /// 删除PR单对象
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="ItemId"></param>
        /// <param name="identityId"></param>
        /// <param name="names"></param>
        public static void DeletePrManage(Innovator inn, string ItemId, List<string> names)
        {
            Item activitys = ActivityDA.GetActivityByNames(inn, names, ItemId, "innovator.B_PRMANAGE");
            if (!activitys.isError())
            {
                string source_id = activitys.getItemByIndex(0).getProperty("id");
                Item activityAssignments = ActivityAssignmentDA.GetActivityAssignment(inn, source_id);
                string ids = "";
                if (!activityAssignments.isError() && activityAssignments.getItemCount() > 0)
                {
                    for (int index = 0; index < activityAssignments.getItemCount(); index++)
                    {
                        var activityAssignment = activityAssignments.getItemByIndex(index);
                        string id = activityAssignment.getProperty("id");

                        if (index != activityAssignments.getItemCount() - 1)
                        {
                            ids += id + ",";
                        }
                        else
                        {
                            ids += id;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ids))
                {
                    ActivityAssignmentDA.deleteActivityAssignment(inn, ids);
                }

            }
        }



        /// <summary>
        /// 单据状态
        /// </summary>
        /// <param name="status"></param>
        public static void SendEmailByOperation(Innovator inn, string b_prRecordNo, string b_Applicant, string b_Buyer, WORKFLOW_PROCESS_PATH choicePath, string itemId = "")
        {
            //邮箱列表
            List<string> listEmail = new List<string>();
            List<string> names = new List<string>();

            string nameStr = "";

            string subject = "";
            string body = "";

            Item activity = ActivityDA.GetActivityById(inn, choicePath.RELATED_ID);
            string keyedName = activity.getProperty("keyed_name").Trim();

            //获取邮件需要发送的人员信息
            Item identitys = IdentityDA.GetIdentityByActivityId(inn, choicePath.RELATED_ID);
            //获取邮箱信息
            UserDA.GetEmailByIdentitys(inn, identitys, listEmail, names);

            if(choicePath.NAME.Contains("Return"))
            {
                SendReturnFinancialAnalystMail(inn, keyedName, choicePath, listEmail, itemId);
            }

            if (names != null && names.Count > 0)
            {
                for (int i = 0; i < names.Count; i++)
                {
                    if (i != names.Count - 1)
                    {
                        nameStr += names[i] + "、";
                    }
                    else
                    {
                        nameStr += names[i];
                    }
                }
            }

            if (choicePath.NAME.Contains("Return"))
            {
                subject = "Your purchase request [" + b_prRecordNo + "] has been rejected.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += "Your purchase request [" + b_prRecordNo + "] has been rejected.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/PrManage/Index";
            }
            else if (keyedName == "End")
            {
                //获取申请人邮箱
                Item applicantIdentity = IdentityDA.GetIdentityByKeyedName(inn, b_Applicant);
                if (!applicantIdentity.isError())
                {
                    UserDA.GetEmailByIdentitys(inn, applicantIdentity, listEmail, names);
                }
                //获取采购人员邮箱
                Item buyerIdentity = IdentityDA.GetIdentityByKeyedName(inn, b_Buyer);
                if (!buyerIdentity.isError())
                {
                    UserDA.GetEmailByIdentitys(inn, buyerIdentity, listEmail, names);
                }
                if (names != null && names.Count > 0)
                {
                    for (int i = 0; i < names.Count; i++)
                    {
                        if (i != names.Count - 1)
                        {
                            nameStr += names[i] + "、";
                        }
                        else
                        {
                            nameStr += names[i];
                        }
                    }
                }
                subject = "Your purchase request [" + b_prRecordNo + "] has been approved.";
                body = "Dear " + nameStr + ",<br/><br/>";
                body += " Your purchase request [" + b_prRecordNo + "] has been approved.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/PrManage/Index";
            }
            else
            {
                subject = b_Applicant + " has submitted a purchase request [" + b_prRecordNo + "] for your approval.";
                body += "Dear " + nameStr + ",<br/><br/>";
                body += b_Applicant + " has submitted a purchase request [" + b_prRecordNo + "]" + " for your approval.<br/>";
                body += "Doc link: " + ConfigurationManager.AppSettings["OASite"] + "/PrManage/Index";
            }

            listEmail = new List<string>();
            listEmail.Add("zhouhui.jiang@bordrin.com");
            listEmail.Add("ping.chen@bordrin.com");
            //listEmail.Add("xiangxiang.zhou@bordrin.com");
            //异步执行
            new Task(() =>
            {
                MailOperator.SendMail(listEmail, subject, body);
            }).Start();
        }

        /// <summary>
        /// Pr单驳回通知预算分析员
        /// </summary>
        public static void SendReturnFinancialAnalystMail(Innovator inn, string keyedName, WORKFLOW_PROCESS_PATH choicePath, List<string> listEmail, string itemId)
        {
            Item activity = ActivityDA.GetActivityById(inn, choicePath.SOURCE_ID);
            string currentKeyedName = activity.getProperty("keyed_name").Trim();
            if (itemId == "")
            {
                return;
            }

            if (currentKeyedName == "Financial Manager" || currentKeyedName == "Financial Director" || currentKeyedName == "CFO" ||
               currentKeyedName == "Receive PR" || currentKeyedName == "Buyer Inquiry" || currentKeyedName == "Contract Registration" ||
               currentKeyedName == "Contract Management" || currentKeyedName == "Purchase Manager" || currentKeyedName == "Purchase Director")
            {
                if (keyedName == "Start")
                {
                    List<string> financialAnalyst = new List<string> { "Financial Analyst" };
                    Item activityObj = ActivityDA.GetActivityByNames(inn, financialAnalyst, itemId, "B_PrManage", "Closed");
                    if(!activityObj.isError() && activityObj.getItemCount()>0)
                    {
                        string activityId = activityObj.getItemByIndex(0).getProperty("id");

                        //获取邮件需要发送的人员信息
                        Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId);
                        List<string> names = new List<string>();
                        //获取邮箱信息
                        UserDA.GetEmailByIdentitys(inn, identitys, listEmail, names);

                    }

                }
            }
        }




        /// <summary>
        /// 根据ID获取PR单
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Item GetPrManageObjById(Innovator inn, string id)
        {
            //先获取数据
            var prManage = inn.newItem("B_PRMANAGE", "get");
            prManage.setAttribute("id", id);
            //获取需求信息
            var GetrequestInfo = inn.newItem("B_REQUESTINFO", "get");
            GetrequestInfo.setAttribute("where", "source_id='" + id + "'");
            GetrequestInfo.setAttribute("select", "related_id");
            prManage.addRelationship(GetrequestInfo);

            //获取询价信息
            var quotationItem = inn.newItem("B_QUOTATIONITEM", "get");
            quotationItem.setAttribute("where", "source_id='" + id + "'");
            quotationItem.setAttribute("select", "related_id");
            prManage.addRelationship(quotationItem);

            //获取重复采购信息
            var repeatedpurchasing = inn.newItem("B_REPEATEDPURCHASING", "get");
            repeatedpurchasing.setAttribute("where", "source_id='" + id + "'");
            repeatedpurchasing.setAttribute("select", "related_id");
            prManage.addRelationship(repeatedpurchasing);

            //获取选择的供应商
            var b_ChoiceSuppliers = inn.newItem("b_ChoiceSuppliers", "get");
            b_ChoiceSuppliers.setAttribute("where", "source_id='" + id + "'");
            b_ChoiceSuppliers.setAttribute("select", "related_id");
            prManage.addRelationship(b_ChoiceSuppliers);


            //获取附件信息
            var prManageFiles = inn.newItem("b_PrManageFiles", "get");
            prManageFiles.setAttribute("where", "source_id='" + id + "'");
            prManageFiles.setAttribute("select", "related_id");
            prManage.addRelationship(prManageFiles);

            var result = prManage.apply();
            return result;

        }

        /// <summary>
        /// 获取挑选的供应商
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static decimal GetPrContractPrice(Innovator inn, string id)
        {
            decimal contractPrice = 0;
            string strSql = "select sum(t.B_CONTRACTPRICE) as CONTRACTPRICE from b_ChoiceSuppliers g inner join B_PrChoiceSuppliers t on g.RELATED_ID=t.id where g.SOURCE_ID='" + id + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            if (!result.isError())
            {
                contractPrice = decimal.Parse(result.getProperty("contractprice"));
            }
            return contractPrice;
        }

        /// <summary>
        /// 创建合同编号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="b_Buyer"></param>
        /// <returns></returns>
        public static string CreatePoNo(Innovator inn, string b_Buyer)
        {
            string poNo = "";
            string year = DateTime.Now.Year.ToString();
            year = year.Substring(2, year.Length - 2);
            poNo = year + "NPPO";
            b_Buyer = Common.GetKuohaoValue(b_Buyer);
            if (!string.IsNullOrEmpty(b_Buyer))
            {
                string firstPinYin = Common.GetPinYinFirstTitle(b_Buyer);
                poNo += "-" + firstPinYin + "-";
            }
            else
            {
                return "";
            }
            return poNo;
        }


        /// <summary>
        /// 根据Id获取Pr编号
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetPrRecordNoById(Innovator inn, string id)
        {
            var item = inn.newItem("B_PRMANAGE", "get");
            item.setAttribute("id", id);
            item.setAttribute("select", "b_prrecordno");
            var result = item.apply();
            if (!result.isError())
            {
                return result.getProperty("b_prrecordno");
            }
            return "";
        }

        /// <summary>
        /// 自动完成任务
        /// </summary>
        public static void AutomaticCompletionTask(Innovator inn, string id, UserInfo userInfo, ref WORKFLOW_PROCESS_PATH ChoicePath)
        {
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.B_PRMANAGE");
            if (!activityItem.isError() && activityItem.getItemCount() > 0)
            {
                string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
                string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");
                //获取当前任务的操作权限
                bool isOperate = false;
                Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId);
                if (!identitys.isError() && identitys.getItemCount() > 0)
                {
                    for (int i = 0; i < identitys.getItemCount(); i++)
                    {
                        string dentityId = identitys.getItemByIndex(i).getProperty("id");
                        if (userInfo.Roles.Contains(dentityId))
                        {
                            isOperate = true;
                        }
                    }
                }
                if ((keyedName == "Dept.Director" || keyedName == "Dept.VP" || keyedName == "GM" || keyedName == "PMT/PAT Leader" || keyedName == "Project Manager" || keyedName == "Project Director") && isOperate)
                {
                    WORKFLOW_PROCESS_PATH newChoicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.B_PRMANAGE");
                    if (newChoicePath != null)
                    {
                        ChoicePath = newChoicePath;
                        AutomaticCompletionTask(inn, id, userInfo, ref ChoicePath);
                    }
                }
            }
        }


        /// <summary>
        /// 自动完成任务
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="ChoicePath"></param>
        public void AutomaticCompletionTask(Innovator inn, string id, string b_PrType, string versionNo, ref WORKFLOW_PROCESS_PATH ChoicePath)
        {
            //获取当前活动
            Item activityItem = ActivityDA.GetActivityByItemId(inn, id, "innovator.B_PRMANAGE");
            if (!activityItem.isError() && activityItem.getItemCount() > 0)
            {
                string activityId = activityItem.getItemByIndex(0).getProperty("activityid");
                string keyedName = activityItem.getItemByIndex(0).getProperty("keyed_name");
                //获取当前任务的操作权限
                bool isOperate = false;
                Item identitys = IdentityDA.GetIdentityByActivityId(inn, activityId);
                if (!identitys.isError() && identitys.getItemCount() == 1)
                {
                    isOperate = true;
                }
                if (isOperate)
                {
                    string lineName = GetLineNameByActivityName(inn, id, activityId, keyedName, b_PrType, versionNo);
                    WORKFLOW_PROCESS_PATH newChoicePath = WorkFlowBll.AutoCompleteActivityByParam(id, "innovator.B_PRMANAGE", lineName);
                    if (newChoicePath != null)
                    {
                        ChoicePath = newChoicePath;
                        AutomaticCompletionTask(inn, id, b_PrType, versionNo, ref ChoicePath);
                    }
                }
            }
        }

        public static bool CheckedContractNoIsExist(Innovator inn, string contractNo, string id)
        {

            string strSql = "select * from B_PrChoiceSuppliers where b_PoNo='" + contractNo + "' and id !='" + id + "'";
            var result = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");

            if (!result.isError() && result.getItemCount() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 获取老的备注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetOldRemarkById(Innovator inn, string id)
        {
            string strSql = "select b_Remark from innovator.B_PrManage where id='" + id + "'";

            var returnItem = inn.applyMethod("bcs_RunQuery", "<sqlCommend>" + strSql + "</sqlCommend>");
            if (!returnItem.isError() && returnItem.getItemCount() > 0)
            {
                return returnItem.getProperty("b_remark");
            }
            return "";
        }


        /// <summary>
        /// 添加备注
        /// </summary>
        /// <returns></returns>
        public static string AddRemark(Innovator inn, string b_Remark, string id, string UserName)
        {
            if (!string.IsNullOrEmpty(b_Remark))
            {
                string oldRemark = PrManageBll.GetOldRemarkById(inn, id);
                if (!string.IsNullOrEmpty(oldRemark))
                {
                    b_Remark = oldRemark + "<br/>" + UserName + ":" + b_Remark;
                }
                else
                {
                    b_Remark = UserName + ":" + b_Remark;
                }
            }
            return b_Remark;
        }


        /// <summary>
        /// 获取PR下一步审核路线
        /// </summary>
        /// <param name="inn"></param>
        /// <param name="id"></param>
        /// <param name="activityId"></param>
        /// <param name="ActivityName"></param>
        /// <param name="b_PrType"></param>
        /// <param name="versionNo"></param>
        /// <returns></returns>
        public static string GetLineNameByActivityName(Innovator inn, string id, string activityId, string ActivityName, string b_PrType, string versionNo)
        {
            string lineName = "agree";
            if (versionNo == "PR_001")
            {
                if (ActivityName == "Start")
                {
                    lineName = "submit";
                }

                if (ActivityName == "Dept.Director")
                {
                    lineName = b_PrType == "project" ? "ProjectPR" : "Dept.PR";
                }

                if (ActivityName == "Purchase Manager")
                {
                    decimal contractPrice = PrManageBll.GetPrContractPrice(inn, id);

                    if (contractPrice >= 250000)
                    {
                        lineName = "(>=250K)";
                    }
                    else
                    {
                        lineName = "(<250K)";
                    }
                }
            }

            if (versionNo == "PR_002")
            {
                if (ActivityName == "Start")
                {
                    lineName = b_PrType == "project" ? "ProjectPR" : "Dept.PR";
                }

                if (ActivityName == "Purchase Manager")
                {
                    decimal contractPrice = PrManageBll.GetPrContractPrice(inn, id);

                    if (contractPrice >= 250000)
                    {
                        lineName = "(>=250K)";
                    }
                    else
                    {
                        lineName = "(<250K)";
                    }
                }
            }
            return lineName;
        }
    }
}
