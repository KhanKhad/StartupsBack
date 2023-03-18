using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StartupsBack.Database
{
    public class MainDb : DbContext
    {
        public DbSet<User> UsersDB { get; set; }
        public DbSet<Startup> StartupsDB { get; set; }

        public MainDb(DbContextOptions<MainDb> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Startup>()
                .HasOne(st => st.Author)
                .WithMany(user => user.PublishedStartups)
                .HasForeignKey(st => st.AuthorForeignKey);

            modelBuilder.Entity<User>()
                .HasMany(user => user.History)
                .WithMany();
        }
    }
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserTypes UserType { get; set; } = UserTypes.Guest;
        public List<Startup> PublishedStartups { get; set; } = new List<Startup>();
        public List<Startup> History { get; set; } = new List<Startup>();
    }
    public class Startup
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int AuthorForeignKey { get; set; }
        
        [JsonIgnore]
        public User Author { get; set; }
    }
    public enum UserTypes
    {
        Guest,
        Creator,
        Developer,
    }
}
