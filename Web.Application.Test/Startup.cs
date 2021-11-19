using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.STS.Background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain.Configs;
using Web.Application.Infrastructure.Data;
using Web.Application.Infrastructure.Interfaces;

using Web.Application.Infrastructure.Repo;
using Web.Application.Infrastructure.Services;
using Web.Product.Api.Background;

namespace Web.Application.Test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services
            //    .Configure<KkbConfig>(Configuration.GetSection("KkbConfig"));
            services
               .Configure<KkbConfig>(Configuration.GetSection("KkbConfig"));
            var connStr = Configuration.GetConnectionString("LocalConnectionString");
            if (string.IsNullOrWhiteSpace(connStr))
                throw new ArgumentNullException(connStr);
            services.AddDbContext<KatanaDbContext>(options =>
            {
                options.UseNpgsql(connStr);
                options.EnableSensitiveDataLogging();
            });

            services.AddHttpClient<TaskService>();
            services.AddHttpClient<Order.Api.Services.StoreService>();
            services.AddHttpClient<TestData>();

            services.AddTransient<IManufacturerRepository, ManufacturerRepository>();
            services.AddTransient<IKatanaRepository, KatanaRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            //
            services.AddTransient<IStoreService, StoreService>();
            services.AddTransient<IKkbProtocolService, KkbProtocolService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
