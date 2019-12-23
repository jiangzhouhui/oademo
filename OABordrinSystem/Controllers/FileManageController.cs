using Aras.IOM;
using OABordrinCommon;
using OABordrinSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OABordrinSystem.Controllers
{
    public class FileManageController : BaseController
    {
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UploadFile(string id, string status, string relationTableName)
        {
            var retModel = new JsonReturnModel();
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    status = "Start";
                }

                FileModel file = new FileModel();
                if (Request.Files == null || Request.Files.Count == 0)
                {
                    retModel.AddError("errorMessage", Common.GetLanguageValueByParam("请选择您要上传的附件！", "PRCommon", "PRItemType", Userinfo.language));
                    return Json(retModel, JsonRequestBehavior.AllowGet);
                }

                string fileName;
                HttpPostedFileBase prfile = Request.Files[0];
                fileName = prfile.FileName.Substring(prfile.FileName.LastIndexOf("\\") + 1, prfile.FileName.Length - (prfile.FileName.LastIndexOf("\\")) - 1);
                string filePath = ConfigurationManager.AppSettings["UploadPath"] + fileName;
                prfile.SaveAs(filePath);
                Item fileItem = inn.newItem("File", "add");
                fileItem.attachPhysicalFile(filePath);
                fileItem.setProperty("filename", fileName);
                fileItem.setProperty("comments", status);

                if (!string.IsNullOrEmpty(id))
                {
                    var relationFile = inn.newItem(relationTableName, "add");
                    relationFile.setProperty("source_id", id);
                    relationFile.setRelatedItem(fileItem);
                    relationFile = relationFile.apply();
                    if (!relationFile.isError())
                    {
                        Item item = relationFile.getRelatedItem();
                        file.id = item.getProperty("id");
                        file.relationId = relationFile.getProperty("id");
                        file.fileName = fileItem.getProperty("filename");
                        file.mimeType = item.getProperty("mimetype");
                        retModel.data = file;
                    }
                    else
                    {
                        retModel.AddError("errorMessage", relationFile.getErrorString());
                    }

                }
                else
                {
                    fileItem = fileItem.apply();
                    if (!fileItem.isError())
                    {
                        file.id = fileItem.getProperty("id");
                        file.fileName = fileItem.getProperty("filename");
                        file.mimeType = fileItem.getProperty("mimetype");
                        retModel.data = file;
                    }
                    else
                    {
                        retModel.AddError("errorMessage", fileItem.getErrorString());
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
        /// 删除文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteFile(string id, string relationId, string relationTableName)
        {
            var retModel = new JsonReturnModel();
            try
            {
                Item fileItem = inn.newItem("File", "delete");
                fileItem.setAttribute("id", id);
                if (!string.IsNullOrEmpty(relationId) && relationId != "null")
                {
                    Item relationFileItem = inn.newItem(relationTableName, "delete");
                    relationFileItem.setAttribute("id", relationId);
                    relationFileItem.setRelatedItem(fileItem);
                    relationFileItem.apply();
                    if (relationFileItem.isError())
                    {
                        retModel.AddError("errorMessage", relationFileItem.getErrorString());
                    }
                }
                else
                {
                    fileItem = fileItem.apply();
                    if (fileItem.isError())
                    {
                        retModel.AddError("errorMessage", fileItem.getErrorString());
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
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        public FilePathResult DownloadAttachment(string id)
        {
            Item item = inn.newItem("File", "get");
            item.setAttribute("id", id);
            Item file = item.apply();
            string checkout_path = "";
            string downloadPath = ConfigurationManager.AppSettings["DownloadPath"];
            if (!file.isError())
            {
                Item result = file.checkout(downloadPath);
                checkout_path = result.getProperty("checkedout_path");
            }
            if (!string.IsNullOrEmpty(checkout_path))
            {
                string contentType = MimeMapping.GetMimeMapping(checkout_path);
                var fileOBJ = new FileInfo(checkout_path);
                if (fileOBJ.Exists)
                {
                    Response.AddHeader("Content-Disposition",
                                  "attachment; filename=" + HttpUtility.UrlEncode(fileOBJ.Name));
                    return File(fileOBJ.FullName, contentType);
                }
            }
            return null;
        }





    }
}