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
        FoodkartModelContainer foodContext = new FoodkartModelContainer();

        public ActionResult AdminHome(Admin admin)
        {
            Session["AdminModel"] = admin;
            Session["AdminFName"] = admin.AdminFName;
            Session["AdminId"] = admin.AdminId;
            Session["AdminMenuId"] = admin.AdminMenuId;
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId && food.FoodQty < 50 orderby 1 select food ).ToList();
            return View(FoodList);
        }

        [HttpPost]
        public ActionResult AdminHome(List<Food> FoodList, FormCollection forms)
        {
            bool menuAvailable = bool.Parse(forms["MenuAvailable"].ToString());
            long menuId = long.Parse(Session["AdminMenuId"].ToString());
            List<Menu> MenuList = (from menu in foodContext.Menus where menu.MenuAvailable == menuAvailable where menu.MenuId == menuId select menu).ToList();
            
            foreach(Menu m in MenuList)
                m.MenuAvailable = !menuAvailable;

            foodContext.SaveChanges();
            Menu menuFound = foodContext.Menus.Find(Session["AdminMenuId"]);
            FoodList = (from food in foodContext.Foods where menuFound.MenuId == food.FoodMenuId && food.FoodQty < 50 orderby 1 select food).ToList();

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

                if (ModelState.IsValid)
                {
                    if (foodContext.SaveChanges() > 0)
                    {
                        ViewBag.Status = "FoodRegistered";
                        return View("AddNewItems", food);
                    }
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
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
            return View(FoodList);
        }

        public ActionResult UpdateItem(long foodId)
        {
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
            Food food = foodContext.Foods.Find(foodId);
            return View(food);
        }

        [HttpPost]
        public ActionResult DeleteItem(FormCollection form)
        {
            long foodId = long.Parse(form["FoodId"].ToString());
            Food food = foodContext.Foods.Find(foodId);
            foodContext.Foods.Remove(food);
            foodContext.SaveChanges();
            return View();
        }

        public ActionResult RestockItems()
        {
            Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
            List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
            return View(FoodList);
        }

        public ActionResult Restock(long foodId)
        {
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
                    CustName = sqlDataReader[3].ToString(),
                    CustMenuId = Session["AdminMenuId"].ToString()
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
        public ActionResult AdminProfile(Admin admin, FormCollection form)
        {
            long adminId = long.Parse(form["AdminId"].ToString());
            Admin currAdmin = foodContext.Admins.Find(adminId);
            bool validate = ValidateUniquePhoneUsername(admin, adminId);

            if (!validate)
                ViewBag.Status = "KeyViolation";
            else
            {
                List<Admin> adminList = (from a in foodContext.Admins where a.AdminId == adminId select a).ToList();

                foreach (Admin adm in adminList)
                {
                    adm.AdminFName = admin.AdminFName;
                    adm.AdminLName = admin.AdminLName;
                    adm.AdminPhone = admin.AdminPhone;
                    adm.AdminUsername = admin.AdminUsername;
                }

                if (ModelState.IsValid)
                    foodContext.SaveChanges();

            }

            Session["AdminModel"] = currAdmin;
            Session["AdminFName"] = currAdmin.AdminFName;

            return View(currAdmin);
        }

        private bool ValidateUniquePhoneUsername(Admin admin, long adminId)
        {
            List<Admin> adminList = (from adm in foodContext.Admins where adm.AdminId != adminId select adm).ToList();
            foreach (Admin a in adminList)
                if (a.AdminPhone == admin.AdminPhone || a.AdminUsername == admin.AdminUsername)
                    return false;
            return true;
        }
    }
}