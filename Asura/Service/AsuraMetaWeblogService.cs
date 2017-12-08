using System.Collections.Generic;
using System.Linq;
using Asura.Comm;
using Asura.Database;
using WilderMinds.MetaWeblog;

namespace Asura.Service
{
    public class AsuraMetaWeblogService: IMetaWeblogProvider
    {
        /// <summary>
        /// 允许操作
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private bool AllowExcute(string username, string password)
        {
            return _dbContext.Users
                       .Count(u => (u.Name == username && MD5Helper.MD5Hash(password) == u.AuthCode)) == 1;
        }
        private readonly AsuraContext _dbContext;

        public AsuraMetaWeblogService(AsuraContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public UserInfo GetUserInfo(string key, string username, string password)
        {
            // TODO
            return null;
        }

        public BlogInfo[] GetUsersBlogs(string key, string username, string password)
        {
            if (!AllowExcute(username, password))
            {
                return new BlogInfo[0];
            }

            var blogs = new List<BlogInfo>();
            blogs.Add(new BlogInfo()
            {
                blogid = "1",
                blogName = "我的笔记",
                url = "https://luodaoyi.com"
                
            });
            return blogs.ToArray();
        }


        public Post GetPost(string postid, string username, string password)
        {
            // TODO
            return null;
        }

        public Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            // TODO
            return null;
        }


        public string AddPost(string blogid, string username, string password, Post post, bool publish)
        {
            if (AllowExcute(username, password))
            {
                return "认证失败";
            }
            return null;
        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            // TODO
            return false;
        }

        public bool EditPost(string postid, string username, string password, Post post, bool publish)
        {
            // TODO
            return false;
        }


        public CategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            if (!AllowExcute(username, password))
            {
                return new CategoryInfo[0];
            }
            var categorys = _dbContext.Metas.Where(w => w.Type == 1).ToList();
            var categoryInfos=new List<CategoryInfo>();
            foreach (var meta in categorys)
            {
                var categoryObj = new CategoryInfo()
                {
                    categoryid = meta.MetaId.ToString(),
                    description = meta.Description,
                    htmlUrl = $"/category/{meta.Slug}.html",
                    rssUrl = $"/rss.xml?category={meta.MetaId}",
                    title = meta.Name
                };
                categoryInfos.Add(categoryObj);
            }
            return categoryInfos.ToArray();
        }

        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            // TODO
            return null;
        }

        public int AddCategory(string key, string username, string password, NewCategory category)
        {
            // TODO
            return 0;
        }
    }
}