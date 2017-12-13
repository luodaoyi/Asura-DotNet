using System;
using System.Collections.Generic;
using Asura.Database;

namespace Asura.Models
{
    public class VewModelBase
    {
        public Service.SiteConfig SiteConfig { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CurrentPage { get; set; }
    }

    public class HomeViewModel : VewModelBase
    {
        public List<Article> Articles { get; set; }
        public int Prev { get; set; }
        public int Next { get; set; }
    }

    public class ArticleSlugViewModel
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class ArticleViewModel : VewModelBase
    {
        public Article Article { get; set; }
        public List<SerieViewModelBase> Series { get; set; }
        public List<Tag> Tags { get; set; }
        public ArticleSlugViewModel Prev { get; set; }
        public ArticleSlugViewModel Next { get; set; }
    }

    public class ArchivesViewModel: VewModelBase
    {
        public List<ArchivesViewModelBase> List { get; set; }
        public string ArchiveSubTitle { get; set; }

    }

    public class ArchivesViewModelBase
    {
        public int Year { get; set; }
        public int Mouth { get; set; }
        public List<ArticleSlugViewModel> Articles { get; set; }
    }

    public class SerieViewModel : VewModelBase
    {
        public List<SerieViewModelBase> List { get; set; }
        public string SeriesSubTitle { get; set; }
    }
    public class SerieViewModelBase
    {
        public List<ArticleSlugViewModel> Articles { get; set; }
        public int SerieId { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
    }
}