using CompagnyTools.Interface;
using CompagnyTools.Models;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
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
                _context.SaveChanges();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

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
    }
}
