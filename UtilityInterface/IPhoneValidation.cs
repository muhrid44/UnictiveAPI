using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityInterface
{
    public interface IPhoneValidation
    {
        bool Validate(UserModel model);
    }
}
