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
            
            
            services.AddDbContext<AsuraContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));          
            
            //增加缓存
            services.AddResponseCaching();
            
            //添加配置options
            services.AddOptions();
            services.Configure<SiteConfig>(Configuration.GetSection("SiteConfig"));
            //增加weblog服务
            services.AddMetaWeblog<AsuraMetaWeblogService>();
            //添加MVC
            services.AddMvc();
            // 压缩
            services.AddResponseCompression();
           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //开发使用的错误页
                app.UseDeveloperExceptionPage();
                //开发使用的静态文件服务 正式部署建议去掉 使用nginx等直接访问 wwwroot目录
                app.UseStaticFiles();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //增加输出缓存
            app.UseResponseCaching();
            
            // metawblog 服务
            app.UseMetaWeblog("/api/xmlrpc");
            
            //错误页处理
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            //使用mvc并且配置默认的路由
            app.UseMvcWithDefaultRoute();
            //压缩
            app.UseResponseCompression();
            


        }
    }
}