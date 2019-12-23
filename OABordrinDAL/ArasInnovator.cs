using Aras.IOM;
using System;
using System.Configuration;

namespace OABordrinDAL
{
    public class ArasInnovator: IDisposable
    {
        private string url = ConfigurationManager.AppSettings["ArasUrl"];
        private string DBName = ConfigurationManager.AppSettings["ArasDB"];
        private HttpServerConnection conn;
        private Innovator inn;

        /// <summary>
        /// aras连接
        /// </summary>
        public Innovator ArasConnection(string userName,string password)
        {
            conn = IomFactory.CreateHttpServerConnection(url, DBName, userName, password);
            Item login_result = conn.Login();
            inn = login_result.getInnovator();
            return inn;
        }

        public void Dispose()
        {
           
        }
    }
}
