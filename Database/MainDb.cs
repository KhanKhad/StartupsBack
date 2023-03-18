using Microsoft.EntityFrameworkCore;

namespace StartupsBack.Database
{
    public class MainDb : DbContext
    {
        public DbSet<Datacell> UserDB { get; set; } = null!;

        public MainDb(DbContextOptions<MainDb> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Datacell>();
        }
    }
    public class Datacell
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string OpenKeyXML { get; set; } = string.Empty;
        public string OpenKeyPem { get; set; } = string.Empty;
        public int keyValid { get; set; }
        public string GettedMessages { get; set; } = string.Empty;
        public string SendedMessages { get; set; } = string.Empty;
    }
}
