using System;
using System.Collections.Generic;
using System.Linq;
using Asura.Comm;
using Asura.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asura.Database
{
    public static class DbInitializer
    {
        public static void Initialize(AsuraContext context,SiteConfig siteConfig)
        {
            context.Database.EnsureCreated();
            
            if (!context.Users.Any())
            {
                context.Users.Add(new User()
                {
                    Name = "admin",
                    AuthCode = MD5Helper.MD5Hash("admin"),
                    Activated = DateTime.Now,
                    Loggeg = DateTime.Now,
                    Group = 1,
                    Created = DateTime.Now,
                    Mail = "admin@admin.com",
                    ScreenName="admin"
                });
            }
            if (!context.Metas.Any())
            {
                context.Metas.Add(new Meta()
                {
                    Slug="defaule",
                    Name = "默认分类",
                    Type = 1,
                    Description = "默认"
                });
            }
            context.SaveChanges();
        }   
    }
}