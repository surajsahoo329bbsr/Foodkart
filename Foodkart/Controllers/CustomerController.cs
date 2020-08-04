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
        public static string connectionString = @"Data Source=SURAJ-PC;Initial Catalog=FoodKartDB;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
        readonly FoodkartModelContainer foodContext = new FoodkartModelContainer();

        public ActionResult CustomerHome(Customer customer)
        {
            try
            {
                Session["CustFName"] = customer.CustFName;
                Session["CustId"] = customer.CustId;
                Session["CustModel"] = customer;
                List<Menu> MenuList = (from menu in foodContext.Menus select menu).ToList();
                return View(MenuList);
            }
            catch(Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult ShowFoodItems(long menuId)
        {
            try
            {
                Session["CustId"] = long.Parse(Session["CustId"].ToString());
                List<Food> FoodList = (from food in foodContext.Foods where food.FoodMenuId == menuId select food).ToList();
                return View(FoodList);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult AddToCart(long foodId)
        {
            try
            {
                Session["CustId"] = long.Parse(Session["CustId"].ToString());
                Food food = foodContext.Foods.Find(foodId);
                return View(food);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }

        }

        [HttpPost]
        public ActionResult AddToCart(FormCollection forms)
        {
            try
            {
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

                ViewBag.Status = "ItemAdded";
                ViewBag.Quantity = long.Parse(forms["FoodQty"].ToString());
                return View(foodContext.Foods.Find(long.Parse(forms["FoodId"].ToString())));
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }

        }

        public ActionResult CustomerCart()
        {
            try
            {
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

                return View(showCartItems);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        [HttpPost]
        public ActionResult CustomerCart(FormCollection formCollection)
        {
            if (formCollection is null)
            {
                throw new ArgumentNullException(nameof(formCollection));
            }

            try
            {
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

                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                foreach (CartItem items in showCartItems)
                {
                    SqlCommand sqlCommand = new SqlCommand("update Foods set FoodQty = FoodQty  - " + items.CartItemQty + " where FoodId = ( select CartItemFoodId from CartItems where CartItemCartId = " + items.CartItemCartId + " );", sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }

                sqlConnection.Close();


                List<Order> OrderList = (from orderList in foodContext.Orders select orderList).ToList();

                foreach (CartItem cartItem in showCartItems)
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
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }         
            
        }

        public ActionResult CustomerOrders()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand("select OrderDate, OrderId, FoodName, FoodCategory, FoodType, OrderItemQty, OrderItemUnitPrice from OrderItems OI join Orders O on OI.OrderItemOrderId = O.OrderId join Foods F on OI.OrderItemFoodId = F.FoodId where OrderCustId = " + Session["CustId"].ToString() + " order by 1 desc;", sqlConnection);
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
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        public ActionResult CustomerProfile(Customer customer){        
            return View(customer);
        }

        [HttpPost]
        public ActionResult CustomerProfile(Customer customer, FormCollection form)
        {
            try
            {
                FoodkartModelContainer foodkartModelContainer = new FoodkartModelContainer();
                long custId = long.Parse(form["CustId"].ToString());
                Customer currCust = foodkartModelContainer.Customers.Find(custId);
                bool validate = ValidateUniquePhoneEmail(foodkartModelContainer, customer, custId);

                if (!validate)
                    ViewBag.Status = "KeyViolation";
                else
                {
                    List<Customer> custList = (from cust in foodkartModelContainer.Customers where cust.CustId == custId select cust).ToList();

                    foreach (Customer c in custList)
                    {
                        c.CustFName = customer.CustFName;
                        c.CustLName = customer.CustLName;
                        c.CustPhone = customer.CustPhone;
                        c.CustEmail = customer.CustEmail;
                    }

                    if (ModelState.IsValid)
                        foodkartModelContainer.SaveChanges();

                }

                Session["CustModel"] = currCust;
                Session["CustFName"] = currCust.CustFName;

                return View(currCust);
            }
            catch (Exception e)
            {
                string exceptionMessage = DateTime.Now + " ActionResult : " + Request.RequestContext.RouteData.Values["action"].ToString() + "Exception : " + e.Message.ToString();
                AuthController.WriteExceptionToFile(exceptionMessage, out string fileExceptionMessage);
                return Content(exceptionMessage + "\n" + fileExceptionMessage);
            }
            
        }

        private bool ValidateUniquePhoneEmail(FoodkartModelContainer foodkartModelContainer, Customer customer, long custId)
        {
            List<Customer> custList = (from cust in foodkartModelContainer.Customers where cust.CustId != custId select cust).ToList();
            foreach (Customer c in custList)
                if (c.CustPhone == customer.CustPhone || c.CustEmail == customer.CustEmail)
                    return false;
            return true;
        }
    }

}