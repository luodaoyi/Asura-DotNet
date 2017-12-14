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


    public class DisqusPostCreate
    {
        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }
        [JsonProperty("errno")]
        public int ErrNo { get; set; }
        [JsonProperty("data")]
        public DisqusCommentsDetail Data { get; set; }

    }

    public class DiqsusPostDetail
    {

    }


    public class DisqusPostCreateResponse
    {
        public int Code { get; set; }
        public DiqsusPostDetail Response { get; set; }
    }
}