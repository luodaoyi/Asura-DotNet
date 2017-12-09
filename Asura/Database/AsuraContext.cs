using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Asura.Comm;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Asura.Database
{
    public class AsuraContext : DbContext
    {
        public AsuraContext(DbContextOptions<AsuraContext> option) : base(option)
        {
        }

        /// <summary>
        /// 用户
        /// </summary>
        public DbSet<Account> Accounts { get; set; }

        /// <summary>
        /// 博客
        /// </summary>
        public DbSet<Blogger> Bloggers { get; set; }

        /// <summary>
        /// 专题系列
        /// </summary>
        public DbSet<Serie> Series { get; set; }

        /// <summary>
        /// 文章
        /// </summary>
        public DbSet<Article> Articles { get; set; }

        /// <summary>
        /// 归档
        /// </summary>
        public DbSet<Archive> Archives { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public DbSet<Tag> Tags { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Serie>().HasIndex(m => m.Name).IsUnique(true);
            modelBuilder.Entity<Serie>().HasIndex(m => m.Slug).IsUnique(true);

            modelBuilder.Entity<Article>().HasIndex(m => m.Slug).IsUnique(true);
        }
    }

    /// <summary>
    /// 文章
    /// </summary>
    public class Article
    {
        public int ArticleId { get; set; }

        /// <summary>
        /// 作者名
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文章名: how-to-get-girlfriend
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// markdown文档
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<Tag> Tags { get; set; }
        
        /// <summary>
        /// 专题ID
        /// </summary>
//        public int SerieId { get; set; }

        /// <summary>
        /// 是否是草稿
        /// </summary>
        public bool IsDraft { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 开始删除时间
        /// </summary>
        public DateTime DeleteTime { get; set; }

        /// <summary>
        /// Header
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 预览信息
        /// </summary>
        public string Excerpt { get; set; }

        /// <summary>
        /// 一句话描述，文章第一句
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// disqus thread
        /// </summary>
        public string Thread { get; set; }

        /// <summary>
        /// 用于排序的字段 越大越在前面
        /// </summary>
        public int SortFlag { get; set; }
    }

    /// <summary>
    /// 归档 档案
    /// </summary>
    public class Archive
    {
        public int ArchiveId { get; set; }
        public DateTime Time { get; set; }
        public List<Article> Articles { get; set; }
    }

    /// <summary>
    /// 专题
    /// </summary>
    public class Serie
    {
        public int SerieId { get; set; }

        /// <summary>
        /// 名称unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 缩略名
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 专题描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public List<Article> Articles { get; set; }

    }


    /// <summary>
    /// 博客实体
    /// </summary>
    public class Blogger
    {
        public int BloggerId { get; set; }

        /// <summary>
        /// 博客名
        /// </summary>
        public string BlogName { get; set; }

        /// <summary>
        /// SubTitle
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
        public string BeiAn { get; set; }

        /// <summary>
        /// 底部title
        /// </summary>
        public string BTitle { get; set; }

        /// <summary>
        /// 版权声明
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// 专题，倒序
        /// </summary>
        public string SeriesSay { get; set; }

        public List<Serie> Series { get; set; }

        /// <summary>
        /// 归档描述
        /// </summary>
        public string ArchivesSay { get; set; }

        public List<Archive> Archives { get; set; }
    }


    /// <summary>
    /// 用户账户
    /// </summary>
    public class Account
    {
        public int AccountId { get; set; }

        /// <summary>
        /// 账户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 账户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 二次验证token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneN { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 登出时间
        /// </summary>
        /// <returns></returns>
        public DateTime LogoutTime { get; set; }

        /// <summary>
        /// 最后登录ip
        /// </summary>
        public string LoginIp { get; set; }
    }

    /// <summary>
    /// 标签
    /// </summary>
    public class Tag
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
    }
}