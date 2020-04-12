using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Login()
        {
            Customer customer = new Customer();
            if (customer.CustEmail != null)
                return View("Login", customer);
            return View(customer);
        }

        [HttpPost]
        public ActionResult Login(Customer customer)
        {
            
            FoodkartDBEntities userContext = new FoodkartDBEntities();
            Customer custFound = userContext.Customers.Find(customer.CustEmail);

            if (custFound != null && custFound.CustPassword != customer.CustPassword)
            {
                customer.CustEmail = "BadCredentials";
                return View(customer);
            }
            else if (custFound != null)
                return View("~/Views/Admin/AdminHome.cshtml", custFound);
            else
            {
                customer.CustEmail = "NotRegistered";
                return View(customer);
            }
        }

        public ActionResult RegisterNew()
        {
            Customer customer = new Customer();
            if (customer.CustEmail != null)
                return View("RegisterNew", customer);
            return View(customer);
        }

        [HttpPost]
        public ActionResult RegisterNew(Customer customer)
        {
            FoodkartDBEntities userContext = new FoodkartDBEntities();
            Customer custFound = userContext.Customers.Find(customer.CustEmail);

            if (customer.CustEmail == null || customer.CustFName == null || customer.CustLName == null)
            {
                customer.CustEmail = "CustNull";
                return View("RegisterNew", customer);
            }
            else if (custFound == null)
            {
                userContext.Customers.Add(customer);
                if (userContext.SaveChanges() > 0)
                {
                    customer.CustEmail = "CustRegistered";
                    return View("RegisterNew", customer);
                }
            }
            else
            {
                customer.CustEmail = "CustExists";
                return View("RegisterNew", customer);
            }

            return View(customer);
        }
    }
}