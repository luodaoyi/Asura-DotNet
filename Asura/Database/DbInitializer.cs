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
            
            if (!context.Accounts.Any())
            {
                context.Accounts.Add(new Account()
                {
                    Username = siteConfig.Account.Username,
                    Address = siteConfig.Account.Address,
                    CreateTime = DateTime.Now,
                    Email = siteConfig.Account.Mail,
                    LoginIp = "127.0.0.1",
                    LoginTime = DateTime.Now,
                    LogoutTime = DateTime.Now,
                    Password =MD5Helper.MD5Hash(siteConfig.Account.Password),
                    PhoneN = siteConfig.Account.PhoneNumber,
                    Token = string.Empty
                });
            }
            if (!context.Bloggers.Any())
            {
                context.Bloggers.Add(new Blogger()
                {
                   BlogName = siteConfig.Blogger.BlogName,
                    BeiAn = siteConfig.Blogger.Beian,
                    BTitle = siteConfig.Blogger.Btitle,
                    Copyright = siteConfig.Blogger.CopyRight,
                    SubTitle = siteConfig.Blogger.SubTitle,
                    
                });
            }
            context.SaveChanges();
        }   
    }
}