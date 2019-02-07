using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesCart.Infrastructure
{
    public class ModelLists
    {
        #region Private Fields
        private static ApplicationDbContext _db;
        private static List<Category> categories;
        private static List<Category> rootCategories;
        private static List<Product> products;
        private static List<ProductImages> images;
        private static ServiceCollection _services;
        #endregion

        #region Constructors

        private ModelLists()
        {
            _services = new ServiceCollection();
            _db = _services.BuildServiceProvider().GetService<ApplicationDbContext>();
            categories = PopulateChildrenCategories(GetCategories());
            rootCategories = GetRootCategories(categories);
            products = GetProducts();
            images = GetProductImages();
        }
        #endregion

        #region Properties
        // Returns list of Categories ordered alphabetically
        public static List<Category> Categories
        {
            get
            {
                if (categories == null)
                {
                    new ModelLists();
                    //categories = new List<Category>();
                }
                return categories;
            }
        }

        // Returns list of root categories
        public static List<Category> RootCategories
        {
            get
            {
                if (categories == null)
                {
                    new ModelLists();
                    //rootCategories = new List<Category>();
                }
                return rootCategories;
            }
        }

        public static List<Product> Products
        {
            get
            {
                if (products == null)
                {
                    new ModelLists();
                    //products = new List<Product>();
                }
                return products;
            }
        }

        public static List<ProductImages> ProductImages
        {
            get
            {
                if (images == null)
                {
                    new ModelLists();
                    //images = new List<ProductImages>();
                }
                return images;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Populates Category object's list of child categories
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Category> PopulateChildrenCategories(List<Category> list)
        {
            foreach (Category cat in list)
            {
                cat.Subcategories = list.Where(subCat => subCat.ParentClassID == cat.ClassId).ToList();
            }
            return list;
        }

        // Gets list of categories from database and sorts them alphabetically by class name
        private List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();
            categories = _db.MenuItems.Include(x => x.Products).OrderBy(x => x.ClassName).ToList();

            return categories;
        }

        /// <summary>
        /// Returns list of root categories
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Category> GetRootCategories(List<Category> list)
        {
            int? nulls = null;
            List<Category> roots = new List<Category>();
            roots = list.Where(cat => cat.ParentClassID == nulls).ToList();

            return roots;
        }

        private List<Product> GetProducts()
        {
            List<Product> productsList = new List<Product>();
            productsList = _db.Products.Include(x => x.ProductImages).OrderBy(x => x.ProductName).ToList();

            return productsList;
        }

        // Returns list of product images.
        private List<ProductImages> GetProductImages()
        {
            List<ProductImages> productsImagesList = new List<ProductImages>();
            productsImagesList = _db.ProductImages.OrderBy(x => x.ProductID).ToList();

            return productsImagesList;
        }
        #endregion

        #region Public Methods
        public static void NewModelList()
        {
            new ModelLists();
        }
        #endregion
    }
}
