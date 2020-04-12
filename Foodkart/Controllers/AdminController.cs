using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminHome()
        {
            return View();
        }
    }
}