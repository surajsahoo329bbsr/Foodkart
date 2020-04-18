using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
            
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            IList<Customer> CustList = (from cust in foodContext.Customers where cust.CustEmail == customer.CustEmail select cust).ToList();
            long custId = 0;
            foreach (Customer cust in CustList) custId = cust.CustId;
            Customer custFound = foodContext.Customers.Find(custId);

            if (custFound != null && custFound.CustPassword != customer.CustPassword)
            {
                customer.CustEmail = "BadCredentials";
                return View(customer);
            }
            else if (custFound != null)
            {
                return RedirectToAction("CustomerHome","Customer", custFound);
            }
            else
            {
                customer.CustEmail = "NotRegistered";
                return View(customer);
            }
        }

        public ActionResult Registration()
        {
            Customer customer = new Customer();
            if (customer.CustEmail != null)
                return View("Registration", customer);
            return View(customer);
        }

        [HttpPost]
        public ActionResult Registration(Customer customer)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            IList<Customer> CustList = (from cust in foodContext.Customers where cust.CustEmail == customer.CustEmail select cust).ToList();
            long custId = 0;
            foreach (Customer cust in CustList) custId = cust.CustId;
            Customer custFound = foodContext.Customers.Find(custId);

            if (customer.CustEmail == null || customer.CustFName == null || customer.CustLName == null)
            {
                customer.CustEmail = "CustNull";
                return View("Registration", customer);
            }
            else if (custFound == null)
            {
                foodContext.Customers.Add(customer);
                if (foodContext.SaveChanges() > 0)
                {
                    customer.CustEmail = "CustRegistered";
                    return View("Registration", customer);
                }
            }
            else
            {
                customer.CustEmail = "CustExists";
                return View("Registration", customer);
            }

            return View(customer);
        }
    }
}