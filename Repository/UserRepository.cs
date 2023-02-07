using Dapper;
using EntityModel;
using HelperClasses;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        public async Task<UserModel> Create(UserModel model)
        {

                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.QueryAsync<int>(@"INSERT INTO ""usertable"" 
                (""name"", ""email"", ""phone"") 
                VALUES
                (@Name, @Email, @Phone) RETURNING ""id"";", model);

                    model.Id = query.FirstOrDefault();
                    return model;
                }
            

        }
        public async Task<string> CreateHobby(UserModel model, string hobby)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.ExecuteAsync($@"INSERT INTO ""hobbytable"" 
                (""userid"", ""hobby"") 
                VALUES
                ('{model.Id}', '{hobby}');");

                    return NotificationModel.Success;
                }
            }
            catch (Exception e)
            {
                return NotificationModel.Failed + ", caused by " + e;
            }
        }
        public async Task<bool> EmailValidation(UserModel model)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.QueryAsync(@"SELECT * FROM ""usertable"" 
                WHERE ""email"" = @Email;", model);

                    if (query.Count() == 0)
                    {
                        return true;
                    }
                    else
                    {
                        NotificationModel.ErrorMessage = "Email already taken by someone, please use another email address";
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<string> CreateUserAuth(UserAuthModel model)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.ExecuteAsync(@"INSERT INTO ""passwordhashtable"" 
                (""email"", ""passwordhash"", ""passwordsalt"") 
                VALUES
                (@Email, @PasswordHash, @PasswordSalt);", model);

                    return NotificationModel.Success;
                }
            }
            catch (Exception e)
            {
                return NotificationModel.Failed + ", caused by " + e;
            }
        }
        public async Task<string> UpdateUserAuth(UserAuthModel model)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.ExecuteAsync(@"UPDATE ""passwordhashtable"" 
                SET ""passwordhash"" = @PasswordHash,
                    ""passwordsalt"" = @PasswordSalt
                    WHERE ""email"" = @Email;", model);

                    return NotificationModel.Success;
                }
            }
            catch (Exception e)
            {
                return NotificationModel.Failed + ", caused by " + e;
            }
        }
        public async Task<bool> EmailExist(UserModel model)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.QueryAsync(@"SELECT * FROM ""passwordhashtable"" 
                WHERE ""email"" = @Email;", model);

                    if (query != null)
                    {
                        return true;
                    }
                    else
                    {
                        NotificationModel.ErrorMessage = "Email is invalid!";
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<UserAuthModel> PasswordVerification(UserModel model)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.QueryAsync<UserAuthModel>(@"SELECT * FROM ""passwordhashtable"" 
                WHERE ""email"" = @Email;", model);

                    if (query.Count() == 1)
                    {
                        return query.FirstOrDefault();
                    }
                    else
                    {
                        NotificationModel.ErrorMessage = "Password is invalid!";
                        return null ;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<UserModel> GetUserId(string email)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.QueryAsync<UserModel>($@"SELECT * FROM ""usertable"" 
                WHERE ""email"" = '{email}';");

                    if (query.Count() == 1)
                    {
                        return query.FirstOrDefault();
                    }
                    else
                    {
                        NotificationModel.ErrorMessage = "Password is invalid!";
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<string>> GetHobbyByUserId(int id)
        {
            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var query = await db.QueryAsync<string>($@"SELECT ""hobby"" FROM ""hobbytable"" 
                WHERE ""userid"" = '{id}';");

                    if (query.Count() > 0)
                    {
                        return query.ToList();
                    }
                    else
                    {
                        NotificationModel.ErrorMessage = "Hobbies not found!";
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<string> UpdateUser(UserModel model, int id, string email)
        {

            try
            {
                using (var db = new NpgsqlConnection(ApplicationSettings.connectionString))
                {
                    var changeEmailForPasswordHashed = await db.ExecuteAsync($@"UPDATE ""passwordhashtable""
                SET ""email"" = @Email
                WHERE ""email"" = '{email}';", model);

                    var query = await db.ExecuteAsync($@"UPDATE ""usertable""
                SET ""name"" = @Name, ""email"" = @Email, ""phone"" = @Phone
                WHERE ""id"" = '{id}';", model);



                    return NotificationModel.Success;
                }
            }
            catch (Exception e)
            {
                return NotificationModel.Failed;
            }
        }

    }
}
