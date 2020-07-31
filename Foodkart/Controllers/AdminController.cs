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
            Session["AdminModel"] = admin;
            Session["AdminFName"] = admin.AdminFName;
            Session["AdminId"] = admin.AdminId;
            Session["AdminMenuId"] = admin.AdminMenuId;
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId orderby 1 select food ).ToList();
            return View(FoodList);
        }

        public ActionResult AdminManageItems()
        {
            return View();
        }

        public ActionResult AddNewItems()
        {
            Food food = new Food();
            if (food.FoodName != null)
                return View("AddNewItems", food);
            return View(food);
        }

        [HttpPost]
        public ActionResult AddNewItems(Food food)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            food.FoodMenuId = long.Parse(Session["AdminMenuId"].ToString());
            IList<Food> FoodList = (from foods in foodContext.Foods where food.FoodName == foods.FoodName select foods).ToList();
            bool foodExists = false;
            foreach(Food foodSearch in FoodList)
            {
                if (foodSearch.FoodName == food.FoodName)
                {
                    foodExists = true;
                    break;
                }
            }

            if (foodExists || food.FoodName == null || food.FoodQty == 0 || food.FoodUnitPrice == 0 || food.FoodCategory == null || food.FoodType == null || food.FoodPhotoUrl == null )
            {
                ViewBag.Status = "FoodInvalid";
                return View("AddNewItems", food);
            }
            else if (!foodExists)
            {
                foodContext.Foods.Add(food);
                if (foodContext.SaveChanges() > 0)
                {
                    ViewBag.Status = "FoodRegistered";
                    return View("AddNewItems", food);
                }
            }
            else
            {
                ViewBag.Status = "FoodExists";
                return View("AddNewItems", food);
            }

            return View(food);
        }

        public ActionResult UpdateDeleteItems()
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
            return View(FoodList);
        }

        public ActionResult UpdateItem(long foodId)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Food food = foodContext.Foods.Find(foodId);
            return View(food);
        }

        [HttpPost]
        public ActionResult UpdateItem(FormCollection form)
        {
            long foodId = long.Parse(form["FoodId"].ToString());
            string foodName = form["FoodName"].ToString();
            long foodUnitPrice = long.Parse(form["FoodUnitPrice"].ToString());
            string foodCategory = form["FoodCategory"].ToString();
            string foodType = form["FoodType"].ToString();
            string foodPhotoUrl = form["FoodPhotoUrl"].ToString();

            SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
            sqlConnection.Open();
            SqlCommand sqlCmd = new SqlCommand("update Foods set FoodName = '" + foodName + "', FoodUnitPrice = " + foodUnitPrice + ", FoodCategory = '" + foodCategory + "', FoodType = '" + foodType + "', FoodPhotoUrl = '"+ foodPhotoUrl + "' where FoodId = " + foodId + ";", sqlConnection);
            sqlCmd.ExecuteNonQuery();
            SqlCommand sqlCmdFetch = new SqlCommand("select * from Foods where FoodId = " + foodId + ";", sqlConnection);
            SqlDataReader sdr = sqlCmdFetch.ExecuteReader();
            Food food = null;

            while (sdr.Read())
            {
                food = new Food
                {
                    FoodId = long.Parse(sdr[0].ToString()),
                    FoodName = sdr[1].ToString(),
                    FoodUnitPrice =  long.Parse(sdr[3].ToString()),
                    FoodCategory = sdr[4].ToString(),
                    FoodType = sdr[5].ToString(),
                    FoodPhotoUrl = sdr[6].ToString()
                };

                ViewBag.Status = "updated";
            }

            sqlConnection.Close();
            return View(food);
        }

        public ActionResult DeleteItem(long foodId)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Food food = foodContext.Foods.Find(foodId);
            return View(food);
        }

        [HttpPost]
        public ActionResult DeleteItem(FormCollection form)
        {
            long foodId = long.Parse(form["FoodId"].ToString());
            FoodkartModelContainer foodkartModelContainer = new FoodkartModelContainer();
            Food food = foodkartModelContainer.Foods.Find(foodId);
            foodkartModelContainer.Foods.Remove(food);
            foodkartModelContainer.SaveChanges();
            return View();
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

        public ActionResult AdminProfile(Admin admin)
        {
            return View(admin);
        }

        [HttpPost]
        public ActionResult AdminProfile(FormCollection form)
        {
            long adminId = long.Parse(form["AdminId"].ToString());
            string adminUsername = form["AdminUsername"].ToString();
            string adminPhone = form["AdminPhone"].ToString();
            string adminFName = form["AdminFName"].ToString();
            string adminLName = form["AdminLName"].ToString();

            SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
            sqlConnection.Open();
            SqlCommand sqlCmd = new SqlCommand("update Admins set AdminFName = '" + adminFName + "', AdminLName = '" + adminLName + "', AdminUsername = '" + adminUsername + "', AdminPhone = '" + adminPhone + "' where AdminId = " + adminId + ";", sqlConnection);
            sqlCmd.ExecuteNonQuery();
            SqlCommand sqlCmdFetch = new SqlCommand("select * from Admins where AdminId = " + adminId + ";", sqlConnection);
            SqlDataReader sdr = sqlCmdFetch.ExecuteReader();
            Admin admin = null;

            while (sdr.Read())
            {
                admin = new Admin
                {
                    AdminId = long.Parse(sdr[0].ToString()),
                    AdminUsername = sdr[1].ToString(),
                    AdminPhone = sdr[2].ToString(),
                    AdminFName = sdr[3].ToString(),
                    AdminLName = sdr[4].ToString()
                };

                ViewBag.Status = "updated";
            }

            Session["AdminModel"] = admin;
            Session["AdminFName"] = admin.AdminFName;

            sqlConnection.Close();
            return View(admin);
        }
    }
}