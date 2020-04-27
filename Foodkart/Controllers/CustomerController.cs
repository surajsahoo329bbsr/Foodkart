using Foodkart.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            Session["CustModel"] = customer;
            if (customer.CustPassword == "CartAdded")
                ViewBag.Status = "added";
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            List<Food> FoodList = (from food in foodContext.Foods select food).ToList(); 
            return View(FoodList);
        }
        
        public ActionResult AddToCart(long foodId)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            Food food = foodContext.Foods.Find(foodId);
            return View(food);

        }

        [HttpPost]
        public ActionResult AddToCart(FormCollection forms)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
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
                CartItemQty = long.Parse(forms["FoodQty"].ToString()),
                CartItemFoodId = long.Parse(forms["FoodId"].ToString())
            };

            foodContext.CartItems.Add(cartItem);
            foodContext.SaveChanges();

            Customer customer = new Customer
            {
                CustId = long.Parse(Session["CustId"].ToString()),
                CustFName = Session["CustFName"].ToString(),
                CustPassword = "CartAdded"
            };

            return View();

        }

        public ActionResult CustomerCart()
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            List<CartItem> cartItemList = foodContext.CartItems.ToList();
            List<Cart> cartList = foodContext.Carts.ToList();
            List<CartItem> showCartItems = new List<CartItem>();
            foreach (CartItem cartItem  in cartItemList)
            {
                foreach(Cart cart in cartList)
                {
                    if(cart.CartCustId == long.Parse(Session["CustId"].ToString()) && cart.CartId == cartItem.CartItemCartId)
                    {
                        showCartItems.Add(cartItem);
                    }
                }
            }

            return View(showCartItems);
        }

        [HttpPost]
        public ActionResult CustomerCart(FormCollection formCollection)
        {
            FoodkartModelContainer foodContext = new FoodkartModelContainer();

            Order order = new Order
            {
                OrderDate = DateTime.Now,
                OrderCustId = long.Parse(Session["CustId"].ToString())
            };

            foodContext.Orders.Add(order);
            foodContext.SaveChanges();


            List<CartItem> cartItemList = foodContext.CartItems.ToList();
            List<Cart> cartList = foodContext.Carts.ToList();
            List<CartItem> showCartItems = new List<CartItem>();
            foreach (CartItem cartItem in cartItemList)
            {
                foreach (Cart cart in cartList)
                {
                    if (cart.CartCustId == long.Parse(Session["CustId"].ToString()) && cart.CartId == cartItem.CartItemCartId)
                    {
                        showCartItems.Add(cartItem);
                    }
                }
            }

            SqlConnection sqlConnection = new SqlConnection(@"Data Source=JOHNDOE-PC\SQLEXPRESS;Initial Catalog=FoodKartDB;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
            sqlConnection.Open();

            foreach (CartItem items in showCartItems)
            {
                SqlCommand sqlCommand = new SqlCommand("update Foods set FoodQty = FoodQty  - " + items.CartItemQty + " where FoodId = ( select CartItemFoodId from CartItems where CartItemCartId = " + items.CartItemCartId + " );", sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }

            sqlConnection.Close();


            List<Order> OrderList = (from orderList in foodContext.Orders select orderList).ToList();

            foreach(CartItem cartItem in showCartItems)
            {
                Food findFood = foodContext.Foods.Find(cartItem.CartItemFoodId); 

                OrderItem orderItem = new OrderItem
                {
                    OrderItemOrderId = OrderList.Last().OrderId,
                    OrderItemFoodId = cartItem.CartItemFoodId,
                    OrderItemQty = cartItem.CartItemQty,
                    OrderItemUnitPrice = findFood.FoodUnitPrice
                };

                foodContext.OrderItems.Add(orderItem);
                foodContext.SaveChanges();
                foodContext.CartItems.Remove(cartItem);
                foodContext.SaveChanges();
                
            }

            foreach (Cart cart in cartList)
            {
                if (cart.CartCustId == long.Parse(Session["CustId"].ToString()))
                {
                    foodContext.Carts.Remove(cart);
                    foodContext.SaveChanges();
                }
            }

            ViewBag.Status = "ordered";            
            showCartItems = new List<CartItem>(); //Adding this hence Null Pointer Exception Doesn't arise
            return View(showCartItems);
        }

        public ActionResult CustomerOrders()
        {
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=JOHNDOE-PC\SQLEXPRESS;Initial Catalog=FoodKartDB;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
            sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("select OrderDate, OrderId, FoodName, FoodCategory, FoodType, OrderItemQty, OrderItemUnitPrice from OrderItems OI join Orders O on OI.OrderItemOrderId = O.OrderId join Foods F on OI.OrderItemFoodId = F.FoodId where OrderCustId = "+ Session["CustId"].ToString() + " order by 1 desc;", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            List<OrdersModel> OrdersModelList = new List<OrdersModel>();

            while (sqlDataReader.Read())
            {
                OrdersModel ordersModel = new OrdersModel
                {
                    OrderDate = DateTime.Parse(sqlDataReader[0].ToString()),
                    OrderId = long.Parse(sqlDataReader[1].ToString()),
                    FoodName = sqlDataReader[2].ToString(),
                    FoodCategory = sqlDataReader[3].ToString(),
                    FoodType = sqlDataReader[4].ToString(),
                    OrderItemQty = long.Parse(sqlDataReader[5].ToString()),
                    OrderItemUnitPrice = long.Parse(sqlDataReader[6].ToString())

                };

                OrdersModelList.Add(ordersModel);
            }

            sqlConnection.Close();
            return View(OrdersModelList);
        }

    }

}