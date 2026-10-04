using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Models
{
    [Table("NewsArticle")]
    public class NewsArticle
    {
        [Key]
        [StringLength(20)]
        public string NewsArticleID { get; set; } = string.Empty;

        [StringLength(400)]
        public string? NewsTitle { get; set; }

        [Required]
        [StringLength(150)]
        public string Headline { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; }

        [StringLength(4000)]
        public string? NewsContent { get; set; }

        [StringLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryID { get; set; }

        public bool? NewsStatus { get; set; } // 1: Active, 0: Inactive

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [ForeignKey(nameof(CategoryID))]
        public virtual Category? Category { get; set; }

        [ForeignKey(nameof(CreatedByID))]
        public virtual SystemAccount? CreatedBy { get; set; }

        public virtual ICollection<NewsTag>? NewsTags { get; set; }
    }
}
