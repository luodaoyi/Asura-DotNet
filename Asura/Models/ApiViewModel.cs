using System.ComponentModel.DataAnnotations;

namespace Asura.Models
{
    public class DisqusPagrViewModel
    {
        public string Title { get; set; }
        public string ATitle { get; set; }
        public string Thread { get; set; }
        public string Slug { get; set; }
    }

    public class DisqusCreateForm
    {

        public string message { get; set; }

        public string author_email { get; set; }

        public string author_name { get; set; }

        public string thread { get; set; }

        public string identifier { get; set; }

        public int? parent { get; set; }
    }
}
