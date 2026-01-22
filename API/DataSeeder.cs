using DAL;
using DTOs.Constants;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public static class DataSeeder
    {
        public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // -----------------------------------------------------------
            // 1. ĐẢM BẢO CÓ USER (Nếu chưa có ai thì tạo mới)
            // -----------------------------------------------------------
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User { FullName = "Admin System", Email = "Admin@Gmail.com", Role = UserRoles.Admin, PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456") },
                    new User { FullName = "Manager Boss", Email = "Manager@Gmail.com", Role = UserRoles.Manager, PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456") },
                    new User { FullName = "Staff Annotator", Email = "Staff@Gmail.com", Role = UserRoles.Annotator, PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456") }
                };
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // -----------------------------------------------------------
            // 2. LẤY ID CỦA MANAGER VÀ STAFF THỰC TẾ TRONG DB
            // (Fix lỗi Foreign Key Crash)
            // -----------------------------------------------------------
            var manager = await context.Users.FirstOrDefaultAsync(u => u.Role == UserRoles.Manager);
            var staff = await context.Users.FirstOrDefaultAsync(u => u.Role == UserRoles.Annotator);

            // Nếu lỡ DB cũ không có Manager/Staff thì tạo tạm để không lỗi
            if (manager == null)
            {
                manager = new User { FullName = "Fallback Manager", Email = "manager_new@test.com", Role = UserRoles.Manager, PasswordHash = "123456" };
                context.Users.Add(manager);
            }
            if (staff == null)
            {
                staff = new User { FullName = "Fallback Staff", Email = "staff_new@test.com", Role = UserRoles.Annotator, PasswordHash = "123456" };
                context.Users.Add(staff);
            }
            await context.SaveChangesAsync(); // Lưu để lấy ID thật

            // -----------------------------------------------------------
            // 3. TẠO PROJECT (Dùng ID thật vừa lấy)
            // -----------------------------------------------------------
            if (!await context.Projects.AnyAsync())
            {
                var project = new Project
                {
                    Name = "Dự án Phân loại Xe cộ (Demo)",
                    Description = "Dự án test Dashboard.",
                    ManagerId = manager.Id, // <--- DÙNG ID THẬT, KHÔNG HARD-CODE
                    PricePerLabel = 5000,
                    TotalBudget = 10000000,
                    Deadline = DateTime.UtcNow.AddDays(7),
                    CreatedDate = DateTime.UtcNow
                };

                project.LabelClasses.Add(new LabelClass { Name = "Car", Color = "#FF0000", GuideLine = "Xe con" });
                project.LabelClasses.Add(new LabelClass { Name = "Bike", Color = "#00FF00", GuideLine = "Xe máy" });

                for (int i = 1; i <= 5; i++)
                {
                    project.DataItems.Add(new DataItem { StorageUrl = "https://via.placeholder.com/150", Status = "New", UploadedDate = DateTime.UtcNow });
                }

                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();

                // -----------------------------------------------------------
                // 4. GIAO VIỆC (Dùng ID thật)
                // -----------------------------------------------------------
                var items = project.DataItems.ToList();
                if (items.Count >= 3)
                {
                    var assignments = new List<Assignment>
                    {
                        new Assignment { ProjectId = project.Id, DataItemId = items[0].Id, AnnotatorId = staff.Id, Status = "InProgress", AssignedDate = DateTime.UtcNow },
                        new Assignment { ProjectId = project.Id, DataItemId = items[1].Id, AnnotatorId = staff.Id, Status = "Submitted", AssignedDate = DateTime.UtcNow },
                        new Assignment { ProjectId = project.Id, DataItemId = items[2].Id, AnnotatorId = staff.Id, Status = "Rejected", AssignedDate = DateTime.UtcNow }
                    };

                    // Update trạng thái DataItem luôn cho đồng bộ
                    items[0].Status = "Assigned";
                    items[1].Status = "Assigned";
                    items[2].Status = "Assigned";

                    await context.Assignments.AddRangeAsync(assignments);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}