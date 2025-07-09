using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TodoApi.Models.Task;
using TodoApi.Models.Team;
using TodoApi.Models.User;

namespace TodoApi.Data
{

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamAdmin> TeamAdmins { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }


        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<ApplicationUser>().ToTable("Users");
            mb.Entity<IdentityUserClaim<string>>().ToTable("UserClaims").HasKey(ur => new { ur.Id });
            mb.Entity<IdentityUserLogin<string>>().ToTable("UserLogins").HasKey(ur => new { ur.LoginProvider, ur.ProviderKey });
            mb.Entity<IdentityUserToken<string>>().ToTable("UserTokens").HasKey(ur => new { ur.UserId, ur.LoginProvider, ur.Name });

            // TEAM
            mb.Entity<Team>().ToTable("Teams");
            mb.Entity<Team>().Navigation(t => t.Members);
            mb.Entity<Team>().Navigation(t => t.Admins);

            mb.Entity<TeamInviteLink>().ToTable("TeamInviteLinks");
            mb.Entity<TeamInviteLink>()
                .Property(e => e.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<ETeamInviteLinkType>(v));


            // TEAM ADMIN
            mb.Entity<TeamAdmin>().HasKey(t => new { t.TeamId, t.UserId });
            mb.Entity<TeamAdmin>()
                .HasOne(ta => ta.Team)
                .WithMany(t => t.Admins)
                .HasForeignKey(ta => ta.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            mb.Entity<TeamAdmin>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.AdminOf)
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // TEAM MEMBER
            mb.Entity<TeamMember>().HasKey(tm => new { tm.UserId, tm.TeamId });
            mb.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(t => t.MemberOf)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }


    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        AppDbContext IDesignTimeDbContextFactory<AppDbContext>.CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }

}