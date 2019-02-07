﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using ShoppingCartApp.Presentation;

namespace RazorPagesCart.Data.Models
{
    public class ShoppingCart
    {
        private ApplicationDbContext db;
        private IHttpContextAccessor context;
        private IServiceProvider serviceProvider;
        private ISession _session => context.HttpContext.Session;

        public ShoppingCart(ApplicationDbContext db, IHttpContextAccessor contextAccessor, IServiceProvider serviceProvider)
        {
            this.db = db;
            this.context = contextAccessor;
            this.serviceProvider = serviceProvider;
        }

        #region Constant Variables
        public const string CartSessionID = "CartID";
        #endregion

        #region Instance Variables
        public string ShoppingCartID { get; set; } 
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns Shopping Cart object with CartID set
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ShoppingCart GetCart()
        {
            var cart = (ShoppingCart)serviceProvider.GetService(typeof(ShoppingCart));
            cart.ShoppingCartID = cart.GetCartID();
            return cart;
        }

        /// <summary>
        /// Takes Controller object as argument, returns Cart object by calling GetCart method
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public ShoppingCart GetCart(Controller controller)
        {
            return GetCart();
        }
        #endregion

        #region MyRegion
        /// <summary>
        /// Returns Cart ID set username if logged in, else current session ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetCartID()
        {
            string sessionID = context.HttpContext.Session.Id;
            if (_session.GetString(CartSessionID) != null)
            {
                if (!string.IsNullOrWhiteSpace(context.HttpContext.User.Identity.Name))
                {
                    if (_session.GetString(CartSessionID) == sessionID)
                    {
                        _session.SetString(CartSessionID, context.HttpContext.User.Identity.Name);
                    }
                }
                else
                {
                    _session.SetString(CartSessionID, sessionID);
                    //context.Session[CartSessionID] = sessionID;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(context.HttpContext.User.Identity.Name))
                {
                    _session.SetString(CartSessionID, context.HttpContext.User.Identity.Name);
                    //context.Session[CartSessionID] = context.User.Identity.Name;
                }
                else
                {
                    _session.SetString(CartSessionID,sessionID);
                    //context.Session[CartSessionID] = sessionID;
                }
            }

            return _session.GetString(CartSessionID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        public void AddProductToCart(Product product, int quantity)
        {
            Cart cartItem = db.ShoppingCarts.SingleOrDefault(x => x.CartID == ShoppingCartID && x.ProductID == product.ProductID);

            if (cartItem == null)
            {
                cartItem = new Cart()
                {
                    ProductID = product.ProductID,
                    CartID = ShoppingCartID,
                    Quantity = quantity,
                    CreatedDate = DateTime.Now
                };

                db.ShoppingCarts.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                db.Entry(cartItem).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int ChangeProductQuantity(int id, int quantity)
        {
            Cart cartItem = db.ShoppingCarts.Single(x => x.ID == id);
            int itemCount = 0;

            if (cartItem != null)
            {
                itemCount = quantity;
                cartItem.Quantity = quantity;
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();
            }

            return itemCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int RemoveProductFromCart(int id)
        {
            Cart cartItem = db.ShoppingCarts.Single(x => x.ID == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                db.ShoppingCarts.Remove(cartItem);
                db.SaveChanges();
            }
            return itemCount;
        }

        /// <summary>
        /// Removes all carts for user
        /// </summary>
        public void EmptyCart()
        {
            List<Cart> cartItems = db.ShoppingCarts.Where(x => x.CartID == ShoppingCartID).ToList();

            foreach (Cart cartItem in cartItems)
            {
                db.ShoppingCarts.Remove(cartItem);
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Gets list of user carts
        /// </summary>
        /// <returns></returns>
        public List<Cart> GetCartProducts()
        {
            return db.ShoppingCarts.Include("Product").Where(x => x.CartID == ShoppingCartID).ToList();
        }

        /// <summary>
        /// Returns total number of products in shopping cart
        /// </summary>
        /// <returns></returns>
        public int GetQuantity()
        {
            int? quantity = db.ShoppingCarts.Where(x => x.CartID == ShoppingCartID).Select(x => (int?)x.Quantity ?? 0).DefaultIfEmpty().Sum();

            return quantity ?? 0;
        }

        /// <summary>
        /// Gets total cost of shopping cart, returns currency formatted string
        /// </summary>
        /// <returns></returns>
        public string GetCartTotal()
        {
            decimal? totalPrice = 0;
            var carts = db.ShoppingCarts.Include("Product").Where(x => x.CartID == ShoppingCartID);
            foreach (var item in carts)
            {
                if (item.Product.WildmanPrice != null)
                {
                    totalPrice += item.Product.WildmanPrice * item.Quantity;
                }
                else
                {
                    totalPrice += item.Product.Price * item.Quantity;
                }
            }

            //return StringFormatting.FormattedPrice(totalPrice ?? 0);
            return string.Empty;
        }

        /// <summary>
        /// Gets total cost of shopping cart, returns decimal
        /// </summary>
        /// <returns></returns>
        public decimal GetCartDecimalTotal()
        {
            decimal? totalPrice = 0;
            var carts = db.ShoppingCarts.Include("Product").Where(x => x.CartID == ShoppingCartID);
            foreach (var item in carts)
            {
                if (item.Product.WildmanPrice != null)
                {
                    totalPrice += item.Product.WildmanPrice * item.Quantity;
                }
                else
                {
                    totalPrice += item.Product.Price * item.Quantity;
                }
            }

            return totalPrice ?? 0;
        }

        /// <summary>
        /// Registers cart to username. Called when user signs in or registers
        /// </summary>
        /// <param name="userName"></param>
        public void RegisteredCart(string userName)
        {
            List<Cart> shoppingCarts = db.ShoppingCarts.Where(x => x.CartID == ShoppingCartID).ToList();

            foreach (Cart cart in shoppingCarts)
            {
                cart.CartID = userName;
                db.Entry(cart).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        #endregion
    }
}