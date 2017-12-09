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
}