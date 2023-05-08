using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StartupsBack.Database;
using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using StartupsBack.ViewModels.ActionsResults;
using System;
using System.Collections.Generic;
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
                userModel.AccountCreated = DateTime.UtcNow;
                userModel.LastModify = DateTime.UtcNow;
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

        public async Task<GetDeltaResult> GetStartupsDelta(int id)
        {
            try
            {
                var author = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Id == id);

                if (author == null) return GetDeltaResult.UserNotFound();

                return GetDeltaResult.Success(author.StartupsDelta);
            }
            catch (Exception ex)
            {
                return GetDeltaResult.UnknownError(ex);
            }
        }

        public async Task<StartupJoinRequestJsonModel[]> GetStartupsJoinRequestes(int id)
        {
            var author = await _dbContext.UsersDB.Where(user => user.Id == id)
                .Include(i=>i.PublishedStartups).ThenInclude(i=>i.WantToJoin).FirstOrDefaultAsync();

            if(author == null) return Array.Empty<StartupJoinRequestJsonModel>();

            var requestes = new List<StartupJoinRequestJsonModel>();

            foreach (var startup in author.PublishedStartups)
            {
                var request = new StartupJoinRequestJsonModel()
                {
                    StartupId = startup.Id,
                    UsersWantToJoin = startup.WantToJoin.Select(i=>i.Id).ToArray(),
                };
                requestes.Add(request);
            }

            return requestes.ToArray();
        }

        public async Task<JoinToStartupResult> TryToJoinToStartup(int id, string hash, int startupId)
        {
            try
            {
                var userWantToJoin = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Id == id);

                if (userWantToJoin == null)
                    return JoinToStartupResult.UserNotFound();

                var myHash = await GetProfileHashAsync(userWantToJoin.Name, userWantToJoin.Token);

                if (myHash != hash)
                    return JoinToStartupResult.AuthenticationFailed();

                if (userWantToJoin.ContributingProjects.FirstOrDefault(st => st.Id == id) != null)
                    return JoinToStartupResult.AlreadyJoined();

                var startup = await _dbContext.StartupsDB.Include(i=>i.Contributors)
                    .Include(i=>i.AccsessDenied).Include(i=>i.WantToJoin).Include(i => i.Author).FirstOrDefaultAsync(i=>i.Id == startupId);

                if(startup == null)
                    return JoinToStartupResult.StartupNotFound();

                if(startup.AccsessDenied.FirstOrDefault(i=>i.Id == id)!= null)
                    return JoinToStartupResult.AccsessDenied();

                if (startup.WantToJoin.FirstOrDefault(i => i.Id == id) != null)
                    return JoinToStartupResult.WaitForAnswer();

                startup.WantToJoin.Add(userWantToJoin);

                if(startup.Author != null)
                    startup.Author.StartupsDelta += 1;

                await _dbContext.SaveChangesAsync();

                return JoinToStartupResult.RequestSended();
            }
            catch (Exception ex)
            {
                return JoinToStartupResult.UnknownError(ex);
            }
        }

        public async Task<AcceptUserResult> AcceptUserToStartup(int id, string hash, int startupId, int userid)
        {
            try
            {
                var author = await _dbContext.UsersDB.Where(u => u.Id == id).FirstOrDefaultAsync();

                if (author == null)
                    return AcceptUserResult.AuthorNotFound();

                var myHash = await GetProfileHashAsync(author.Name, author.Token);

                if (myHash != hash)
                    return AcceptUserResult.AuthenticationFailed();

                var startup = await _dbContext.StartupsDB.Where(st => st.Id == startupId)
                    .Include(st => st.WantToJoin).Include(st => st.AccsessDenied).Include(st => st.Contributors).FirstOrDefaultAsync();

                if (startup == null)
                    return AcceptUserResult.StartupNotFound();

                if(startup.AuthorForeignKey != id)
                    return AcceptUserResult.NotAuthor();

                var user = await _dbContext.UsersDB.Where(u => u.Id == userid).FirstOrDefaultAsync();

                if (user == null)
                    return AcceptUserResult.UserNotFound();

                if(startup.Contributors.Contains(user))
                    return AcceptUserResult.AlreadyJoined();

                if (startup.WantToJoin.Contains(user))
                {
                    startup.WantToJoin.Remove(user);

                    startup.Contributors.Add(user);

                    author.StartupsDelta -= 1;

                    await _dbContext.SaveChangesAsync();

                    return AcceptUserResult.SuccessJoined();
                }
                else if (startup.AccsessDenied.Contains(user))
                {
                    startup.AccsessDenied.Remove(user);
                    startup.Contributors.Add(user);

                    author.StartupsDelta -= 1;

                    await _dbContext.SaveChangesAsync();

                    return AcceptUserResult.SuccessJoined();
                }
                else
                    return AcceptUserResult.UserDontSendRequest();
                
            }
            catch (Exception ex)
            {
                return AcceptUserResult.UnknownError(ex);
            }
        }
        public async Task<RejectUserResult> RejectUserToStartup(int id, string hash, int startupId, int userid)
        {
            try
            {
                var author = await _dbContext.UsersDB.Where(u => u.Id == id).FirstOrDefaultAsync();

                if (author == null)
                    return RejectUserResult.AuthorNotFound();

                var myHash = await GetProfileHashAsync(author.Name, author.Token);

                if (myHash != hash)
                    return RejectUserResult.AuthenticationFailed();

                var startup = await _dbContext.StartupsDB.Where(st => st.Id == startupId)
                    .Include(st => st.WantToJoin).Include(st => st.AccsessDenied).Include(st => st.Contributors).FirstOrDefaultAsync();

                if (startup == null)
                    return RejectUserResult.StartupNotFound();

                if (startup.AuthorForeignKey != id)
                    return RejectUserResult.NotAuthor();

                var user = await _dbContext.UsersDB.Where(u => u.Id == userid).FirstOrDefaultAsync();

                if (user == null)
                    return RejectUserResult.UserNotFound();

                if (startup.AccsessDenied.Contains(user))
                    return RejectUserResult.AlreadyDenied();

                if (startup.WantToJoin.Contains(user))
                {
                    startup.WantToJoin.Remove(user);

                    startup.AccsessDenied.Add(user);

                    author.StartupsDelta -= 1;

                    await _dbContext.SaveChangesAsync();

                    return RejectUserResult.SuccessDenied();
                }
                else if (startup.Contributors.Contains(user))
                {
                    startup.Contributors.Remove(user);

                    startup.AccsessDenied.Add(user);

                    author.StartupsDelta -= 1;

                    await _dbContext.SaveChangesAsync();

                    return RejectUserResult.SuccessDenied();
                }
                else
                    return RejectUserResult.UserDontSendRequest();

            }
            catch (Exception ex)
            {
                return RejectUserResult.UnknownError(ex);
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

        private const string _profileHashKey = "My PProfile?";
        public static async Task<string> GetProfileHashAsync(string name, string token)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(name + token + _profileHashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", "");
        }
    }
}
