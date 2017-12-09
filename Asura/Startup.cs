using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asura.Database;
using Asura.Service;
using WilderMinds.MetaWeblog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Asura
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
            services.AddMetaWeblog<AsuraMetaWeblogService>();
            services.AddDbContext<AsuraContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            
            //添加options
            services.AddOptions();
            services.Configure<SiteConfig>(Configuration.GetSection("SiteConfig"));
            //添加MVC
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseMetaWeblog("/api/xmlrpc");

            
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
            
        }
    }
}