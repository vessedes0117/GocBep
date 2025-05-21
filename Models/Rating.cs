using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GocBep.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá là bắt buộc.")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao.")]
        [Display(Name = "Số Sao")]
        public int Score { get; set; }

        // --- Mối quan hệ với Công thức (Recipe) ---
        [Required]
        public int RecipeId { get; set; } // Khóa ngoại

        [ForeignKey("RecipeId")]
        public virtual Recipe? Recipe { get; set; } // Thuộc tính điều hướng

        // --- Mối quan hệ với Người dùng (User) ---
        [Required]
        public string UserId { get; set; } // Khóa ngoại

        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; } // Thuộc tính điều hướng
    }
}