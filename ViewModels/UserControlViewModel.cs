using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StartupsBack.Controllers;
using StartupsBack.Database;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace StartupsBack.ViewModels
{
    public class UserControlViewModel
    {
        private readonly ILogger _logger;
        private readonly MainDb _dbContext;
        public UserControlViewModel(ILogger logger, MainDb dbContext) 
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<(UserCreateResult, User?)> CreateUserAsync(string name, string pass)
        {
            try
            {
                var user = new User
                {
                    AccountCreated = DateTime.UtcNow,
                    Name = name,
                    Password = await GetHashAsync(pass),
                };
                await _dbContext.UsersDB.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return (UserCreateResult.Success, user);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqliteException sqEx)
                {
                    if (false &&( sqEx.SqliteExtendedErrorCode == 2067 || sqEx.SqliteErrorCode == 19))
                    {
                        return (UserCreateResult.AlreadyExist, null);
                    }
                }
                return (UserCreateResult.UnknownError, null);
            }
            catch(Exception ex)
            {
                return (UserCreateResult.UnknownError, null);
            }
           
        }

        public async Task<(AuthenticationResult, User?)> AuthenticationAsync(string name, string pass)
        {
            var passHash = await GetHashAsync(pass);
            var user = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Name == name && user.Password == passHash);
            if (user != null) return (AuthenticationResult.Success, user);

            user = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Name == name);
            if(user != null) return (AuthenticationResult.WrongPassword, null);

            return (AuthenticationResult.WrongLogin, null);
        }



        private const string _hashKey = "To be or not to be?";
        public static async Task<string> GetHashAsync(string input)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(input + _hashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult);
        }
    }
    public enum UserCreateResult
    {
        Success,
        AlreadyExist,

        UnknownError
    }
    public enum AuthenticationResult
    {
        Success,
        WrongPassword,
        WrongLogin
    }
}
