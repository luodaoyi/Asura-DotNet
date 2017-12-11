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
            
            // 增加内存中的缓存
            services.AddMemoryCache();
            
            //添加MVC
            services.AddMvc();
            // 压缩
            services.AddResponseCompression();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseExceptionHandler("/Home/Error");
//            }
            //开发使用的错误页
            app.UseDeveloperExceptionPage();
            //开发使用的静态文件服务 正式部署建议去掉 使用nginx等直接访问 wwwroot目录
            app.UseStaticFiles();
            // metawblog 服务
            app.UseMetaWeblog("/api/xmlrpc");
            //错误页处理
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseMvcWithDefaultRoute();
            //压缩
            app.UseResponseCompression();


        }
    }
}