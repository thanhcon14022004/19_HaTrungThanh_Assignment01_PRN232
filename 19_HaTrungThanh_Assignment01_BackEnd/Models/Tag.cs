using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _19_HaTrungThanh_Assignment01_BackEnd.Models
{
    [Table("Tag")]
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TagID { get; set; }

        [StringLength(50)]
        public string? TagName { get; set; }

        [StringLength(400)]
        public string? Note { get; set; }

        [JsonIgnore]
        public virtual ICollection<NewsTag>? NewsTags { get; set; }
    }
}
