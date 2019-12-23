using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OABordrinCommon
{
    public class PagerEntity
    {
        private int _PageSize = 15;

        public int PageSize
        {
            get { return _PageSize; }
            set { _PageSize = value; }
        }

        private int _PageIndex = 1;

        public int PageIndex
        {
            get { return _PageIndex; }
            set { _PageIndex = value; }
        }

        private string _Orderby = "";
        public string Orderby
        {
            get { return _Orderby; }
            set { _Orderby = value; }
        }
        public int RecordCount { get; set; }

        public int PagesCount { get; set; }
    }
}
