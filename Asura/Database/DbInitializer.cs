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
        public static void Initialize(AsuraContext context)
        {
            context.Database.EnsureCreated();
            if(context.Articles.Any())
                return;
            var account = new Account()
            {
                Username = "admin",
                Address = "hahah",
                CreateTime = DateTime.Now,
                Email = "luodaoyi@gmail.com",
                LoginIp = "127.0.0.1",
                LoginTime = DateTime.Now,
                LogoutTime = DateTime.Now,
                Password = MD5Helper.MD5Hash("admin"),
                PhoneN = "13888888888",
                Token = string.Empty
            };
            context.Accounts.Add(account);
            
            var blogger = new Blogger()
            {
                BlogName = "瞎几把写",
                BeiAn = "浙备 adadwawd",
                BTitle = "记事本",
                Copyright = "hahah",
                SubTitle = "1111",
                
            };
            context.Bloggers.Add(blogger);


            var tag = new Tag()
            {
                TagName = "默认"
            };
            context.Tags.Add(tag);
            var article = new Article()
            {
                
                Tags = context.Tags.ToList(),
                Title = "Hello World",
                Slug = "hello-world",
                Author = "luodaoyi",
                Content = "# Hello World \n ## 你好世界！\n 如果你看到这条信息说明就好了",
                Count = 10,
                CreateTime = DateTime.Now,
                DeleteTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                SortFlag = 1,
                Desc = "瞎搞",
                IsDraft = false,
                Excerpt = "本片文章用来记录我在Golang开发学习过程中遇到的有关error的一些坑。或许你也遇到，或许你能在这里找到答案。当然通过error的例子，你也应该联想到其它场景。"
            };
            context.Articles.Add(article);
            
            context.SaveChanges();
        }
    }
}