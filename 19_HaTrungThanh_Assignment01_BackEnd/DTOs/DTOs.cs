using System.ComponentModel.DataAnnotations;
using _19_HaTrungThanh_Assignment01_BackEnd.Models;

namespace _19_HaTrungThanh_Assignment01_BackEnd.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public short AccountID { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin", "Staff", "Lecturer"
        public int? RoleId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class AccountCreateUpdateDto
    {
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
        public string AccountEmail { get; set; } = string.Empty;

        [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer).")]
        public int? AccountRole { get; set; } = 1;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(70, ErrorMessage = "Password cannot exceed 70 characters.")]
        public string AccountPassword { get; set; } = string.Empty;
    }

    public class CategoryCreateUpdateDto
    {
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string CategoryDesciption { get; set; } = string.Empty;

        public short? ParentCategoryID { get; set; }

        public bool? IsActive { get; set; } = true;
    }

    public class NewsArticleCreateUpdateDto
    {
        [StringLength(20, ErrorMessage = "Article ID cannot exceed 20 characters.")]
        public string? NewsArticleID { get; set; }

        [StringLength(400, ErrorMessage = "Title cannot exceed 400 characters.")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required.")]
        [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters.")]
        public string Headline { get; set; } = string.Empty;

        public DateTime? CreatedDate { get; set; }

        [StringLength(4000, ErrorMessage = "Content cannot exceed 4000 characters.")]
        public string? NewsContent { get; set; }

        [StringLength(400, ErrorMessage = "Source cannot exceed 400 characters.")]
        public string? NewsSource { get; set; }

        public short? CategoryID { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class ReportStatisticDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalNews { get; set; }
        public List<NewsArticle> Articles { get; set; } = new List<NewsArticle>();
    }
}
