using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class AuthController : Controller
    {
        public static bool WriteExceptionToFile(string exception, out string fileException)
        {
            string exceptionFile = @"D:\VisualStudioProjects\Foodkart\Foodkart\ExceptionFiles\FoodkartCustomerException.txt";

            try
            {
                FileInfo fileInfo = new FileInfo(exceptionFile);
                if (!fileInfo.Exists)
                    using (fileInfo.CreateText()) { };

                using (StreamWriter streamWriter = fileInfo.AppendText())
                {
                    streamWriter.WriteLine(exception);
                }

                fileException = null;
                return true;
            }
            catch (Exception e)
            {
                fileException = DateTime.Now + " CustomerFileWriteException : " + e.Message.ToString();
                return false;
            }
        }

        public ActionResult Index()
        {
            try
            {
                Customer customer = new Customer();
                if (customer != null)
                {
                    return View("Index", customer);
                }
                return View(customer);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        [HttpPost]
        public ActionResult Index(Customer customer, FormCollection forms)
        {
            try
            {
                FoodkartModelContainer foodContext = new FoodkartModelContainer();
                IList<Customer> CustList = (from cust in foodContext.Customers where cust.CustEmail == customer.CustEmail || cust.CustPhone == customer.CustPhone select cust).ToList();
                long custId = 0;
                foreach (Customer cust in CustList) custId = cust.CustId;
                Customer custFound = foodContext.Customers.Find(custId);
                bool auth = forms["userReg"].ToString() != "reg";

                if (auth) //login
                {
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
                else // register
                {
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
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }

        }
    }
}