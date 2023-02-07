using EntityModel;
using HelperClasses;
using Microsoft.IdentityModel.Tokens;
using RepositoryInterface;
using ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using UtilityInterface;

namespace Service
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        IHobbyRepository _hobbyRepository;
        IPhoneValidation _phoneValidation;
        public UserService(IUserRepository userRepository, IPhoneValidation phoneValidation, IHobbyRepository hobbyRepository)
        {
            _userRepository = userRepository;
            _phoneValidation = phoneValidation;
            _hobbyRepository = hobbyRepository;
        }

        public async Task<string> Create(UserModel model)
        {
            PasswordUtility.PasswordHashGenerator(model.Password, out byte[] passwordHash, out byte[] passwordSalt);

            UserAuthModel userAuthModel = new UserAuthModel();
            userAuthModel.Email = model.Email;
            userAuthModel.PasswordHash = passwordHash;
            userAuthModel.PasswordSalt = passwordSalt;

            //email validation
            var emailValidation = await _userRepository.EmailValidation(model);

            if (emailValidation)
            {
                //phone validation
                var phoneValidation = _phoneValidation.Validate(model);

                if (phoneValidation)
                {
                    await _userRepository.Create(model);

                    var result = await _userRepository.CreateUserAuth(userAuthModel);

                    await CreateHobby(model);

                    return result;
                }
                else
                {
                    return NotificationModel.ErrorMessage;
                }

            }
            else
            {
                return NotificationModel.ErrorMessage;
            }
        }
        public async Task<string> CreateHobby(UserModel model)
        {
            if(model.Hobbies.Count() != 0)
            {
                foreach(var hobby in model.Hobbies)
                {
                    await _userRepository.CreateHobby(model, hobby);
                }

                return "OK";
            }

            return "OK";
        }

        public async Task<bool> EmailExist(UserModel model)
        {
            return await _userRepository.EmailExist(model);
        }

        public async Task<bool> PasswordVerification(UserModel model)
        {
            var passwordHashed = await _userRepository.PasswordVerification(model);

            if(passwordHashed != null)
            {
                ApplicationSettings.TokenLogin = JWTUtility.CreateToken(passwordHashed);
            }

            return PasswordUtility.VerifyPasswordHash(model.Password, passwordHashed.PasswordHash, passwordHashed.PasswordSalt);
        }

        public async Task<UserModel> GetUserProfile(string email)
        {
            var userID = await _userRepository.GetUserId(email);

            UserModel userProfile = new UserModel();
            userProfile.Name = userID.Name;
            userProfile.Id = userID.Id;
            userProfile.Email = email;
            userProfile.Phone = userID.Phone;

            var hobbies = await _userRepository.GetHobbyByUserId(userProfile.Id);

            List<string> Hobbies = new List<string>();

            if(hobbies != null)
            {
                foreach (var i in hobbies)
                {
                    Hobbies.Add(i);
                }

                userProfile.Hobbies = Hobbies;
            }

            return userProfile;

        }

        public async Task<string> Update(UserModel model, UserModel updated)
        {
            UserModel updatedUser = new UserModel();

            if (String.IsNullOrWhiteSpace(updated.Name))
            {
                updatedUser.Name = model.Name;
            } else
            {
                updatedUser.Name = updated.Name;
            }

            if (String.IsNullOrWhiteSpace(updated.Email))
            {
                updatedUser.Email = model.Email;


                if (String.IsNullOrWhiteSpace(updated.Phone))
                {
                    updatedUser.Phone = model.Phone;

                    var result = await _userRepository.UpdateUser(updatedUser, model.Id, model.Email);

                    if (!String.IsNullOrWhiteSpace(updated.Password))
                    {
                        PasswordUtility.PasswordHashGenerator(updated.Password, out byte[] passwordHash, out byte[] passwordSalt);

                        UserAuthModel userAuthModel = new UserAuthModel();
                        userAuthModel.Email = updatedUser.Email;
                        userAuthModel.PasswordHash = passwordHash;
                        userAuthModel.PasswordSalt = passwordSalt;
                        await _userRepository.UpdateUserAuth(userAuthModel);
                    }

                    return result;

                }
                else
                {
                    var phoneValidation = _phoneValidation.Validate(updated);

                    updatedUser.Phone = updated.Phone;

                    if (phoneValidation)
                    {
                        var result = await _userRepository.UpdateUser(updatedUser, model.Id, model.Email);

                        if (!String.IsNullOrWhiteSpace(updated.Password))
                        {
                            PasswordUtility.PasswordHashGenerator(updated.Password, out byte[] passwordHash, out byte[] passwordSalt);

                            UserAuthModel userAuthModel = new UserAuthModel();
                            userAuthModel.Email = updatedUser.Email;
                            userAuthModel.PasswordHash = passwordHash;
                            userAuthModel.PasswordSalt = passwordSalt;
                            await _userRepository.UpdateUserAuth(userAuthModel);
                        }

                        return result;

                    }
                    else
                    {
                        return NotificationModel.ErrorMessage;
                    }

                }
            } else
            {
                var emailValidation = await _userRepository.EmailValidation(updated);

                updatedUser.Email = updated.Email;

                if (emailValidation)
                {
                    if (String.IsNullOrWhiteSpace(updated.Phone))
                    {
                        updatedUser.Phone = model.Phone;

                        var result = await _userRepository.UpdateUser(updatedUser, model.Id, model.Email);

                        if (!String.IsNullOrWhiteSpace(updated.Password))
                        {
                            PasswordUtility.PasswordHashGenerator(updated.Password, out byte[] passwordHash, out byte[] passwordSalt);

                            UserAuthModel userAuthModel = new UserAuthModel();
                            userAuthModel.Email = updatedUser.Email;
                            userAuthModel.PasswordHash = passwordHash;
                            userAuthModel.PasswordSalt = passwordSalt;
                            await _userRepository.UpdateUserAuth(userAuthModel);
                        }

                        return result;

                    }
                    else
                    {
                        updatedUser.Phone = updated.Phone;

                        var phoneValidation = _phoneValidation.Validate(updatedUser);

                        if (phoneValidation)
                        {
                            var result = await _userRepository.UpdateUser(updatedUser, model.Id, model.Email);

                            if (!String.IsNullOrWhiteSpace(updated.Password))
                            {
                                PasswordUtility.PasswordHashGenerator(updated.Password, out byte[] passwordHash, out byte[] passwordSalt);

                                UserAuthModel userAuthModel = new UserAuthModel();
                                userAuthModel.Email = updatedUser.Email;
                                userAuthModel.PasswordHash = passwordHash;
                                userAuthModel.PasswordSalt = passwordSalt;
                                await _userRepository.UpdateUserAuth(userAuthModel);
                            }

                            return result;

                        }
                        else
                        {
                            return NotificationModel.ErrorMessage;
                        }
                    }
                }
                else
                {
                    return NotificationModel.ErrorMessage;

                }

            }



            return NotificationModel.ErrorMessage;

        }

        public async Task<string> AddHobby(UserModel model, string hobby)
        {
            var userID = await _userRepository.GetUserId(model.Email);

            UserModel userProfile = new UserModel();
            userProfile.Id = userID.Id;

            var result = await _hobbyRepository.AddHobby(userProfile, hobby);
            return result;
        }

        public async Task<string> DeleteHobby(UserModel model, string hobby)
        {
            var userID = await _userRepository.GetUserId(model.Email);

            UserModel userProfile = new UserModel();
            userProfile.Id = userID.Id;
            userProfile.Email = model.Email;

            var result = await _hobbyRepository.DeleteHobby(userProfile, hobby);
            return result;


        }




    }
}
