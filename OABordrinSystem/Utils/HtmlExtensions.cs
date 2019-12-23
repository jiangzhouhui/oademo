
using System.IO;
namespace System.Web.Mvc
{
    public static class HtmlExtensions
    {
        public static string Version(this UrlHelper urlHelper, string path)
        {
            string p = path;
            path = urlHelper.RequestContext.HttpContext.Server.MapPath(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                p = urlHelper.Content(p) + "?v=" + fi.LastWriteTime.ToString("yyyyMMddHHmmss");
            }
            return p;
        }
    }
}
