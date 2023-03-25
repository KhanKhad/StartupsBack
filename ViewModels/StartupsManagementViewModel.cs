using Microsoft.EntityFrameworkCore;
using StartupsBack.Database;
using StartupsBack.Models.DbModels;
using StartupsBack.Models.JsonModels;
using StartupsBack.ViewModels.ActionsResults;

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
                var author = await _dbContext.UsersDB.Include(user => user.PublishedStartups)
                    .FirstOrDefaultAsync(user => user.Token == startupJsonModel.AuthorToken);
                
                if(author == null) 
                    return StartupCreateResult.AuthenticationFailed();

                if(author.PublishedStartups.FirstOrDefault(st=>st.Name == startupJsonModel.Name)!= null)
                    return StartupCreateResult.AlreadyExists();

                var startup = new StartupModel
                {
                    Author = author,
                    Name = startupJsonModel.Name,
                    Description = startupJsonModel.Description,
                    StartupPublished = DateTime.UtcNow,
                };
                startup.Contributors.Add(author);

                var res = await _dbContext.StartupsDB.AddAsync(startup);
                await _dbContext.SaveChangesAsync();
                startup.Id = res.Entity.Id;

                return StartupCreateResult.Success(startup);
            }
            catch (Exception ex)
            {
                return StartupCreateResult.UnknownError(ex);
            }
        }
    }
}
