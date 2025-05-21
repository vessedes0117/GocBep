using GocBep.Data;
using GocBep.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GocBep.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                string sampleUserId = "81a2ddca-d1d3-4692-8073-ae2922f29308";

                var userExists = await context.Users.AnyAsync(u => u.Id == sampleUserId);
                if (!userExists)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi SeedData: Không tìm thấy User ID: {sampleUserId}. Công thức mẫu sẽ không được thêm.");
                    return;
                }


                if (context.Recipes.Any())
                {
                    return;
                }

                var recipes = new Recipe[]
                {
                    new Recipe
                    {
                        Name = "Gà Nướng Mật Ong",
                        Description = "Món gà nướng mật ong thơm lừng, da giòn, thịt mềm, đậm đà hương vị.",
                        Ingredients = "- 1 con gà (khoảng 1.2 - 1.5kg)\n- 3 muỗng canh mật ong\n- 2 muỗng canh nước tương\n- 1 muỗng canh dầu hào\n- 1 muỗng cà phê tỏi băm\n- 1/2 muỗng cà phê gừng băm\n- Gia vị: muối, tiêu",
                        Instructions = "1. Sơ chế gà: Gà làm sạch, để ráo nước. Có thể khứa nhẹ vài đường trên đùi và ức gà để gia vị thấm đều.\n2. Pha sốt ướp: Trộn đều mật ong, nước tương, dầu hào, tỏi băm, gừng băm, một ít muối và tiêu.\n3. Ướp gà: Thoa đều hỗn hợp sốt lên khắp mình gà, cả bên trong lẫn bên ngoài. Ướp ít nhất 30 phút, hoặc tốt nhất là 2-4 tiếng trong tủ lạnh.\n4. Nướng gà: Làm nóng lò nướng ở 200°C. Cho gà vào khay nướng, nướng khoảng 45-60 phút tùy kích thước gà. Cứ khoảng 15-20 phút, lấy gà ra phết thêm một lớp sốt ướp để gà không bị khô và có màu đẹp.\n5. Hoàn thành: Gà chín vàng đều, da giòn là được. Lấy gà ra, để nguội bớt rồi chặt miếng vừa ăn.",
                        ImageUrl = "/images/placeholder/ga_nuong_mat_ong.jpg",
                        PrepTime = 20,
                        CookTime = 60,
                        Servings = 4,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UserId = sampleUserId
                    },
                    new Recipe
                    {
                        Name = "Canh Chua Cá Lóc",
                        Description = "Món canh chua cá lóc đậm đà hương vị miền Tây, chua thanh, ngọt dịu, kích thích vị giác.",
                        Ingredients = "- 1 con cá lóc (khoảng 500-700g)\n- 1/4 trái thơm (dứa)\n- 2 trái cà chua\n- 100g đậu bắp\n- 1 vắt me nhỏ\n- Giá đỗ, rau ngổ, ngò gai\n- Tỏi, ớt, hành lá\n- Gia vị: nước mắm, đường, muối, bột ngọt",
                        Instructions = "1. Sơ chế: Cá lóc làm sạch, cắt khoanh vừa ăn, ướp với ít nước mắm, tiêu. Thơm gọt vỏ, bỏ mắt, cắt miếng. Cà chua cắt múi cau. Đậu bắp cắt xéo. Me dầm với nước nóng, lọc lấy nước cốt.\n2. Phi thơm tỏi, xào sơ cá cho săn lại rồi gắp ra.\n3. Cho nước vào nồi, đun sôi. Cho nước cốt me, thơm, cà chua vào nấu. Nêm nếm gia vị vừa ăn (chua, ngọt, mặn hài hòa).\n4. Khi nước sôi lại, cho cá và đậu bắp vào nấu chín.\n5. Tắt bếp, cho giá đỗ, rau ngổ, ngò gai, hành lá, vài lát ớt vào. Múc canh ra tô, dùng nóng với cơm.",
                        ImageUrl = "/images/placeholder/canh_chua_ca_loc.jpg",
                        PrepTime = 25,
                        CookTime = 30,
                        Servings = 4,
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UserId = sampleUserId
                    },
                    new Recipe
                    {
                        Name = "Sườn Xào Chua Ngọt",
                        Description = "Sườn xào chua ngọt với vị chua, ngọt, mặn hài hòa, màu sắc hấp dẫn, rất hao cơm.",
                        Ingredients = "- 500g sườn non\n- 1 củ hành tây nhỏ\n- 1/2 trái ớt chuông xanh (hoặc đỏ/vàng)\n- Tỏi băm, hành tím băm\n- Gia vị ướp sườn: nước mắm, đường, tiêu, hạt nêm\n- Sốt chua ngọt: 3 muỗng canh đường, 3 muỗng canh giấm, 2 muỗng canh nước mắm, 1 muỗng canh tương cà, 1 muỗng canh tương ớt (tùy chọn), 1/2 chén nước lọc.",
                        Instructions = "1. Sơ chế: Sườn non rửa sạch, chặt miếng vừa ăn. Trần sơ sườn qua nước sôi để loại bỏ tạp chất, rửa lại. Ướp sườn với gia vị trong khoảng 20-30 phút.\n2. Chiên sườn: Đun nóng dầu, cho sườn vào chiên vàng đều các mặt. Vớt sườn ra để ráo dầu.\n3. Pha sốt: Trộn đều các nguyên liệu của phần sốt chua ngọt.\n4. Xào: Phi thơm hành tỏi băm, cho hành tây, ớt chuông vào xào sơ. Cho sườn đã chiên vào đảo nhanh. Đổ hỗn hợp sốt chua ngọt vào, đảo đều cho sườn ngấm gia vị và sốt sánh lại.\n5. Hoàn thành: Nêm nếm lại cho vừa khẩu vị. Tắt bếp, gắp sườn ra đĩa.",
                        ImageUrl = "/images/placeholder/suon_xao_chua_ngot.jpg",
                        PrepTime = 20,
                        CookTime = 40,
                        Servings = 3,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UserId = sampleUserId
                    }
                };

                await context.Recipes.AddRangeAsync(recipes);
                await context.SaveChangesAsync();
            }
        }
    }
}