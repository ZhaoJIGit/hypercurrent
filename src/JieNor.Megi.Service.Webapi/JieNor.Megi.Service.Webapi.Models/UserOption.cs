using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.Service.Webapi.JieNor.Megi.Service.Webapi.Models
{
    public class UserOption
    {
        public string token { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }

    }
    public class ChangeOption
    {
        public string token { get; set; }
        public string mItemID { get; set; }
        public int status { get; set; }


    }
    public class RenewOption
    {
        public string token { get; set; }
        public string mItemID { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}