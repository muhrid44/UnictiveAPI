using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryInterface
{
    public interface IHobbyRepository
    {
        Task<string> AddHobby(UserModel model, string hobby);
        Task<string> DeleteHobby(UserModel model, string hobby);

    }
}
