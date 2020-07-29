using Foodkart.Models;
using System;
using System.Collections.Generic;
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
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            List<Menu> MenuList = (from menu in foodContext.Menus select menu).ToList();
            return View(MenuList);
        }

        public ActionResult ShowFoodItems(long menuId)
        {
            Session["CustId"] = long.Parse(Session["CustId"].ToString());
            FoodkartModelContainer foodContext = new FoodkartModelContainer();
            List<Food> FoodList = (from food in foodContext.Foods where food.FoodMenuId == menuId select food).ToList();
            return View(FoodList);
        }
        
        public ActionResult AddToCart(long foodId)
        {
            Session["CustId"] = long.Parse(Session["CustId"].ToString());
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

            SqlConnection sqlConnection = new SqlConnection(@"Data Source=SURAJ-PC;Initial Catalog=FoodKartDB;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
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
            SqlConnection sqlConnection = new SqlConnection(@"Data Source=SURAJ-PC;Initial Catalog=FoodKartDB;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework");
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

        public ActionResult CustomerProfile(Customer customer)
        {
            return View(customer);
        }

        [HttpPost]
        public ActionResult CustomerProfile(FormCollection form)
        {
            long custId = long.Parse(form["CustId"].ToString());
            string custEmail = form["CustEmail"].ToString();
            string custPhone = form["CustPhone"].ToString();
            string custFName = form["CustFName"].ToString();
            string custLName = form["CustLName"].ToString();

            SqlConnection sqlConn = new SqlConnection(@"Data Source=SURAJ-PC;Initial Catalog=FoodkartDB;Integrated Security=True");
            sqlConn.Open();
            SqlCommand sqlCmd = new SqlCommand("update Customers set CustFName = '" + custFName + "', CustLName = '" + custLName + "', CustEmail = '" + custEmail + "', CustPhone = '" + custPhone + "' where CustId = " + custId + ";", sqlConn);
            sqlCmd.ExecuteNonQuery();
            SqlCommand sqlCmdFetch = new SqlCommand("select * from Customers where CustId = " + custId + ";", sqlConn);
            SqlDataReader sdr = sqlCmdFetch.ExecuteReader();
            Customer customer = null;

            while (sdr.Read())
            {
                customer = new Customer
                {
                    CustId = long.Parse(sdr[0].ToString()),
                    CustEmail = sdr[1].ToString(),
                    CustPhone = sdr[2].ToString(),
                    CustFName = sdr[3].ToString(),
                    CustLName = sdr[4].ToString()
                };

                ViewBag.Status = "updated";
            }

            Session["CustModel"] = customer;
            Session["CustFName"] = customer.CustFName;

            sqlConn.Close();
            return View(customer);
        }

    }

}