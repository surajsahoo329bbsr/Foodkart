using Foodkart.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminHome(Admin admin)
        {
            Session["AdminFName"] = admin.AdminFName;
            Session["AdminId"] = admin.AdminId;
            Session["AdminMenuId"] = admin.AdminMenuId;
            Session["AdminModel"] = admin;
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
            return View(FoodList);
        }

        public ActionResult AdminManageItems()
        {
            return View();
        }

        public ActionResult AddNewItems()
        {
            return View();
        }

        public ActionResult UpdateDeleteItems()
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
            return View(FoodList);
        }

        public ActionResult UpdateItem()
        {
            return View();
        }

        public ActionResult DeleteItem(long foodId)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Food food = foodContext.Foods.Find(foodId);
            return View(food);
        }

        public ActionResult RestockItems()
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
            return View(FoodList);
        }

        public ActionResult Restock(long foodId)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Food food = foodContext.Foods.Find(foodId);
            return View(food);
        }

        [HttpPost]
        public ActionResult Restock(FormCollection forms)
        {
            if (forms is null)
            {
                throw new ArgumentNullException(nameof(forms));
            }

            SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("update Foods set FoodQty = FoodQty  + " + forms["FoodQty"].ToString() + " where FoodId = " + forms["FoodId"].ToString() + ";", sqlConnection);
            sqlCommand.ExecuteNonQuery();

            SqlCommand sqlModifyDate = new SqlCommand("update Menus set MenuModifyDate = '" + DateTime.Now.ToString() + "' where MenuId = " + Session["AdminMenuId"].ToString() + ";", sqlConnection);
            sqlModifyDate.ExecuteNonQuery();

            sqlConnection.Close();

            return View();
        }

        public ActionResult AdminCustOrders()
        {
            SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("SELECT DISTINCT O.OrderId, O.OrderDate, O.OrderCustId, CONCAT(C.CustFName,' ', C.CustLName) FROM ORDERS O INNER JOIN CUSTOMERS C ON O.OrderCustId = C.CustId INNER JOIN ORDERITEMS OI ON OI.OrderItemOrderId = O.OrderId INNER JOIN FOODS F ON OI.OrderItemFoodId = F.FoodId WHERE F.FoodMenuId = "+Session["AdminMenuId"].ToString()+" ORDER BY 1 DESC; ", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            List<CustOrdersModel> CustOrdersModelList = new List<CustOrdersModel>();

            while (sqlDataReader.Read())
            {
                CustOrdersModel custOrdersModel = new CustOrdersModel
                {
                    CustOrderId = long.Parse(sqlDataReader[0].ToString()),
                    CustOrderDate = DateTime.Parse(sqlDataReader[1].ToString()),
                    CustId = long.Parse(sqlDataReader[2].ToString()),
                    CustName = sqlDataReader[3].ToString()
                };

                CustOrdersModelList.Add(custOrdersModel);
            }

            sqlConnection.Close();
            return View(CustOrdersModelList);
        }
    }
}