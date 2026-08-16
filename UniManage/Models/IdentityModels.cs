using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniManage.Models;

namespace UniManage.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // කලින් දුන්න Cascade Delete Fix එක මෙතනට දාන්න
            modelBuilder.Entity<UserMessage>()
                .HasRequired(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserMessage>()
                .HasRequired(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);

            // Prerequisite ටේබල් එකේ කැස්කේඩ් ඩිලීට් එක නවත්වමු
            modelBuilder.Entity<Prerequisite>()
                .HasRequired(p => p.Course)
                .WithMany()
                .HasForeignKey(p => p.CourseId)
                .WillCascadeOnDelete(false); // මෙන්න මේකයි වැදගත්

            modelBuilder.Entity<Prerequisite>()
                .HasRequired(p => p.PrerequisiteCourse)
                .WithMany()
                .HasForeignKey(p => p.PrerequisiteCourseId)
                .WillCascadeOnDelete(false);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Prerequisite> Prerequisites { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
    }
    public class DbInitializer
    {
        public static void SeedRoles(ApplicationDbContext context)
        {
            // Roles ටික Database එකේ නැත්නම් විතරක් ඇතුළත් කරයි
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var store = new Microsoft.AspNet.Identity.EntityFramework.RoleStore<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(context);
                var manager = new Microsoft.AspNet.Identity.RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(store);

                manager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Admin"));
                manager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Lecturer"));
                manager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole("Student"));
            }
        }
    }
}