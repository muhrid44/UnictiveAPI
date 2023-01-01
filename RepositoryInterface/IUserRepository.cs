using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface
{
    public interface IUserRepository
    {
        Task<string> Create(UserModel model);
        Task<string> CreateHobby(UserModel UserID, string hobby);
        Task<string> CreateUserAuth(UserAuthModel model);
        Task<bool> EmailValidation(UserModel model);
        Task<bool> EmailExist(UserModel model);
        Task<UserAuthModel> PasswordVerification(UserModel model);
        Task<UserModel> GetUserId(string email);
        Task<List<string>> GetHobbyByUserId(int id);
        Task<string> UpdateUser(UserModel model, int id, string email);
    }
}
