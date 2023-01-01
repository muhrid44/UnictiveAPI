using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface
{
    public interface IUserService
    {
        Task<string> Create(UserModel model);
        Task<string> CreateHobby(UserModel hobbyModel);
        Task<bool> EmailExist(UserModel model);
        Task<bool> PasswordVerification(UserModel model);
        Task<UserModel> GetUserProfile(string email);
        Task<string> Update(UserModel model, UserModel updated);
        Task<string> AddHobby(UserModel model, string hobby);
        Task<string> DeleteHobby(UserModel model, string hobby);

    }
}
