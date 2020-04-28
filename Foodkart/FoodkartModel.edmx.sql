
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/28/2020 01:16:28
-- Generated from EDMX file: C:\Users\John Doe\source\repos\Foodkart\Foodkart\FoodkartModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FoodKartDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AdminMenu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Admins] DROP CONSTRAINT [FK_AdminMenu];
GO
IF OBJECT_ID(N'[dbo].[FK_CartItemCart]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CartItems] DROP CONSTRAINT [FK_CartItemCart];
GO
IF OBJECT_ID(N'[dbo].[FK_FoodCartItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CartItems] DROP CONSTRAINT [FK_FoodCartItem];
GO
IF OBJECT_ID(N'[dbo].[FK_CustomerCart]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Carts] DROP CONSTRAINT [FK_CustomerCart];
GO
IF OBJECT_ID(N'[dbo].[FK_CustomerOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_CustomerOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_FoodOrderItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderItems] DROP CONSTRAINT [FK_FoodOrderItem];
GO
IF OBJECT_ID(N'[dbo].[FK_OrderOrderItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderItems] DROP CONSTRAINT [FK_OrderOrderItem];
GO
IF OBJECT_ID(N'[dbo].[FK_FoodMenu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Foods] DROP CONSTRAINT [FK_FoodMenu];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Admins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Admins];
GO
IF OBJECT_ID(N'[dbo].[CartItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CartItems];
GO
IF OBJECT_ID(N'[dbo].[Carts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Carts];
GO
IF OBJECT_ID(N'[dbo].[Customers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Customers];
GO
IF OBJECT_ID(N'[dbo].[Foods]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Foods];
GO
IF OBJECT_ID(N'[dbo].[Menus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Menus];
GO
IF OBJECT_ID(N'[dbo].[OrderItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrderItems];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Admins'
CREATE TABLE [dbo].[Admins] (
    [AdminId] bigint IDENTITY(1,1) NOT NULL,
    [AdminFName] nvarchar(20)  NOT NULL,
    [AdminLName] nvarchar(20)  NOT NULL,
    [AdminPassword] nvarchar(20)  NOT NULL,
    [AdminMenuId] bigint  NOT NULL
);
GO

-- Creating table 'CartItems'
CREATE TABLE [dbo].[CartItems] (
    [CartItemId] bigint IDENTITY(1,1) NOT NULL,
    [CartItemCartId] bigint  NOT NULL,
    [CartAddDate] datetime  NOT NULL,
    [CartItemQty] bigint  NOT NULL,
    [CartItemFoodId] bigint  NOT NULL
);
GO

-- Creating table 'Carts'
CREATE TABLE [dbo].[Carts] (
    [CartId] bigint IDENTITY(1,1) NOT NULL,
    [CartCustId] bigint  NOT NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [CustId] bigint IDENTITY(1,1) NOT NULL,
    [CustEmail] nvarchar(50)  NOT NULL,
    [CustPhone] nchar(10)  NOT NULL,
    [CustFName] nvarchar(20)  NOT NULL,
    [CustLName] nvarchar(20)  NOT NULL,
    [CustPassword] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'Foods'
CREATE TABLE [dbo].[Foods] (
    [FoodId] bigint IDENTITY(1,1) NOT NULL,
    [FoodName] nvarchar(40)  NOT NULL,
    [FoodQty] bigint  NOT NULL,
    [FoodUnitPrice] bigint  NOT NULL,
    [FoodCategory] nvarchar(30)  NOT NULL,
    [FoodType] nchar(1)  NOT NULL,
    [FoodPhotoUrl] nvarchar(max)  NULL,
    [FoodMenuId] bigint  NOT NULL
);
GO

-- Creating table 'Menus'
CREATE TABLE [dbo].[Menus] (
    [MenuId] bigint IDENTITY(1,1) NOT NULL,
    [MenuName] nvarchar(30)  NOT NULL,
    [MenuAvailable] bit  NOT NULL,
    [MenuAddDate] datetime  NOT NULL,
    [MenuModifyDate] datetime  NULL,
    [MenuLogoUrl] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'OrderItems'
CREATE TABLE [dbo].[OrderItems] (
    [OrderItemId] bigint IDENTITY(1,1) NOT NULL,
    [OrderItemOrderId] bigint  NOT NULL,
    [OrderItemFoodId] bigint  NOT NULL,
    [OrderItemQty] bigint  NOT NULL,
    [OrderItemUnitPrice] bigint  NOT NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [OrderId] bigint IDENTITY(1,1) NOT NULL,
    [OrderDate] datetime  NOT NULL,
    [OrderCustId] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AdminId] in table 'Admins'
ALTER TABLE [dbo].[Admins]
ADD CONSTRAINT [PK_Admins]
    PRIMARY KEY CLUSTERED ([AdminId] ASC);
GO

-- Creating primary key on [CartItemId] in table 'CartItems'
ALTER TABLE [dbo].[CartItems]
ADD CONSTRAINT [PK_CartItems]
    PRIMARY KEY CLUSTERED ([CartItemId] ASC);
GO

-- Creating primary key on [CartId] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [PK_Carts]
    PRIMARY KEY CLUSTERED ([CartId] ASC);
GO

-- Creating primary key on [CustId] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([CustId] ASC);
GO

-- Creating primary key on [FoodId] in table 'Foods'
ALTER TABLE [dbo].[Foods]
ADD CONSTRAINT [PK_Foods]
    PRIMARY KEY CLUSTERED ([FoodId] ASC);
GO

-- Creating primary key on [MenuId] in table 'Menus'
ALTER TABLE [dbo].[Menus]
ADD CONSTRAINT [PK_Menus]
    PRIMARY KEY CLUSTERED ([MenuId] ASC);
GO

-- Creating primary key on [OrderItemId] in table 'OrderItems'
ALTER TABLE [dbo].[OrderItems]
ADD CONSTRAINT [PK_OrderItems]
    PRIMARY KEY CLUSTERED ([OrderItemId] ASC);
GO

-- Creating primary key on [OrderId] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([OrderId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AdminMenuId] in table 'Admins'
ALTER TABLE [dbo].[Admins]
ADD CONSTRAINT [FK_AdminMenu]
    FOREIGN KEY ([AdminMenuId])
    REFERENCES [dbo].[Menus]
        ([MenuId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdminMenu'
CREATE INDEX [IX_FK_AdminMenu]
ON [dbo].[Admins]
    ([AdminMenuId]);
GO

-- Creating foreign key on [CartItemCartId] in table 'CartItems'
ALTER TABLE [dbo].[CartItems]
ADD CONSTRAINT [FK_CartItemCart]
    FOREIGN KEY ([CartItemCartId])
    REFERENCES [dbo].[Carts]
        ([CartId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CartItemCart'
CREATE INDEX [IX_FK_CartItemCart]
ON [dbo].[CartItems]
    ([CartItemCartId]);
GO

-- Creating foreign key on [CartItemFoodId] in table 'CartItems'
ALTER TABLE [dbo].[CartItems]
ADD CONSTRAINT [FK_FoodCartItem]
    FOREIGN KEY ([CartItemFoodId])
    REFERENCES [dbo].[Foods]
        ([FoodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FoodCartItem'
CREATE INDEX [IX_FK_FoodCartItem]
ON [dbo].[CartItems]
    ([CartItemFoodId]);
GO

-- Creating foreign key on [CartCustId] in table 'Carts'
ALTER TABLE [dbo].[Carts]
ADD CONSTRAINT [FK_CustomerCart]
    FOREIGN KEY ([CartCustId])
    REFERENCES [dbo].[Customers]
        ([CustId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerCart'
CREATE INDEX [IX_FK_CustomerCart]
ON [dbo].[Carts]
    ([CartCustId]);
GO

-- Creating foreign key on [OrderCustId] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_CustomerOrder]
    FOREIGN KEY ([OrderCustId])
    REFERENCES [dbo].[Customers]
        ([CustId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerOrder'
CREATE INDEX [IX_FK_CustomerOrder]
ON [dbo].[Orders]
    ([OrderCustId]);
GO

-- Creating foreign key on [OrderItemFoodId] in table 'OrderItems'
ALTER TABLE [dbo].[OrderItems]
ADD CONSTRAINT [FK_FoodOrderItem]
    FOREIGN KEY ([OrderItemFoodId])
    REFERENCES [dbo].[Foods]
        ([FoodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FoodOrderItem'
CREATE INDEX [IX_FK_FoodOrderItem]
ON [dbo].[OrderItems]
    ([OrderItemFoodId]);
GO

-- Creating foreign key on [OrderItemOrderId] in table 'OrderItems'
ALTER TABLE [dbo].[OrderItems]
ADD CONSTRAINT [FK_OrderOrderItem]
    FOREIGN KEY ([OrderItemOrderId])
    REFERENCES [dbo].[Orders]
        ([OrderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_OrderOrderItem'
CREATE INDEX [IX_FK_OrderOrderItem]
ON [dbo].[OrderItems]
    ([OrderItemOrderId]);
GO

-- Creating foreign key on [FoodMenuId] in table 'Foods'
ALTER TABLE [dbo].[Foods]
ADD CONSTRAINT [FK_FoodMenu]
    FOREIGN KEY ([FoodMenuId])
    REFERENCES [dbo].[Menus]
        ([MenuId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FoodMenu'
CREATE INDEX [IX_FK_FoodMenu]
ON [dbo].[Foods]
    ([FoodMenuId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------