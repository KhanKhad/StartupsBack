using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;
using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using StartupsBack.ViewModels.ActionsResults;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StartupsBack.ViewModels
{
    public class ProfileManagmentViewModel
    {
        private readonly ILogger _logger;
        private readonly MainDb _dbContext;
        public ProfileManagmentViewModel(ILogger logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<UserCreateResult> CreateUserAsync(UserJsonModel userJsonModel)
        {
            var user = new UserModel
            {
                AccountCreated = DateTime.UtcNow,
                Name = userJsonModel.Name,
                PasswordHash = userJsonModel.Password,
                ProfilePic = userJsonModel.ProfilePic,
            };
            return await CreateUserAsync(user);
        }

        public async Task<UserCreateResult> CreateUserAsync(UserModel userModel)
        {
            try
            {
                userModel.PasswordHash = await GetHashAsync(userModel.PasswordHash);
                var user = await _dbContext.UsersDB.AddAsync(userModel);
                await _dbContext.SaveChangesAsync();

                user.Entity.ProfilePicFileName = $"id#{user.Entity.Id}_{userModel.ProfilePicFileName}";
                await _dbContext.SaveChangesAsync();

                return UserCreateResult.Success(user.Entity);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqliteException sqEx)
                {
                    if (sqEx.SqliteExtendedErrorCode == 2067 || sqEx.SqliteErrorCode == 19)
                    {
                        return UserCreateResult.AlreadyExists();
                    }
                }
                return UserCreateResult.UnknownError(ex);
            }
            catch (Exception ex)
            {
                return UserCreateResult.UnknownError(ex);
            }
        }

        public async Task<AuthenticationResult> AuthenticationAsync(string name, string password)
        {
            try
            {
                var passHash = await GetHashAsync(password);
                var user = await _dbContext.UsersDB.FirstOrDefaultAsync(i => i.Name == name && i.PasswordHash == passHash);

                if (user == null)
                {
                    user = await _dbContext.UsersDB.FirstOrDefaultAsync(i => i.Name == name);
                    if (user != null) return AuthenticationResult.WrongPassword();
                    return AuthenticationResult.WrongLogin();
                }
                else
                {
                    string token;
                    do
                    {
                        token = GenerateToken();
                    }
                    while (await _dbContext.UsersDB.FirstOrDefaultAsync(x => x.Token == token) != null);

                    user.Token = token;
                    await _dbContext.SaveChangesAsync();
                    return AuthenticationResult.Success(user);
                }
            }
            catch (Exception ex)
            {
                return AuthenticationResult.UnknownError(ex);
            }
        }

        public async Task<JoinToStartupResult> TryToJoinToStartup(int id, string hash, int startupId)
        {
            try
            {
                var userWantToJoin = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Id == id);

                if (userWantToJoin == null)
                    return JoinToStartupResult.UserNotFound();

                var myHash = await GetHashAsync(userWantToJoin.Name + userWantToJoin.Token);

                if (myHash != hash)
                    return JoinToStartupResult.AuthenticationFailed();

                if (userWantToJoin.ContributingProjects.FirstOrDefault(st => st.Id == id) != null)
                    return JoinToStartupResult.AlreadyJoined();

                var startup = await _dbContext.StartupsDB.Include(i=>i.Contributors)
                    .Include(i=>i.AccsessDenied).Include(i=>i.WantToJoin).FirstOrDefaultAsync(i=>i.Id == startupId);

                if(startup == null)
                    return JoinToStartupResult.StartupNotFound();

                if(startup.AccsessDenied.FirstOrDefault(i=>i.Id == id)!= null)
                    return JoinToStartupResult.AccsessDenied();

                if (startup.WantToJoin.FirstOrDefault(i => i.Id == id) != null)
                    return JoinToStartupResult.WaitForAnswer();

                startup.WantToJoin.Add(userWantToJoin);

                await _dbContext.SaveChangesAsync();

                return JoinToStartupResult.RequestSended();
            }
            catch (Exception ex)
            {
                return JoinToStartupResult.UnknownError(ex);
            }
        }

        public async Task<JoinConfurmResult> ConfurmJoin(int id, string hash, int startupId, int userid)
        {
            try
            {
                var author = await _dbContext.UsersDB.Where(u => u.Id == id)
                    .Include(user => user.PublishedStartups).FirstOrDefaultAsync();

                if (author == null)
                    return JoinConfurmResult.AuthenticationFailed();

                var myHash = await GetHashAsync("");

                if (myHash != hash)
                    return JoinConfurmResult.AuthenticationFailed();

                if (author.PublishedStartups.FirstOrDefault(st => st.Name == startup.Name) != null)
                    return JoinConfurmResult.AlreadyJoined();

                startup.Author = author;
                startup.Contributors.Add(author);

                await _dbContext.SaveChangesAsync();

                return JoinConfurmResult.RequestSended();
            }
            catch (Exception ex)
            {
                return JoinConfurmResult.UnknownError(ex);
            }
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        private const string _hashKey = "How was your weekend?";
        public static async Task<string> GetHashAsync(string input)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(input + _hashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult);
        }
    }
}
