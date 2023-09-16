using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace CompagnyTools.Services
{
    public class LoginService : ILogin
    {
        private readonly Access _context;
        public LoginService(Access context)
        {
            _context = context;
        }

        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public Users? Login(LoginModel model)
        {
            try
            {
                model.Username = model.Username.Trim();

                Users? checkUser = _context.Users.FirstOrDefault(x => x.Username.Trim().ToLower() == model.Username.ToLower());
                checkUser.UsersRoles = _context.UsersRoles.Where(x => x.Id == checkUser.Id).ToList();

                if (checkUser != null)
                {
                    bool checkPassword = VerifyPassword(model.Password, checkUser.Password, checkUser.Salt);

                    if (checkUser != null && checkPassword)
                    {
                        return checkUser;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool VerifyPassword(string password, string hash, byte[] saltPass)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, saltPass, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

    }
}
