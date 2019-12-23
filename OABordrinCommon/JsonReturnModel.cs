using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OABordrinCommon
{
    public class JsonReturnModel
    {
        public List<ErrorMessage> error = new List<ErrorMessage>();
        public object data;
        public void AddError(string key, string value)
        {
            error.Add(new ErrorMessage() { key = key, value = value });
        }
        public bool HasError
        {
            get
            {
                return error == null || error.Count > 0;
            }
        }
    }

    public class ErrorMessage
    {
        public string key { get; set; }
        public string value { get; set; }
    }


  

}