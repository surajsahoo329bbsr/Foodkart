using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Foodkart.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult CustomerHome(Customer customer)
        {
            Session["CustFName"] = customer.CustFName;
            Session["CustId"] = customer.CustId;
            FoodkartDBEntities foodContext = new FoodkartDBEntities();
            List<Food> FoodList = (from food in foodContext.Foods select food).ToList(); 
            return View(FoodList);
        }
        
        public ActionResult AddToCart(long foodId, long foodUnitPrice, FormCollection formCollection)
        {
            FoodkartDBEntities foodContext = new FoodkartDBEntities();
            Cart cart = new Cart
            {
                CartCustId = long.Parse(Session["CustId"].ToString())
            };

            foodContext.Carts.Add(cart);
            foodContext.SaveChanges();

            List<Cart> CartList = (from cartList in foodContext.Carts select cartList).ToList();

            CartItem cartItem = new CartItem
            {
                CartItemCartId = CartList.Last().CartId,
                CartAddDate = DateTime.Now,
                CartItemQty = 0,
                CartItemUnitPrice = foodUnitPrice,
                CartItemFoodId = foodId
            };

            foodContext.CartItems.Add(cartItem);
            foodContext.SaveChanges();

            Customer customer = new Customer
            {
                CustId = long.Parse(Session["CustId"].ToString()),
                CustFName = Session["CustFName"].ToString()
            };

            return RedirectToAction("CustomerHome", "Customer", customer);
        }

    }
}