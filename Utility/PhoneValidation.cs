using EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityInterface;

namespace Utility
{
    public class PhoneValidation : IPhoneValidation
    {
        string ErrorMessage = "Please only insert a number";

        public bool Validate(UserModel model)
        {
            bool IsSuccess = long.TryParse(model.Phone, out long number);

            if (IsSuccess)
            {
                return true;
            }
            else
            {
                NotificationModel.ErrorMessage = ErrorMessage;
                return false;
            }
        }

    }
}
