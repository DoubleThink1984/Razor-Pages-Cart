//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using RazorPagesCart.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace RazorPagesCart
//{
//    public class InitializeAdimn
//    {
//        #region Private Fields
//        private ApplicationDbContext context;
//        #endregion

//        #region Constructor
//        public InitializeAdimn(ApplicationDbContext db)
//        {
//            context = db;
//            InitializeIdentityAdmin(context);
//        }
//        #endregion

//        #region Private Methods
//        /// <summary>
//        /// Creates Admin Role and Administrator account if they don't exist.
//        /// </summary>
//        /// <param name="context"></param>
//        private void InitializeIdentityAdmin(ApplicationDbContext context)
//        {
//            var UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));
//            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
//            string name = "Admin";
//            string password = "P@ssw0rd";
//            string email = "administrator@catch.com";

//            //Create Role Admin if it does not exist
//            if (!RoleManager.RoleExists(name))
//            {
//                var roleresult = RoleManager.Create(new IdentityRole(name));
//            }

//            //Create User=Admin with password
//            if (!UserManager.Users.Any(u => u.UserName == email))
//            {
//                var user = new ApplicationUser();
//                user.UserName = email;
//                user.Email = email;
//                user.LockoutEnabled = true;
//                var adminresult = UserManager.Create(user, password);

//                if (adminresult.Succeeded)
//                {
//                    //Add User Admin to Role Admin
//                    var result = UserManager.AddToRole(user.Id, name);
//                }
//            }
//        }
//        #endregion
//    }
//}
