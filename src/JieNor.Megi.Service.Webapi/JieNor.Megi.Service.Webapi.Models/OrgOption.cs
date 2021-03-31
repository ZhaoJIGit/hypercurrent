using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.Service.Webapi.JieNor.Megi.Service.Webapi.Models
{
    public class OrgOption
    {
        public string token { get; set; }
        public string name { get; set; }
        public int pageSize { get; set; }
        public int pageIndex { get; set; }
    }
}