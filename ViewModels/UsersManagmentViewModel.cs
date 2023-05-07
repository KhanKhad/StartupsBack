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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StartupsBack.ViewModels
{
    public class UsersManagmentViewModel
    {
        private readonly ILogger _logger;
        private readonly MainDb _dbContext;
        public UsersManagmentViewModel(ILogger logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<UserModel?> GetUserById(int id)
        {
            var user = await _dbContext.UsersDB.FirstOrDefaultAsync(i => i.Id == id);
            return user;
        }
    }
}
