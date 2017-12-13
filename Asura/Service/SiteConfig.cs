using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Asura.Service
{
    /// <summary>
    /// 站点配置
    /// </summary>
    public class SiteConfig
    {
        /// <summary>
        /// # superfeedr url
        /// </summary>
        public string FeedrUrl { get; set; }

        /// <summary>
        /// 热词配置
        /// </summary>
        public List<string> HotWords { get; set; }

        /// <summary>
        /// ping rpcs 地址
        /// </summary>
        public List<string> PingRpCs { get; set; }

        /// <summary>
        /// 站点常规配置
        /// </summary>
        public General General { get; set; }

        /// <summary>
        /// 评论相关
        /// </summary>
        public DisqusConfig Disqus { get; set; }

        /// <summary>
        /// google analytice
        /// </summary>
        public GoogleAnalytics GoogleAnalytics { get; set; }

        /// <summary>
        /// 七牛相关配置
        /// </summary>
        public QiNiu QiNiu { get; set; }

        /// <summary>
        /// 管理员初始化配置
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// 博客细节配置
        /// </summary>
        public Blogger Blogger { get; set; }

        /// <summary>
        /// 禁止访问的黑名单
        /// </summary>
        public List<string> BlackIP { get; set; }
    }

    /// <summary>
    /// 常规配置
    /// </summary>
    public class General
    {
        /// <summary>
        /// 首页展示文章数量
        /// </summary>
        public int PageNum { get; set; }

        /// <summary>
        /// 管理界面
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 文章描述前缀
        /// </summary>
        public string DescpreFix { get; set; }

        /// <summary>
        /// 截取预览标识
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 自动截取预览, 字符数
        /// </summary>
        public int Length { get; set; }
    }

    /// <summary>
    /// 评论相关
    /// </summary>
    public class DisqusConfig
    {
        public string Shortname { get; set; }
        public string Publickey { get; set; }
        public string Accesstoken { get; set; }
        /// <summary>
        /// 国内不能访问Disqus 这里是用来在服务器上调用相关api的反向代理域名配置
        /// </summary>
        public string ApiDomain { get; set; }


        /// <summary>
        /// 获取评论数量间隔
        /// </summary>
        public int Interval { get; set; }
    }

    /// <summary>
    /// google 分析配置
    /// </summary>
    public class GoogleAnalytics
    {
        public string Url { get; set; }
        public string Tid { get; set; }
        public string V { get; set; }
        public string T { get; set; }
    }

    /// <summary>
    /// 七牛配置
    /// </summary>
    public class QiNiu
    {
        public string Bucket { get; set; }
        public string Domain { get; set; }
        public string Accesskey { get; set; }
        public string Secretkey { get; set; }
    }

    /// <summary>
    /// 初始化管理员
    /// </summary>
    public class Account
    {
        /// <summary>
        /// 后台登录用户名
        /// </summary>
        public string Username { get; set; }

        public string Password { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { set; get; }
        public string Address { get; set; }
    }

    /// <summary>
    /// 博客细节配置
    /// </summary>
    public class Blogger
    {
        public string Domain { get; set; }
        /// <summary>
        /// left显示名称
        /// </summary>
        public string BlogName { get; set; }

        /// <summary>
        /// 小标题
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
        public string Beian { get; set; }

        /// <summary>
        /// footer显示名称及tab标题
        /// </summary>
        public string Btitle { get; set; }

        /// <summary>
        /// 版权声明
        /// </summary>
        public string CopyRight { get; set; }
        /// <summary>
        /// 头像base64
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 主题页子标题内容
        /// </summary>
        public string SeriesSubTitle { get; set; }
        /// <summary>
        /// 归档页子标题内容
        /// </summary>
        public string ArchiveSubTitle { get; set; }
        /// <summary>
        /// 缓存过期时间
        /// </summary>
        public int CacheExpireSecont { get; set; }
    }
}