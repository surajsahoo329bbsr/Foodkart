using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult CustomerHome()
        {
            return View();
        }
    }
}