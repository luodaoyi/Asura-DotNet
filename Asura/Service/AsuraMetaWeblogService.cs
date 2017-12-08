using System.Collections.Generic;
using System.Linq;
using Asura.Comm;
using Asura.Database;
using WilderMinds.MetaWeblog;

namespace Asura.Service
{
    public class AsuraMetaWeblogService: IMetaWeblogProvider
    {
        
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
            // TODO
            return null;
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
            // TODO
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
            // TODO
            return null;
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