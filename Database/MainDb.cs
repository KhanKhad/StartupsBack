using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

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
                .HasForeignKey(st => st.AuthorForeignKey)
                .HasPrincipalKey(user => user.Id);

            modelBuilder.Entity<Startup>()
                .HasMany(st => st.Contributors)
                .WithMany();

            modelBuilder.Entity<User>()
                .HasMany(user => user.History)
                .WithMany();

            modelBuilder.Entity<User>()
                .HasMany(user => user.Projects)
                .WithMany();
        }
    }

    [Index(nameof(Name), IsUnique = true)]
    [Index(nameof(Token), IsUnique = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserTypes UserType { get; set; } = UserTypes.Guest;

        public DateTime AccountCreated { get; set; }
        public List<Startup> PublishedStartups { get; set; } = new List<Startup>();
        public List<Startup> History { get; set; } = new List<Startup>();
        public List<Startup> Projects { get; set; } = new List<Startup>();
    }

    [Index(nameof(Name), IsUnique = true)]
    public class Startup
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime StartupPublished { get; set; }

        public int AuthorForeignKey { get; set; }
        
        public User Author { get; set; }

        public List<User> Contributors { get; set; } = new List<User>(); 
    }
    public enum UserTypes
    {
        Guest,
        Creator,
        Developer,
    }
}
