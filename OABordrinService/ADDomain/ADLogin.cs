using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Web;

namespace OABordrinService.ADDomain
{
    public class ADLogin
    {
        //AD域登陆
        public static string LoginAD(string username, string password)
        {
            string DomainName = "bordrin.com";
            string ldapPath = "LDAP://" + DomainName;
            string errorMsg = "";
            //string userName = txtUser.Text;
            //string pwd = txtADPwd.Text;

            if (!TryAuthenticate(DomainName, username, password))
            {
                errorMsg = "AD Login Error";
            }
            else
            {
                errorMsg = "AD Login OK";
            }
            return errorMsg;
        }


        public static bool TryAuthenticate(string domain, string username, string password)
        {
            bool isLogin = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(string.Format("LDAP://{0}", domain), username, password);
                entry.RefreshCache();
          
                isLogin = true;
            }
            catch (Exception ex)
            {
                isLogin = false;
            }
            return isLogin;
        }

    }
}