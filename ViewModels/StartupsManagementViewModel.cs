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
    public class StartupsManagementViewModel
    {
        private readonly ILogger _logger;
        private readonly MainDb _dbContext;
        public StartupsManagementViewModel(ILogger logger, MainDb dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<StartupCreateResult> CreateStartupAsync(StartupJsonModel startupJsonModel)
        {
            try
            {
                var startup = new StartupModel
                {
                    Name = startupJsonModel.Name,
                    Description = startupJsonModel.Description,
                };

                return await CreateStartupAsync(startup, startupJsonModel.AuthorName, startupJsonModel.Hash);
            }
            catch (Exception ex)
            {
                return StartupCreateResult.UnknownError(ex);
            }
        }
        public async Task<StartupCreateResult> CreateStartupAsync(StartupModel startup, string authorName, string hash)
        {
            try
            {
                var author = await _dbContext.UsersDB.Where(user => user.Name == authorName)
                    .Include(user => user.PublishedStartups).FirstOrDefaultAsync();

                if (author == null)
                    return StartupCreateResult.AuthorNotFound();

                var myHash = await CalculateHash(authorName, author.Token);

                if(myHash != hash)
                    return StartupCreateResult.AuthenticationFailed();

                if (author.PublishedStartups.FirstOrDefault(st => st.Name == startup.Name) != null)
                    return StartupCreateResult.AlreadyExists();

                startup.Author = author;
                startup.Contributors.Add(author);
                startup.StartupPublished = DateTime.UtcNow;
                startup.LastModify = DateTime.UtcNow;

                var res = await _dbContext.StartupsDB.AddAsync(startup);
                await _dbContext.SaveChangesAsync();

                res.Entity.StartupPicFileName = $"id#{res.Entity.Id}_{startup.StartupPicFileName}";
                await _dbContext.SaveChangesAsync();

                return StartupCreateResult.Success(res.Entity);
            }
            catch (Exception ex)
            {
                return StartupCreateResult.UnknownError(ex);
            }
        }

        public async Task<int[]> GetStartupsIds(int pageNumber, int pageSize)
        {
            var ids = await _dbContext.StartupsDB.Select(i=>i.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync();
            return ids;
        }
        public async Task<int[]> GetMyStartupsIds(int id, int pageNumber, int pageSize)
        {
            var user = await _dbContext.UsersDB.Where(i => i.Id == id).Include(i=>i.ContributingProjects).FirstOrDefaultAsync();

            if(user == null) return Array.Empty<int>();
            var allStartups = user.ContributingProjects.Select(i=>i.Id).ToList();

            var ids = allStartups.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray();
            return ids;
        }
        public async Task<StartupModel?> GetStartupModelAsync(int id)
        {
            var startup = await _dbContext.StartupsDB.Include(i => i.Contributors)
                .FirstOrDefaultAsync(startup => startup.Id == id);

            return startup;
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
                .Include(i => i.PublishedStartups).ThenInclude(i => i.WantToJoin).FirstOrDefaultAsync();

            if (author == null) return Array.Empty<StartupJoinRequestJsonModel>();

            var requestes = new List<StartupJoinRequestJsonModel>();

            foreach (var startup in author.PublishedStartups)
            {
                var request = new StartupJoinRequestJsonModel()
                {
                    StartupId = startup.Id,
                    UsersWantToJoin = startup.WantToJoin.Select(i => i.Id).ToArray(),
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

                var myHash = await CalculateHash(userWantToJoin.Name, userWantToJoin.Token);

                if (myHash != hash)
                    return JoinToStartupResult.AuthenticationFailed();

                var startup = await _dbContext.StartupsDB.Include(i => i.Contributors)
                    .Include(i => i.AccsessDenied).Include(i => i.WantToJoin).Include(i => i.Author).FirstOrDefaultAsync(i => i.Id == startupId);

                if (startup == null)
                    return JoinToStartupResult.StartupNotFound();

                if (startup.Contributors.FirstOrDefault(i => i.Id == id) != null)
                    return JoinToStartupResult.AlreadyJoined();

                if (startup.AccsessDenied.FirstOrDefault(i => i.Id == id) != null)
                    return JoinToStartupResult.AccsessDenied();

                if (startup.WantToJoin.FirstOrDefault(i => i.Id == id) != null)
                    return JoinToStartupResult.WaitForAnswer();

                startup.WantToJoin.Add(userWantToJoin);

                if (startup.Author != null)
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

                var myHash = await CalculateHash(author.Name, author.Token);

                if (myHash != hash)
                    return AcceptUserResult.AuthenticationFailed();

                var startup = await _dbContext.StartupsDB.Where(st => st.Id == startupId)
                    .Include(st => st.WantToJoin).Include(st => st.AccsessDenied).Include(st => st.Contributors).FirstOrDefaultAsync();

                if (startup == null)
                    return AcceptUserResult.StartupNotFound();

                if (startup.AuthorForeignKey != id)
                    return AcceptUserResult.NotAuthor();

                var user = await _dbContext.UsersDB.Where(u => u.Id == userid).FirstOrDefaultAsync();

                if (user == null)
                    return AcceptUserResult.UserNotFound();

                if (startup.Contributors.Contains(user))
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

                var myHash = await CalculateHash(author.Name, author.Token);

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

        private const string _hashKey = "It's my startup!";
        private static async Task<string> CalculateHash(string authorName, string authorToken)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _hashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult).Replace("+", "").Replace("/", "");
        }
    }
}
