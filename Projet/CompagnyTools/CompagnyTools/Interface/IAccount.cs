using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.Interface
{
    public interface IAccount
    {
        public Users CreateUser(LoginModel model);
        public bool DeleteUser(int[] Ids);
        public List<Users>? GetAllUsers();
    }
}
