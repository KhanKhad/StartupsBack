using Microsoft.EntityFrameworkCore;
using StartupsBack.Models.DbModels;

namespace StartupsBack.Database
{
    public class MainDb : DbContext
    {
        public DbSet<UserModel> UsersDB { get; set; }
        public DbSet<StartupModel> StartupsDB { get; set; }
        public DbSet<MessageModel> MessagesDB { get; set; }

        public MainDb(DbContextOptions<MainDb> options)
            : base(options)
        {
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

            modelBuilder.Entity<MessageModel>()
                .HasOne(msg => msg.Sender)
                .WithMany(user => user.SendedMessages)
                .HasForeignKey(st => st.SenderForeignKey)
                .HasPrincipalKey(user => user.Id);

            modelBuilder.Entity<MessageModel>()
                .HasOne(msg => msg.Recipient)
                .WithMany(user => user.GettedMessages)
                .HasForeignKey(st => st.RecipientForeignKey)
                .HasPrincipalKey(user => user.Id);

            modelBuilder.Entity<UserModel>()
                .HasMany(user => user.History)
                .WithMany();

            modelBuilder.Entity<UserModel>()
                .HasMany(user => user.FavoriteStartups)
                .WithMany();

            modelBuilder.Entity<UserModel>()
                .HasMany(user => user.Projects)
                .WithMany();
        }
    }
}
