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
            ViewBag.CustFName = customer.CustFName;
            Session["CustId"] = customer.CustId;
            FoodkartDBEntities foodContext = new FoodkartDBEntities();
            List<Food> FoodList = (from food in foodContext.Foods select food).ToList(); 
            return View(FoodList);
        }
        
        public ActionResult AddToCart(long foodId, long foodUnitPrice)
        {
            FoodkartDBEntities foodContext = new FoodkartDBEntities();
            Cart cart = new Cart
            {
                CartCustId = long.Parse(Session["CustId"].ToString())
            };

            foodContext.Carts.Add(cart);
            foodContext.SaveChanges();

            List<Cart> CartList = (from cartList in foodContext.Carts select cartList).ToList();
            long cartItemCartId = CartList.Last().CartCustId;

            CartItem cartItem = new CartItem
            {
                CartItemCartId = cartItemCartId,
                CartAddDate = DateTime.Now,
                CartRemoveDate = DateTime.Now,
                CartItemQty = 4,
                CartItemUnitPrice = foodUnitPrice,
                CartItemFoodId = foodId

            };

            foodContext.CartItems.Add(cartItem);
            foodContext.SaveChanges();

            ViewBag.Status = "success";
            List<Food> FoodList = (from foodItem in foodContext.Foods select foodItem).ToList();
            return View("~/_Layout.cshtml");
        }

    }
}