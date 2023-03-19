using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StartupsBack.Models.DbModels
{
    [Index(nameof(Name), IsUnique = true)]
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserTypes UserType { get; set; } = UserTypes.Guest;

        public DateTime AccountCreated { get; set; }
        public List<StartupModel> PublishedStartups { get; set; } = new List<StartupModel>();
        public List<StartupModel> History { get; set; } = new List<StartupModel>();
        public List<StartupModel> Projects { get; set; } = new List<StartupModel>();
    }
    public enum UserTypes
    {
        Guest,
        Creator,
        Developer,
    }
}
