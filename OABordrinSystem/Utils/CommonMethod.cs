using OABordrinEntity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Web;

namespace OABordrinSystem.Utils
{
    public class CommonMethod
    {
        public static DirectoryEntry GetUserEntryByAccount(DirectoryEntry entry, string account)
        {
            DirectorySearcher searcher = new DirectorySearcher(entry);
            searcher.Filter = "(&(objectClass=user)(SAMAccountName=" + account + "))";
            SearchResult result = searcher.FindOne();
            entry.Close();
            if (result != null)
            {
                return result.GetDirectoryEntry();
            }
            return null;
        }

        /// <summary>
        /// 获取windows验证时的用户名称
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserLoginName(HttpContextBase context)
        {
            if (context == null)
                return null;

            if (context.Request.IsAuthenticated == false)
                return null;

            string userName = context.User.Identity.Name;

            string[] array = userName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if(array.Length==2)
            {
                return array[1];
            }
            return null;
        }



        public static void GetAdInfoByUser(UserInfo user, string domain)
        {
            DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}", domain));

            DirectoryEntry obj = GetUserEntryByAccount(entry, user.LoginName);
            //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            if (obj != null && obj.Parent != null && obj.Parent.Properties["ou"].Value != null && !string.IsNullOrEmpty(obj.Parent.Properties["ou"].Value.ToString()))
            {
                user.department = obj.Parent.Properties["ou"].Value.ToString();
            }
        }

        /// <summary>
        /// MD5  string16
        /// </summary>
        /// <param name="inputStr"></param>
        /// <param name="forceSha"></param>
        /// <returns></returns>
        public static string md5string16(string inputStr, bool forceSha)
        {
            System.Security.Cryptography.HashAlgorithm hashAlg = null;
            StringBuilder sb = new StringBuilder(16);
            try
            {
                if (!forceSha)
                {
                    try
                    {
                        hashAlg = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    }
                    catch
                    {
                        hashAlg = null;
                    }
                }
                if (hashAlg == null)
                {
                    hashAlg = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                }
                var asciiEncoding = new ASCIIEncoding();
                Byte[] data = asciiEncoding.GetBytes(inputStr);
                Byte[] resulthash = hashAlg.ComputeHash(data);

                for (int j = 0; j < resulthash.Length; j++)
                {
                    sb.AppendFormat("{0:X2}", resulthash[j]);
                }
            }
            finally
            {
                if (hashAlg != null)
                {
                    hashAlg.Dispose();
                }
            }
            return sb.ToString();
        }






    }
}