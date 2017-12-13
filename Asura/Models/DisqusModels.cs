using System.Collections.Generic;
using Newtonsoft.Json;

namespace Asura.Models
{
    public class DisqusComments
    {
        [JsonProperty("errno")]
        public int ErrNo { get; set; }
        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }
        [JsonProperty("data")]
        public DisqusCommentsData Data { get; set; }
    }

    public class DisqusCommentsData
    {
        [JsonProperty("next")]
        public string Next { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("comments")]
        public List<DisqusCommentsDetail> Comments { get; set; }
        [JsonProperty("thread")]
        public string Thread { get; set; }
    }

    public class DisqusCommentsDetail
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("parent")]
        public long Parent { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("createdAtStr")]
        public string CreatedAtStr { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }
    }


    public class PostCreate
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        //Message string `json:"message"`
        //Parent string `json:"parent"`
        //Thread string `json:"thread"`
        //AuthorEmail string `json:"author_email"`
        //AuthorName string `json:"autor_name"`
        //IpAddress string `json:"ip_address"`
        //Identifier string `json:"identifier"`
        //UserAgent string `json:"user_agent"`
    }
}