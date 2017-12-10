using System;
using System.Collections.Generic;
using Asura.Database;

namespace Asura.Models
{
    public class HomeViewModel
    {
        public List<Article> Articles { get; set; }
        public int Prev { get; set; }
        public int Next { get; set; }
    }

    public class ArticleViewModel
    {
        public Article Article { get; set; }
        public List<SerieViewModel> Series { get; set; }
        public List<Tag> Tags { get; set; }
        public ArticleSlugViewModel Prev { get; set; }
        public ArticleSlugViewModel Next { get; set; }
    }

    public class ArticleSlugViewModel
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class SerieViewModel
    {
        public List<ArticleSlugViewModel> Articles { get; set; }
        public int SerieId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }
}