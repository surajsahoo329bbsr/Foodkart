using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class AuthController : Controller
    {
        public ActionResult Index()
        {
            Customer customer = new Customer();
            if (customer != null) {
                return View("Index", customer);
            }
            return View(customer);
        }

        [HttpPost]
        public ActionResult Index(Customer customer, FormCollection forms)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            IList<Customer> CustList = (from cust in foodContext.Customers where cust.CustEmail == customer.CustEmail select cust).ToList();
            long custId = 0;
            foreach (Customer cust in CustList) custId = cust.CustId;
            Customer custFound = foodContext.Customers.Find(custId);
            bool auth;

            if (forms["userReg"].ToString() == "reg")
                auth = false; // register
            else
                auth = true; // login

            if (auth)
            {
                //login

                if (custFound != null && custFound.CustPassword != customer.CustPassword)
                {
                    customer.CustEmail = "BadCredentials";
                    return View(customer);
                }
                else if (custFound != null)
                {
                    return RedirectToAction("CustomerHome", "Customer", custFound);
                }
                else
                {
                    customer.CustEmail = "NotRegistered";
                    return View(customer);
                }
            }
            else
            {
                // register

                if (customer.CustEmail == null || customer.CustFName == null || customer.CustLName == null || customer.CustPhone == null || customer.CustPassword != forms["ConfirmPassword"].ToString())
                {
                    customer.CustEmail = "CustInvalid";
                    return View("Index", customer);
                }
                else if (custFound == null)
                {
                    foodContext.Customers.Add(customer);
                    if (foodContext.SaveChanges() > 0)
                    {
                        customer.CustEmail = "CustRegistered";
                        return View("Index", customer);
                    }
                    else
                        return View(customer);
                }
                else
                {
                    customer.CustEmail = "CustExists";
                    return View("Index", customer);
                }
            }            

        }
    }
}