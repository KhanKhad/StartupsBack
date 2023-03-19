using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StartupsBack.Controllers;
using StartupsBack.Database;
using StartupsBack.ViewModels.ActionsResults;
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

        public async Task<UserCreateResult> CreateUserAsync(string name, string pass)
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

                return UserCreateResult.Success(user);
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
            catch(Exception ex)
            {
                return UserCreateResult.UnknownError(ex);
            }
        }

        public async Task<AuthenticationResult> AuthenticationAsync(string name, string pass)
        {
            try
            {
                var passHash = await GetHashAsync(pass);
                var user = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Name == name && user.Password == passHash);
                if (user != null)
                {
                    user.Token = GenerateToken();
                    await _dbContext.SaveChangesAsync();
                    return AuthenticationResult.Success(user);
                }

                user = await _dbContext.UsersDB.FirstOrDefaultAsync(user => user.Name == name);
                if (user != null) return AuthenticationResult.WrongPassword();

                return AuthenticationResult.WrongLogin();
            }
            catch (Exception ex) 
            {
                return AuthenticationResult.UnknownError(ex);
            }
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        private const string _hashKey = "To be or not to be?";
        private static async Task<string> GetHashAsync(string input)
        {
            using SHA256 mySHA256 = SHA256.Create();
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(input + _hashKey));
            var byteResult = await mySHA256.ComputeHashAsync(stream);
            return Convert.ToBase64String(byteResult);
        }
    }
}
