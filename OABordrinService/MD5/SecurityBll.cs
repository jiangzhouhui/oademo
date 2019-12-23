using Aras.IOM;
using OABordrinBll;
using OABordrinDAL;
using OABordrinEntity;
using OAEntitys;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OABordrinService.MD5
{
    public class SecurityBll
    {
        private static string KEY_64 = "zxcvbnml";
        private static string IV_64 = "asdfqwer";

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="Key_64">密钥</param>
        /// <param name="Iv_64">向量</param>
        /// <returns></returns>
        public static string Encode(string data)
        {
            //string KEY_64 = Key_64;// "VavicApp";
            //string IV_64 = Iv_64;// "VavicApp";
            try
            {
                byte[] byKey = ASCIIEncoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = ASCIIEncoding.ASCII.GetBytes(IV_64);
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                int i = cryptoProvider.KeySize;
                MemoryStream ms = new MemoryStream();
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cst);
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <param name="Key_64">密钥</param>
        /// <param name="Iv_64">向量</param>
        /// <returns></returns>
        public static string Decode(string data)
        {
            //string KEY_64 = Key_64;// "VavicApp";密钥
            //string IV_64 = Iv_64;// "VavicApp"; 向量
            try
            {
                byte[] byKey = ASCIIEncoding.ASCII.GetBytes(KEY_64);
                byte[] byIV = ASCIIEncoding.ASCII.GetBytes(IV_64);
                byte[] byEnc;
                byEnc = Convert.FromBase64String(data); //把需要解密的字符串转为8位无符号数组
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }


        /// <summary>
        /// 校验令牌是否有效
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool VerifyToken(string userName, string token)
        {
            string decodeStr = Decode(token);
            if (userName == decodeStr)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  登陆获取用户信息
        /// </summary>
        /// <param name="loginName"></param>
        public static void LogIn(string loginName, UserInfo user)
        {
            string url = ConfigurationManager.AppSettings["ArasUrl"];
            string dbName = ConfigurationManager.AppSettings["ArasDB"];

            //获取用户信息
            USER userObJ = UserDA.GetUserByLoginName(loginName);
            if(userObJ!=null)
            {
                user.UserId = userObJ.ID;
                user.UserName = userObJ.KEYED_NAME;
                user.LoginName = userObJ.LOGIN_NAME;
                user.Password = userObJ.PASSWORD;
                user.b_JobNumber = userObJ.B_JOBNUMBER;
                user.Email = userObJ.EMAIL;
                HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, dbName, user.LoginName, user.Password);
                Item login_result = conn.Login();
                if (login_result.isError())
                {
                    if (conn != null) { conn.Logout(); }
                }
                else
                {
                    var inn = login_result.getInnovator();
                    if (inn != null)
                    {
                        //获取当前角色身份
                        List<string> listRoles = IdentityDA.getIdentityListByUserID(inn, user.UserId);
                        user.Roles = listRoles;

                        if ((user.AgentAuth == null && user.AgentCreateTime == null) || (user.AgentCreateTime != null))
                        {
                            List<AgentSetEntity> AgentSetList = AgentSetBll.GetAgentSetByUserName(user.UserName);
                            if (AgentSetList.Count > 0)
                            {
                                AgentSetBll.GetAgentRoles(inn, user, AgentSetList);
                            }
                        }
                        user.inn = inn;
                    }
                }
            }
        }
    }
}