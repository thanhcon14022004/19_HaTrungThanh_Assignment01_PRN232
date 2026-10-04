using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Models
{
    [Table("SystemAccount")]
    public class SystemAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short AccountID { get; set; }

        [StringLength(100)]
        public string? AccountName { get; set; }

        [StringLength(70)]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; } // 1: Staff, 2: Lecturer

        [StringLength(70)]
        public string? AccountPassword { get; set; }

        [JsonIgnore]
        public virtual ICollection<NewsArticle>? CreatedArticles { get; set; }
    }
}
