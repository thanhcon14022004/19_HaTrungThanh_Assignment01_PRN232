using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryID { get; set; }

        public bool? IsActive { get; set; }

        [ForeignKey(nameof(ParentCategoryID))]
        [JsonIgnore]
        public virtual Category? ParentCategory { get; set; }

        [JsonIgnore]
        public virtual ICollection<Category>? SubCategories { get; set; }

        [JsonIgnore]
        public virtual ICollection<NewsArticle>? NewsArticles { get; set; }
    }
}
