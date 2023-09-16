using CompagnyTools.Models;
using DAL.Entities;

namespace CompagnyTools.Interface
{
    public interface ILogin
    {
        public Users? Login(LoginModel model);
    }
}
