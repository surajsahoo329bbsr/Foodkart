using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodkart.Models
{
    public class OrdersModel
    {
        public DateTime OrderDate { get; set; }
        public long OrderId { get; set; }
        public string FoodName { get; set; }
        public string FoodCategory { get; set; }
        public string FoodType { get; set; }
        public long OrderItemQty { get; set; }
        public long OrderItemUnitPrice { get; set; }
    }
}