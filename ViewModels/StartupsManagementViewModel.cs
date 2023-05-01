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
                var author = await _dbContext.UsersDB.Include(user => user.PublishedStartups)
                    .FirstOrDefaultAsync(user => user.Name == authorName);

                if (author == null)
                    return StartupCreateResult.AuthorNotFound();

                var myHash = await CalculateHash(authorName, author.Token);

                if(myHash != hash)
                    return StartupCreateResult.AuthenticationFailed();

                if (author.PublishedStartups.FirstOrDefault(st => st.Name == startup.Name) != null)
                    return StartupCreateResult.AlreadyExists();

                startup.Author = author;
                startup.Contributors.Add(author);

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

        public async Task<StartupModel?> GetStartupModelAsync(int id)
        {
            var startup = await _dbContext.StartupsDB.FirstOrDefaultAsync(startup => startup.Id == id);
            return startup;
        }


        private const string _hashKey = "It's my startup!";
        private static async Task<string> CalculateHash(string authorName, string authorToken)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(authorName + authorToken + _hashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult);
        }
    }
}
