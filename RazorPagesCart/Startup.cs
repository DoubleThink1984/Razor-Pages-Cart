using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPagesCart.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RazorPagesCart.Data.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.FileProviders;
using System.IO;
using RazorPagesCart.Hubs;

namespace RazorPagesCart
{
    public class Startup
    {
        public Startup(IConfiguration configuration,
            IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddTransient<ShoppingCart, ShoppingCart>();
            services.AddAuthorization(options => 
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options =>
                {
                    //options.Conventions.AddPageRoute("Products/Details", "Products/{Name}");
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                    options.Conventions.AuthorizeAreaFolder("Administrator", "/", "RequireAdministratorRole");
                })
                .AddSessionStateTempDataProvider();

            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddDistributedMemoryCache();

            services.AddDirectoryBrowser();

            if (!_env.IsDevelopment())
            {
                services.Configure<MvcOptions>(o => 
                    o.Filters.Add(new RequireHttpsAttribute()));
            }

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseDatabaseErrorPage();

            //    app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //    {
            //        FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            //        RequestPath = "/MyFiles"
            //    });
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    app.UseHsts();
            //}

            app.UseHttpsRedirection();

            var cachePeriod = env.IsDevelopment() ? "600" : "604800";
            app.UseStaticFiles(new StaticFileOptions
            {
                //FileProvider = new PhysicalFileProvider(
                //    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                //RequestPath = "/MyFiles",

                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, maz-age={cachePeriod}");
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                    RequestPath = "/MyFiles"
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add(
                        "Content-Security-Policy",
                        "script-src 'self'; " +
                        "style-src 'self'; " +
                        "img-src 'self'");

                    await next();
                });
            }
            //app.UseDirectoryBrowser(new DirectoryBrowserOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
            //    RequestPath = "/MyFiles"
            //});

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseCors(c => c.WithOrigins("https://bank.com"));

            app.UseSignalR(routes => 
            {
                routes.MapHub<ChatHub>("/chatHub");
            });

            app.UseMvc();

            var createUserTask = CreateUserRoles(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>());
            createUserTask.GetAwaiter().GetResult();
        }

        private async Task CreateUserRoles(IServiceScopeFactory scopeFactory)
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                //RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Seed database code goes here
                var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                IdentityResult roleResult;
                //Adding Admin Role
                var roleCheck = await RoleManager.RoleExistsAsync("Admin");
                if (!roleCheck)
                {
                    //create the roles and seed them to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
                }
                //Assign Admin role to the main User here we have given our newly registered 
                //login id for Admin management
                var user = await UserManager.FindByEmailAsync("administrator@catch.com");
                if (user == null)
                {
                    user = new IdentityUser { UserName = "administrator@catch.com", Email = "administrator@catch.com" };
                    var result = await UserManager.CreateAsync(user, "P@ssw0rd");
                }
                await UserManager.AddToRoleAsync(user, "Admin");
            }            
        }
    }

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }        
}
