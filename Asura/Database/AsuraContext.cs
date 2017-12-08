using System;
using System.Collections.Generic;
using System.Linq;
using Asura.Comm;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asura.Database
{
    
    public class AsuraContext : DbContext
    {
        public AsuraContext(DbContextOptions<AsuraContext> option):base(option)
        {
            
        }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<Jump> Jumps { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>().HasIndex(m => m.Slug).IsUnique(true);
            modelBuilder.Entity<Content>().HasIndex(m => m.Title).IsUnique(true);

            modelBuilder.Entity<Jump>().HasIndex(m => m.Slug).IsUnique(true);
        }
    }

    /// <summary>
    /// 内容
    /// </summary>
    public class Content
    {
        public int ContentId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }

        public string Text { get; set; }
        public int Order { get; set; }

        /// <summary>
        /// 类型 1博客博文 2单页
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 状态 0草稿 1发布
        /// </summary>
        public int Status { get; set; }

        public string Password { get; set; }
        public bool AllowComment { get; set; }
        public User Author { get; set; }
        
    }

    public class Meta
    {
        public int MetaId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        /// <summary>
        /// 类型 1分类 2标签
        /// </summary>
        public int Type { get; set; }

        public string Description { get; set; }
        public int Order { get; set; }
        public int Parent { get; set; }
        public List<Content> Contents { get; set; }
    }

    public class Jump
    {
        public int JumpId { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string ScreenName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Activated { get; set; }
        public DateTime Loggeg { get; set; }

        /// <summary>
        /// 用户组 0普通用户 1管理员
        /// </summary>
        public int Group { get; set; }

        public string AuthCode { get; set; }
    }
}