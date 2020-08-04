using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodkart.Models
{
    public class CustOrdersModel
    {
        public long CustOrderId { get; set; }
        public DateTime CustOrderDate { get; set; }
        public long CustId { get; set; }
        public string CustName { get; set; }
        public string CustMenuId { get; set; }

    }
}