using System.ComponentModel.DataAnnotations;

namespace _19_HaTrungThanh_Assignment01_FrontEnd.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseModel
    {
        public short AccountID { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AccountViewModel
    {
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [StringLength(70)]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        public int? AccountRole { get; set; } = 1; // 1: Staff, 2: Lecturer

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(70)]
        public string AccountPassword { get; set; } = string.Empty;
    }

    public class CategoryViewModel
    {
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chuyên mục.")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mô tả chuyên mục.")]
        [StringLength(250)]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryID { get; set; }

        public bool? IsActive { get; set; } = true;

        public CategoryViewModel? ParentCategory { get; set; }
    }

    public class TagViewModel
    {
        public int TagID { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

    public class NewsTagViewModel
    {
        public string NewsArticleID { get; set; } = string.Empty;
        public int TagID { get; set; }
        public TagViewModel? Tag { get; set; }
    }

    public class NewsArticleViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã bài viết.")]
        [StringLength(20)]
        public string NewsArticleID { get; set; } = string.Empty;

        [StringLength(400)]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tóm tắt (Headline).")]
        [StringLength(150)]
        public string Headline { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; }

        [StringLength(4000)]
        public string? NewsContent { get; set; }

        [StringLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryID { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public CategoryViewModel? Category { get; set; }

        public AccountViewModel? CreatedBy { get; set; }

        public List<NewsTagViewModel>? NewsTags { get; set; }

        public List<int> SelectedTagIds { get; set; } = new List<int>();
    }

    public class ReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalNews { get; set; }
        public List<NewsArticleViewModel> Articles { get; set; } = new List<NewsArticleViewModel>();
    }
}
