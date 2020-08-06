using Foodkart.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using WebMatrix.Data;

namespace Foodkart.Controllers
{
    public class AdminController : Controller
    {
        readonly FoodkartModelContainer foodContext = new FoodkartModelContainer();

        // GET: Admin
        public ActionResult AdminHome(Admin admin)
        {
            try
            {
                Session["AdminModel"] = admin;
                Session["AdminFName"] = admin.AdminFName;
                Session["AdminId"] = admin.AdminId;
                Session["AdminMenuId"] = admin.AdminMenuId;
                Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
                List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId && food.FoodQty < 50 orderby 1 select food).ToList();
                return View(FoodList);
            }
            catch(Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        [HttpPost]
        public ActionResult AdminHome(List<Food> FoodList, FormCollection forms)
        {
            try
            {
                bool menuAvailable = bool.Parse(forms["MenuAvailable"].ToString());
                long menuId = long.Parse(Session["AdminMenuId"].ToString());
                List<Menu> MenuList = (from menu in foodContext.Menus where menu.MenuAvailable == menuAvailable where menu.MenuId == menuId select menu).ToList();

                foreach (Menu m in MenuList)
                    m.MenuAvailable = !menuAvailable;

                foodContext.SaveChanges();
                Menu menuFound = foodContext.Menus.Find(Session["AdminMenuId"]);
                FoodList = (from food in foodContext.Foods where menuFound.MenuId == food.FoodMenuId && food.FoodQty < 50 orderby 1 select food).ToList();

                return View(FoodList);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult AdminManageItems()
        {
            return View();
        }

        public ActionResult AddNewItems()
        {
            try
            {
                Food food = new Food();
                if (food.FoodName != null)
                    return View("AddNewItems", food);
                return View(food);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        [HttpPost]
        public ActionResult AddNewItems(Food food)
        {
            try
            {
                food.FoodMenuId = long.Parse(Session["AdminMenuId"].ToString());
                IList<Food> FoodList = (from foods in foodContext.Foods where food.FoodName == foods.FoodName select foods).ToList();
                bool foodExists = false;
                foreach (Food foodSearch in FoodList)
                    if (foodSearch.FoodName == food.FoodName)
                    {
                        foodExists = true;
                        break;
                    }

                if (foodExists || food.FoodName == null || food.FoodQty == 0 || food.FoodUnitPrice == 0 || food.FoodCategory == null || food.FoodType == null || food.FoodPhotoUrl == null)
                {
                    ViewBag.Status = "FoodInvalid";
                    return View("AddNewItems", food);
                }
                else if (!foodExists)
                {
                    foodContext.Foods.Add(food);

                    if (ModelState.IsValid && foodContext.SaveChanges() > 0)
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
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult UpdateDeleteItems()
        {
            try
            {
                Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
                List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
                return View(FoodList);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult UpdateItem(long foodId)
        {
            try
            {
                Food food = foodContext.Foods.Find(foodId);
                return View(food);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }

        }

        [HttpPost]
        public ActionResult UpdateItem(FormCollection form)
        {
            try
            {
                long foodId = long.Parse(form["FoodId"].ToString());
                string foodName = form["FoodName"].ToString();
                long foodUnitPrice = long.Parse(form["FoodUnitPrice"].ToString());
                string foodCategory = form["FoodCategory"].ToString();
                string foodType = form["FoodType"].ToString();
                string foodPhotoUrl = form["FoodPhotoUrl"].ToString();

                SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
                sqlConnection.Open();
                SqlCommand sqlCmd = new SqlCommand("update Foods set FoodName = '" + foodName + "', FoodUnitPrice = " + foodUnitPrice + ", FoodCategory = '" + foodCategory + "', FoodType = '" + foodType + "', FoodPhotoUrl = '" + foodPhotoUrl + "' where FoodId = " + foodId + ";", sqlConnection);
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
                        FoodUnitPrice = long.Parse(sdr[3].ToString()),
                        FoodCategory = sdr[4].ToString(),
                        FoodType = sdr[5].ToString(),
                        FoodPhotoUrl = sdr[6].ToString()
                    };

                    ViewBag.Status = "updated";
                }

                sqlConnection.Close();
                return View(food);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }            
        }

        public ActionResult DeleteItem(long foodId)
        {
            try
            {
                Food food = foodContext.Foods.Find(foodId);
                return View(food);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        [HttpPost]
        public ActionResult DeleteItem(FormCollection form)
        {
            try
            {
                long foodId = long.Parse(form["FoodId"].ToString());
                Food food = foodContext.Foods.Find(foodId);
                foodContext.Foods.Remove(food);
                foodContext.SaveChanges();
                return View();
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
           
        }

        public ActionResult RestockItems()
        {
            try
            {
                Menu menu = foodContext.Menus.Find(Session["AdminMenuId"]);
                List<Food> FoodList = (from food in foodContext.Foods where menu.MenuId == food.FoodMenuId select food).ToList();
                return View(FoodList);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult Restock(long foodId)
        {
            try
            {
                ViewBag.Status = "";
                Food food = foodContext.Foods.Find(foodId);
                return View(food);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        [HttpPost]
        public ActionResult Restock(FormCollection forms)
        {
            if (forms is null)
            {
                throw new ArgumentNullException(nameof(forms));
            }

            try
            {
                SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("update Foods set FoodQty = FoodQty  + " + forms["FoodQty"].ToString() + " where FoodId = " + forms["FoodId"].ToString() + ";", sqlConnection);
                SqlCommand sqlModifyDate = new SqlCommand("update Menus set MenuModifyDate = '" + DateTime.Now.ToString() + "' where MenuId = " + Session["AdminMenuId"].ToString() + ";", sqlConnection);
                sqlCommand.ExecuteNonQuery();
                sqlModifyDate.ExecuteNonQuery();
                sqlConnection.Close();
                ViewBag.Status = "Restock";
                return View(foodContext.Foods.Find(long.Parse(forms["FoodId"].ToString())));

            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
        }

        public ActionResult Graph()
        {
            try
            {
                return Json(Result(), JsonRequestBehavior.AllowGet);

            }
            catch(Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public List<OrdersModel> Result()
        {
            try
            {
                List<OrdersModel> ListOrdersModel = new List<OrdersModel>();

                SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("SELECT CAST(O.OrderDate AS DATE), (SELECT SUM(OI.OrderItemQty)) FROM OrderItems OI INNER JOIN Orders O ON O.OrderId = OI.OrderItemOrderId INNER JOIN Foods F ON OI.OrderItemFoodId = F.FoodId WHERE F.FoodMenuId = " + long.Parse(Session["AdminMenuId"].ToString()) + " GROUP BY CAST(O.OrderDate AS DATE);", sqlConnection);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    OrdersModel ordersModel = new OrdersModel
                    {
                        OrderDate = DateTime.Parse(sqlDataReader[0].ToString()),
                        OrderItemQty = long.Parse(sqlDataReader[1].ToString())
                    };

                    ListOrdersModel.Add(ordersModel);
                }

                sqlConnection.Close();
                return ListOrdersModel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult AdminCustOrders()
        {            
            try
            {
                SqlConnection sqlConnection = new SqlConnection(CustomerController.connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("SELECT DISTINCT O.OrderId, O.OrderDate, O.OrderCustId, CONCAT(C.CustFName,' ', C.CustLName) FROM ORDERS O INNER JOIN CUSTOMERS C ON O.OrderCustId = C.CustId INNER JOIN ORDERITEMS OI ON OI.OrderItemOrderId = O.OrderId INNER JOIN FOODS F ON OI.OrderItemFoodId = F.FoodId WHERE F.FoodMenuId = " + Session["AdminMenuId"].ToString() + " ORDER BY 1 DESC; ", sqlConnection);
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
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }            
        }

        public ActionResult AdminProfile(Admin admin)
        {
            return View(admin);
        }

        [HttpPost]
        public ActionResult AdminProfile(Admin admin, FormCollection form)
        {
            try
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
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AdminAuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
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