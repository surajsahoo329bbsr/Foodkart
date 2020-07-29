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
    }
}