using Microsoft.EntityFrameworkCore;
using StartupsBack.Models.DbModels;

namespace StartupsBack.Database
{
    public class MainDb : DbContext
    {
        public DbSet<UserModel> UsersDB { get; set; }
        public DbSet<StartupModel> StartupsDB { get; set; }

        public MainDb(DbContextOptions<MainDb> options)
            : base(options)
        {
#if DEBUG
            Database.EnsureDeleted();
#endif
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StartupModel>()
                .HasOne(st => st.Author)
                .WithMany(user => user.PublishedStartups)
                .HasForeignKey(st => st.AuthorForeignKey)
                .HasPrincipalKey(user => user.Id);

            modelBuilder.Entity<StartupModel>()
                .HasMany(st => st.Contributors)
                .WithMany();

            modelBuilder.Entity<UserModel>()
                .HasMany(user => user.History)
                .WithMany();

            modelBuilder.Entity<UserModel>()
                .HasMany(user => user.Projects)
                .WithMany();
        }
    }
}
