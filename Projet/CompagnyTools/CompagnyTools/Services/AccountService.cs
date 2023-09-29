using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Context;
using DAL.Entities;
using System.Security.Cryptography;
using System.Text;

namespace CompagnyTools.Services
{
    public class AccountService : IAccount
    {
        private readonly Access _context;
        public AccountService(Access context)
        {
            _context = context;
        }

        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        /// <summary>
        /// Creating a user and assigning admin rights
        /// </summary>
        /// <param name="model">User data</param>
        /// <returns></returns>
        public Users CreateUser(LoginModel model)
        {
            try
            {
                string hash = HashPasword(model.Password, out var salt);

                Users user = new()
                {
                    Email = model.Email,
                    Password = hash,
                    Username = model.Username,
                    Salt = salt,
                };
              
                _context.Users.Add(user);

                UsersRoles usersRoles = new() {
                    UserId = user.Id,
                    UserRight = "Admin",
                };

                user.UsersRoles.Add(usersRoles);

                _context.UsersRoles.AddRange(user.UsersRoles);
                _context.SaveChanges();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Create hash for a given password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        private string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }

        /// <summary>
        /// Select all user in DB
        /// </summary>
        /// <returns></returns>
        public List<Users>? GetAllUsers()
        {
            try
            {
                List<Users>? result = new();
                result = _context.Users.ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete User by his Id
        /// </summary>
        /// <param name="Id">User's id</param>
        public bool DeleteUser(int[] Ids)
        {
            try
            {
                for (int i = 0; i < Ids.Count(); i++)
                {
                    int userId = Convert.ToInt32(Ids[i]);   

                    Users? user = _context.Users.SingleOrDefault(x => x.Id == userId);

                    if (user != null)
                    {
                        _context.UsersRoles.RemoveRange(user.UsersRoles);
                        _context.Users.Remove(user);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
