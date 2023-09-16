using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.Interface
{
    public interface IAccount
    {
        public Users CreateUser(LoginModel model);
        public List<Users>? GetAllUsers();
    }
}
