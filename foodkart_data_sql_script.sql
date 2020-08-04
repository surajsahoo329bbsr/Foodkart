IF EXISTS(SELECT 1 FROM sys.databases WHERE name='FoodKartDB')
	DROP DATABASE FoodKartDB;
    
CREATE DATABASE [FoodKartDB];

USE FoodKartDB;

select OrderDate, OrderId, FoodName, FoodCategory, FoodType, OrderItemQty, OrderItemUnitPrice from OrderItems OI join Orders O on OI.OrderItemOrderId = O.OrderId join Foods F on OI.OrderItemFoodId = F.FoodId where OrderCustId = 1 order by 1 desc;

SELECT * FROM CUSTOMERS;
SELECT * FROM MENUS;
SELECT * FROM FOODS;
SELECT FoodCategory FROM FOODS GROUP BY FoodCategory;

UPDATE CUSTOMERS SET CustFName = 'Suraj' WHERE CustId = 1;
SELECT * FROM ORDERS;
SELECT * FROM OrderItems;

SELECT 
SUM(F.FoodUnitPrice * OI.OrderItemQty),
SUM(OI.OrderItemQty),
(SELECT FoodName FROM Foods 
WHERE FoodId = 
(SELECT TOP 1 OrderItemFoodId
FROM OrderItems OI
INNER JOIN Foods F ON
OI.OrderItemFoodId = F.FoodId
WHERE F.FoodMenuId = 1
GROUP BY OrderItemFoodId
ORDER BY SUM(OrderItemQty) DESC))
FROM Foods F INNER JOIN OrderItems OI 
ON F.FoodId = OI.OrderItemFoodId
WHERE F.FoodMenuId = 1;

SELECT SUM(F.FoodUnitPrice * OI.OrderItemQty), SUM(OI.OrderItemQty), (SELECT FoodName FROM Foods WHERE FoodId = (SELECT TOP 1 OrderItemFoodId FROM OrderItems OI INNER JOIN Foods F ON OI.OrderItemFoodId = F.FoodId WHERE F.FoodMenuId = 1 GROUP BY OrderItemFoodId ORDER BY SUM(OrderItemQty) DESC)) FROM Foods F INNER JOIN OrderItems OI ON F.FoodId = OI.OrderItemFoodId WHERE F.FoodMenuId = 1;

SELECT FoodName FROM Foods F
WHERE FoodId = 
(SELECT TOP 1 OrderItemFoodId
FROM OrderItems OI
INNER JOIN Foods F ON
OI.OrderItemFoodId = F.FoodId
WHERE F.FoodMenuId = 1
GROUP BY OrderItemFoodId
ORDER BY SUM(OrderItemQty) DESC);

ALTER TABLE MENUS ADD CONSTRAINT DF_MENUS DEFAULT GETDATE() FOR MenuAddDate;
INSERT INTO 
MENUS 
(MenuName, MenuAvailable, MenuModifyDate, MenuLogoUrl) 
VALUES 
('KFC', 1, NULL, 'https://i.pinimg.com/originals/aa/92/89/aa9289de1ed2865bccd7c7457f246482.jpg'),
('Dominos', 1, NULL, 'https://upload.wikimedia.org/wikipedia/commons/thumb/7/74/Dominos_pizza_logo.svg/1200px-Dominos_pizza_logo.svg.png'),
('Burger King', 0, NULL, 'https://upload.wikimedia.org/wikipedia/commons/thumb/7/79/Burger_King_logo.svg/759px-Burger_King_logo.svg.png'),
('Baskin Robbins', 0, NULL, 'https://upload.wikimedia.org/wikipedia/commons/thumb/d/d8/Baskin-Robbins_logo.svg/768px-Baskin-Robbins_logo.svg.png'),
('McDonalds', 1, NULL, 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTMzs3nbyV7ofFcOY3gLhPTDzSHT7irxA1JfewKu4GBxNjVVA&s');

/*KFC*/
INSERT INTO 
FOODS
(FoodName, FoodQty, FoodUnitPrice, FoodCategory, FoodType, FoodPhotoUrl, FoodMenuId) 
VALUES 
('Chicken Zinger', 250, 150, 'Burgers', 'N', 'https://online.kfc.co.in/Content/OnlineOrderingImages/Menu/Items/mixed-zinger-burger-duo.jpg?v=13', 1),
('Veg Zinger', 200, 130, 'Burgers', 'V', 'https://online.kfc.co.in/Content/OnlineOrderingImages/Menu/Items/CAT99-4067.jpg?v=13', 1),
('Hot & Crispy Smokey Grilled', 450, 360, 'Chicken', 'N', 'https://akm-img-a-in.tosshub.com/indiatoday/images/story/201612/kfc1-kfcindia_official-story_647_120516035620.jpg', 1),
('Dips Bucket', 300, 479, 'Chicken', 'N', 'https://online.kfc.co.in/Content/OnlineOrderingImages/Menu/Items/lg2x/dips-bucket-12-pcs.png', 1),
('Rice Bowlz', 220, 165, 'Rice', 'N', 'https://online.kfc.co.in/Content/OnlineOrderingImages/Menu/Items/chicken-rice-bowl.jpg?v=13', 1);

/*DOMINOS*/
INSERT INTO 
FOODS
(FoodName, FoodQty, FoodUnitPrice, FoodCategory, FoodType, FoodPhotoUrl, FoodMenuId) 
VALUES 
('Garlic Breadsticks', 250, 99, 'Breads', 'V', 'https://i.ytimg.com/vi/S3BnDDC2mfA/hqdefault.jpg', 2),
('Choco Lava Cake', 200, 99, 'Cakes', 'V', 'https://imagesvc.meredithcorp.io/v3/mm/image?q=85&c=sc&poi=face&url=https%3A%2F%2Fcdn-image.foodandwine.com%2Fsites%2Fdefault%2Ffiles%2Fstyles%2F4_3_horizontal_-_1200x900%2Fpublic%2F1580747466%2Fmolten-chocolate-cake-FT-RECIPE0220.jpg%3Fitok%3DYIlx4xGI', 2),
('Work From Home Non-Veg Treat', 450, 199, 'Meals', 'N', 'https://images.dominos.co.in/meal_4_1_classic_nvg.jpg', 2),
('Double Cheese Margherita', 300, 185, 'Pizza', 'V', 'https://www.dominos.co.in/blog/wp-content/uploads/2019/12/new_double_cheese_margherita.jpg', 2),
('Veg Extravaganza', 220, 235, 'Pizza', 'V', 'https://www.dominos.co.in//files/items/Veg_Extravaganz.jpg', 2);

/*Burger King*/
INSERT INTO 
FOODS
(FoodName, FoodQty, FoodUnitPrice, FoodCategory, FoodType, FoodPhotoUrl, FoodMenuId) 
VALUES 
('Crispy Veg', 250, 45, 'Burgers', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR1fvNhswX9rJGFZPOiH-oubJ0IO75L3wM-acAC46lvFNZbca8&s', 3),
('Crispy Chicken Supreme', 200, 135, 'Regular Meal', 'N', 'https://bk-apac-prd.s3.amazonaws.com/sites/burgerkingindia.in/files/VegWhopper-Thumb_0_0.png', 3),
('BK Grill Chicken', 450, 89, 'Burgers', 'N', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRWNK6mt8oJb22caJ_oT2WceYKprKOEa2jtx7mD_vlQfrJ0ku0&s', 3),
('Chilli Cheese', 300, 179, 'Regular Meal', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSbA0vnG3iy56SU9bszD7XkQUxdST_foUPhcaZ5E1j34rCwo-K2&s', 3),
('BK Veggie', 220, 79, 'Burgers', 'V', 'https://bk-apac-prd.s3.amazonaws.com/sites/burgerkingindia.in/files/BK-Veggie-Thumb_0.png', 3);


/*Baskin Robbins*/
INSERT INTO 
FOODS
(FoodName, FoodQty, FoodUnitPrice, FoodCategory, FoodType, FoodPhotoUrl, FoodMenuId) 
VALUES 
('Banana ''N'' Strawberries', 250, 119, 'Favourite', 'V', 'https://4.imimg.com/data4/BY/YN/GLADMIN-28700997/174-500x500.jpg', 4),
('Chocolate', 200, 119, 'Favourite', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRy5ZD7V_IcbJM69tVPwo_0Xi-B208KCnlX33kjWrsqh9TI6Mk&s', 4),
('Alphonso Gold', 450, 129, 'Divine', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR22dTs_lv4QVfkGxJguIfaw75tselzvLyv_SelvrTetxSdt_c&s', 4),
('Hop Scotch Butterscotch', 300, 158, 'Divine', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRradn-7g530LKnsMBI7D87A29kLeeFXWD4SvWNdXGdNDuisAo&s', 4),
('Multed Chocolate Fudge', 220, 178, 'Divine', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTZLEM3jpEIQZM6gmJ_l1_DUwJJY-MKFb9vgN7lbL8G5gE3-d1e&s', 4);


/*McDonalds*/
INSERT INTO 
FOODS
(FoodName, FoodQty, FoodUnitPrice, FoodCategory, FoodType, FoodPhotoUrl, FoodMenuId) 
VALUES 
('McSpicy', 250, 254, 'Spicy Meals', 'N', 'https://d1nqx6es26drid.cloudfront.net/app/uploads/2019/06/19203703/P-Mcspicy-set.png', 5),
('Maharaja Mac', 200, 185, 'Burger', 'V', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQXkWP0zTdij5EDjhr6LHTiClUkIpQnOEgle1XxwgM0M8fEo-Q&s', 5),
('Chicken McNuggets', 450, 123, 'Add Ons', 'N', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQk6NPqvMvhh7wER3jcIvpXO1UF17eD7hDdwQyH4LJAcf8VKls&s', 5),
('McFlurry Oreo', 300, 70, 'Desserts', 'V', 'https://www.mcdonalds.com/is/image/content/dam/usa/nfl/nutrition/items/hero/desktop/t-mcdonalds-McFlurry-with-OREO-Cookies-12floz-cup.jpg', 5),
('McVeggie', 220, 90, 'Burger', 'V', 'https://dynaimage.cdn.cnn.com/cnn/c_fill,g_auto,w_1200,h_675,ar_16:9/https%3A%2F%2Fcdn.cnn.com%2Fcnnnext%2Fdam%2Fassets%2F191206113141-mcdonalds-veggie-burger.jpg', 5);

/*Admins*/
ALTER TABLE ADMINS
ADD UNIQUE (AdminUsername, AdminPhone);

SELECT * FROM ADMINS;

INSERT INTO 
ADMINS 
(AdminUsername, AdminPhone, AdminFName, AdminLName, AdminPassword, AdminMenuId)
VALUES
('surajsahoo1', '7978753611', 'Suraj', 'Sahoo', 'password1', 1),
('surajsahoo2', '7978753612', 'Rahul', 'Sharma', 'password2', 2),
('surajsahoo3', '7978753613', 'Satya', 'Sahoo', 'password3', 3),
('surajsahoo4', '7978753614', 'Rohit', 'Verma', 'password4', 4),
('surajsahoo5', '7978753615', 'Simran', 'Dutta', 'password5', 5);

SELECT * FROM ORDERS;
SELECT * FROM OrderItems;

SELECT DISTINCT O.OrderId, O.OrderDate, O.OrderCustId, CONCAT(C.CustFName,' ', C.CustLName)
FROM ORDERS O 
INNER JOIN CUSTOMERS C 
ON O.OrderCustId = C.CustId
INNER JOIN ORDERITEMS OI 
ON OI.OrderItemOrderId = O.OrderId
INNER JOIN FOODS F
ON OI.OrderItemFoodId = F.FoodId
WHERE F.FoodMenuId = 1
ORDER BY 1 DESC;

SELECT * FROM Orders;

SELECT F.FoodName, OI.OrderItemQty, F.FoodUnitPrice, (OI.OrderItemQty * F.FoodUnitPrice) 
FROM ORDERS O INNER JOIN ORDERITEMS OI 
ON OI.OrderItemOrderId = O.OrderId INNER JOIN FOODS F 
ON OI.OrderItemFoodId = F.FoodId 
WHERE O.OrderId = 10007 AND F.FoodMenuId = 1 ORDER BY 3 DESC;