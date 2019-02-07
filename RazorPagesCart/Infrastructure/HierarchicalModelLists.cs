using Microsoft.Extensions.DependencyInjection;
using RazorPagesCart.Data;
using RazorPagesCart.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesCart.Infrastructure
{
    public class HierarchicalModelLists
    {
        #region Private Fields
        private static List<Category> categories;
        private static List<Category> hierarchicalCategories;
        //private ServiceCollection _services;
        #endregion

        #region Constructors
        public HierarchicalModelLists()
        {
            //this._services = new ServiceCollection();
            categories = ModelLists.RootCategories;
            hierarchicalCategories = new List<Category>();
            CreateMenu(categories);
        }
        #endregion

        #region Properties
        // Returns hierarchical list of Categories
        public static List<Category> Categories
        {
            get
            {
                if (hierarchicalCategories == null)
                {
                    new HierarchicalModelLists();
                }

                return hierarchicalCategories;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sorts categories hierarchically
        /// </summary>
        /// <param name="categories"></param>
        private static void CreateMenu(List<Category> categories)
        {
            foreach (Category cat in categories)
            {
                hierarchicalCategories.Add(cat);
                if (cat.Subcategories.Count > 0)
                {
                    CreateMenu(cat.Subcategories);
                }
            }
        }
        #endregion

        #region Public Methods
        public static void NewHierarchicalModelLists()
        {
            new HierarchicalModelLists();
        }
        #endregion
    }
}
