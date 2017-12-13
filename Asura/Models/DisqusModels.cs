using System.Collections.Generic;
using Newtonsoft.Json;

namespace Asura.Models
{
//    public class DisqusPostListResponse
//    {
//        public DisqusCursor Cursor { get; set; }
//        public int Code { get; set; }
//        public List<DisqusPostDetail> Response { get; set; }
//    }
//
//
//    public class DisqusPostDetail
//    {
//        public string Id { get; set; }
//        public int Parent { get; set; }
//        public string CreatedAt { get; set; }
//        public string Message { get; set; }
//        public bool IsDeleted { get; set; }
//        public string Thread { get; set; }
//        public DisqusAuthor Author { get; set; }
//    }
//
//    public class DisqusCursor
//    {
//        public bool HasNext { get; set; }
//        public string Next { get; set; }
//    }
//
//    public class DisqusAuthor
//    {
//        public string Name { get; set; }
//        public string ProfileUrl { get; set; }
//        public DisqusAvatar Avatar { get; set; }
//    }
//
//    public class DisqusAvatar
//    {
//        public string Cache { get; set; }
//    }

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
}