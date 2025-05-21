using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GocBep.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên món ăn là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên món ăn không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên Món Ăn")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô Tả Ngắn")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Nguyên liệu là bắt buộc.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Nguyên Liệu")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Hướng dẫn là bắt buộc.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Hướng Dẫn Thực Hiện")]
        public string Instructions { get; set; }

        [Display(Name = "Đường Dẫn Ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Thời gian chuẩn bị (phút)")]
        [Range(0, int.MaxValue, ErrorMessage = "Thời gian chuẩn bị phải là số không âm.")]
        public int? PrepTime { get; set; }

        [Display(Name = "Thời gian nấu (phút)")]
        [Range(0, int.MaxValue, ErrorMessage = "Thời gian nấu phải là số không âm.")]
        public int? CookTime { get; set; }

        [Display(Name = "Khẩu Phần (người)")]
        [Range(1, int.MaxValue, ErrorMessage = "Khẩu phần phải ít nhất là 1.")]
        public int? Servings { get; set; }

        [Display(Name = "Ngày Đăng")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; }

        public virtual ICollection<Rating>? Ratings { get; set; }

        public virtual ICollection<UserFavoriteRecipe>? FavoritedByUsers { get; set; }

        [NotMapped]
        [Display(Name = "Đánh Giá Trung Bình")]
        public double AverageRating { get; set; }
    }
}
