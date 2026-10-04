using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Models
{
    [Table("NewsTag")]
    public class NewsTag
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        public string NewsArticleID { get; set; } = string.Empty;

        [Key, Column(Order = 1)]
        public int TagID { get; set; }

        [ForeignKey(nameof(NewsArticleID))]
        [JsonIgnore]
        public virtual NewsArticle? NewsArticle { get; set; }

        [ForeignKey(nameof(TagID))]
        public virtual Tag? Tag { get; set; }
    }
}
