using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class AdminAuthController : Controller
    {

        public ActionResult AdminIndex()
        {
            Admin admin = new Admin();
            if (admin.AdminUsername != null)
                return View("AdminIndex", admin);
            return View(admin);
        }

        [HttpPost]
        public ActionResult AdminIndex(Admin admin, FormCollection forms)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            IList<Admin> AdminList = (from adm in foodContext.Admins where adm.AdminUsername == admin.AdminUsername select adm).ToList();
            long adminId = 0;
            foreach (Admin adm in AdminList) adminId = adm.AdminId;
            Admin adminFound = foodContext.Admins.Find(adminId);

            bool auth;

            if (forms["userReg"].ToString() == "reg")
                auth = false; // register
            else
                auth = true; // login

            if (auth)
            {
                // login

                if (adminFound != null && adminFound.AdminPassword != admin.AdminPassword)
                {
                    admin.AdminUsername = "BadCredentials";
                    return View(admin);
                }
                else if (adminFound != null)
                {
                    return RedirectToAction("AdminHome", "Admin", adminFound);
                }
                else
                {
                    admin.AdminUsername = "NotRegistered";
                    return View(admin);
                }

            }
            else
            {
                //register

                if (admin.AdminUsername == null || admin.AdminFName == null || admin.AdminLName == null || admin.AdminPhone == null || admin.AdminPassword != forms["ConfirmPassword"].ToString())
                {
                    admin.AdminUsername = "AdminInvalid";
                    return View("AdminIndex", admin);
                }
                else if (adminFound == null)
                {
                    foodContext.Admins.Add(admin);
                    if (foodContext.SaveChanges() > 0)
                    {
                        admin.AdminUsername = "AdminRegistered";
                        return View("AdminIndex", admin);
                    }
                    else
                    {
                        return View(admin);
                    }
                }
                else
                {
                    admin.AdminUsername = "AdminExists";
                    return View("AdminIndex", admin);
                }

            }

        }        
    }
}